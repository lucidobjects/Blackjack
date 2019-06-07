using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack
{
    public class Shoe : ICardContainer
    {
        private readonly int minSize = 2;
        private readonly int maxSize = 8;
        private readonly int deckCount = 52;
        private int topCard = 0;
        private int cutCard;

        public int Size { get; private set; }
        public int StartingCount { get; private set; }
        public bool IsEmpty => Remaining <= cutCard;
        public int Remaining => Cards.Count;
        public bool SuppressShuffle { get; private set; }

        private List<Deck> _decks;
        private List<Deck> decks
        {
            get
            {
                _decks = _decks ?? getDecks();
                return _decks;
            }
        }

        private List<Card> _cards;
        public List<Card> Cards
        {
            get
            {
                _cards = _cards ?? getCards();
                return _cards;
            }
        }

        public Shoe(int size)
        {
            if (size < minSize || size > maxSize)
            {
                throw new Exception($"Please enter a number of decks for the shoe between {minSize} and {maxSize}");
            }
            else
            {
                Size = size;
                StartingCount = Size * deckCount;
            }
        }
        
        public Card Next()
        {
            try
            {
                var card = Cards.ElementAt(topCard);
                Cards.RemoveAt(topCard);
                return card;
            }
            catch
            {
                throw new Exception("Shoe is empty.");
            }
        }

        public void Reload(List<Card> cards) => _cards = cards;

        //Playing until the end of a shoe can give card counters a big advantage
        //so the dealer places the cut card near the back of the stack of cards 
        //to stop play before the shoe is empty.
        //Typical casino protocol is to slice off around 2 decks from the back.
        public void Slice(int sliceLocation) => cutCard = sliceLocation;

        public List<Card> Top(int top) => Cards.Take(top).ToList();

        public List<Card> Filter(eCardType type) => Cards.Where(c => c.Type == type).ToList();

        public void SkipShuffle() => SuppressShuffle = true;

        public void Draw() => Draw(int.MaxValue);

        public void Draw(int num) => Top(num).ForEach(c => c.Draw());

        private List<Deck> getDecks() => 
            Enumerable.Range(1, Size).Select(i => new Deck()).ToList();

        private List<Card> getCards() => decks.SelectMany(d => d.Cards).ToList();
    }
}
