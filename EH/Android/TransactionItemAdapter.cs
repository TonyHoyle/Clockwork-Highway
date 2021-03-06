using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using TonyHoyle.EH;
using Android.Util;
using System.Threading;

namespace ClockworkHighway.Android
{
    class TransactionItemAdapter : ArrayAdapter<EHApi.ContractAccount>
    {
        public List<EHApi.ContractAccount> Transactions
        {
            set
            {
                this.Clear();
                this.AddAll(value);
                NotifyDataSetInvalidated();
            }
        }

        private class TransactionTags : Java.Lang.Object
        {
            public TextView date;
            public TextView session;
            public TextView pump;
            public TextView cost;
        }

        public TransactionItemAdapter(Context context) : base(context, 0)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            TransactionTags tags;

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.transactionline, parent, false);
                tags = new TransactionTags();
                tags.date = view.FindViewById<TextView>(Resource.Id.date);
                tags.session = view.FindViewById<TextView>(Resource.Id.session);
                tags.pump = view.FindViewById<TextView>(Resource.Id.pump);
                tags.cost = view.FindViewById<TextView>(Resource.Id.cost);
                view.Tag = tags;
            }

            tags = (TransactionTags)view.Tag;

            var item = GetItem(position);

            tags.session.Text = item.sessionId;
            tags.date.Text = DateTime.ParseExact(item.date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToLongDateString();
            tags.cost.Text = item.totalCost.ToString("C2");

            if (SharedData.api.pumpConnectorsAreCached(item.pumpId))
                tags.pump.Text = SharedData.api.getPumpConnectorsAsync(item.pumpId).Result.name;
            else
            {
                tags.pump.Text = item.pumpId.ToString();
                loadPumpName(item.pumpId);
            }

            return view;
        }

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        private async void loadPumpName(int pumpId)
        {
            await _lock.WaitAsync();
            try
            {
				/*var pumpDetails =*/ await SharedData.api.getPumpConnectorsAsync(pumpId);
                NotifyDataSetChanged(); // Do this to refresh the list properly, since items get re-used.  
            }
            catch(EHApi.EHApiException e)
            {
                Log.Debug(SharedData.APP, e.Message);
            }
            _lock.Release();
        }
    }
}