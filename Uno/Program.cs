using System;

namespace Uno
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select the number of players [2 - 8]: ");
            var game = new Game(int.Parse(Console.ReadLine() ?? throw new Exception()));
        }
    }
}