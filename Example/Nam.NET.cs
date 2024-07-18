using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Example
{
    public class Lesson
    {
        #region Phần 13 - Static 

        private int _nonStatic = 0;
        // Biến static chỉ được khởi tạo một lần duy nhất, gắn liền với class chứ không phải với instance
        // Biến static được gọi thông qua tên của class chứ không phải thông qua instance
        // Biến static được khởi tạo khi class được load vào bộ nhớ, và tồn tại cho đến khi chương trình kết thúc
        private static int _static = 0;

        public void NonStaticExample()
        {
            Console.WriteLine("Static variable: " + _static);
            Console.WriteLine("Non static variable: " + _nonStatic);
        }

        public static void StaticExample()
        {
            Console.WriteLine("Static variable: " + _static);
            // Không thể truy cập biến non-static từ method static
            // Console.WriteLine("Non static variable: " + _nonStatic);
        }

        #endregion

        #region Phần 7 - Stream và File

        private string _directoryTree = string.Empty;

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

        private void DirectoryTreeRecursion(DirectoryInfo dir, int level)
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

        public void LamdaExample()
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

        private void InspectType(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                Console.WriteLine($"Type: {type.FullName}");
                InspectField(type);
                InspectMethod(type);
            }
        }

        private void InspectField(Type type)
        {
            foreach (var field in type.GetFields())
            {
                Console.WriteLine($"\tField:  {field.FieldType} - {field.Name}");
            }
        }

        private void InspectMethod(Type type)
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

        public void ThreadExample()
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

        #region Phần 32 - Semaphore 

        /*
         * Bài toán:
         * Có n máy chuyển hàng tự động có nhiệm vụ chuyển hàng từ kho vào xe tải.
         * Mỗi xe tải chỉ chứa được một số lượng hàng nhất định.
         * Nếu số lượng hàng cần chuyển để đầy xe tải nhỏ hơn n
         * và tất cả máy chuyển hàng đều chuyển hàng vào xe tải thì sẽ là lỗi.
         */

        private static Random random = new Random();
        private static int CurrentItemNumber = 0;
        private static int MaxItemNumber = 10;
        private static Semaphore semaphore = new Semaphore(MaxItemNumber, MaxItemNumber);
        private AutoResetEvent moveDoneEvent = new AutoResetEvent(false);

        public void SemaphoreExample()
        {
            for (int i = 1; i < 5; i++)
            {
                var thread = new Thread(new ParameterizedThreadStart(MoveItemThread)) { IsBackground = true };
                thread.Start(i.ToString());
            }

            new Thread(MoveDone) { IsBackground = true }.Start();
        }

        private void MoveItemThread(object i)
        {
            var threadNumber = i.ToString();

            while (true)
            {
                semaphore.WaitOne();

                Console.WriteLine($"{threadNumber} - Moving item ...");

                Thread.Sleep(random.Next(250, 500));
                MoveItem();
                Thread.Sleep(random.Next(2500, 5000));

                Console.WriteLine($"{threadNumber} - DONE ...\n");
            }
        }

        private void MoveItem()
        {
            Console.WriteLine($"Current item number: {++CurrentItemNumber}");

            if (CurrentItemNumber == MaxItemNumber)
                moveDoneEvent.Set();
        }

        private void MoveDone()
        {
            while (true)
            {
                moveDoneEvent.WaitOne();

                Console.WriteLine("Move full item, DONE ...\n");

                CurrentItemNumber = 0;
                semaphore.Release(MaxItemNumber);
            }
        }

        #endregion

        #region Phần 34 - Xử lý bất đồng bộ với hàm Async

        public async Task AsyncExample()
        {
            Sync();
            await AwaitButSync();
            await Async();
            await ArrayTaskAsync();
        }

        private void Sync()
        {
            var stopwatch = new Stopwatch();
            Console.WriteLine("Synchronous Start ...\n");
            stopwatch.Start();

            FirstTaskSync();
            SecondTaskSync();
            ThirdTaskSync();

            stopwatch.Stop();
            Console.WriteLine($"\nSynchronous Done in {stopwatch.Elapsed}\n");
        }

        private async Task AwaitButSync()
        {
            var stopwatch = new Stopwatch();
            Console.WriteLine("Await Synchronous Start ...\n");
            stopwatch.Start();

            await FirstTaskAsync();
            await SecondTaskAsync();
            await ThirdTaskAsync();

            stopwatch.Stop();
            Console.WriteLine($"\nAwait Synchronous Done in {stopwatch.Elapsed}\n");
        }

        private async Task Async()
        {
            var stopwatch = new Stopwatch();
            Console.WriteLine("Asynchronous Start ...\n");
            stopwatch.Start();

            var firstTask = FirstTaskAsync();
            var secondTask = SecondTaskAsync();
            var thirdTask = ThirdTaskAsync();

            await firstTask;
            await secondTask;
            await thirdTask;

            stopwatch.Stop();
            Console.WriteLine($"\nAsynchronous Done in {stopwatch.Elapsed}\n");
        }

        private async Task ArrayTaskAsync()
        {
            var stopwatch = new Stopwatch();
            Console.WriteLine("Array Task Asynchronous Start ...\n");
            stopwatch.Start();

            Task[] arrayTask = new Task[] { FirstTaskAsync(), SecondTaskAsync(), ThirdTaskAsync() };
            //Task.WaitAll(arrayTask);
            await Task.WhenAll(arrayTask);

            stopwatch.Stop();
            Console.WriteLine($"\nArray Task Asynchronous Done in {stopwatch.Elapsed}\n");
        }

        private void FirstTaskSync()
        {
            Console.WriteLine("First Task Sync Start ...");
            Task.Delay(1000).Wait();
            Console.WriteLine("First Task Sync Done ...");
        }

        private void SecondTaskSync()
        {
            Console.WriteLine("Second Task Sync Start ...");
            Task.Delay(2000).Wait();
            Console.WriteLine("Second Task Sync Done ...");
        }

        private void ThirdTaskSync()
        {
            Console.WriteLine("Third Task Sync Start ...");
            Task.Delay(3000).Wait();
            Console.WriteLine("Third Task Sync Done ...");
        }

        private async Task FirstTaskAsync()
        {
            Console.WriteLine("First Task Async Start ...");
            await Task.Delay(1000);
            Console.WriteLine("First Task Async Done ...");
        }

        private async Task SecondTaskAsync()
        {
            Console.WriteLine("Second Task Async Start ...");
            await Task.Delay(2000);
            Console.WriteLine("Second Task Async Done ...");
        }

        private async Task ThirdTaskAsync()
        {
            Console.WriteLine("Third Task Async Start ...");
            await Task.Delay(3000);
            Console.WriteLine("Third Task Async Done ...");
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
