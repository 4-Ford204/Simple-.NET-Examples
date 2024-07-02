using System;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var redis = new NRedisStack();
            redis.IndexAndQuery();
        }
    }
}
