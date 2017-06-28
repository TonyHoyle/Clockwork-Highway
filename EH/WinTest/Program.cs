using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using TonyHoyle.EH;

namespace WinTest
{
    class Program
    {
        static public string getLocalIPV4()
        {
            // Returns ipv4 IP used to make an external connection, so is somewhat resilient to
            // multihoming.
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("54.214.245.161", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        static public int Secs(DateTime dt)
        {
            var delta = dt - new DateTime(1970, 1, 1);
            return Convert.ToInt32(delta.TotalSeconds);
        }

        static void Main(string[] args)
        {
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            var httpClientHandler = new HttpClientHandler
            {
//                Proxy = new WebProxy("http://localhost:8888", false),
//                UseProxy = true
            };
            var httpClient = new HttpClient(httpClientHandler);

            EHApi api = new EHApi(httpClient);

			StreamWriter file = new StreamWriter("locations.csv");
            file.AutoFlush = true;

            if (!api.Login.LoginWithPassword("TonyHoyle", "", "00000000").Result)
                return;

            for (int locationId = 1; locationId < 200; locationId++)
            {
                try
                {
                    Debug.WriteLine("Trying " + locationId.ToString());
                    var details = api.getLocationDetailsAsync(locationId).Result;
                    if (details != null)
                    {
                        string status = "";
                        int count = 0;
 
                        var pumpData = new List<string>();
                        foreach(var pump in details)
                        {
                            if (pump.status != "Swipe card only")
                                count++;
                            pumpData.Add(pump.pumpId.ToString() + " " + pump.pumpModel);
                        }
                        if (count > 0)
                            status = count.ToString() + "/"+details.Count.ToString()+" pumps live";

                        pumpData.Sort();
                        string line = locationId.ToString() + ", \"" + details[0].name + "\", \""+ details[0].location + "\", \"" + details[0].postcode + "\", " + details[0].latitude + ", " + details[0].longitude + ", \"" + status + "\", \""+ string.Join(", ", pumpData) + "\"";
                        Console.WriteLine(line);
                        file.WriteLine(line);
                    }
                }
                catch (EHApi.EHApiException e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            file.Close();
        }
    }
}
