using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using EH.Common;
using Newtonsoft.Json;
using Android.Support.V7.App;
using System.Collections.Generic;
using Android.Widget;
using System;
using System.Threading;
using Android.Content;

namespace EH.Android
{
    public class StartChargeFragment : DialogFragment
    {
        private string _connectorId;
        private string _cardId;
        private string _pumpId;
        private TextView _cvv;
        private List<EHApi.ConnectorCost> _connectorCost;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Calling GetLayoutInflator for the dialog here causes a recursive loop as DialogFragment.GetLayoutInflator
            // contains a call to OnCreateDialog (which seems bogus but unfixed in latest android).
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.StartCharge, null);

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

            pumpId.Text = location.pumpId;
            locationName.Text = location.name;
            float pp;
            float pm;

            // This is all because async is a bloody virus.. there's no way of calling the API on a non-void function.
            using (var h = new Handler(Looper.MainLooper))
                h.Post(async () =>
                {
                    try
                    {
                        var eh = new EHApi();
                        var connectorDetails = await eh.getPumpConnectorsAsync(SharedData.login.Username, SharedData.login.Password, Convert.ToInt32(location.pumpId), SharedData.deviceId, SharedData.login.Vehicle);
                        pp = (float)Convert.ToDouble(connectorDetails.connectorCost[0].baseCost);
                        pm = Convert.ToInt32(connectorDetails.connectorCost[0].sessionDuration);
                        _connectorCost = connectorDetails.connectorCost;
                    }
                    catch (EHApi.EHApiException e)
                    {
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
                return;

            var eh = new EHApi();
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
                return;

            var result = await eh.startChargeSessionAsync(SharedData.login.Username, SharedData.login.Password, SharedData.deviceId, _pumpId, _connectorId, cvv, _cardId, sessionId);
            if(result)
                Dismiss();
            else
            {
                var t = Toast.MakeText(Context, "Unable to initiate charge", ToastLength.Long);
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