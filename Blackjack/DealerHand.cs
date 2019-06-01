using System;
using System.Collections.Generic;

namespace Blackjack
{
    public class DealerHand : Hand
    {
        private bool hasEligibleUpcard() => HasBeenDealt && (UpCard()?.Type == eCardType.Ace || UpCard()?.HasValueOfTen == true);
        public bool TurnPending { get; private set; } = true;
        public bool HasUpcard => FirstCard != null;

        public void IsDealersTurn() => TurnPending = false;
        public Card UpCard() => FirstCard;
        public bool IsPossibleBlackjack() => hasEligibleUpcard();
       
        public void Draw()
        {
            if (TurnPending)
            {
                if (HasBeenDealt)
                {
                    //draw downcard as hidden
                    SecondCard.DrawHidden(addSpace: true);
                }

                if (HasUpcard)
                {
                    UpCard().Draw();
                }
            }
            else if (HasCards)
            {
                Cards.ForEach(c => c.Draw(addSpace: true));
                Console.WriteLine($"  ({ToValueString()}){bustedToken}");
            }
        }
    }
}