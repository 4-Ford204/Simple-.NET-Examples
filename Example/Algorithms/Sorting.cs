namespace Example.Algorithms
{
    public class Sorting
    {
        public int[] BubbleSort(int[] nums)
        {
            for (int i = 0; i < nums.Length; i++)
            {
                bool swap = false;

                for (int j = 0; j < nums.Length - i - 1; j++)
                {
                    if (nums[j] > nums[j + 1])
                    {
                        (nums[j + 1], nums[j]) = (nums[j], nums[j + 1]);
                        swap = true;
                    }
                }

                if (!swap) break;
            }

            return nums;
        }

        public int[] SelectionSort(int[] nums)
        {
            for (int i = 0; i < nums.Length - 1; i++)
            {
                int min = i;

                for (int j = i + 1; j < nums.Length; j++)
                {
                    if (nums[min] > nums[j]) min = j;
                }

                (nums[i], nums[min]) = (nums[min], nums[i]);
            }

            return nums;
        }
    }
}
