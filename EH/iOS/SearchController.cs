using System;
using CoreLocation;
using Foundation;
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
            resultSearchController = new UISearchController(locationSearchTable);
            resultSearchController.SearchResultsUpdater = locationSearchTable;
            locationSearchTable.lastLocation = location;

			var searchBar = resultSearchController.SearchBar;
			searchBar.SizeToFit();
			searchBar.Placeholder = "Search for places";
			NavigationItem.TitleView = searchBar;

			resultSearchController.HidesNavigationBarDuringPresentation = false;
            resultSearchController.DimsBackgroundDuringPresentation = true;
            DefinesPresentationContext = true;
   		}

		[Export("locationManager:didChangeAuthorizationStatus:")]
		public void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
		{
			if (status == CLAuthorizationStatus.AuthorizedWhenInUse)
				locationManager.RequestLocation();
		}

		[Export("locationManager:didUpdateLocations:")]
		public void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
		{
			location = locations[0];
			locationSearchTable.lastLocation = location;
		}

		[Export("locationManager:didFailWithError:")]
		public void Failed(CLLocationManager manager, NSError error)
		{
			location = new CLLocation();
		}
	}
}
