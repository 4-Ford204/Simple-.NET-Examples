using System;

namespace Example.Learning
{
    public class Operator()
    {
    }

    public class BinaryOperator : Operator
    {
        public static void Bitwise()
        {
            Random random = new Random();
            int first = random.Next(100);
            int second = random.Next(100);

            Console.WriteLine($"{first,3} = {Convert.ToString(first, 2).PadLeft(8, '0')}");
            Console.WriteLine($"{second,3} = {Convert.ToString(second, 2).PadLeft(8, '0')}");
            Console.WriteLine();

            int AND = first & second;
            Console.WriteLine("AND Operator -> &");
            Console.WriteLine($"{AND,3} = {Convert.ToString(AND, 2).PadLeft(8, '0')}");
            Console.WriteLine();

            int OR = first | second;
            Console.WriteLine("OR Operator -> |");
            Console.WriteLine($"{OR,3} = {Convert.ToString(OR, 2).PadLeft(8, '0')}");
            Console.WriteLine();

            int XOR = first ^ second;
            Console.WriteLine("XOR Operator -> ^");
            Console.WriteLine($"{XOR,3} = {Convert.ToString(XOR, 2).PadLeft(8, '0')}");
            Console.WriteLine();

            int firstNOT = ~first;
            int secondNOT = ~second;
            Console.WriteLine("One's Complement Operator -> ~");
            Console.WriteLine($"{firstNOT,3} = {Convert.ToString(firstNOT, 2).PadLeft(8, '0')}");
            Console.WriteLine($"{secondNOT,3} = {Convert.ToString(secondNOT, 2).PadLeft(8, '0')}");
            Console.WriteLine();

            int shift = random.Next(1, 5);

            int firstSHL = first << shift;
            int secondSHL = second << shift;
            Console.WriteLine($"Left Shift Operator -> << {shift}");
            Console.WriteLine($"{firstSHL,3} = {Convert.ToString(firstSHL, 2).PadLeft(8, '0')}");
            Console.WriteLine($"{secondSHL,3} = {Convert.ToString(secondSHL, 2).PadLeft(8, '0')}");
            Console.WriteLine();

            int firstSHR = first >> shift;
            int secondSHR = second >> shift;
            Console.WriteLine($"Right Shift Operator -> >> {shift}");
            Console.WriteLine($"{firstSHR,3} = {Convert.ToString(firstSHR, 2).PadLeft(8, '0')}");
            Console.WriteLine($"{secondSHR,3} = {Convert.ToString(secondSHR, 2).PadLeft(8, '0')}");
            Console.WriteLine();
        }
    }
}
