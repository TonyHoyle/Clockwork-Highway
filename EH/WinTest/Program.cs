using EH.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WinTest
{
    class Program
    {
        static void Main(string[] args)
        {
            EHApi eh = new EHApi();

            var Vehicle = new EHApi.Vehicle()
            {
                make = "Nissan",
                model = "Leaf"
            };

            StreamWriter file = new StreamWriter("locations.csv");
            file.AutoFlush = true;

            for (int locationId = 1; locationId < 200; locationId++)
            {
                try
                {
                    Debug.WriteLine("Trying " + locationId.ToString());
                    var details = eh.getLocationDetailsAsync(locationId, Vehicle).Result;
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
