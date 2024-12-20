using Example.Learning;
using System;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var indexer = new Indexer();

            for (int i = 0; i < Indexer.Length; i++)
                Console.Write($"{indexer[i]} ");

            Console.WriteLine();

            for (int i = 0; i < Indexer.Length; i++)
                Console.Write($"{indexer[(char)('a' + i)]} ");

            Console.ReadKey();
        }
    }
}
