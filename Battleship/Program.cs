using System;
using Battleship.Implementations;

namespace Battleship
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(new GameFieldBuilder().GenerateRandomField(random));
                Console.WriteLine();
                Console.WriteLine("-------------");
                Console.WriteLine();
            }
        }
    }
}
