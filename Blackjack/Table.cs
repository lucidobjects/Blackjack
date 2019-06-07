using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack
{
    public class Table
    {
        //table rules and limits
        public static readonly int MinBet = 5;
        public static readonly int MaxBet = 500;
        public static readonly int NumberOfSeats = 7;
        public static readonly int NumberOfDecksInShoe = 6;                
        public static readonly int DealerHardStandValue = 17;
        public static readonly int DealerSoftStandValue = 18;
        public static readonly int MaxSplitHands = 4;        
        public static readonly decimal BlackjackPayout = 1.5M;
        //future
        //public static readonly bool SingleCardOnSplitAces = true;
        //public static readonly decimal InsurancePayout = 2M;

        public Dealer Dealer { get; private set; }
        public Shoe Shoe { get; private set; }
        public DiscardTray Tray { get; private set; }
        public List<Player> Players { get; private set; } = new List<Player>();
        public List<Player> BrokePlayers => Players.Where(p => p.Chips == 0).ToList();
        public List<Player> ActivePlayers => Players.Where(p => p.IsStillIn).ToList();        
        public List<PlayerHand> ActiveHands => ActivePlayers.SelectMany(p => p.ActiveHands).ToList();
        
        public decimal Rack { get; private set; }
        public bool GameHasStarted { get; private set; }
        public bool HasOpenSeats => Players.Count < NumberOfSeats;
        public bool HasPlayers => Players.Any();
        public bool HasActivePlayers => ActivePlayers.Any();

        public Table(Dealer dealer, Shoe shoe, DiscardTray tray)
        {
            Dealer = dealer;
            Shoe = shoe;
            Tray = tray;
        }

        public void Sit(Player player) => Players.Add(player);

        public void StartGame() => GameHasStarted = true;

        public void Win(decimal chips) => Rack += chips;

        public void Pay(decimal chips) => Rack -= chips;

        public void Leave(Player player) => Players.Remove(player);

        public List<Player> PlayersWithBlackJack() => Players.Where(p => p.Hand.IsBlackjack()).ToList();

        public void Draw()
        {
            Console.Clear();
            Console.WriteLine($"Blackjack ${MinBet} to ${MaxBet}");
            Dealer.Draw();
            Console.WriteLine("\n");            
            Players.ForEach(p => p.Draw());
            Console.WriteLine($"{new string('-', 60)}");            
        }
    }
}