using System;
using System.Threading.Tasks;

namespace Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Lesson lesson = new Lesson();
            await lesson.AsyncExample();
            Console.ReadLine();
        }
    }
}
