using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Helpers;
using API.Base.Web.Base.Misc;

namespace API.Base.Logging.Managers.LogManager
{
    internal class LogManager : ILogManager, IDisposable
    {
        public static bool WriteInStdOut { get; set; } = true;

        private StreamWriter _streamWriter;
        private ulong _logFileSize;
        private const ulong MaxLogFileSize = 1024 * 1024 * 100; // 100MB
        private const int FlushInterval = 3 * 1000; // 3 seconds
        private bool _needsFlush = false;
        private static readonly object CreationLock = new object();
        private readonly string _directory;
        private readonly Thread _flushThread;

        private readonly ConcurrentQueue<string> _logQueue;

        internal LogManager()
        {
            _directory = EnvVarManager.Get("LOGS_DIRECTORY") ?? "../logs";
            _logQueue = new ConcurrentQueue<string>();
            try
            {
//                Console.WriteLine("using log directory: " + _directory);
                if (!Directory.Exists(_directory))
                {
                    Console.WriteLine($"creating dir: '{_directory}'");
                    Directory.CreateDirectory(_directory);
                }

                CreateStreamWriter();
                _flushThread = new Thread(async () => { await WorkerMethod(); });
                _flushThread.Start();
            }
            catch (Exception exc)
            {
                Console.WriteLine($"LogManager.constructor Exception: {exc.Message} for dir '{_directory}'");
                throw;
            }
        }

        private void CreateStreamWriter()
        {
            lock (CreationLock)
            {
                if (StreamOk) return;

                var directory = new DirectoryInfo(_directory);

                var lastFile = directory.GetFiles()
                    .Where(file => file.FullName.EndsWith(".log"))
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault()?.Name;
                var opened = false;
                if (!string.IsNullOrEmpty(lastFile))
                {
                    try
                    {
                        Console.WriteLine("Using existing log file: " + lastFile);
                        TryOpenStream(lastFile);
                        _logFileSize = (ulong) _streamWriter.BaseStream.Length;
                        opened = true;
                        if (_logFileSize > MaxLogFileSize)
                        {
                            Console.WriteLine("Or not, it is already too big.");
                            _streamWriter.Close();
                            _streamWriter = null;
                            opened = false;
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("Can't use existing log file: " + exc.Message);
                    }
                }


                if (!opened)
                {
                    var dt = DateTime.Now;
                    var logFileName = $"Log_{dt:yyyy-MM-dd_HH-mm-ss}_{Utilis.GenerateRandomHexString(10)}.log";
                    Console.WriteLine("Creating log file: " + logFileName);
                    TryOpenStream(logFileName);
                    _logFileSize = 0;
                }
            }
        }

        private void TryOpenStream(string path)
        {
            var fullPath = Path.Combine(_directory, path).Replace("\\", "/");
            _streamWriter = new StreamWriter(new FileStream(fullPath, FileMode.Append));
        }

        private void CheckStreamWriter()
        {
            if (_logFileSize > MaxLogFileSize
                || !StreamOk)
            {
                CreateStreamWriter();
            }
        }

        public void StoreAsync(LogEntity log)
        {
            CheckStreamWriter();
            var logStr = log.Created.ToString("[yyyy-M-d HH:mm:ss.fff]") + $"[{log.Level}]" +
                         (string.IsNullOrEmpty(log.Tag) ? "" : "[" + log.Tag + "]") + $": {log.Message}";
            _logFileSize += (uint) logStr.Length;
            _needsFlush = true;
            if (WriteInStdOut)
            {
                Console.WriteLine(logStr);
            }

            _logQueue.Enqueue(logStr);
        }

        private async Task WorkerMethod()
        {
//            Console.WriteLine("Logs worker started");
            _working = true;
            while (_working)
            {
                if (_needsFlush && StreamOk)
                {
                    while (_logQueue.Count > 0 && _working && StreamOk)
                    {
                        if (_logQueue.TryDequeue(out var currentLog))
                        {
                            _streamWriter.WriteLine(currentLog);
                        }
                    }

                    _streamWriter.Flush();
                    _needsFlush = false;
                }

                await Task.Delay(FlushInterval);
            }

            _streamWriter.Close();

            Console.WriteLine("Logs worker end");
        }

        public bool StreamOk
            => _streamWriter?.BaseStream != null && _streamWriter.BaseStream.CanWrite;

        public void StopWorker()
        {
            _working = false;
        }

        private bool _working = false;

        public void Dispose()
        {
            _working = false;
            _streamWriter.Flush();
            _streamWriter.Dispose();
        }
    }
}