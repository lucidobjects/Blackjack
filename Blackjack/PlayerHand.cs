using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Blackjack
{
    public enum eActions
    {
        None, Hit, Stand, Double, Split
        //future: Insurance, Surrender
    }

    public enum eResults
    {
        Pending, Win, Lose, Push, Blackjack 
        //future: InsuranceWin, InsuranceLose
    }

    public class PlayerHand : Hand
    {        
        private string blackJackToken => IsBlackjack() ? " BLACKJACK!" : string.Empty;
        private string descriptor => IsBusted() ? "BUST" : IsDoubled ? "Doubled" : IsStood ? "Stood" : string.Empty;

        public Player Player { get; private set; }
        public decimal Wager { get; private set; }
        public decimal BlackjackPays => Math.Round(Wager * Table.BlackjackPayout,2);
        public eResults Outcome { get; private set; } = eResults.Pending;
        public int Id { get; private set; }

        public bool IsSplit { get; private set; }
        public bool IsStood { get; private set; }
        public bool IsDoubled { get; private set; }
        public bool HasWager => Wager > 0;
        public bool CanBeSplit => IsFirstTurn && Cards.ElementAt(0).Type == Cards.ElementAt(1).Type;
        public bool IsFirstTurn => Cards.Count == 2;
        public bool IsActive => Outcome == eResults.Pending && HasWager;

        public PlayerHand(Player player)
        {
            Player = player;
            Id = player.NextHandId();
        }

        public PlayerHand(Player player, decimal wager, Card firstCard)
        {
            Player = player;
            Wager = wager;
            Cards.Add(firstCard);
            Id = player.NextHandId();
        }

        public void Bet(decimal chips) => Wager += chips;

        public void SetAsDoubled()
        {
            IsDoubled = true;
            IsStood = true;
        }

        public void Stand() => IsStood = true;

        public PlayerHand Split()
        {
            var newHand = new PlayerHand(Player, Math.Min(Wager, Player.BettableChips), Cards.ElementAt(1));
            Cards.RemoveAt(1);
            IsSplit = true;
            return newHand;
        }

        public void Blackjack()
        {
            Outcome = eResults.Blackjack;
            Player.Blackjack(this);
        }

        public void Win()
        {
            Outcome = eResults.Win;
            Player.Win(this);
        }

        public void Lose()
        {
            Outcome = eResults.Lose;            
            Player.Lose(this);
        }

        public void Push()
        {
            Outcome = eResults.Push;
            Player.Push(this);
        }

        public void Draw()
        {
            if (!HasWager && !HasCards)
            {
                Console.WriteLine();
            }
            else
            {
                if (HasWager)
                {
                    Console.Write($"{Wager.ToString("C", CultureInfo.CurrentCulture)}\t");
                    if (!HasCards)
                    {
                        Console.WriteLine();
                    }
                }
                if (HasCards)
                {
                    Cards.ForEach(c => c.Draw(addSpace: true));
                    if (HasBeenDealt)
                    {
                        var s = IsBlackjack() ? blackJackToken : $"  ({ToValueString()})\t{descriptor}";
                        Console.WriteLine(s);
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}