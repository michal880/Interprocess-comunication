using PixelComLib;
using System;
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
