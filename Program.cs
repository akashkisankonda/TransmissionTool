using System.Net;
using System.Net.Sockets;
using System.Text;
using TransmissionTool;

class Program
{
    public static int port;
    static void Main(string[] args)
    {
        TcpListener tcpListener;
        UdpClient udpClient;

        Console.Write("Enter port number: ");
        port = int.Parse(Console.ReadLine());

        udpClient = new UdpClient(port);
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();

        Console.WriteLine($"Listening on port {port}...");

        Thread receiveMessageThread = new Thread(MessageReceiver.StartReceivingMessages);
        receiveMessageThread.Start(udpClient);

        Thread receiveFileThread = new Thread(FileReceiver.StartReceivingFiles);
        receiveFileThread.Start(tcpListener);

        HandleMessaging();


        void HandleMessaging()
        {
            while (true)
            {
                string message = Console.ReadLine();
                if (message.StartsWith("sendfile "))
                {
                    string filePath = message.Substring(9);
                    Thread fileSendThread = new Thread(() => FileSender.SendFile(filePath));
                    fileSendThread.Start();
                }
                else
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, port));
                }
            }
        }
    }
}
