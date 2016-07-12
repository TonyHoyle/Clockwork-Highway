using EH.Common;
using System;
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
                        string line = locationId.ToString() + ", \"" + details[0].name + "\", \"" + details[0].postcode + "\", " + details[0].latitude + ", " + details[0].longitude + ", \"" + details[0].status + "\"";
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
