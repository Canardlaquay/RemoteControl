using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;


public class clnt
{

    public static void Main()
    {

        try
        {
            Console.WriteLine("Please enter the IP Adress of the server you want to connect:");
            string ipStr = Console.ReadLine();

            while (true)
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect(ipStr, 8001);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");
                Console.Write("Enter the string to be transmitted : ");


                String str = Console.ReadLine();
                while(str == "")
                {
                    Console.WriteLine("You can't send empty strings");
                    str = Console.ReadLine();
                }
                if(str != "")
                {
                    Stream stm = tcpclnt.GetStream();

                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(str);
                    Console.WriteLine("Transmitting.....");

                    stm.Write(ba, 0, ba.Length);


                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);

                    for (int i = 0; i < k; i++)
                        Console.Write(Convert.ToChar(bb[i]));


                    tcpclnt.Close();
                }
            }
            
        }

        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }

}
