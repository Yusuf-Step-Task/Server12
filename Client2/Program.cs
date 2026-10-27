using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// Client 
public class Program
{
    public static async Task Main(string[] args)
    {
        var client = new UdpClient();

        var endPoint = IPAddress.Parse("127.0.0.1");
        var port = 1234;

        var connectEP = new IPEndPoint(endPoint, port);
        while (true)
        {
            Console.WriteLine("\nEnter command \n1. GetById \n2. GetAll \n3. Add \n4. Update \n5. Delete\nSecim: ");
            string command = Console.ReadLine() ?? "1";
            var bytes = Encoding.UTF8.GetBytes(command);
            await client.SendAsync(bytes, bytes.Length, connectEP);
            //------
            var result = await client.ReceiveAsync();
            string response = Encoding.UTF8.GetString(result.Buffer);

            Console.WriteLine(response);
        }
    }
}