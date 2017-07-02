using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using TonyHoyle.EH;
using Newtonsoft.Json;
using Android.Support.V7.App;
using System.Collections.Generic;
using Android.Widget;
using System;
using Android.Content;
using Android.Util;

namespace ClockworkHighway.Android
{
    public class StartChargeFragment : DialogFragment
    {
        private int _connectorId;
        private string _cardId;
        private int _pumpId;
        private TextView _cvv;
        private EHApi.Quote _quote;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Calling GetLayoutInflator for the dialog here causes a recursive loop as DialogFragment.GetLayoutInflator
            // contains a call to OnCreateDialog (which seems bogus but unfixed in latest android).
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.startcharge, null);

            var location = JsonConvert.DeserializeObject<EHApi.LocationDetails>(Arguments.GetString("location"));
            var connectors = new List<string>();
            var cards = new List<string>();

            var cardList = view.FindViewById<Spinner>(Resource.Id.cardList);
            var connectorPrompt = view.FindViewById<TextView>(Resource.Id.connectorListText);
            var connectorList = view.FindViewById<ListView>(Resource.Id.connectorList);
            var locationName = view.FindViewById<TextView>(Resource.Id.locationName);
            var pumpId = view.FindViewById<TextView>(Resource.Id.pumpId);
            var payment = view.FindViewById<LinearLayout>(Resource.Id.payment);
            var progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);

            payment.Visibility = ViewStates.Gone;
            progressBar.Visibility = ViewStates.Visible;

            _cvv = view.FindViewById<TextView>(Resource.Id.cvv);

            foreach (var c in SharedData.api.Login.Cards)
            {
                cards.Add(c.cardType + " " + c.lastDigits);
            }

            cardList.Adapter = new ArrayAdapter<string>(Context, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, cards.ToArray());
            cardList.SetSelection(SharedData.api.Login.DefaultCardIndex);
            cardList.ItemSelected += (obj, e) => { _cardId = SharedData.api.Login.Cards[e.Position].cardId; };

            var compatibleConnectors = new List<EHApi.Connector>();

            foreach (var c in location.connector)
            {
                if (c.compatible.Length > 0)
                {
                    connectors.Add(c.name);
                    compatibleConnectors.Add(c);
                }
            }

            if (compatibleConnectors.Count == 0)
                compatibleConnectors.AddRange(location.connector);

            connectorList.Adapter = new ArrayAdapter<string>(Context, global::Android.Resource.Layout.SimpleListItemSingleChoice, connectors.ToArray());
            connectorList.SetItemChecked(0, true);
            connectorList.ItemSelected += (obj, e) => { _connectorId = compatibleConnectors[e.Position].connectorId; };

            if (connectors.Count < 2)
            {
                connectorList.Visibility = ViewStates.Gone;
                connectorPrompt.Visibility = ViewStates.Gone;
            }
            if (compatibleConnectors.Count > 0)
                _connectorId = compatibleConnectors[0].connectorId;
            else
                _connectorId = 0;
            if (SharedData.api.Login.Card != null)
                _cardId = SharedData.api.Login.Card.cardId;
            else
                _cardId = "0";
            _pumpId = location.pumpId;

            pumpId.Text = location.pumpId.ToString();
            locationName.Text = location.name;

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.startCharge)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { });

            new Handler(Looper.MainLooper).Post(() => InitDialog(view));
                                          
            return builder.Create();
        }

        async void InitDialog(View view)
        {
			var payment = view.FindViewById<LinearLayout>(Resource.Id.payment);
			var progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
			var priceList = view.FindViewById<LinearLayout>(Resource.Id.priceList);

			try
            {
                _quote = await SharedData.api.quoteAsync(_pumpId, _connectorId);
            }
            catch (EHApi.EHApiException e)
            {
                Log.Debug(SharedData.APP, "Unable to quote: "+e.Message);
				Toast.MakeText(Context.ApplicationContext, "Unable to communicate with Electric Highway Servers", ToastLength.Short).Show();
				Dismiss();

				_quote = null;
            }

            if (_quote != null)
            {
                if (_quote.@fixed == 0 && _quote.variable.value == 0)
                    payment.Visibility = ViewStates.Gone;
                else
                    payment.Visibility = ViewStates.Visible;
                progressBar.Visibility = ViewStates.Gone;

                var adapter = new PriceListAdapter(Context, _quote.sessionPricing);
                for (int i = 0; i < adapter.Count; i++)
                {
                    View item = adapter.GetView(i, null, null);
                    var parms = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    priceList.LayoutParameters = parms;
                    priceList.AddView(item);
                }
                priceList.Invalidate();
            }
            else
            {
                payment.Visibility = ViewStates.Gone;
                progressBar.Visibility = ViewStates.Gone;
                // Shouldn't happen
            }
         }

        private async void DoCharge()
        {
            var cvv = _cvv.Text;

            if (_quote == null)
                return;

            bool free = _quote.@fixed == 0 && _quote.variable.value == 0;

            if (!free && cvv.Length != 3)
            {
                _cvv.Error = Context.Resources.GetString(Resource.String.entercvv);
                return;
            }

            var eh = SharedData.api;
            string sessionId = _quote.sessionId;

            if (sessionId == null)
            {
                var t = Toast.MakeText(Context.ApplicationContext, "No session id", ToastLength.Long);
                t.Show();
                return;
            }

            var progressDialog = global::Android.App.ProgressDialog.Show(Context, Context.GetString(Resource.String.startCharge), Context.GetString(Resource.String.requestingCharge));
            try
            {
                var result = await eh.startChargeSessionAsync(_pumpId, _connectorId, free ? "" : cvv, free ? "0" : _cardId, sessionId);
                progressDialog.Dismiss();
                if (result.result)
                {
                    Intent i = new Intent(Context, typeof(ChargingActivity));
                    i.PutExtra("sessionId", sessionId);
                    i.PutExtra("pumpId", _pumpId);
                    i.PutExtra("connectorId", _connectorId);
                    StartActivity(i);
                    Dismiss();
                }
                else
                {
                    string text;

                    if (string.IsNullOrEmpty(result.message))
                        text = "Unable to initiate charge";
                    else
                        text = result.message;

                    var t = Toast.MakeText(Context.ApplicationContext, text, ToastLength.Short);
                    t.Show();
                }
            }
            catch (EHApi.EHApiException e)
            {
                Log.Debug(SharedData.APP, "Unable to initiate charge: " + e.Message);
                Toast.MakeText(Context.ApplicationContext, "Unable to initiate charge", ToastLength.Short).Show();
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            AlertDialog dlg = (AlertDialog)Dialog;
            if (dlg != null)
            {
                Button positivePutton = dlg.GetButton((int)DialogButtonType.Positive);
                positivePutton.Click += (sender, args) => { DoCharge(); };
            }
        }
    }
}