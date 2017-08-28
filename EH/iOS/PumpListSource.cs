using System;
using Foundation;
using TonyHoyle.EH;
using UIKit;

namespace ClockworkHighway.iOS
{
    public class PumpListSource : UITableViewSource
    {
        EHApi.Pump[] _pumps;

		public PumpListSource(EHApi.Pump[] pumps)
        {
            _pumps = pumps;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
			// in a Storyboard, Dequeue will ALWAYS return a cell,
			UITableViewCell cell = tableView.DequeueReusableCell("cell");
			// now set the properties as normal
			cell.TextLabel.Text = _pumps[indexPath.Row].name;
			return cell;
		}

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _pumps.Length;
        }
    }
}
