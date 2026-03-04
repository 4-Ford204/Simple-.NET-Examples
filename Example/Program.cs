using Example.Algorithms;
using Example.Learning;
using System;
using System.Collections.Generic;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var random = new Random();
            var number = 0;
            var seen = new HashSet<int>();

            for (int i = 0; i < 50; i++)
            {
                while (seen.Contains(number))
                {
                    number = random.Next(1, 100);
                }
                seen.Add(number);
                Console.WriteLine("Number: {0}, Bits: {1}", Convert.ToString(number, 2), BitManipulation.CountBits(number));
            }
        }
    }
}
