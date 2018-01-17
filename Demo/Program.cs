using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PixelComLib;
namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            const string pipeName = "pixelPipe";

            if (!PixelCom.HostExists())
            {
                Console.WriteLine("Original server");
                PixelCom.Instance.OnMessageReceive += Instance_OnMessageReceive;
                PixelCom.Instance.StartServer(pipeName);
                Console.WriteLine(PixelCom.MessageReceived);
                while (true) { }
            }
            else
            {
                Console.WriteLine("Duplicate server(client)");
                PixelCom.SendMessage(pipeName, "test");
            }
            
        }
        
        private static void Instance_OnMessageReceive(string mess)
        {
            Console.WriteLine(mess);
        }
    }
}
