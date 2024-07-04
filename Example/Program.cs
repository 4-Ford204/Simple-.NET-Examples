using System;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var lesson = new Lesson();
            var assembly = lesson.InspectAssembly("D:\\Visual Studio 2022\\C#\\Simple .NET Examples\\Example\\bin\\Debug\\net8.0\\Example.dll");
            lesson.LoadObjectFromAssembly(assembly, "Example.NRedisStack", "Example");
        }
    }
}
