using System.Collections.Generic;
using System.Linq;

namespace Blackjack
{
    abstract public class Hand
    {
        private readonly int aceLowValue = 1;
        private readonly int aceHighValue = 11;
        private readonly int maxHandValue = 21;
        
        private List<Card> hardCards => Cards.Where(c => c.IsSingleValue).ToList();
        private List<Card> aces => Cards.Where(c => c.IsAce).ToList();
        private int hardCardsSum => hardCards.Sum(c => c.Value1);

        protected string bustedToken => IsBusted() ?  " BUST" : string.Empty;

        public Card FirstCard => HasCards ? Cards.ElementAt(0) : null;
        public Card SecondCard => HasBeenDealt ? Cards.ElementAt(1) : null;
        public int HardValue => IsSoft ? SoftLow :  hardCardsSum;
        public int SoftLow => aceLowValue * aces.Count + hardCardsSum;
        //only 1 Ace can be soft. The rest count as 1.
        public int SoftHigh => aceHighValue + SoftLow - 1 ;

        public bool HasCards => Cards.Count > 0;
        public bool HasBeenDealt => Cards.Count > 1;
        public bool HasAces => aces.Any();
        public bool IsSoft => HasAces && SoftHigh <= maxHandValue;
   
        public List<Card> Cards { get; private set; } = new List<Card>();
        
        public void Deal(Card card)
        {
            Cards.Add(card);       
        }

        public bool IsBusted() => FinalValue() > 21;

        public bool IsBlackjack() => FirstCard?.IsAce == true && SecondCard?.HasValueOfTen == true ||
                                     FirstCard?.HasValueOfTen == true && SecondCard?.IsAce == true;

        public int FinalValue() => IsSoft ? SoftHigh : HasAces ? SoftLow : HardValue;

        public string ToValueString()
        {
            var s = string.Empty;
            if (IsBlackjack())
            {
                s = "Blackjack";
            }
            else
            {
                s = IsSoft ? $"{SoftLow} or {SoftHigh}" : FinalValue().ToString();
            }
            return s;
        }
        
        public override string ToString() => string.Join(" ", Cards.Select(c => c.ToString()));

        public List<Card> Sweep()
        {
            var cardToSweep = Cards.Select(c=>c).ToList();
            Cards.Clear();
            return cardToSweep;
        }
    }
}