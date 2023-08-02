using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Data.Hopper;
using System.Data.Impeller;
using System.Data.NWpfUI.ViewModels;
using System.Data.SolutionCore;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.DabberUT
{
    [TestClass]
    public class FunctionUT
    {
        [TestMethod]
        public void RenameFiles()
        {
            var path = System.IO.Path.GetFullPath(@"F:\works\zex\nsolutionui\src\MaterialDesignWpfToolkit\Themes");
            var dir = new System.IO.DirectoryInfo(path);
            foreach (var item in dir.GetFiles())
            {
                var fileName = item.Name;
                if (fileName.StartsWith("MaterialDesignTheme"))
                {
                    item.MoveTo(System.IO.Path.Combine(path, fileName.Replace("MaterialDesignTheme", "Theme")));
                }
                if (fileName.EndsWith(" - 副本.xaml"))
                {
                    item.MoveTo(System.IO.Path.Combine(path, "NET40." + fileName.Replace(" - 副本", "")));
                }
            }
        }
        [TestMethod]
        public void ToEvenCalc()
        {
            // 单精度浮点数(Float)
            Console.WriteLine(Math.Round(18.254550f, 4, MidpointRounding.ToEven)); // 18.2546
            Console.WriteLine(Math.Round(18.745450f, 4, MidpointRounding.ToEven)); // 18.7454
            // 双精度浮点数(Double)
            Console.WriteLine(Math.Round(18.254550d, 4, MidpointRounding.ToEven)); // 18.2545
            Console.WriteLine(Math.Round(18.745450d, 4, MidpointRounding.ToEven)); // 18.7455
            // 高精度浮点数(Decimal)
            Console.WriteLine(Math.Round(18.254550m, 4, MidpointRounding.ToEven)); // 18.2546
            Console.WriteLine(Math.Round(18.745450m, 4, MidpointRounding.ToEven)); // 18.7454

            Console.WriteLine(string.Format("{0:f4}", 18.254550d));
            Console.WriteLine(string.Format("{0:f4}", 18.745450d));

            Console.WriteLine(string.Format("{0:f4}", 18.254550m));
            Console.WriteLine(string.Format("{0:f4}", 18.745450m));
        }
        [TestMethod]
        public void CreateGuidString()
        {
            Console.WriteLine(Guid.NewGuid());
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(Guid.NewGuid().GetString());
            }
        }
        [TestMethod]
        public void TestRandomNumber()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(new Random().Next(1, 100));
            }
        }
        /// <summary>
        /// 创建
        /// </summary>
        [TestMethod]
        public void CreateGuid()
        {
            var times = 10;
            for (int i = 0; i < times; i++)
            {
                Console.WriteLine(Guid.NewGuid().GetString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestHashSet()
        {
            var hashSet = new HashSet<Tuble<int, String>>();
            for (int i = 0; i < 100; i++)
            {
                hashSet.Add(new Tuble<int, String>(i, UserPassword.GetMd5Hash(Path.GetRandomFileName())));
                hashSet.Add(new Tuble<int, String>(i, ExtterCaller.GetRandomInt32().ToString("0000") + Path.GetRandomFileName()));
            }
            for (int i = 10; i < 20; i++)
            {
                var item = hashSet.First(s => i == s.Item1);
                hashSet.Add(item);
            }
            foreach (var item in hashSet)
            {
                Console.WriteLine($"{item.Item1:0000} => {item.Item2}");
            }
        }
        /// <summary>
        /// 测试Nano及Guid效率
        /// </summary>
        [TestMethod]
        public void TestGuidNanoID()
        {
            var times = 100;
            var now = DateTime.Now;
            Console.WriteLine(Guid.NewGuid().GetString());
            Console.WriteLine(Nanoid.Generate());
            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                _ = Guid.NewGuid().ToString("N");
            }
            Console.WriteLine($"Guid    =>   {DateTime.Now - now}");
            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                _ = Nanoid.Generate();
            }
            Console.WriteLine($"Nanoid  =>   {DateTime.Now - now}");
        }
        /// <summary>
        /// 测试时间区间
        /// </summary>
        [TestMethod]
        public void TestDateTimeOffset()
        {
            Console.WriteLine(DateTime.Now.GetDateTimeString());
            Console.WriteLine(DateTime.UtcNow.GetDateTimeString());
            Console.WriteLine(DateTimeOffset.Now.GetDateTimeString());
            Console.WriteLine(DateTimeOffset.UtcNow.GetDateTimeString());
            Console.WriteLine(new DateTimeOffset(DateTime.Now).GetDateTimeString());
            Console.WriteLine(new DateTimeOffset(DateTime.UtcNow).GetDateTimeString());
        }
        /// <summary>
        /// 获取空闲时间
        /// </summary>
        [TestMethod]
        public void TestGetIdleTime()
        {
            var inputInfo = new USER32.PLASTINPUTINFO();
            inputInfo.CbSize = Marshal.SizeOf(inputInfo);
            if (!USER32.GetLastInputInfo(ref inputInfo))
            {
                Console.WriteLine("获取失败");
            }
            var mili = TimeSpan.FromMilliseconds((long)Environment.TickCount - (long)inputInfo.DwTime);
            Console.WriteLine($"距离上次输入已经过去{mili}毫秒");
        }
        /// <summary>
        /// 测试超链接
        /// </summary>
        [TestMethod]
        public void TestShortCutLink()
        {
            var shortPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "命令提示符.lnk");
            var targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "Cmd.exe");
            var shellLink = ExtterCaller.CreateShortcut2(shortPath, targetPath);
            shellLink.Flags = ShellLink.DATA_FLAGS.RunAsUser; // 以管理员身份运行
            shellLink.Save(shortPath);
            Console.WriteLine(shellLink.Path);
            Console.WriteLine(shellLink.FileInfo.GetJsonFormatString());
        }
        /// <summary>
        /// 测试字典加锁
        /// </summary>
        [TestMethod]
        public void DictionaryLocker()
        {
            var _CctDic = new ConcurrentDictionary<int, string>();
            var _CctDicClass = new ConcurrentDictionary<int, Tuble<string, int>>();
            var _Dic = new Dictionary<int, string>();
            var _DicClass = new Dictionary<int, Tuble<string, int>>();
            var _Ht = new Hashtable();
            var _HtClass = new Hashtable();
            var _CurrentItem = "";
            var _Item = "字符串";
            var _NUM = 10000000;//执行次数 
            Tuble<string, int> _CurrentStudent = null;
            Tuble<string, int> student = new Tuble<string, int>(_Item, _NUM);
            Stopwatch _SW = new Stopwatch();

            //字符串写入字典（无锁）
            _SW.Start();

            for (int i = 0; i < _NUM; i++)
            {
                _Dic[i] = _Item;
            }
            _SW.Stop();
            Console.WriteLine("向字典写入【字符串】不添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //字符串写入字典（有锁）
            _Dic = new Dictionary<int, string>();
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _Dic[i] = _Item;
                }
            }
            _SW.Stop();
            Console.WriteLine("向字典写入【字符串】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入字典（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _DicClass[i] = student;
            }
            _SW.Stop();
            Console.WriteLine("向子典写入【学生类】不添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入字典（有锁）
            _DicClass = new Dictionary<int, Tuble<string, int>>();
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_DicClass)
                {
                    _DicClass[i] = student;
                }
            }
            _SW.Stop();
            Console.WriteLine("向子典写入【学生类】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("----------------------------------------------------");

            //字符串写入HashTable（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _Ht[i] = _Item;
            }
            _SW.Stop();
            Console.WriteLine("向HashTable写入【字符串】不添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //字符串写入HashTable（有锁）
            _Ht = new Hashtable();
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Ht)
                {
                    _Ht[i] = _Item;
                }
            }
            _SW.Stop();
            Console.WriteLine("向HashTable写入【字符串】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入HashTable（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _HtClass[i] = student;
            }
            _SW.Stop();
            Console.WriteLine("向HashTable写入【学生类】不添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入HashTable（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_HtClass)
                {
                    _HtClass[i] = student;
                }
            }
            _SW.Stop();
            Console.WriteLine("向HashTable写入【学生类】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("----------------------------------------------------------");

            //字符串写入ConcurrentDictionary
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CctDic[i] = _Item;
            }
            _SW.Stop();
            Console.WriteLine("向ConcurrentDictionary写入【字符串】 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入ConcurrentDictionary
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CctDicClass[i] = student;
            }
            _SW.Stop();
            Console.WriteLine("向ConcurrentDictionary写入【学生类】 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("--------------------------------------------------------");

            //遍历普通字典（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentItem = _Dic[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【普通】字典（无锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历普通字典（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _CurrentItem = _Dic[i];
                }
            }
            _SW.Stop();
            Console.WriteLine("遍历【普通】字典（有锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历类字典（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentStudent = _DicClass[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【学生类】字典（无锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历类字典（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _CurrentStudent = _DicClass[i];
                }
            }
            _SW.Stop();
            Console.WriteLine("遍历【学生类】字典（有锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("--------------------------------------------------------");

            //遍历HashTable（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentItem = _Ht[i].ToString();
            }
            _SW.Stop();
            Console.WriteLine("遍历【HashTable】字典（无锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历HashTable（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _CurrentItem = _Ht[i].ToString();
                }
            }
            _SW.Stop();
            Console.WriteLine("遍历【HashTable】字典（有锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历HashTable类（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentStudent = (Tuble<string, int>)_HtClass[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【HashTable学生类】字典（无锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历HashTable类（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _CurrentStudent = (Tuble<string, int>)_HtClass[i];
                }
            }
            _SW.Stop();
            Console.WriteLine("遍历【HashTable学生类】字典（有锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("--------------------------------------------------------");

            //遍历ConCurrent字典
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentItem = _CctDic[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【ConCurrent字典】（字符串） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历ConCurrent字典（类）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentStudent = _CctDicClass[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【ConCurrent字典】（学生类） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("--------------------------------------------------------");
            _SW.Restart();
            Console.WriteLine("-------------------结束---------------------------");
        }
        [TestMethod]
        public void TestFiled()
        {
            Transform t = new Transform();
            var v = t.v;
            v.x = 1;
            t.v = v;
            t.ShowV();
        }
        struct Vector
        {
            public float x;
            public float y;
            public float z;
        }

        class Transform
        {
            public Vector v { get; set; }
            public void ShowV()
            {
                Console.WriteLine(v.x + "..." + v.y + "..." + v.z);
            }
        }

        [TestMethod]
        public void TestSocket()
        {
            var tcpServer = new TcpClient(AddressFamily.InterNetwork);
            tcpServer.Connect(IPAddress.Any, 29988);
            var stream = tcpServer.GetStream();
            var data = "{\"i\":1,\"c\":\"playOutsideVoice\",\"m\":\"与子米通数据交换异常，请检查网络情况。与子米通数据交换异常，请检查网络情况。与子米通数据交换异常，请检查网络情况。\",\"p\":\"{\\\"v\\\":\\\"3\\\"}\",\"t\":\"20221213083600\",\"l\":false}".GetUTF8Bytes();
            stream.Write(data, 0, data.Length);
            var dataR = new byte[4096];
            var res = stream.Read(dataR, 0, dataR.Length);
            Console.WriteLine(dataR.GetUTF8String());
            Thread.Sleep(30000);
        }
        [TestMethod]
        public void TestTaskAndFactory()
        {
            Task.Factory.StartNew((Func<Task>)TestTaskAndFactoryAsync);
            Task.Run((Func<Task>)TestTaskAndFactoryAsync);
            Thread.Sleep(1000);
        }
        private async Task TestTaskAndFactoryAsync()
        {
            await Task.Factory.StartNew(() => Console.WriteLine("我调用成功了"));

        }
        [TestMethod]
        public void TestCharSort()
        {
            var list = new List<String>()
            {
                "国","内"
            };
            Console.WriteLine(list.OrderBy(s => s).JoinString());
            Console.WriteLine(list.OrderByDescending(s => s).JoinString());
            Console.WriteLine("=================================================");
            var list2 = new List<char>()
            {
                '国','内'
            };
            Console.WriteLine(list2.OrderBy(s => s).JoinString());
            Console.WriteLine(list2.OrderByDescending(s => s).JoinString());
        }
        [TestMethod]
        public void TestRefAction()
        {
            var actions = new List<Action>();
            for (int i = 0; i < 10; i++)
            {
                var ini = i;
                Action Test = () =>
                {
                    var text = $"{i}";
                    Thread.Sleep((ini % 2) * 1000);
                    text = $"{text}={i}";
                    Console.WriteLine($"{text}={i++}");
                };
                actions.Add(Test);
                //Test.Invoke();
            }
            Parallel.For(0, actions.Count, new ParallelOptions { MaxDegreeOfParallelism = 15 }, i =>
            {
                Console.WriteLine(i);
                actions[i].Invoke();
            });
        }
        [TestMethod]
        public void TestNumber()
        {
            bool isOption = false;
            Parallel.For(0, 1000, new ParallelOptions { MaxDegreeOfParallelism = 100 }, i =>
            {
                if (isOption) { return; }
                isOption = true;
                Console.WriteLine(i);
                Thread.Sleep(1000);
                Console.WriteLine(i);
                //actions[i].Invoke();
            });
        }
        [TestMethod]
        public void TestEnum()
        {
            var values = System.Enum.GetValues(typeof(StoreType));
            var allEnums = (StoreType[])values;
            var allValues = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var val = values.GetValue(i);
                allValues[i] = (int)Convert.ChangeType(val, typeof(int));
            }
            for (int i = 0; i < allEnums.Length; i++)
            {
                Console.WriteLine($"{allEnums[i]} => {allValues[i]}");
            }
        }
        [TestMethod]
        public void TestDateTime()
        {
            var sec = 1675907328;
            var date = new DateTime(TimeSpan.TicksPerSecond * sec + ExtterCaller.TicksFrom1970, DateTimeKind.Utc);
            Console.WriteLine(date.ToLocalTime().GetDateTimeString());
        }
        [TestMethod]
        public void TestEncoding()
        {
            var x = Guid.NewGuid().GetString();
            Console.WriteLine(x);
            Console.WriteLine(x.GetUTF8Bytes().Length);
            Console.WriteLine(x.GetASCIIBytes().Length);
        }
        [TestMethod]
        public void TestFlags()
        {
            ConsoleTest(typeof(TestTry));
            ConsoleTest(typeof(UserPassword));
            ConsoleTest(typeof(SettingJsonFile));
            ConsoleTest(typeof(PropertyAccess));
            static void ConsoleTest(Type type)
            {
                foreach (var item in type.GetProperties(Reflection.BindingFlags.Instance | Reflection.BindingFlags.Public))
                {
                    Console.WriteLine($"实例公共成员 => {type.Name}.{item.Name}");
                }
                foreach (var item in type.GetProperties(Reflection.BindingFlags.Static | Reflection.BindingFlags.Public))
                {
                    Console.WriteLine($"静态公共成员 => {type.Name}.{item.Name}");
                }
            }
        }
        [TestMethod]
        public void TestStructureBytes()
        {
            var model = new Tuple<string, bool, int, double, Tuple<string, object>>("123", true, 123, 123, new Tuple<string, object>("456", new { Test = 456 }));
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(model);
            writer.Flush();
            ms.Position = 0;

            var msr = new MemoryStream(ms.ToArray());
            var reader = new StreamReader(msr);
            var res = reader.ReadToEnd();
        }

        [TestMethod]
        public void TestJwtToken()
        {
            var token = ARequestWebComponent.CreateToken("xbcd54343223423423432324", 8, "ErikZhouXin", "erikzhouxin", "ezhouxin", "EZXA");
            Console.WriteLine(token);
        }
        [TestMethod]
        public void TestWindowAuth()
        {
            var res = ExecuteCommandViewModel.DefaultAuthCommand("ErikZhouXin");
            Console.WriteLine(res);
        }
        [TestMethod]
        public void TestRegex()
        {
            var reg = new Regex("^((?<protocal>\\w+):((//)|(\\\\)))?(?<host>((((\\w)+)\\.?)+))(:(?<port>\\d+))?(?<path>(((\\)|(/))?(\\w*))*))(\\?(?<args>((&?(([\\w]+)=([^&#]*))?)*)))?(#(?<anchor>([\\w\\W]*)))?$");
            var urls = new List<string>()
            {
                "socket://localhost:80/test?length=4096&key=#gate",
                "tcp://localhost:80/test/gate?length=4096&key=333#gate",
                "localhost:80/test?length=4096&key=&#gate",
                "localhost/test?length=4096#gate",
                "socket://localhost/test?length=4096#gate",
                "socket://192.168.1.110:80?length=4096#gate",
                "socket://www.localhost.com:80/?length=4096#gate",
                "socket://localhost:80/test?length=4096",
                "socket://localhost:80/test?",
                "socket://localhost:80/test",
                "socket://localhost:80/test?length=4096#",
                "socket://localhost:80/test#gate",
                "socket://localhost:80/#gate",
                "socket://localhost:80#gate",
                "localhost:80",
                "localhost",
                "socket://192.168.1.27:29988",
            };
            foreach (var item in urls)
            {
                Assert.IsTrue(reg.IsMatch(item));
                Console.WriteLine(GetUrlModel(item).GetJsonString());
            }
        }

        private static Dictionary<string, Tuble<string, string, int, string, string, int, string, string>> ServerDic { get; } = new();
        private Tuble<string, string, int, string, string, int, string, string> GetUrlModel(string url)
        {
            if (ServerDic.TryGetValue(url, out var server)) { return server; }
            var reg = new Regex("^((?<protocal>\\w+):((//)|(\\\\)))?(?<host>((((\\w)+)\\.?)+))(:(?<port>\\d+))?(?<path>(((\\)|(/))?(\\w*))*))(\\?(?<args>((&?(([\\w]+)=([^&#]*))?)*)))?(#(?<anchor>([\\w\\W]*)))?$");
            var match = reg.Match(url);
            if (!match.Success)
            {
                return ServerDic[url] = new Tuble<string, string, int, string, string, int, string, string>("socket", "localhost", 80, "", "", 4096, "", "");
            }
            var protocal = GetDefaultString(match.Groups["protocal"].Value, "socket");
            var host = GetDefaultString(match.Groups["host"].Value, "localhost");
            var port = GetDefaultString(match.Groups["port"].Value, "80").ToPInt32(80);
            var path = GetDefaultString(match.Groups["path"].Value, "");
            var anchor = GetDefaultString(match.Groups["anchor"].Value, "");
            var args = GetDefaultString(match.Groups["args"].Value, "");

            var length = 4096;
            var key = "";
            var spliter = args.Split("&", StringSplitOptions.RemoveEmptyEntries);
            var dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in spliter)
            {
                var ix = item.IndexOf('=');
                if (ix <= 0) { continue; }
                var property = item.Substring(0, ix);
                var value = string.Empty;
                if (ix + 1 < length) { value = item.Substring(ix + 1); }
                dic[property] = value;
            }
            return ServerDic[url] = new Tuble<string, string, int, string, string, int, string, string>(protocal, host, port, path, key, length, args, anchor);
            static string GetDefaultString(string value, string defVal)
            {
                return string.IsNullOrWhiteSpace(value) ? defVal : value.Trim();
            }
        }

        private int _Sequence = 0;
        [TestMethod]
        public void TestTaskable()
        {
            TaskRevokable task = new TaskRevokable(ReadTask).Start();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                int num = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(num * 100);
                    _Sequence++;
                }));
            }
            tasks.Add(Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                task.Cancel();
            }));
            Task.WaitAll(tasks.ToArray());
            void ReadTask()
            {
                Console.WriteLine(_Sequence);
                Thread.Sleep(100);
            }
        }
        [TestMethod]
        public void 测试锁()
        {
            TestLockerAndTask();
            Thread.Sleep(12000);
            Console.WriteLine($"{DateTime.Now}我退出程序了");
            //Task.WaitAll(task1, task2);
        }
        public void TestLockerAndTask()
        {
            var task1 = new Task(() =>
            {
                var locker = CacheLockModel<FunctionUT>.Get("123");
                lock (locker)
                {
                    Console.WriteLine($"{DateTime.Now}我生成锁了");
                    Thread.Sleep(10000);
                    Console.WriteLine($"{DateTime.Now}我释放锁了");
                }
            });
            task1.Start();
            var task2 = new Task(() =>
            {
                Thread.Sleep(1000);
                var lk = CacheLockModel<FunctionUT>.Get("123");
                Console.WriteLine($"{DateTime.Now}我获取到对象了");
                lock (lk)
                {
                    Console.WriteLine($"{DateTime.Now}我获取到锁了");
                }
                Console.WriteLine($"{DateTime.Now}我要抛出异常了");
                throw new Exception();
            });
            task2.Start();
        }
        [TestMethod]
        public void TestLocker()
        {
            ConcurrentDictionary<int, string> _CctDic = new ConcurrentDictionary<int, string>();
            ConcurrentDictionary<int, Student> _CctDicClass = new ConcurrentDictionary<int, Student>();
            Dictionary<int, string> _Dic = new Dictionary<int, string>();
            Dictionary<int, Student> _DicClass = new Dictionary<int, Student>();
            Hashtable _Ht = new Hashtable();
            Hashtable _HtClass = new Hashtable();
            string _CurrentItem = "";
            const string _Item = "字符串";
            const int _NUM = 10000000;//执行次数 
            Student _CurrentStudent = null;
            Student student = new Student { Name = _Item, Age = 23 };
            Stopwatch _SW = new Stopwatch();

            //字符串写入字典（无锁）
            _SW.Start();

            for (int i = 0; i < _NUM; i++)
            {
                _Dic[i] = _Item;
            }
            _SW.Stop();
            Console.WriteLine("向字典写入【字符串】不添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //字符串写入字典（有锁）
            _Dic = new Dictionary<int, string>();
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _Dic[i] = _Item;
                }
            }
            _SW.Stop();
            Console.WriteLine("向字典写入【字符串】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //字符串写入字典（有锁）
            _Dic = new Dictionary<int, string>();
            _SW.Restart();
            lock (_Dic)
            {
                for (int i = 0; i < _NUM; i++)
                {
                    _Dic[i] = _Item;
                }
            }
            _SW.Stop();
            Console.WriteLine("向字典写入【字符串】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入字典（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _DicClass[i] = student;
            }
            _SW.Stop();
            Console.WriteLine("向子典写入【学生类】不添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入字典（有锁）
            _DicClass = new Dictionary<int, Student>();
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_DicClass)
                {
                    _DicClass[i] = student;
                }
            }
            _SW.Stop();
            Console.WriteLine("向子典写入【学生类】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            _DicClass = new Dictionary<int, Student>();
            _SW.Restart();
            lock (_DicClass)
            {
                for (int i = 0; i < _NUM; i++)
                {
                    _DicClass[i] = student;
                }
            }
            _SW.Stop();
            Console.WriteLine("向子典写入【学生类】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("----------------------------------------------------");

            //字符串写入HashTable（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _Ht[i] = _Item;
            }
            _SW.Stop();
            Console.WriteLine("向HashTable写入【字符串】不添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //字符串写入HashTable（有锁）
            _Ht = new Hashtable();
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Ht)
                {
                    _Ht[i] = _Item;
                }
            }
            _SW.Stop();
            Console.WriteLine("向HashTable写入【字符串】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入HashTable（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _HtClass[i] = student;
            }
            _SW.Stop();
            Console.WriteLine("向HashTable写入【学生类】不添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入HashTable（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_HtClass)
                {
                    _HtClass[i] = student;
                }
            }
            _SW.Stop();
            Console.WriteLine("向HashTable写入【学生类】添加锁（Lock）花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("----------------------------------------------------------");

            //字符串写入ConcurrentDictionary
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CctDic[i] = _Item;
            }
            _SW.Stop();
            Console.WriteLine("向ConcurrentDictionary写入【字符串】 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //类写入ConcurrentDictionary
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CctDicClass[i] = student;
            }
            _SW.Stop();
            Console.WriteLine("向ConcurrentDictionary写入【学生类】 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("--------------------------------------------------------");

            //遍历普通字典（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentItem = _Dic[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【普通】字典（无锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历普通字典（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _CurrentItem = _Dic[i];
                }
            }
            _SW.Stop();
            Console.WriteLine("遍历【普通】字典（有锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历类字典（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentStudent = _DicClass[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【学生类】字典（无锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历类字典（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _CurrentStudent = _DicClass[i];
                }
            }
            _SW.Stop();
            Console.WriteLine("遍历【学生类】字典（有锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("--------------------------------------------------------");

            //遍历HashTable（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentItem = _Ht[i].ToString();
            }
            _SW.Stop();
            Console.WriteLine("遍历【HashTable】字典（无锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历HashTable（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _CurrentItem = _Ht[i].ToString();
                }
            }
            _SW.Stop();
            Console.WriteLine("遍历【HashTable】字典（有锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历HashTable类（无锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentStudent = (Student)_HtClass[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【HashTable学生类】字典（无锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历HashTable类（有锁）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                lock (_Dic)
                {
                    _CurrentStudent = (Student)_HtClass[i];
                }
            }
            _SW.Stop();
            Console.WriteLine("遍历【HashTable学生类】字典（有锁） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("--------------------------------------------------------");

            //遍历ConCurrent字典
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentItem = _CctDic[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【ConCurrent字典】（字符串） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);

            //遍历ConCurrent字典（类）
            _SW.Restart();
            for (int i = 0; i < _NUM; i++)
            {
                _CurrentStudent = _CctDicClass[i];
            }
            _SW.Stop();
            Console.WriteLine("遍历【ConCurrent字典】（学生类） 花费时间为:{0} 毫秒", _SW.Elapsed.TotalMilliseconds);
            Console.WriteLine("--------------------------------------------------------");
            _SW.Restart();
            Console.WriteLine("-------------------结束---------------------------");
            Console.ReadLine();
        }
        [TestMethod]
        public void TestTrim()
        {
            Console.WriteLine("this is \n 哈哈\n \0 ".Trim().Trim('\0').Trim());
            Console.WriteLine("this is \n 哈哈\n ");
        }
        public class Student
        {
            public string Name;
            public int Age;
        }
        [TestMethod]
        public void TestVersion()
        {
            Console.WriteLine(Environment.OSVersion.Version.Major);
        }
        [TestMethod]
        public void TestUrlModel()
        {
            var model = ConvertModel("vzlpr://192.168.1.100:80/test?account=test&u=test%23U&password=test23!@%23");
            var url = HopperUrlModelV1.GetModelArgs(model);
            Console.WriteLine(model.GetJsonFormatString());
            Console.WriteLine(url);
            TestModel3333 ConvertModel(string lprLeft)
            {
                var defModel = CobberBuilder.CreateSampleInstance<TestModel3333>();
                var urlModel = new HopperUrlModelV1(lprLeft);
                var jsonObject = new Dictionary<string, object>
                {
                    { nameof(TestModel3333.Address), urlModel.Get(nameof(urlModel.Host),"192.168.1.1") },
                    { nameof(TestModel3333.PortRate), urlModel.Get(nameof(urlModel.Port), 80) },
                    { nameof(TestModel3333.Account), urlModel.Get(nameof(defModel.Account), "admin") },
                    { nameof(TestModel3333.Password), urlModel.Get(nameof(defModel.Password), "admin") },
                    { nameof(TestModel3333.U), urlModel.Get(nameof(defModel.U), "admin") },
                };
                var access = PropertyAccess.GetAccess(defModel);
                foreach (var kv in jsonObject)
                {
                    TestTry.Try(access.FuncSetValue, defModel, kv.Key, kv.Value);
                }
                return defModel;
            }
            Console.WriteLine(Uri.EscapeDataString("socket://192.168.1.27:29988"));
            Console.WriteLine(HopperUrlModelV1.ConvertArray<string>("无牌车,_无牌车_,无车牌", new string[0]).GetJsonString());
        }
        [TestMethod]
        public void TestWMI()
        {
            if (AppSystem.TryGetWin32WmiDiskDriveID(out var test))
            {
                Console.WriteLine(test.ToString());
            }
            if (AppSystem.TryGetWin32WmiDiskDriveSerialNumber(out test))
            {
                Console.WriteLine(test.ToString());
            }
            if (AppSystem.TryGetWin32WmiMacAddress(out test))
            {
                Console.WriteLine(test.ToString());
            }
            if (AppSystem.TryGetWin32WmiMacsAddress(out var tests))
            {
                Console.WriteLine(tests.GetJsonString());
            }
            if (AppSystem.TryGetWin32WmiOperatingSystemID(out test))
            {
                Console.WriteLine(test.ToString());
            }
            if (AppSystem.TryGetWin32WmiProcessorID(out test))
            {
                Console.WriteLine(test.ToString());
            }
        }
        public interface TestModel3333
        {
            String Address { get; }
            Int32 PortRate { get; }
            String Account { get; }
            String Password { get; }

            String U { get; }
        }
        [TestMethod]
        public void TestTaskScheduler()
        {
            ExtterCaller.CreateStartTask("C:\\Windows\\System32\\Cmd.exe", "cmd.start");
        }
    }
}
