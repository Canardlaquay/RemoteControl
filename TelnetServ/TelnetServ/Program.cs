using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace TelnetServ
{

    public class serv
    {

        
        public static void Main()
        {
            
            try
            {
                Console.WriteLine("Please enter your IP (e.g: 8.8.8.8 or 192.168.1.42)");
                string ipStr = Console.ReadLine();
                IPAddress ipAd = IPAddress.Parse(ipStr);

                TcpListener myList = new TcpListener(ipAd, 8001);
                myList.Start();

                Console.WriteLine("The server is running at port 8001...");
                Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");

                while (true)
                {
                   
                    Socket s = myList.AcceptSocket();
                    Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                    byte[] b = new byte[100];
                    int k = s.Receive(b);
                    Console.WriteLine("Recieved...");
                    string reponse = "";
                    for (int i = 0; i < k; i++)
                        reponse += Convert.ToChar(b[i]);
                    Console.WriteLine(reponse);
                    if (CheckCommand(reponse))
                    {
                        ProcessCommand(reponse);
                    }
                    else
                    {
                    }
                    ASCIIEncoding asen = new ASCIIEncoding();
                    s.Send(asen.GetBytes("The string was recieved by the server.\nThis one too."));
                    Console.WriteLine("\nSent Acknowledgement");

                    /* clean up */
                    s.Close();

                    
                }
                

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
                Console.ReadLine();
            }
        }

        private static bool CheckCommand(string rep)
        {
            List<string> AvailCmds = new List<string>();
            createList(ref AvailCmds);
            for (int i = 0; i < AvailCmds.Count; i++)
            {
                if (rep == AvailCmds[i].ToString())
                    return true;
            }
            return false;
        }














        public static void createList(ref List<string> AvailCmds)
        {
            AvailCmds.Clear();
            AvailCmds.Add("exit");
            AvailCmds.Add("screen");
            //AvailCmds.Add("
        }
        public static void ProcessCommand(string rep)
        {
            switch (rep)
            {
                case("exit"):
                    Console.WriteLine("Exit command received, exiting now.");
                    Console.ReadLine();
                    Environment.Exit(0);
                    break;
                case("screen"):
                    Process.Start("screen");
                    break;
                default:
                    break;
            }
        }













    }

}
 
