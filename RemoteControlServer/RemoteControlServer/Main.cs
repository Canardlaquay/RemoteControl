using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using iTunesLib;

namespace RemoteControlServer
{

    public class serv
    {
        
        
        public static void Main(string[] arg)
        {
            List<string> nomCmd = new List<string>();
            List<string> typCmd = new List<string>();
            List<string> tgtCmd = new List<string>();
            List<string> ipList = new List<string>();
            iTunesControl itunes = new iTunesControl();
            string operatingSystem = "";
            string LocalIPv6 = "";
            string LocalIPv4 = "";
            string InternetIPv4 = "";

            try
            {
                if (!File.Exists("config.xml"))
                {
                    Console.WriteLine("No configuration file found, creating one.");
                    XmlCreate("config.xml");
                    XmlRead(ref nomCmd, ref typCmd, ref tgtCmd, ref operatingSystem, ref LocalIPv6, ref LocalIPv4, ref InternetIPv4);
                    Console.WriteLine("New and fresh config file created !");
                }
                else
                {
                    Console.WriteLine("Configuration file found, reading it !");
                    XmlRead(ref nomCmd, ref typCmd, ref tgtCmd, ref operatingSystem, ref LocalIPv6, ref LocalIPv4, ref InternetIPv4);
                    Console.WriteLine("Reading finished !");
                }

                    string ans = "";
                    while (ans == "")
                    {
                        Console.WriteLine("Which IP Adress do you want to choose as the host ?");
                        Console.WriteLine("1) Your local IPv6 which is: {0}", LocalIPv6);
                        Console.WriteLine("2) Your local IPv4 which is: {0}", LocalIPv4);
                        Console.WriteLine("3) Your public IPv4 which is: {0}", InternetIPv4);
                        Console.Write("Write the number of the choice you decided: ");
                        ans = Console.ReadLine();
                        if (Convert.ToInt32(ans) != 1 && Convert.ToInt32(ans) != 2 && Convert.ToInt32(ans) != 3)
                        {
                            Console.WriteLine("Wrong number entered, please try again.");
                            ans = "";
                        }
                        else
                        {
                            break;
                        }
                    }


                    ipList.Add(LocalIPv6);
                    ipList.Add(LocalIPv4);
                    ipList.Add(InternetIPv4);
                IPAddress ipAd = IPAddress.Parse(ipList[Convert.ToInt32(ans) - 1]);

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
                    if (CheckCommand(reponse, ref nomCmd))
                    {
                        ProcessCommand(reponse, ref nomCmd, ref typCmd, ref tgtCmd, ref operatingSystem);
                    }
                    else
                    {
                        Console.WriteLine("No command to execute found ! Skipping...");
                    }
                    ASCIIEncoding asen = new ASCIIEncoding();
                    // answer TODO: s.Send(asen.GetBytes("This is the answer"));
                    //Console.WriteLine("Proof that the answer was sent");
                    s.Close();

                    
                }
                

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
                Console.WriteLine("Exiting...");
                Console.ReadLine();
                Environment.Exit(0);
                
            }
            
        }


        public static bool CheckCommand(string rep, ref List<string> nomCmd)
        {
            for (int i = 0; i < nomCmd.Count; i++)
            {
                if (rep == nomCmd[i].ToString())
                    return true;
            }
            return false;
        }

        public static void XmlRead(ref List<string> nomCmd, ref List<string> typCmd, ref List<string> tgtCmd, ref string operatingSystem, ref string LocalIPv6, ref string LocalIPv4, ref string InternetIPv4)
        {
            XmlTextReader reader = new XmlTextReader("config.xml");
            while (reader.Read())
            {
                if(reader.NodeType == XmlNodeType.Element)
                {
                        string nameElem = reader.Name;
                        while (reader.MoveToNextAttribute())
                        {// Read the attributes.
                            nomCmd.Add(nameElem);
                            typCmd.Add(reader.Name);
                            tgtCmd.Add(reader.Value);
                        }
                }

            }

            XmlDocument xml = new XmlDocument();
            xml.Load("config.xml");
            XmlNodeList xnList = xml.SelectNodes("/GlobalInfo");
            foreach (XmlNode xn in xnList)
            {
                operatingSystem = xn["OS"].InnerText;
                LocalIPv6 = xn["LocalIPv6"].InnerText;
                LocalIPv4 = xn["LocalIPv4"].InnerText;
                InternetIPv4 = xn["InternetIPv4"].InnerText;
            }


        }

        public static void XmlCreate(string path)
        {

            XmlTextWriter myXmlTextWriter = new XmlTextWriter("config.xml", null);
            myXmlTextWriter.Formatting = Formatting.Indented;
            myXmlTextWriter.WriteStartDocument(false);
            myXmlTextWriter.WriteComment("This is the configuration file for commands that are listened by the server. You can put your owns there.");
            myXmlTextWriter.WriteComment("You can easily add commands by adding an other node with the target attribute next to it.");
            myXmlTextWriter.WriteComment("example: <nameofthecommand target=\"command to execute in console\" />");
            myXmlTextWriter.WriteComment("Do not modify any information in the GlobalInfo node");


            myXmlTextWriter.WriteStartElement("GlobalInfo");
            myXmlTextWriter.WriteElementString("OS", Environment.OSVersion.ToString());
            string host = Dns.GetHostName();
            IPHostEntry ip = Dns.GetHostEntry(host);
            string ipLocalv6 = "";
            string ipLocalv4 = "";
            WebClient webClient = new WebClient();
            string ipIntv4 = webClient.DownloadString("http://myip.ozymo.com/");
            foreach (IPAddress ipp in ip.AddressList)
            {
                if (ipp.IsIPv6LinkLocal == true)
                {
                    ipLocalv6 = ipp.ToString();
                }
                if (ipp.ToString().StartsWith("192.") || ipp.ToString().StartsWith("172.") || ipp.ToString().StartsWith("10."))
                {
                    ipLocalv4 = ipp.ToString();
                }
            }
            myXmlTextWriter.WriteElementString("LocalIPv6", ipLocalv6);
            myXmlTextWriter.WriteElementString("LocalIPv4", ipLocalv4);
            myXmlTextWriter.WriteElementString("InternetIPv4", ipIntv4);
            myXmlTextWriter.WriteElementString("Version", "1000");



            myXmlTextWriter.WriteStartElement("Unix");
            myXmlTextWriter.WriteStartElement("writesomething");
            myXmlTextWriter.WriteAttributeString("exec", "echo this was written in the console");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteStartElement("exit");
            myXmlTextWriter.WriteAttributeString("exec", "exit");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteStartElement("screen");
            myXmlTextWriter.WriteAttributeString("exec", "screen");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteEndElement();


            myXmlTextWriter.WriteStartElement("Windows");
            myXmlTextWriter.WriteComment("Use the exec parameter with an argument to execute files easily");
            myXmlTextWriter.WriteStartElement("calculator");
            myXmlTextWriter.WriteAttributeString("exec", "calc");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteComment("Just specify the correct path for the app !");
            myXmlTextWriter.WriteStartElement("calculator");
            myXmlTextWriter.WriteAttributeString("exec", "C:\\Windows\\notepad.exe");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteComment("Here, it's remotely controlling iTunes (via a proper library).");
            myXmlTextWriter.WriteComment("All the commands will be listed in the README at https://github.com/Canardlaquay/RemoteControl");
            myXmlTextWriter.WriteStartElement("itunesPause");
            myXmlTextWriter.WriteAttributeString("itunes", "pause");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteStartElement("itunesNextTrack");
            myXmlTextWriter.WriteAttributeString("itunes", "playNextTrack");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteStartElement("itunesPlay");
            myXmlTextWriter.WriteAttributeString("itunes", "play");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteStartElement("itunesPreviousTrack");
            myXmlTextWriter.WriteAttributeString("itunes", "playPreviousTrack");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteStartElement("itunesIncreaseVolume");
            myXmlTextWriter.WriteAttributeString("itunes", "volIncrease");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteStartElement("itunesDecreaseVolume");
            myXmlTextWriter.WriteAttributeString("itunes", "volDecrease");
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteEndElement();



            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
        }

        public static void ProcessCommand(string rep, ref List<string> nomCmd, ref List<string> typCmd, ref List<string> tgtCmd, ref string operatingSystem)
        {
            for(int i = 0; i < nomCmd.Count; i++)
            {
                if (rep == nomCmd[i])
                {
                    
                         if (operatingSystem.Contains("Windows"))
                         {
                             switch (typCmd[i])
                             {
                                     //console commands only, mainly for executing apps.
                                 case "exec":
                                     Process.Start(tgtCmd[i]);
                                     break;

                                     //itunes Part !
                                 case "itunes":
                                     iTunesControl it = new iTunesControl();
                                     switch (tgtCmd[i])
                                     {
                                         case "play":
                                             it.playMusic();
                                             break;
                                         case "stop":
                                             it.stopMusic();
                                             break;
                                         case "pause":
                                             it.pauseMusic();
                                             break;
                                         case "volIncrease":
                                             it.volume += 10;
                                             break;
                                         case "volDecrease":
                                             it.volume -= 10;
                                             break;
                                         case "playNextTrack":
                                             it.playNextTrack();
                                             break;
                                         case "playPreviousTrack":
                                             it.playPreviousTrack();
                                             break;
                                         default:
                                             break;
                                     }
                                     break;
                                 default:
                                     break;
                             }
                         }
                         if (operatingSystem.Contains("Unix"))
                         {
                             switch (typCmd[i])
                             {
                                 case "exec":
                                     Process.Start(tgtCmd[i]);
                                     break;
                                 default:
                                     break;
                             }
                         }
                    
                }
            }
        }


    }

}
 
