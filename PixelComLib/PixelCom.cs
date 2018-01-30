using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;

namespace PixelComLib
{
    /// <summary>
    /// A singleton class used to access the library features
    /// </summary>
    public class PixelCom
    {
        #region Private fields
        private static PixelCom instance;
        private static string message;
        #endregion
        public delegate void MessageReceive(string mess);
        public static PixelCom Instance
        {
            get { return instance ?? (instance = new PixelCom()); }
        }
        #region Methods
        /// <summary>
        /// Method invoked to start NamedPipeServer with the given pipeName
        /// </summary>
        /// <param name="pipeName"></param>
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
                            OnMessageReceive?.Invoke(message);                   
                        }
                        if (!server.IsConnected)
                            server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1);
                    }

                }


            });
        }
        /// <summary>
        /// Sends message using NamedPipeClient to the server 
        /// of the given pipeName
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="message"></param>
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
        /// <summary>
        /// Checks whether there's already an instance of the program
        /// </summary>
        /// <returns></returns>
        public static bool HostExists()
        {
            return System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
        }
        #endregion

        /// <summary>
        /// Use this event to access the message when the server receives it
        /// </summary>
        public event MessageReceive OnMessageReceive;

    }
}
