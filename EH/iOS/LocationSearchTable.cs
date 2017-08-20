using Foundation;
using System;
using UIKit;
using MapKit;
using CoreLocation;
using System.Text;
using AddressBookUI;

namespace ClockworkHighway.iOS
{
    public partial class LocationSearchTable : UITableViewController, IUISearchResultsUpdating
    {
        MKMapItem[] matchingItems;
        public CLLocation lastLocation { get; set; }

        public LocationSearchTable (IntPtr handle) : base (handle)
        {
        }

        public void UpdateSearchResultsForSearchController(UISearchController searchController)
        {
            var request = new MKLocalSearchRequest();
            request.NaturalLanguageQuery = searchController.SearchBar.Text;
            if(lastLocation != null)
                request.Region = new MKCoordinateRegion(lastLocation.Coordinate, new MKCoordinateSpan(500,500));
            var search = new MKLocalSearch(request);
            search.Start((response, error) =>
            {
                if (response != null)
                {
                    matchingItems = response.MapItems;
                    TableView.ReloadData();
                }
                else
                    Console.WriteLine(error);
            });
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            if (matchingItems != null)
                return matchingItems.Length;
            else
                return 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("cell", indexPath);
            var selectedItem = matchingItems[indexPath.Row].Placemark;
            cell.TextLabel.Text = selectedItem.Name;
            cell.DetailTextLabel.Text = FormatAddress(selectedItem);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var selectedItem = matchingItems[indexPath.Row].Placemark;

            DismissViewController(true, null);       
        }

        private string FormatAddress(MKPlacemark item)
        {
            NSArray dict = (NSArray)item.AddressDictionary["FormattedAddressLines"];
            StringBuilder sb = new StringBuilder();
            for (nuint i = 0; i < dict.Count; i++)
            {
                if(i > 0) sb.Append(", ");
                sb.Append(dict.GetItem<NSString>(i));
            }
            return sb.ToString(); 
        }
    }
}