using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using EH.Common;

namespace EH.Android
{
    class LocationAdapter : ArrayAdapter<EHApi.ConnectorDetails>
    {
        private class LocationTags : Java.Lang.Object
        {
            public TextView status;
            public TextView pumpId;
            public ListView pumpList;
        }

        public LocationAdapter(Context context, List<EHApi.ConnectorDetails> locations) : base(context, 0, locations.ToArray())
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            LocationTags tags;

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.LocationLine, parent, false);
                tags = new LocationTags();
                tags.pumpId = view.FindViewById<TextView>(Resource.Id.pumpid);
                tags.status = view.FindViewById<TextView>(Resource.Id.status);
                tags.pumpList = view.FindViewById<ListView>(Resource.Id.pumplist);
                tags.pumpList.Adapter = new LocationPumpAdapter(Context);             
                view.Tag = tags;
            }

            tags = (LocationTags)view.Tag;

            var item = GetItem(position);

            tags.pumpId.Text = "Pump "+item.pumpId;
            tags.status.Text = item.status;

            ((LocationPumpAdapter)tags.pumpList.Adapter).SetItems(item.connector, item.connectorCost);

            return view;
        }
    }
}