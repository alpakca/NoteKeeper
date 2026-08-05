using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace NoteKeeper
{
    internal static class SingleInstanceChecker
    {
        private const string MutexName = "Local\\NoteKeeper_4d19fceb-402b-41df-bce1-7d7b3f3c68e5";
        private const string PipeName = "NoteKeeper_4d19fceb-402b-41df-bce1-7d7b3f3c68e5";

        private static Mutex? _mutex;

        public static bool IsServer { get; private set; }
        public static event Action<string[]>? ArgsReceived;

        public static bool TryStart(string[] args)
        {
            _mutex = new Mutex(initiallyOwned: true, MutexName, out bool createdNew);

            if (createdNew)
            {
                StartPipeServer();
                IsServer = true;
                return true;
            }
            else
            {
                SendArgsToServer(args);
                IsServer = false;
                return false;
            }
        }

        public static void ReleaseMutex()
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
        }

        private static async void StartPipeServer()
        {
            while (true)
            {
                try
                {
                    using NamedPipeServerStream server = new NamedPipeServerStream(PipeName, PipeDirection.In);

                    await server.WaitForConnectionAsync();

                    using StreamReader reader = new StreamReader(server);
                    string? data = await reader.ReadToEndAsync();

                    string[] args = string.IsNullOrEmpty(data) ? [] : data.Split('\0');
                    ArgsReceived?.Invoke(args);
                }
                catch
                {
                }
            }
        }

        private static void SendArgsToServer(string[] args)
        {
            try
            {
                using NamedPipeClientStream client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);

                client.Connect(2000);

                string toSend = string.Join('\0', args);
                byte[] byteLoad = Encoding.UTF8.GetBytes(toSend);

                client.Write(byteLoad);
                client.Flush();
            }
            catch (TimeoutException)
            {
            }

        }
    }
}
