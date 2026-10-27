// Server

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Transactions;

public class Program
{
    public static async Task Main(string[] args)
    {
        var server = new UdpClient(1234);
        var remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            var result = await server.ReceiveAsync();
            byte[] command = result.Buffer;
            remoteEP = result.RemoteEndPoint;
            string message = Encoding.UTF8.GetString(command);
            int secim1 = Convert.ToInt32(message);
            //--------------------------------------------------------
            static async Task HandleClient(UdpClient server, IPEndPoint remoteEP, string ClientDataMessage)
            {
                byte[] data = Encoding.UTF8.GetBytes(ClientDataMessage);
                await server.SendAsync(data, remoteEP);
            }
            string ClientData = "Operation Successfully!";
            //--------------------------------------------------------
            if (secim1 == 1)
            {
                Console.WriteLine($"Car GetById: {Car.GetById}");
                Console.WriteLine("Enter Car ID : ");
                string id = Console.ReadLine() ?? "1";
                int ID2 = Convert.ToInt32(id);
                //    foreach (var car in CarData.Cars)
                //    {
                //        Console.WriteLine($"Id: {car.Id}");
                //        Console.WriteLine($"Make: {car.Make}");
                //        Console.WriteLine($"Model: {car.Model}");
                //        Console.WriteLine($"Year: {car.Year}");
                //        Console.WriteLine($"VIN: {car.VIN}");
                //        Console.WriteLine($"Color: {car.Color}");
                //    }
                //    await HandleClient(server, remoteEP, ClientData);

                Car car = null;
                for (int i = 0; i < CarData.Cars.Count; i++)
                {
                    if (CarData.Cars[i].Id == ID2)
                    {
                        car = CarData.Cars[i];
                        break;
                    }
                }
                if (car != null)
                {
                    ClientData = $"Id: {car.Id} | Make: {car.Make} | Model: {car.Model} | Year: {car.Year} | VIN: {car.VIN} | Color: {car.Color}";
                }
                else
                {
                    ClientData = "Error: Id ile masin yoxdur !!!!";
                }
                await HandleClient(server, remoteEP, ClientData);
            }
            else if (secim1 == 2)
            {
                //for (int i = 0;i < CarData.Cars.Count; i++)
                //{
                //    Console.WriteLine("------------------------------");
                //    Console.WriteLine($"Id: {CarData.Cars[i].Id}");
                //    Console.WriteLine($"Make: {CarData.Cars[i].Make}");
                //    Console.WriteLine($"Model: {CarData.Cars[i].Model}");
                //    Console.WriteLine($"Year: {CarData.Cars[i].Year}");
                //    Console.WriteLine($"VIN: {CarData.Cars[i].VIN}");
                //    Console.WriteLine($"Color: {CarData.Cars[i].Color}");
                //    Console.WriteLine("------------------------------");
                //}
                string allCarsData = "";
                for (int i = 0; i < CarData.Cars.Count; i++)
                {
                    var car = CarData.Cars[i];
                    allCarsData += $"Id: {car.Id} | Make: {car.Make} | Model: {car.Model} | Year: {car.Year} | Color: {car.Color}\n";
                }
                if (allCarsData != "")
                {
                    ClientData = allCarsData;
                }
                else
                {
                    ClientData = "Error: Masin yoxdur !!!";
                }
                await HandleClient(server, remoteEP, ClientData);
            }
            else if (secim1 == 3)
            {
                Console.WriteLine("Enter Information Pleas....");
                Console.WriteLine("Id: ");
                int countId = 0;
                for (int i = 0; i<CarData.Cars.Count; i++)
                {
                    countId++;
                }
                string id = Console.ReadLine() ?? $"{countId + 1}";//eger istifadeci Id daxil etmese defaulr olaraq qaldiqi yerden devam etmesi ucun kicik system
                int id2 = Convert.ToInt32(id);
                if (id2 < 0 || id2 < countId)
                {
                    Console.WriteLine("Id ya istifade edilir yadaki 0 dan kicik reqem daxil edilindir");// Bunun bir seyi psidriki eger isifadeci evvelceden 100 daxil etse ve yene 100 yazsa men onu tuta bilmirme - 1 budur  
                }
                else
                {
                    Console.WriteLine("Make: ");
                    string make = Console.ReadLine() ?? "BMW";
                    Console.WriteLine("Model: ");
                    string model = Console.ReadLine() ?? "X5";
                    Console.WriteLine("Year: ");
                    string year = Console.ReadLine() ?? "XXXX";
                    Console.WriteLine("VIN: ");
                    string vin = Console.ReadLine() ?? "NN-XX-NNN";
                    Console.WriteLine("Color: ");
                    string color = Console.ReadLine() ?? "Black";
                    var newCar = new Car
                    {
                        Id = id2,
                        Make = make,
                        Model = model,
                        Year = Convert.ToUInt16(year),
                        VIN = vin,
                        Color = color
                    };
                    CarData.Cars.Add(newCar);
                }
                ClientData = "Car successfully added to database................";
                await HandleClient(server, remoteEP, ClientData);
            }
            else if (secim1 == 4)
            {
                Console.WriteLine("Enter Pleas Id : ");
                string id3 = Console.ReadLine() ?? "1";
                int id4 = Convert.ToInt32(id3);
                try
                {
                    Console.WriteLine("1.Make\n2.Model\n3.Year\n4.VIN\n5.Color");
                    Console.WriteLine("Hansini deyisdirmek isteyirsiniz ?\nSecim: ");
                    string secim4 = Console.ReadLine() ?? "Make";
                    // Bu qisimde yaxsi fikirim var idi demeli men istedimki bunu her defesinde if ile yoxlamaq yerine 
                    // bir dene method yaradim odaki bele olsunda 
                    //Console.WriteLine($"Old Name: {CarData.Cars[id4].secim4}"); <---
                    //Console.WriteLine("Enter New Name: ");
                    //string Newname = Console.ReadLine() ??;
                    //CarData.Cars[id4].secim4 = Newname;
                    // ama burda secim4 str olduqu ucun goturmedi yoxsa codeni yaxsi kisaltmisdim.

                    if (secim4 == "1" || secim4 == "Make")
                    {
                        Console.WriteLine($"Old Name: {CarData.Cars[id4].Make}");
                        Console.WriteLine("Enter New Name: ");
                        string NewMake = Console.ReadLine() ??"Kia";
                        CarData.Cars[id4].Make = NewMake;
                    }
                    else if (secim4 == "2" || secim4 == "Model")
                    {
                        Console.WriteLine($"Old Model: {CarData.Cars[id4].Model}");
                        Console.Write("Enter New Model: ");
                        CarData.Cars[id4].Model = Console.ReadLine() ?? "Sportage";
                    }
                    else if (secim4 == "3" || secim4 == "Year")
                    {
                        Console.WriteLine($"Old Year: {CarData.Cars[id4].Year}");
                        Console.Write("Enter New Year: ");
                        CarData.Cars[id4].Year = ushort.Parse(Console.ReadLine() ?? "2026");
                    }
                    else if (secim4 == "4" || secim4 == "VIN")
                    {
                        Console.WriteLine($"Old VIN: {CarData.Cars[id4].VIN}");
                        Console.Write("Enter New VIN: ");
                        CarData.Cars[id4].VIN = Console.ReadLine() ?? "";
                    }
                    else if (secim4 == "5" || secim4 == "Color")
                    {
                        Console.WriteLine($"Old Color: {CarData.Cars[id4].Color}");
                        Console.Write("Enter New Color: ");
                        CarData.Cars[id4].Color = Console.ReadLine() ?? "Black";
                    }
                    else
                    {
                        Console.WriteLine("Error: Secimininiz ya evvelinde verilen Numbur ile secim yadaki Name daxil edilerken ilk herf boyuk digerlerini kicik daxil edin .....");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                await HandleClient(server, remoteEP, ClientData);
            }
            else if (secim1 == 5) // Delete 
            {
                Console.WriteLine("Enter Id Please : ");
                string iddelete = Console.ReadLine() ?? "1";
                int idDel = Convert.ToInt32(iddelete);
                var carToRemove = CarData.Cars.FirstOrDefault(c => c.Id == idDel); // ai idi bunu tapmadim hele size de yazdim day vaxtim yoxdu deyem aidan destek aldim ama basa dusdum
                if (carToRemove != null)
                {
                    CarData.Cars.Remove(carToRemove);
                    Console.WriteLine("Car Information Delete Sucsesfuly");
                }
                else
                {
                    Console.WriteLine("bele Id de masin yoxdur !!!");
                }
                await HandleClient(server, remoteEP, ClientData);
            }
        }
    }
}