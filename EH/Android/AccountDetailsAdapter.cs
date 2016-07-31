using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace EH.Android
{
    class AccountDetailsLine
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    };

    class AccountDetailsAdapter : ArrayAdapter<AccountDetailsLine>
    {
        private class AccountDetailsTags : Java.Lang.Object
        {
            public TextView title;
            public TextView text;
        }

        public AccountDetailsAdapter(Context context, List<AccountDetailsLine> details) : base(context, 0, details.ToArray())
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            AccountDetailsTags tags;

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.accountdetailsline, parent, false);
                tags = new AccountDetailsTags();
                tags.title = view.FindViewById<TextView>(Resource.Id.title);
                tags.text = view.FindViewById<TextView>(Resource.Id.text);
                view.Tag = tags;
            }

            tags = (AccountDetailsTags)view.Tag;

            var item = GetItem(position);

            tags.title.Text = item.Title;
            tags.text.Text = item.Text;

            return view;
        }
    }
}