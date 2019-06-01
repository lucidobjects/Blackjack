using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Blackjack
{
    public class Dealer 
    {
        private readonly string name = "Dealer";
        private readonly int delay = 500;
        private readonly int MinSlice = 50;
        private readonly int MaxSlice = 100;

        private Random rnd = new Random();
        private Interactions interactions = new Interactions();

        public DealerHand Hand { get; private set; } = new DealerHand();
        public bool KeepPlayingSoft => Hand.IsSoft && Hand.SoftHigh < Table.DealerSoftStandValue;
        public bool KeepPlayingHard => !Hand.IsSoft && Hand.HardValue < Table.DealerHardStandValue;

        public Dealer()
        {
        }

        public void BootBrokePlayers(Table table)
        {
            table.BrokePlayers.ForEach(p =>
            {
                Console.WriteLine($"{p.Name}, looks like you went broke. Better luck next time.");
                extendedPause();
            });

            depart(table, table.BrokePlayers);
        }

        public void PlaceYourBets(Table table)
        {
            var leavingPlayers = new List<Player>();

            table.Players.ForEach(p =>
            {
                if (p.Chips >= Table.MinBet)
                {
                    var bet = interactions.GetBet(p);

                    if(bet == 0)
                    {
                        leavingPlayers.Add(p);
                    }
                    else
                    {
                        p.InitialBet(bet);
                    }
                    table.Draw();
                }
            });

            if (leavingPlayers.Any())
            {
                depart(table, leavingPlayers);
            }
            
            table.Draw();
        }

        public void DealInitial(Table table)
        {
            dealAll(table);  //first card
            dealAll(table);  //second card
        }

        public void Play(Table table)
        {
            //pay blackjack winners before playing the other hands
            table.PlayersWithBlackJack()
                .Select(p => p.Hand)
                .ToList()
                .ForEach(h => payBlackjack(table, h));

            table.ActivePlayers.ForEach(p => Play(table, p.Hand));      
        }

        //main player decision tree
        public void Play(Table table, PlayerHand hand)
        {
            var player = hand.Player;
            var name = player.Name;

            if (player.HasMultipleHands)
            {
                showCurrentHand(hand, name);
            }

            var action = eActions.None;

            if (hand.CanBeSplit && hand.Player.CanSplit && interactions.WantsToSplit(name))
            {
                action = eActions.Split;
            }
            else
            {
                action = interactions.PlayerFirst(hand.Player);
            }

            table.Draw();

            if(action == eActions.Hit)
            {
                playUntilDone(table, hand);
            }
            else if (action == eActions.Double)
            {
                doubleDown(table, hand);
            }
            else if (action == eActions.Split)
            {
                split(table, hand);
            }
            else if(action == eActions.Stand)
            {
                hand.Stand();
                table.Draw();
            }
        }

        public void Play(Table table, DealerHand hand)
        {
            hand.IsDealersTurn();
            table.Draw();

            if (table.HasActivePlayers)
            {
                while (KeepPlaying())
                {
                    Deal(table.Shoe, hand);
                    table.Draw();
                }
            }

            postHandPause();
        }

        public void FinishAllHands(Table table)
        {
            table.ActivePlayers.SelectMany(p => p.ActiveHands).ToList().ForEach(h =>
            {
                Finish(table, h, Hand);
                postHandPause();
            });

            Sweep(Hand, table.Tray);
            reset();
            postHandPause();
        }

        public void Finish(Table table, PlayerHand playerHand, DealerHand dealerHand)
        {
            var outcome = calculateOutcome(playerHand, dealerHand);

            var wager = playerHand.Wager;
            if (outcome == eResults.Win)
            {
                Console.WriteLine($"{playerHand.Player.Name} you won!");
                playerHand.Win();
                table.Pay(wager);
            }
            else if (outcome == eResults.Lose)
            {
                Console.WriteLine($"{playerHand.Player.Name} you lost...");
                playerHand.Lose();
                table.Win(wager);
            }
            else
            {
                Console.WriteLine($"{playerHand.Player.Name} you pushed.");
                playerHand.Push();
            }
            postHandPause();
            Sweep(playerHand, table.Tray);
            table.Draw();
        }

        public void Sweep(Hand hand, DiscardTray tray) => tray.Add(hand.Sweep());

        public void Shuffle(Shoe shoe, DiscardTray tray)
        {
            var raw = shoe.Cards.Concat(tray.Cards).ToList();
            var shuffled = new List<Card>();
            while (raw.Any())
            {
                var index = rnd.Next(raw.Count);
                shuffled.Add(raw.ElementAt(index));
                raw.RemoveAt(index);
            }

            shoe.Reload(shuffled);
        }

        public void Burn(Card card, DiscardTray tray)
        {
            Console.Write("Burn Card: ");
            card.Draw();
            tray.Add(card);
            intermediatePause();
        }

        public void Deal(Shoe shoe, Hand hand)
        {
            dealPause();
            hand.Deal(shoe.Next());
        }
    
        public bool MayHaveBlackjack() => Hand.IsPossibleBlackjack();

        public bool HasBlackjack()
        {
            Console.WriteLine("Checking for dealer blackjack.");
            intermediatePause();
            var isBlackjack = Hand.IsBlackjack();
            var msg = isBlackjack ? "Dealer has blackjack" : "Dealer does not have blackjack.";
            Console.WriteLine(msg);
            intermediatePause();
            return isBlackjack;
        }

        public void Blackjack()
        {
            Console.WriteLine("Dealer has blackjack");
            postHandPause();
        }

        public bool KeepPlaying() => !Hand.IsBusted() && (KeepPlayingSoft || KeepPlayingHard);

        public int Slice() => rnd.Next(MinSlice, MaxSlice);

        public void Draw()
        {
            Console.Write($"{name}\t");
            Hand.Draw();
        }

        private void dealAll(Table table)
        {
            table.ActiveHands.ForEach(h =>
            {
                Deal(table.Shoe, h);
                table.Draw();
            });

            Deal(table.Shoe, Hand);
            table.Draw();
        }

        private void playUntilDone(Table table, PlayerHand hand)
        {
            hit(table, hand);

            if (hand.Player.HasMultipleHands)
            {
                showCurrentHand(hand, hand.Player.Name);
            }

            //finish playing the hand
            var action = eActions.None;
            while (!hand.IsBusted() && action != eActions.Stand)
            {
                action = interactions.PlayerNext(hand.Player);
                if (action == eActions.Hit)
                {
                    hit(table, hand);
                }
            }
            if (action == eActions.Stand)
            {
                hand.Stand();
                table.Draw();
            }
            else
            {
                checkBust(table, hand);
            }
        }

        private void hit(Table table, PlayerHand hand)
        {
            Deal(table.Shoe, hand);
            table.Draw();
        }

        private void doubleDown(Table table, PlayerHand hand)
        {
            //double down gets a single card only
            var doubleBet = Math.Min(hand.Wager, hand.Player.BettableChips);
            Console.WriteLine($"Doubling down for ${doubleBet}");
            hand.Player.Bet(hand, doubleBet);
            dealPause();

            Deal(table.Shoe, hand);
            table.Draw();
            if (hand.IsBusted())
            {
                bust(table, hand);
            }
            else
            {
                hand.SetAsDoubled();
                table.Draw();
            }
        }

        private void split(Table table, PlayerHand hand)
        {
            var newHand = hand.Split();
            hand.Player.AddSplitHand(newHand);
            table.Draw();
            Deal(table.Shoe, hand);
            table.Draw();
            Deal(table.Shoe, newHand);
            table.Draw();
            Play(table, hand);
            Play(table, newHand);
        }

        private eResults calculateOutcome(PlayerHand playerHand, DealerHand dealerHand)
        {
            eResults o;

            var player = playerHand.FinalValue();
            var dealer = dealerHand.FinalValue();

            if (dealerHand.IsBlackjack())
            {
                o = playerHand.IsBlackjack() ? eResults.Push : eResults.Lose;
            }            
            else if (dealerHand.IsBusted())
            {
                o = eResults.Win;
            }
            else if (player > dealer)
            {
                o = eResults.Win;
            }
            else if (player < dealer)
            {
                o = eResults.Lose;
            }
            else
            {
                o = eResults.Push;
            }

            return o;
        }

        private void checkBust(Table table, PlayerHand hand)
        {
            if (hand.IsBusted())
            {
                bust(table, hand);
            }
        }

        private void bust(Table table, PlayerHand hand)
        {
            hand.Lose();
            table.Win(hand.Wager);
            Sweep(hand, table.Tray);
            postHandPause();
            table.Draw();

            if (!table.HasActivePlayers)
            {
                Hand.IsDealersTurn(); //enables dealer to expose hole card
                table.Draw();
                postHandPause();
                Sweep(Hand, table.Tray); //sweep own hand
                reset();
                table.Draw();
                postHandPause();
            }
        }

        private void payBlackjack(Table table, PlayerHand h)
        {
            table.Pay(h.BlackjackPays);
            h.Blackjack();
            postHandPause();
            Sweep(h, table.Tray);
            table.Draw();
        }

        private void depart(Table table, List<Player> leavingPlayers)
        {
            leavingPlayers.ForEach(p =>
            {
                Console.WriteLine($"{p.Name} has left the table.");
                Console.WriteLine($"{p.Name} stats: {p.ToStatsString()}");
                table.Leave(p);
            });
            if (leavingPlayers.Any())
            {
                Console.Write("Press any key to continue... ");
                Console.ReadKey(true);
            }
        }

        private void showCurrentHand(PlayerHand hand, string name)
        {
            Console.WriteLine($"\nCurrent hand for {name}");
            hand.Draw();
            Console.WriteLine();
        }

        private void reset() => Hand = new DealerHand();
        private void dealPause() => Thread.Sleep(delay);
        private void intermediatePause() => Thread.Sleep(delay * 2);
        private void postHandPause() => Thread.Sleep(delay * 4);
        private void extendedPause() => Thread.Sleep(delay * 8);
    }
}