using System;

namespace Blackjack
{
    public enum eSuits
    {
        Spades, Hearts, Diamonds, Clubs
    }

    public class Suit
    {
        public eSuits Type { get; private set; }
        public char Symbol { get; private set; }
        public ConsoleColor Color { get; private set; }

        public string Abbreviation => Type.ToString().Substring(0, 1);

        public Suit(eSuits type, char symbol, ConsoleColor color)
        {
            Type = type;
            Symbol = symbol;
            Color = color;
        }

        public void Draw()
        {
            Console.ForegroundColor = Color;
            Console.Write(Symbol);
            Console.ResetColor();
        }

        public override string ToString() => Abbreviation;
    }
}