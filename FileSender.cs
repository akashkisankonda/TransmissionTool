using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TransmissionTool
{
	public class FileSender
	{
        private static int packetSize = 524288; // Default 500KB in bytes

        public static void SendFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            byte[] fileData = File.ReadAllBytes(filePath);
            long fileSize = fileData.Length;
            packetSize = Math.Min(524288, (int)fileSize);
            int totalPackets = (int)Math.Ceiling((double)fileSize / packetSize);
            string fileName = Path.GetFileName(filePath);

            Console.WriteLine($"Sending file {fileName} ({fileSize} bytes) in {totalPackets} packets...");

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Loopback, Program.port);
            NetworkStream stream = tcpClient.GetStream();

            // Send file header
            string header = $"FILE|{fileName}|{fileSize}|{totalPackets}";
            byte[] headerData = Encoding.UTF8.GetBytes(header + "\n");
            stream.Write(headerData, 0, headerData.Length);

            for (int i = 0; i < totalPackets; i++)
            {
                int offset = i * packetSize;
                int size = Math.Min(packetSize, fileData.Length - offset);
                stream.Write(fileData, offset, size);
                Console.WriteLine($"Sent packet {i + 1}/{totalPackets}");
            }

            tcpClient.Close();
            Console.WriteLine("File sent successfully.");
        }
    }
}

