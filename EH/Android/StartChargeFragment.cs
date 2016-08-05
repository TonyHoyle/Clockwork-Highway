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

namespace ClockworkHighway.Android
{
    public class StartChargeFragment : DialogFragment
    {
        private int _connectorId;
        private string _cardId;
        private int _pumpId;
        private TextView _cvv;
        private List<EHApi.ConnectorCost> _connectorCost;

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
            var price = view.FindViewById<TextView>(Resource.Id.price);
            _cvv = view.FindViewById<TextView>(Resource.Id.cvv);

            foreach (var c in SharedData.login.Cards)
            {
                cards.Add(c.cardType + " " + c.lastDigits);
            }

            cardList.Adapter = new ArrayAdapter<string>(Context, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, cards.ToArray());
            cardList.SetSelection(SharedData.login.DefaultCardIndex);
            cardList.ItemSelected += (obj, e) => { _cardId = SharedData.login.Cards[e.Position].cardId; };

            var compatibleConnectors = new List<EHApi.Connector>();

            foreach (var c in location.connector)
            {
                if (c.compatible.Length > 0)
                {
                    connectors.Add(c.name);
                    compatibleConnectors.Add(c);
                }

            }

            connectorList.Adapter = new ArrayAdapter<string>(Context, global::Android.Resource.Layout.SimpleListItemSingleChoice, connectors.ToArray());
            connectorList.SetItemChecked(0, true);
            connectorList.ItemSelected += (obj, e) => { _connectorId = compatibleConnectors[e.Position].connectorId; };

            if (connectors.Count < 2)
            {
                connectorList.Visibility = ViewStates.Gone;
                connectorPrompt.Visibility = ViewStates.Gone;
            }
            _connectorId = compatibleConnectors[0].connectorId;
            _cardId = SharedData.login.Card.cardId;
            _pumpId = location.pumpId;

            pumpId.Text = location.pumpId.ToString();
            locationName.Text = location.name;
            decimal pp;
            int pm;

            // This is all because async is a bloody virus.. there's no way of calling the API on a non-void function.
            using (var h = new Handler(Looper.MainLooper))
                h.Post(async () =>
                {
                    try
                    {
                        var eh = SharedData.login.Api;
                        var connectorDetails = await eh.getPumpConnectorsAsync(SharedData.login.Username, SharedData.login.Password, Convert.ToInt32(location.pumpId), SharedData.deviceId, SharedData.login.Vehicle);
                        pp = connectorDetails.connectorCost[0].baseCost;
                        pm = connectorDetails.connectorCost[0].sessionDuration;
                        _connectorCost = connectorDetails.connectorCost;
                    }
                    catch (EHApi.EHApiException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        pp = 5;
                        pm = 30;
                    }

                    price.Text = "Ecotricity charge £" + pp.ToString() + " per " + pm.ToString() + " minute charge session.  All transactions are strictly between Ecotricity and the Car Owner.";
                });


            price.Text = "";
  
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.startCharge)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { });

            return builder.Create();
        }

        private async void DoCharge()
        {
            var cvv = _cvv.Text;

            if (cvv.Length != 3)
            {
                _cvv.Error = Context.Resources.GetString(Resource.String.entercvv);
                return;
            }

            if (_connectorCost == null)
            {
                var t = Toast.MakeText(Context, "No connector price available", ToastLength.Long);
                t.Show();
                return;
            }

            var eh = SharedData.login.Api;
            string sessionId = null;

            foreach(var c in _connectorCost)
            {
                if(c.connectorId == _connectorId)
                {
                    sessionId = c.sessionId;
                    break;
                }
            }

            if (sessionId == null)
            {
                var t = Toast.MakeText(Context, "No session id", ToastLength.Long);
                t.Show();
                return;
            }

            var progressDialog = global::Android.App.ProgressDialog.Show(Context, Context.GetString(Resource.String.startCharge), Context.GetString(Resource.String.requestingCharge));
            var result = await eh.startChargeSessionAsync(SharedData.login.Username, SharedData.login.Password, SharedData.deviceId, _pumpId, _connectorId, cvv, _cardId, sessionId);
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

                var t = Toast.MakeText(Context, text, ToastLength.Long);
                t.Show();
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