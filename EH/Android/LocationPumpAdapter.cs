using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using EH.Common;

namespace EH.Android
{
    class LocationPumpDetails
    {
        public EHApi.Connector connector { get; set; }
        public EHApi.ConnectorCost cost { get; set; }
    }

    class LocationPumpAdapter : ArrayAdapter<LocationPumpDetails>
    {
        private class LocationPumpTags : Java.Lang.Object
        {
            public TextView name;
            public CheckedTextView compatible;
            public TextView status;
            public TextView cost;
        }

        public LocationPumpAdapter(Context context) : base(context, 0)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            LocationPumpTags tags;

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.LocationPumpLine, parent, false);
                tags = new LocationPumpTags();
                tags.name = view.FindViewById<TextView>(Resource.Id.name);
                tags.compatible = view.FindViewById<CheckedTextView>(Resource.Id.compatible);
                tags.status = view.FindViewById<TextView>(Resource.Id.status);
                tags.cost = view.FindViewById<TextView>(Resource.Id.cost);
                view.Tag = tags;
            }

            tags = (LocationPumpTags)view.Tag;

            var item = GetItem(position);

            tags.name.Text = item.connector.name;
            tags.compatible.Checked = item.connector.compatible.Length > 0;
            tags.status.Text = item.connector.status;
            tags.cost.Text = "£" + item.cost.baseCost + " per " + item.cost.sessionDuration + " minutes";

            return view;
        }

        public void SetItems(List<EHApi.Connector> connectors, List<EHApi.ConnectorCost> costs)
        {
            this.Clear();
            for(int i=0; i<connectors.Count; i++)
            {
                var details = new LocationPumpDetails();
                details.connector = connectors[i];
                details.cost = costs[i];
                this.Add(details);
            }
            NotifyDataSetChanged();
        }
    }
}