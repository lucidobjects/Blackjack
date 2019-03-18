using System;
using System.Linq;

namespace Blackjack
{
    public class Interactions
    {
        private enum eAnswer
        {
            None, Yes, No
        }

        public bool YesNo(string message)
        {
            var answer = eAnswer.None;

            while(answer == eAnswer.None)
            {
                Console.Write(message);
                var c = Console.ReadKey().KeyChar;
                if ("yY".Contains(c))
                {
                    answer = eAnswer.Yes;
                }
                else if ("nN".Contains(c))
                {
                    answer = eAnswer.No;
                }
                Console.WriteLine();
            }

            return answer == eAnswer.Yes;
        }

        public Player WelcomePlayer()
        {
            Console.Write("Welcome to the casino, what's your name? ");
            var name = Console.ReadLine();
            Console.Write($"Hello {name}, what is your bankroll today? ");
            var bankroll = int.Parse(Console.ReadLine());
            return new Player(name, bankroll);
        }

        public bool MorePlayers() => YesNo("Are more players joining us today? ");

        public int GetBet(Player p)
        {
            var bet = 0;
            var betIsValid = false;

            while (!betIsValid)
            {
                Console.Write($"{p.Name} what's your bet? ");
                try
                {
                    var input = Console.ReadLine();
                    try
                    {
                        bet = int.Parse(input);
                    }
                    catch
                    {
                        throw new ArgumentOutOfRangeException("bet", $"Please enter a whole dollar amount.");
                    }

                    if (bet == 0)
                    {
                        betIsValid = true;
                    }
                    else if (bet > p.Chips)
                    {
                        throw new ArgumentOutOfRangeException("bet", $"Please bet within your bankroll of {p.Chips}");
                    }
                    else if (bet < Table.MinBet || bet > Table.MaxBet)
                    {
                        throw new ArgumentOutOfRangeException("bet", $"Please bet within table limits: ${Table.MinBet} - ${Table.MaxBet}.");
                    }
                    else
                    {
                        betIsValid = true;
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine(e.Message.Split('\n').First());
                }
            }

            return bet;
        }

        public bool WantsToSplit(string name)=> YesNo($"{name}, would you like to split? ");        

        public eActions PlayerFirst(Player player)
        {
            var action = eActions.None;

            if (player.HasBettableChips)
            {
                Console.Write($"{player.Name} what's your play? hit, stand, or double  ");
                while (action == eActions.None)
                {
                    action = toInitialAction(Console.ReadKey().KeyChar);
                }
            }
            else
            {
                //if they have no bettable chips, suppress "double" option
                action = PlayerNext(player);
            }
  
            return action;
        }

        public eActions PlayerNext(Player player)
        {
            var action = eActions.None;
            Console.Write($"{player.Name} what's your play? hit or stand ");
            while(action == eActions.None)
            {
                action = toNextAction(Console.ReadKey().KeyChar);
            }

            return action;
        }

        private eActions toInitialAction(char c)
        {
            var action = eActions.None;

            //check lower and upper case characters
            if ("dD".Contains(c))
            {
                action = eActions.Double;
            }
            else
            {
                action = toNextAction(c);
            }
            return action;
        }

        private eActions toNextAction(char c)
        {            
            var action = eActions.None;

            //check lower and upper case characters
            if ("sS".Contains(c))
            {
                action = eActions.Stand;
            }
            else if ("hH".Contains(c))
            {
                action = eActions.Hit;
            }

            return action;
        }
    }
}