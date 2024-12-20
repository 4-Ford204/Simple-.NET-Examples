namespace Example.Learning
{
    public class Indexer
    {
        public static int Length { get; set; } = 'z' - 'a' + 1;
        private char[] Chars { get; set; } = new char[Length];

        public Indexer()
        {
            for (int i = 0; i < Length; i++) Chars[i] = (char)('a' + i);
        }

        public char this[int index]
        {
            get => (index < 0 || index >= Length) ? ' ' : Chars[index];

            set
            {
                if (index >= 0 && index < Length) Chars[index] = value;
            }
        }

        public int this[char value]
        {
            get
            {
                int index = Length;

                while (--index > 0)
                    if (Chars[index] == value) return index;

                return index;
            }
        }
    }
}
