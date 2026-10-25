using System;
using System.Buffers.Binary;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
namespace Client
{
    public partial class MainWindow : Window
    {
        public string ServerIp = "127.0.0.1";
        public int Port = 4567;
        public MainWindow()
        {
            InitializeComponent();
        }
        public async void ReceiveButton_Click( object sender , RoutedEventArgs e)
        {
            ReceiveButton.IsEnabled = false;
            StatusText.Text = "Picture Dowlading ....";
            try
            {
                BitmapImage image = await ReceiveScreenshotAsync();
                ScreenshotImage.Source = image;
                StatusText.Text = "Photo Secsecfuly Upload ....";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Error";
                MessageBox.Show(
                    ex.Message,
                    "Xeta",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                ReceiveButton.IsEnabled = true;
            }
        }
        public async Task<BitmapImage> ReceiveScreenshotAsync()
        {
            IPAddress serverAddress = IPAddress.Parse(ServerIp);
            IPEndPoint serverEndPoint = new IPEndPoint( serverAddress , Port );
            using Socket client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp
            );
            byte[] request = Encoding.UTF8.GetBytes("SCREENSHOT");
            await client.SendToAsync(  request.AsMemory(), SocketFlags.None , serverEndPoint );
            byte[] buffer = new byte[65507];
            byte[] []chunks = Array.Empty<byte[]?>();
            int totalChunks = 0;
            int receivedChunks = 0;
            using CancellationTokenSource timeout =
                new CancellationTokenSource( TimeSpan.FromSeconds(20) );
            while (true)
            {
                EndPoint remoteEndPoint = new IPEndPoint( IPAddress.Any, 0 );
                SocketReceiveFromResult result =  await client.ReceiveFromAsync( buffer.AsMemory() , SocketFlags.None , remoteEndPoint , timeout.Token );
                if (result.RemoteEndPoint is not IPEndPoint sender)
                {
                    continue;
                }
                if (!sender.Address.Equals(serverAddress) || sender.Port != Port)
                {
                    continue;
                }
                int packetLength = result.ReceivedBytes;
                if (packetLength < 8)
                { 
                    continue;
                }
                int chunkIndex = BinaryPrimitives.ReadInt32BigEndian( buffer.AsSpan(0, 4) );
                int packetTotal = BinaryPrimitives.ReadInt32BigEndian( buffer.AsSpan(4, 4) );
                if (packetTotal <= 0 || packetTotal > 10000 || chunkIndex < 0 || chunkIndex >= packetTotal)
                {
                    continue;
                }
                if (totalChunks == 0)
                {
                    totalChunks = packetTotal;
                    chunks = new byte[]?[totalChunks];
                }
                if (packetTotal != totalChunks)
                {
                    continue;
                }
                if (chunks[chunkIndex] != null)
                { 
                    continue;
                }
                int dataLength = packetLength - 8;
                byte[] chunk = new byte[dataLength];
                Buffer.BlockCopy( buffer , 8 , chunk , 0 , dataLength );
                chunks[chunkIndex] = chunk;
                receivedChunks++;
                StatusText.Text = $"Paketler: {receivedChunks}/{totalChunks}";

                if (receivedChunks == totalChunks)
                {
                    break;
                }
            }
            using MemoryStream imageStream = new MemoryStream();

            foreach (byte[]? chunk in chunks)
            {
                if (chunk == null) 
                {
                    throw new InvalidDataException( "Error sekil paketin xetaso" );
                }

                await imageStream.WriteAsync(chunk);
            }
            imageStream.Position = 0;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = imageStream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }
}
