using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Widget;
using Android.Locations;
using Android.Views;
using Java.Util;

namespace EH.Android
{
    class AddressFilter : Filter
    {
        private ArrayAdapter<Address> _adapter;
        private Context _context;
        private List<Address> _lastResult;

        public AddressFilter(Context context, ArrayAdapter<Address> adapter)
        {
            _context = context;
            _adapter = adapter;
            _lastResult = new List<Address>();
        }

        protected override FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
        {
            if (constraint == null)
                return null;

            if (!Geocoder.IsPresent)
                return null;

            var address = constraint.ToString();
            var result = new FilterResults();

            var geocoder = new Geocoder(_context, Locale.Uk);
            var addresses = geocoder.GetFromLocationName(address, 50);
            var resultList = new Java.Util.ArrayList();


            foreach(var A in addresses)
            {
                if (A.CountryCode != "GB")
                    continue;

                resultList.Add(A);
            }

            result.Values = resultList;
            return result;
        }

        protected override void PublishResults(Java.Lang.ICharSequence constraint, FilterResults results)
        {
            _adapter.Clear();
            if(results != null)
            {
                var list = (Java.Util.ArrayList)results.Values;
                for(int i=0; i<list.Size(); i++)
                    _adapter.Add(list.Get(i));

            }
            _adapter.NotifyDataSetChanged();  // notifies the data with new filtered values
        }
    }

    class AddressListAdapter : ArrayAdapter<Address>
    {
        private AddressFilter _filter;

        public AddressListAdapter(Context context) : base(context, global::Android.Resource.Layout.SimpleDropDownItem1Line)
        {
            _filter = new AddressFilter(context, this);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
           View view = base.GetView(position, convertView, parent);

            var item = GetItem(position);
            view.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = DescribeAddress(item);

            return view;
        }

        public override Filter Filter { get { return _filter; } }

        private string DescribeAddress(Address addr)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0; i<addr.MaxAddressLineIndex; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(addr.GetAddressLine(i));
            }
            return sb.ToString();
        }
    }
}
