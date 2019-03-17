using System;
using System.Text;

//inspired by: 
//https://codereview.stackexchange.com/questions/214390/my-blackjack-game-in-c-console
namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            //to enter or leave test mode, toggle the commenting on these two lines
            var runTests = false;
            //var runTests = true;

            if (!runTests)
            {
                //normal operation
                var casino = new Casino();
                casino.Open();
                casino.WelcomePlayers();
                casino.Operate();
                casino.Close();
            }
            else
            {
                //test mode
                new Tests().Run();
            } 
        }
    }
}