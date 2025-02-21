using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TransmissionTool
{
	public class MessageReceiver
	{
        public static void StartReceivingMessages(object udpClientObj)
        {
            UdpClient udpClient = (UdpClient)udpClientObj;
            IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                byte[] data = udpClient.Receive(ref senderEndPoint);
                string message = Encoding.UTF8.GetString(data);

                if (!message.StartsWith("FILE"))
                {
                    Console.WriteLine($"User [{senderEndPoint}]: {message}");
                }
            }
        }
    }
}

