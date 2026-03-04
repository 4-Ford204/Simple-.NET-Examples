namespace Example.Algorithms
{
    public static class BitManipulation
    {
        public static int CountBits(int number)
        {
            int result = 0;

            while (number > 0)
            {
                number &= (number - 1);
                result++;
            }

            return result;
        }

        public static int SetBit(int number, int position)
        {
            return number | (1 << position);
        }

        public static int UnsetBit(int number, int position)
        {
            return number & ~(1 << position);
        }
    }
}
