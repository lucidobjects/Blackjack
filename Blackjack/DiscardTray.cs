using System.Collections.Generic;
using System.Linq;

namespace Blackjack
{
    public class DiscardTray : ICardContainer
    {
        public List<Card> Cards { get; private set; } = new List<Card>();

        public void Add(Card card) => Cards.Add(card);

        public void Add(List<Card> cards) => Cards.AddRange(cards);

        public List<Card> Empty()
        {
            var returnCards = Cards.Select(c=>c).ToList();
            Cards.Clear();
            return returnCards;
        }
    }
}
