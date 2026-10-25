using System;
using System.Buffers.Binary;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Server
{
    internal class Program
    {
        public const int Port = 4567;
        public const int ChunkSize = 60000;
        static async Task Main(string[] args)
        {
            using Socket server = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp
            );
            IPEndPoint serverEndPoint = new IPEndPoint( IPAddress.Any , Port );
            server.Bind(serverEndPoint);
            Console.WriteLine("Server ayaqdadir");
            Console.WriteLine($"Port: {Port}");
            byte[] buffer = new byte[4690];
            while (true)
            {
                EndPoint clientEndPoint = new IPEndPoint( IPAddress.Any , 0 );
                SocketReceiveFromResult result =
                    await server.ReceiveFromAsync(
                        buffer.AsMemory(),
                        SocketFlags.None,
                        clientEndPoint
                    );
                string request = Encoding.UTF8.GetString( buffer, 0, result.ReceivedBytes );
                if (request != "SCREENSHOT")
                {
                    continue;
                }
                try
                {
                    byte[] imageBytes = TakeScreenShot();
                    await SendImageAsync(
                        server,
                        result.RemoteEndPoint,
                        imageBytes
                    );
                    Console.WriteLine("photo send to client");
                }
                catch (Exception ex)
                {
                    Console.WriteLine( $"Error: {ex.Message}" );
                }
            }
        }
        private static byte[] TakeScreenShot()
        {
            Rectangle bounds = SystemInformation.VirtualScreen;

            using Bitmap bitmap = new Bitmap(
                bounds.Width,
                bounds.Height
            );

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(
                    bounds.Left,
                    bounds.Top,
                    0,
                    0,
                    bounds.Size
                );
            }

            using MemoryStream stream = new MemoryStream();

            bitmap.Save(stream, ImageFormat.Png);

            return stream.ToArray();
        }
        private static async Task SendImageAsync(
            Socket socket,
            EndPoint clientEndPoint,
            byte[] imageBytes)
        {
            int totalChunks = (int)Math.Ceiling( (double)imageBytes.Length / ChunkSize );
            for (int i = 0; i < totalChunks; i++)
            {
                int offset = i * ChunkSize;
                int length = Math.Min( ChunkSize , imageBytes.Length - offset );
                byte[] packet = new byte[8 + length];
                BinaryPrimitives.WriteInt32BigEndian( packet.AsSpan(0, 4), i);
                BinaryPrimitives.WriteInt32BigEndian(
                    packet.AsSpan(4, 4),
                    totalChunks
                );
                Buffer.BlockCopy(
                    imageBytes,
                    offset,
                    packet,
                    8,
                    length
                );
                await socket.SendToAsync(
                    packet.AsMemory(),
                    SocketFlags.None,
                    clientEndPoint
                );
            }
        }
    }
}
