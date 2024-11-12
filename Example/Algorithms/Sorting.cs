using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace Example.Algorithms
{
    [SimpleJob(baseline: true)]
    [MemoryDiagnoser]
    public class Sorting
    {
        public int[] Numbers { get; set; }

        public Sorting()
        {
            Numbers = new int[1000];
            Random random = new Random();

            for (int i = 0; i < 1000; i++) Numbers[i] = random.Next();
        }

        public Sorting(int[] numbers)
        {
            Numbers = numbers;
        }

        [Benchmark]
        public int[] BubbleSort()
        {
            for (int i = 0; i < Numbers.Length; i++)
            {
                bool swap = false;

                for (int j = 0; j < Numbers.Length - i - 1; j++)
                {
                    if (Numbers[j] > Numbers[j + 1])
                    {
                        (Numbers[j], Numbers[j + 1]) = (Numbers[j + 1], Numbers[j]);
                        swap = true;
                    }
                }

                if (!swap) break;
            }

            return Numbers;
        }

        [Benchmark]
        public int[] InsertionSort()
        {
            for (int i = 1; i < Numbers.Length; i++)
            {
                for (int j = i; j > 0; j--)
                {
                    if (Numbers[j] < Numbers[j - 1])
                        (Numbers[j - 1], Numbers[j]) = (Numbers[j], Numbers[j - 1]);
                    else
                        break;
                }
            }

            return Numbers;
        }

        [Benchmark]
        public int[] QuickSort()
        {
            SortingExtension.QuickSortExtension(Numbers, 0, Numbers.Length - 1);

            return Numbers;
        }

        [Benchmark]
        public int[] SelectionSort()
        {
            for (int i = 0; i < Numbers.Length - 1; i++)
            {
                int min = i;

                for (int j = i + 1; j < Numbers.Length; j++)
                {
                    if (Numbers[min] > Numbers[j]) min = j;
                }

                (Numbers[i], Numbers[min]) = (Numbers[min], Numbers[i]);
            }

            return Numbers;
        }
    }

    public static class SortingExtension
    {
        public static void QuickSortExtension(int[] numbers, int left, int right)
        {
            if (left < right)
            {
                int middle = new Func<int>(() =>
                {
                    int tracking = left - 1;

                    for (int i = left; i < right; i++)
                    {
                        if (numbers[right] > numbers[i])
                        {
                            tracking++;
                            (numbers[i], numbers[tracking]) = (numbers[tracking], numbers[i]);
                        }
                    }

                    (numbers[right], numbers[++tracking]) = (numbers[tracking], numbers[right]);
                    return tracking;
                })();

                QuickSortExtension(numbers, left, middle - 1);
                QuickSortExtension(numbers, middle + 1, right);
            }
        }

        public static void RunBenchmark() => BenchmarkRunner.Run<Sorting>();
    }
}
