using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelComLib
{
    public class PixelCom
    {
        private static PixelCom instance;

        public static PixelCom Instance
        {
            get { return instance ?? (instance = new PixelCom()); }
        }

        private static string message;
        public static string MessageReceived => message;

        public void StartServer(string pipeName)
        {
            Task.Factory.StartNew(() =>
            {

                var server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1);
                while (true)
                {
                    server.WaitForConnection();

                    if (server.IsConnected)
                    {
                        using (StreamReader sr = new StreamReader(server))
                        {
                            message = sr.ReadLine();
                            {
                                Console.WriteLine("{0}: {1}", DateTime.Now, message);
                            }
                            OnMessageReceive?.Invoke(message);
                        }
                        Console.WriteLine("Connection closed");
                        if (!server.IsConnected)
                            server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1);
                    }

                }


            });
        }

        public static void SendMessage(string pipeName, string message)
        {
            var client = new NamedPipeClientStream(pipeName);
            try
            {
                client.Connect();
                using (StreamWriter sw = new StreamWriter(client))
                {
                    sw.Write(message);
                }
            }
            finally
            {
                client.Close();
                client.Dispose();
            }
        }

        public static bool HostExists()
        {
            return System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
        }
        
        public delegate void MessageReceive(string mess);

        public event MessageReceive OnMessageReceive;
    }
}
