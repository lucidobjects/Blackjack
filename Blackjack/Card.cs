using System;

namespace Blackjack
{
    public enum eCardType
    {
        Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
    }

    public class Card
    {
        private readonly string hiddenToken = "??";

        public eCardType Type { get; private set; }
        public int Value1 { get; private set; }
        public int Value2 { get; private set; }
        public Suit Suit { get; private set; }   
        public string Token { get; private set; }
        public bool IsSingleValue => Value2 == 0;
        public bool HasValueOfTen => Value1 == 10;
        public bool IsAce => Type.Equals(eCardType.Ace);
        
        public Card(eCardType type, Suit suit, string token, int value1, int value2 = 0)
        {
            Type = type;
            Suit = suit;
            Token = token;
            Value1 = value1;
            Value2 = value2;
        }

        public void Draw(bool addSpace = false)
        {
            Console.Write(Token);
            Suit.Draw();
            if (addSpace) { Console.Write(" "); }
        }

        public void DrawHidden(bool addSpace = false)
        {
            Console.Write(hiddenToken);
            if (addSpace) { Console.Write(" "); }
        }

        public override string ToString() => $"{Token}{Suit.ToString()}";
    }
}