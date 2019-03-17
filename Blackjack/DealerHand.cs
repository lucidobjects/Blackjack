using System;
using System.Collections.Generic;

namespace Blackjack
{
    public class DealerHand : Hand
    {        
        public bool TurnPending { get; private set; } = true;
        public bool HasUpcard => FirstCard != null;

        public void IsDealersTurn() => TurnPending = false;
        public Card UpCard() => FirstCard;        
        public bool HasEligibleUpcard() => HasBeenDealt && (UpCard()?.Type == eCardType.Ace || UpCard()?.Value1 == 10);

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