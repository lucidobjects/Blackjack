using System.Collections.Generic;
using System.Linq;

namespace Blackjack
{
    public class Tests
    {
        public Casino Casino { get; private set; }

        public void Run()
        {
            //standard operation
            //var shoe = new Shoe(6);

            //soft hand test - deal all aces
            var shoe = getShoeAces();

            //push / split test - deal all 10's.
            //var shoe = getShoeTens();

            //blackjack test - deal blackjacks to 2 players
            //var shoe = getShoeBlackjackTwoPlayers();

            //give the dealer blackjack against one player
            //var shoe = getShoeDealerBlackjackOnePlayer();

            shoe.SkipShuffle();

            var dealer = new Dealer();
            var table = new Table(dealer, shoe, new DiscardTray());
            table.Sit(new Player("Pat", 500));
            table.Sit(new Player("Mel", 500));
            var casino = new Casino(table, dealer);
            casino.Operate();
        }

        private Shoe getShoeAces() => getShoeSingleType(eCardType.Ace);

        private Shoe getShoeTens() => getShoeSingleType(eCardType.Ten);

        private Shoe getShoeSingleType(eCardType type)
        {
            var shoe = new Shoe(6);
            var cards = shoe.Filter(type);
            shoe.Reload(cards.ToList());
            return shoe;
        }

        private Shoe getShoeDealerBlackjackOnePlayer()
        {
            var shoe = new Shoe(6);
            var aces = shoe.Filter(eCardType.Ace);
            var tens = shoe.Filter(eCardType.Ten);
            var sixes = shoe.Filter(eCardType.Six);
            var i = 0;
            var stacked = new List<Card>();
            stacked.Add(sixes.Skip(i).First());
            stacked.Add(aces.Skip(i).First());

            i++;
            stacked.Add(sixes.Skip(i).First());
            stacked.Add(tens.Skip(i).First());
            shoe.Reload(stacked);
            return shoe;
        }

        private Shoe getShoeBlackjackTwoPlayers()
        {
            var shoe = new Shoe(6);
            var aces = shoe.Filter(eCardType.Ace);
            var tens = shoe.Filter(eCardType.Ten);
            var sixes = shoe.Filter(eCardType.Six);
            var numBlackjacks = 5;
            var stacked = new List<Card>();
            for(var i = 0; i < numBlackjacks; i++)
            {
                stack(aces, tens, sixes, i, stacked);
            }
            shoe.Reload(stacked);
            return shoe;
        }

        private void stack(List<Card> aces, List<Card> tens, List<Card> sixes, int i, List<Card> stacked)
        {
            stacked.Add(aces.Skip(i).First());
            stacked.Add(tens.Skip(i).First());
            stacked.Add(sixes.Skip(i).First());
            i++;
            stacked.Add(tens.Skip(i).First());
            stacked.Add(aces.Skip(i).First());
            stacked.Add(sixes.Skip(i).First());
        }
    }
}