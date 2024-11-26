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
                    if (Numbers[j - 1] > Numbers[j])
                        (Numbers[j - 1], Numbers[j]) = (Numbers[j], Numbers[j - 1]);
                    else
                        break;
                }
            }

            return Numbers;
        }

        [Benchmark]
        public int[] MergeSort()
        {
            SortingExtension.MergeSortExtension(Numbers, 0, Numbers.Length - 1);

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
        public static void MergeSortExtension(int[] numbers, int left, int right)
        {
            if (left < right)
            {
                int middle = left + (right - left) / 2;

                MergeSortExtension(numbers, left, middle);
                MergeSortExtension(numbers, middle + 1, right);

                int leftLength = middle - left + 1;
                int rightLength = right - middle;
                int[] leftNumbers = new int[leftLength];
                int[] rightNumbers = new int[rightLength];
                int i, j, k;

                for (i = 0; i < leftLength; i++)
                    leftNumbers[i] = numbers[left + i];
                for (j = 0; j < rightLength; j++)
                    rightNumbers[j] = numbers[middle + 1 + j];

                i = 0;
                j = 0;
                k = left;

                while (i < leftLength && j < rightLength)
                {
                    if (leftNumbers[i] < rightNumbers[j])
                        numbers[k++] = leftNumbers[i++];
                    else
                        numbers[k++] = rightNumbers[j++];
                }

                while (i < leftLength)
                    numbers[k++] = leftNumbers[i++];

                while (j < rightLength)
                    numbers[k++] = rightNumbers[j++];
            }
        }

        public static void QuickSortExtension(int[] numbers, int left, int right)
        {
            if (left < right)
            {
                int middle = new Func<int>(() =>
                {
                    int tracking = left - 1;

                    for (int i = left; i < right; i++)
                    {
                        if (numbers[i] < numbers[right])
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
