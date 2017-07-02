using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using Android.Views;
using TonyHoyle.EH;
using Android.Util;

namespace ClockworkHighway.Android
{
    class FoundAddress
    {
        public FoundAddress()
        {
        }

        public FoundAddress(GoogleApi.Address addr)
        {
            Title = addr.formatted_address;
            Location = new GoogleApi.LatLong() { lat = addr.geometry.location.lat, lng = addr.geometry.location.lng };
            PlaceId = addr.place_id;
        }

        public string Title { get; set; }
        public GoogleApi.LatLong Location { get; set; }
        public string PlaceId { get; set; }
    }

    class FoundAddressJava : Java.Lang.Object
    {
        public FoundAddress Addr { get; private set; }
        public FoundAddressJava(FoundAddress addr)
        {
            Addr = addr;
        }
    }


    class AddressFilter : Filter
    {
        private ArrayAdapter<FoundAddress> _adapter;

        public AddressFilter(Context context, ArrayAdapter<FoundAddress> adapter)
        {
            _adapter = adapter;
        }

        protected override FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
        {
            if (constraint == null)
                return null;

            var address = constraint.ToString();
            var result = new FilterResults();

            try
            {
                var addresses = SharedData.googleApi.autocompleteAsync(address,"uk", SharedData.lastLocation).GetAwaiter().GetResult();
                var resultList = new Java.Util.ArrayList();

                foreach (var A in addresses)
                {
                    resultList.Add(new FoundAddressJava(new FoundAddress() { Title = A.description, PlaceId = A.place_id }));
                }

                result.Values = resultList;
            }
            catch(GoogleApi.GoogleApiException e)
            {
                Log.Debug(SharedData.APP, e.Message);
            }
            return result;
        }

        protected override void PublishResults(Java.Lang.ICharSequence constraint, FilterResults results)
        {
            _adapter.Clear();
            if(results != null && results.Values != null)
            {
                var list = (Java.Util.ArrayList)results.Values;
                for(int i=0; i<list.Size(); i++)
                    _adapter.Add(((FoundAddressJava)list.Get(i)).Addr);

            }
            _adapter.NotifyDataSetChanged();  // notifies the data with new filtered values
        }
    }

    class AddressListAdapter : ArrayAdapter<FoundAddress>
    {
        private class AddressTags : Java.Lang.Object
        {
            public TextView text;
        }

        private AddressFilter _filter;

        public AddressListAdapter(Context context) : base(context, 0)
        {
            _filter = new AddressFilter(context, this);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            AddressTags tags;

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.dropdownline, parent, false);
                tags = new AddressTags();
                tags.text = view.FindViewById<TextView>(Resource.Id.text);
                view.Tag = tags;
            }

            tags = (AddressTags)view.Tag;

            var item = GetItem(position);

            tags.text.Text = item.Title;

            return view;
        }

        public override Filter Filter { get { return _filter; } }
    }
}
