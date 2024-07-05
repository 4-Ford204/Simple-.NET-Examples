using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Example
{
    public class Lesson
    {
        #region Phần 7 - Stream và File

        public string _directoryTree = string.Empty;

        public void GetDirectoryAndFile()
        {
            var path = $"C:\\";
            var dir = new DirectoryInfo(path);

            //var directories = Directory.GetDirectories(path);
            var directories = dir.GetDirectories();
            foreach (var directory in directories)
            {
                Console.WriteLine($"{directory.LastWriteTime:dd/MM/yyyy HH:mm}\t<DIR>\t\t{directory.Name}");
            }

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                Console.WriteLine($"{file.LastWriteTime:dd/MM/yyyy HH:mm}\t{file.Length,20:#,###} bytes\t\t{file.Name}");
            }
        }

        public void Copy()
        {
            var source = @"C:\{source}";
            var destination = @"C:\{destination}";

            var buffer = new byte[1024];
            using var inputStream = File.OpenRead(source);
            using var outputStream = File.OpenRead(destination);

            int n = inputStream.Read(buffer, 0, buffer.Length);
            while (n > 0)
            {
                Console.WriteLine(n.ToString());

                outputStream.Write(buffer, 0, n);

                n = inputStream.Read(buffer, 0, buffer.Length);
            }

            //inputStream.Close();
            //outputStream.Close();
        }

        public string GetDirectoryTree(string path)
        {
            _directoryTree = string.Empty;
            var dir = new DirectoryInfo(path);

            DirectoryTreeRecursion(dir, 0);

            return _directoryTree;
        }

        public void DirectoryTreeRecursion(DirectoryInfo dir, int level)
        {
            var directories = dir.GetDirectories();
            int tab = level;

            if (directories.Length == 0) { }
            else
            {
                foreach (var directory in directories)
                {
                    _directoryTree += string.Concat(new String('\t', tab), $"|-{directory.Name}\n");
                    DirectoryTreeRecursion(directory, tab + 1);
                }
            }

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                _directoryTree += string.Concat(new String('\t', tab), $"|-{file.Name}\n");
            }
        }

        #endregion

        #region Phần 9 - Biểu thức Lamda

        public void Lamda()
        {
            Func<int, int, int> sum = (a, b) => a + b;
            Action<string> print = (string message) => Console.WriteLine(message);
            var compare = object (int a, int b) => a > b ? true : "False";
        }

        #endregion

        #region Phần 29 - Reflection 

        /// <summary>
        /// Lấy thông tin của một Assembly
        /// </summary>
        /// <param name="fileName">Đường dẫn đến Assembly (file đuôi dll)</param>
        public Assembly? InspectAssembly(string fileName)
        {
            var assembly = Assembly.LoadFile(fileName);

            if (assembly != null)
            {
                Console.WriteLine($"Assembly: {fileName}\n");
                InspectType(assembly);
            }

            return assembly;
        }

        public void InspectType(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                Console.WriteLine($"Type: {type.FullName}");
                InspectField(type);
                InspectMethod(type);
            }
        }

        public void InspectField(Type type)
        {
            foreach (var field in type.GetFields())
            {
                Console.WriteLine($"\tField:  {field.FieldType} - {field.Name}");
            }
        }

        public void InspectMethod(Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (method.DeclaringType == type)
                {
                    Console.WriteLine($"\tMethod: {method.Name}");
                }
            }
        }

        public void LoadObjectFromAssembly(Assembly assembly, string typeName = "", string methodName = "")
        {
            var type = assembly.GetType(typeName);

            if (type != null)
            {
                // Ví dụ: NRedisStack
                var constructor = type.GetConstructor([]);
                var constructorParam = type.GetConstructor([typeof(int)]);
                var obj = constructor.Invoke([]);
                var objParam = constructorParam.Invoke([0]);

                if (objParam != null)
                {
                    var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, []);

                    if (method != null)
                    {
                        method.Invoke(objParam, []);
                    }
                }
            }
            else
            {
                Console.WriteLine("Constructor not found");
            }
        }

        #endregion

        #region Phần 30 - Thread

        public void Thread()
        {
            //var firstThread = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Console.WriteLine("Thread 1");
            //        System.Threading.Thread.Sleep(1000);
            //    }
            //});

            //var firstThread = new Thread(() => PrintThread(1));

            // Không tham số
            //var firstThread = new Thread(new ThreadStart(PrintThread));

            var firstThread = new Thread(new ParameterizedThreadStart(PrintThread));
            var secondThread = new Thread(new ParameterizedThreadStart(PrintThread));
            var thirdThread = new Thread(new ParameterizedThreadStart(PrintThread));

            CancellationTokenSource cts = new CancellationTokenSource();

            firstThread.Start(new ThreadParam() { Name = "1", Delay = 1000, CancellationToken = cts.Token });
            secondThread.Start(new ThreadParam() { Name = "2", Delay = 2500, CancellationToken = cts.Token });
            thirdThread.Start(new ThreadParam() { CancellationToken = cts.Token });

            // Background Thread - Khi chương trình chính kết thúc thì các thread cũng kết thúc
            // Foreground Thread - Khi chương trình chính kết thúc thì các thread vẫn chạy
            //firstThread.IsBackground = true;
            //secondThread.IsBackground = true;
            //thirdThread.IsBackground = true;

            Console.ReadLine();

            cts.Cancel();
        }

        private void PrintThread(object? i)
        {
            //var threadParam = (ThreadParam)i; //Nếu không cast được thì sẽ báo lỗi
            var threadParam = i as ThreadParam; //Nếu không cast được thì sẽ trả về null

            if (threadParam != null)
            {
                while (!threadParam.CancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine($"Thread {threadParam.Name ?? "No Name"}");
                    System.Threading.Thread.Sleep(threadParam.Delay ?? 5000);
                }
            }
        }

        #endregion
    }

    public class ThreadParam
    {
        public string? Name { get; set; }
        public int? Delay { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
