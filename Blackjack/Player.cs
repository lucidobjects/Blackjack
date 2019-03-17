using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Blackjack
{
    public class Player
    {
        public string Name { get; private set; }

        private PlayerHand _hand;
        public PlayerHand Hand
        {
            get
            {
                _hand = _hand ?? new PlayerHand(this);
                return _hand;
            }
        }

        public List<PlayerHand> SplitHands { get; private set; }
        public List<PlayerHand> ActiveHands => AllHands.Where(h => h.IsActive).ToList();
        public List<PlayerHand> AllHands
        {
            get
            {
                var all = new List<PlayerHand>();
                if (Hand != null)
                {
                    all.Add(Hand);
                }
                if (HasSplit)
                {
                    all.AddRange(SplitHands);
                }
                return all;
            }
        }

        public decimal Chips { get; private set; }
        public decimal BettableChips => Chips - AllHands.Sum(h => h.Wager);
        public int TotalHandCount => WinCount + LossCount + PushCount;
        public int WinCount { get; private set; }
        public int LossCount { get; private set; }
        public int PushCount { get; private set; }
        public int BlackjackCount { get; private set; }
        public bool HasBettableChips => BettableChips > 0;
        public bool HasMultipleHands => ActiveHands.Count > 1;
        public bool CanSplit => HasBettableChips && (SplitHands == null || SplitHands.Count() + 1 < Table.MaxSplitHands);
        public bool HasSplit => SplitHands != null;
        public bool IsStillIn => ActiveHands.Any();

        public Player(string name, int bankroll)
        {
            Name = name;
            Chips = bankroll;
        }

        public void InitialBet(decimal chips) => Hand.Bet(chips);

        public void Bet(PlayerHand hand, decimal chips) => hand.Bet(chips);

        public void AddSplitHand(PlayerHand hand)
        {
            SplitHands = SplitHands ?? new List<PlayerHand>();
            SplitHands.Insert(0, hand);            
        }

        public void Win(PlayerHand hand)
        {
            Chips += hand.Wager;
            WinCount++;
            eliminate(hand);
        }

        public void Lose(PlayerHand hand)
        {
            Chips -= hand.Wager;
            LossCount++;
            eliminate(hand);
        }

        public void Push(PlayerHand hand)
        {
            PushCount++;
            eliminate(hand);
        }

        public void Blackjack(PlayerHand hand)
        {
            Console.WriteLine($"{Name} you got Blackjack!");
            Chips += hand.BlackjackPays;
            BlackjackCount++;
            WinCount++;
            eliminate(hand);
        }

        public int NextHandId() => SplitHands?.Count ?? 0;

        public string ToStatsString() => 
            $"Hands: {TotalHandCount}\tWon: {WinCount}\tLost: {LossCount}\tPush: {PushCount}\tBlackjack: {BlackjackCount}";

        public void Draw()
        {
            Console.WriteLine($"{Name}\t{BettableChips.ToString("C", CultureInfo.CurrentCulture)}");
            ActiveHands.ForEach(h=> 
            {
                h.Draw();
            });
            Console.WriteLine();
        }

        private void eliminate(PlayerHand hand)
        {
            if (hand.Id == 0)
            {
                _hand = null;
            }
            else
            {
                SplitHands?.Remove(hand);
            }
        }
    }
}
