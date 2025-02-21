using System.Net.Sockets;

namespace TransmissionTool
{
	public class FileReceiver
	{
        public static void StartReceivingFiles(object tcpListenerObj)
        {
            TcpListener tcpListener = (TcpListener)tcpListenerObj;

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                NetworkStream stream = tcpClient.GetStream();
                StreamReader reader = new StreamReader(stream);

                string header = reader.ReadLine();
                if (header.StartsWith("FILE|"))
                {
                    string[] headerParts = header.Split('|');
                    string fileName = headerParts[1];
                    long fileSize = long.Parse(headerParts[2]);
                    int totalPackets = int.Parse(headerParts[3]);

                    Console.WriteLine($"Receiving file {fileName} ({fileSize} bytes) in {totalPackets} packets...");

                    using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[524288];
                        int bytesRead;

                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }

                    Console.WriteLine($"File {fileName} received and saved.");
                }

                tcpClient.Close();
            }
        }
    }
}

