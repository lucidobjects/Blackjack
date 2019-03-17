namespace Blackjack
{
    public static class Extensions
    {
        public static void Draw(this ICardContainer container) =>
            container.Cards.ForEach(c => c.Draw());
    }
}
