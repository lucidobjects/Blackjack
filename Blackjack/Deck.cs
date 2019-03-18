using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    public class Deck : ICardContainer
    {
        private List<Suit> _suits;
        public List<Suit> Suits
        {
            get
            {
                _suits = _suits ?? getSuits();
                return _suits;
            }
        }

        private List<Card> _cards;
        public List<Card> Cards
        {
            get
            {
                _cards = _cards ?? initCards(Suits);
                return _cards;
            }
        }

        public Suit Suit(eSuits suit) => Suits.Where(s => s.Type == suit).Single();

        private List<Suit> getSuits() => 
            new List<Suit>
            {
                new Suit(eSuits.Spades, '♠', ConsoleColor.Black),
                new Suit(eSuits.Hearts, '♥', ConsoleColor.Red),
                new Suit(eSuits.Diamonds, '♦', ConsoleColor.Red),
                new Suit(eSuits.Clubs, '♣', ConsoleColor.Black)
            };

        private List<Card> initCards(List<Suit> suits)
        {
            var cards = new List<Card>();

            suits.ForEach(s =>
            {
                cards.Add(new Card(eCardType.Ace, s, "A", 1, 11));
                cards.Add(new Card(eCardType.Two, s, "2", 2));
                cards.Add(new Card(eCardType.Three, s, "3", 3));
                cards.Add(new Card(eCardType.Four, s, "4", 4));
                cards.Add(new Card(eCardType.Five, s, "5", 5));
                cards.Add(new Card(eCardType.Six, s, "6", 6));
                cards.Add(new Card(eCardType.Seven, s, "7",  7));
                cards.Add(new Card(eCardType.Eight, s, "8", 8));
                cards.Add(new Card(eCardType.Nine, s, "9", 9));
                cards.Add(new Card(eCardType.Ten, s, "10", 10));
                cards.Add(new Card(eCardType.Jack, s, "J", 10));
                cards.Add(new Card(eCardType.Queen, s, "Q", 10));
                cards.Add(new Card(eCardType.King, s, "K", 10));
            });

            return cards;
        }
    }
}