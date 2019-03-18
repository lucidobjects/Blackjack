using System;
using System.Threading;

namespace Blackjack
{
    public class Casino
    {
        public Dealer Dealer { get; private set; }
        public Table Table { get; private set; }

        public Casino()
        {
        }

        //Test with a table that has been constructed externally
        //(The argument could be made that this is a better architecture overall)
        public Casino(Table table, Dealer dealer)
        {
            Table = table;
            Dealer = dealer;
        }

        public void Open()
        {
            Dealer = new Dealer();
            Table = new Table(Dealer, new Shoe(Table.NumberOfDecksInShoe), new DiscardTray());
        }

        public void WelcomePlayers()
        {
            var interactions = new Interactions();
            invitePlayer(interactions);

            while (Table.HasOpenSeats && interactions.MorePlayers())
            {
                invitePlayer(interactions);
            }
            if (!Table.HasOpenSeats)
            {
                Console.WriteLine("Table is full. Let's play!");
                Thread.Sleep(2500);
            }
        }
       
        public void Operate()
        {
            while (Table.HasPlayers)
            {                
                var shoe = Table.Shoe;

                Table.Draw();

                if (Table.GameHasStarted)
                {
                    Dealer.BootBrokePlayers(Table);
                    Table.Draw();
                }

                Dealer.PlaceYourBets(Table);

                if (Table.HasPlayers) //all players may have gone broke or bet $0.
                {
                    if (!shoe.SuppressShuffle && (!Table.GameHasStarted || shoe.IsEmpty))
                    {
                        shuffle(shoe);
                    }

                    Dealer.DealInitial(Table);

                    if (Dealer.MayHaveBlackjack())
                    {
                        if (Dealer.HasBlackjack())
                        {
                            Dealer.Hand.IsDealersTurn(); //flip hole card
                            Table.Draw();
                            Dealer.FinishAllHands(Table);
                        }
                        else
                        {
                            Table.Draw();
                            Dealer.Play(Table); //play the players' hands
                        }
                    }
                    else
                    {
                        Dealer.Play(Table); //play the players' hands                    
                    }

                    if (Table.HasActivePlayers)
                    {
                        Dealer.Play(Table, Dealer.Hand); //dealer plays own hand
                        Dealer.FinishAllHands(Table);
                    }
                    else
                    {
                        Dealer.Sweep(Dealer.Hand, Table.Tray);
                    }
                }              
            }
        }

        public void Close()
        {
            Console.WriteLine("Thanks for playing.\nThe casino is closed.");
            Thread.Sleep(4000);
        }

        private void invitePlayer(Interactions interactions)
        {
            Table.Draw();
            Table.Sit(interactions.WelcomePlayer());
            Table.Draw();
        }

        private void shuffle(Shoe shoe)
        {
            Console.WriteLine("Shuffling...");
            Thread.Sleep(3000);
            Dealer.Shuffle(shoe, Table.Tray);
            shoe.Slice(Dealer.Slice());
            Table.StartGame();
            Dealer.Burn(shoe.Next(), Table.Tray);
        }
    }
}