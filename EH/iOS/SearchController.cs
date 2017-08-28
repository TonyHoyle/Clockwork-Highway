using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using TonyHoyle.EH;
using UIKit;

namespace ClockworkHighway.iOS
{
	public partial class SearchController : UITableViewController, ICLLocationManagerDelegate
	{
		CLLocationManager locationManager = new CLLocationManager();
		CLLocation location;
        UISearchController resultSearchController;
        LocationSearchTable locationSearchTable;

        public SearchController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			locationManager.Delegate = this;
			locationManager.DesiredAccuracy = CLLocation.AccuracyBest;
			locationManager.RequestWhenInUseAuthorization();
			locationManager.RequestLocation();

            locationSearchTable = (LocationSearchTable)Storyboard.InstantiateViewController("LocationSearchTable");
            resultSearchController = new UISearchController(locationSearchTable)
            {
                SearchResultsUpdater = locationSearchTable
            };
            locationSearchTable.lastLocation = location;

			var searchBar = resultSearchController.SearchBar;
			searchBar.SizeToFit();
			searchBar.Placeholder = "Search for places";
			NavigationItem.TitleView = searchBar;

			resultSearchController.HidesNavigationBarDuringPresentation = false;
            resultSearchController.DimsBackgroundDuringPresentation = true;
            DefinesPresentationContext = true;

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("LoggedIn"), async (obj) => { await UpdatePumps(); });
        }

		[Export("locationManager:didChangeAuthorizationStatus:")]
		public void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
		{
			if (status == CLAuthorizationStatus.AuthorizedWhenInUse)
				locationManager.RequestLocation();
		}

		[Export("locationManager:didUpdateLocations:")]
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public async void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
			location = locations[0];
			locationSearchTable.lastLocation = location;
            if(Shared.api.Login.IsLoggedIn)
                await UpdatePumps();
		}

		[Export("locationManager:didFailWithError:")]
		public void Failed(CLLocationManager manager, NSError error)
		{
			location = new CLLocation();
		}

        private async Task UpdatePumps()
        {
            List<EHApi.Pump> pumps;

            var eh = Shared.api;

			try
			{
                pumps = await eh.getPumpListAsync(location.Coordinate.Latitude, location.Coordinate.Longitude);
                TableView.Source = new PumpListSource(pumps.ToArray());
                TableView.ReloadData();
			}
			catch (EHApi.EHApiException e)
			{
				Console.WriteLine("Couldn't get pump list: " + e.Message);
				Toaster.Toast(this, "Error communicating with Electric Highway Servers");
			}
		}
	}
}
