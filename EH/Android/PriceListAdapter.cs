using System;
using System.Collections.Generic;

using Android.Content;
using Android.Views;
using Android.Widget;
using TonyHoyle.EH;

namespace ClockworkHighway.Android
{
    class PriceListAdapter : ArrayAdapter<EHApi.SessionPricing>
    {
        private class SessionTags : Java.Lang.Object
        {
            public TextView title;
            public TextView text;
        }

        public PriceListAdapter(Context context, List<EHApi.SessionPricing> sessions) : base(context, 0, sessions.ToArray())
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            SessionTags tags;

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.priceline, parent, false);
                tags = new SessionTags();
                tags.title = view.FindViewById<TextView>(Resource.Id.priceTitle);
                tags.text = view.FindViewById<TextView>(Resource.Id.priceText);
                view.Tag = tags;
            }

            tags = (SessionTags)view.Tag;

            var item = GetItem(position);

            tags.title.Text = item.title;
            string text = "";
            foreach(var s in item.pricingData)
            {
                text = s.title + " " + s.value + '\n';
            }
            text = text.Remove(text.Length - 1);
            tags.text.Text = text;

            return view;
        }
    }
}