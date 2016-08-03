using System;
using System.Collections.Generic;

using Android.Content;
using Android.Views;
using Android.Widget;
using TonyHoyle.EH;

namespace ClockworkHighway.Android
{
    class PumpListAdapter : ArrayAdapter<EHApi.Pump>
    {
        private class PumpTags : Java.Lang.Object
        {
            public TextView description;
            public TextView location;
            public TextView model;
            public TextView distance;
            public CheckedTextView available;
            public TextView swipeonly;
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
                view = inflater.Inflate(Resource.Layout.pumpline, parent, false);
                tags = new PumpTags();
                tags.description = view.FindViewById<TextView>(Resource.Id.description);
                tags.location = view.FindViewById<TextView>(Resource.Id.location);
                tags.model = view.FindViewById<TextView>(Resource.Id.model);
                tags.distance = view.FindViewById<TextView>(Resource.Id.distance);
                tags.available = view.FindViewById<CheckedTextView>(Resource.Id.available);
                tags.swipeonly = view.FindViewById<TextView>(Resource.Id.swipeonly);
                view.Tag = tags;
            }

            tags = (PumpTags)view.Tag;

            var item = GetItem(position);

            tags.description.Text = item.name;
            tags.location.Text = item.location + ", " + item.postcode;
            tags.model.Text = item.pumpModel;
            tags.distance.Text =  Math.Round(item.distance, 2).ToString() + " Miles";
            if (!item.swipeOnly)
            {
                tags.swipeonly.Visibility = ViewStates.Gone;
                tags.available.Visibility = ViewStates.Visible;
            }
            else
            {
                tags.swipeonly.Visibility = ViewStates.Visible;
                tags.available.Visibility = ViewStates.Gone;
            }
            tags.available.Checked = item.available;

            return view;
        }
    }
}