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
    }
}
