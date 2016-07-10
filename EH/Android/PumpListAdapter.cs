using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EH.Common;

namespace EH.Android
{
    class PumpListAdapter : ArrayAdapter<EHApi.Pump>
    {
        private class PumpTags : Java.Lang.Object
        {
            public TextView description;
            public TextView model;
            public TextView distance;
            public CheckedTextView available;
            public CheckedTextView swipeonly;
        }

        public PumpListAdapter(Context context, List<EHApi.Pump> pumps) : base(context, 0, pumps.ToArray())
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            PumpTags tags;

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.PumpLine, parent, false);
                tags = new PumpTags();
                tags.description = view.FindViewById<TextView>(Resource.Id.description);
                tags.model = view.FindViewById<TextView>(Resource.Id.model);
                tags.distance = view.FindViewById<TextView>(Resource.Id.distance);
                tags.available = view.FindViewById<CheckedTextView>(Resource.Id.available);
                tags.swipeonly = view.FindViewById<CheckedTextView>(Resource.Id.swipeonly);
                view.Tag = tags;
            }

            tags = (PumpTags)view.Tag;

            var item = GetItem(position);

            tags.description.Text = item.name + ", " + item.location + ", " + item.postcode;
            tags.model.Text = item.pumpModel;
            tags.distance.Text =    Math.Round(item.distance, 2).ToString() + " Miles";
            tags.swipeonly.Checked = item.swipeOnly;
            tags.available.Checked = item.available;

            return view;
        }
    }
}