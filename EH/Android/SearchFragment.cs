using System;

using Android;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Locations;
using Android.Views.InputMethods;
using Android.Content;
using Android.Views;
using Android.Support.V4.App;
using TonyHoyle.EH;
using Newtonsoft.Json;
using Android.Support.V4.View;
using Android.Util;
using System.Diagnostics.CodeAnalysis;

namespace ClockworkHighway.Android
{
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class SearchFragment : Fragment,
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener,
        global::Android.Gms.Location.ILocationListener
    {
        private GoogleApiClient _googleApiClient;
        private FoundAddress _lastAddress;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.search, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            if (savedInstanceState != null)
            {
                if(savedInstanceState.ContainsKey("lastAddress"))
                   _lastAddress = JsonConvert.DeserializeObject<FoundAddress>(savedInstanceState.GetString("lastAddress"));
            }



			var qrcode = Activity.FindViewById<ImageButton>(Resource.Id.qrcode);
            if (!Context.PackageManager.HasSystemFeature(PackageManager.FeatureCamera))
                qrcode.Visibility = ViewStates.Gone;
            qrcode.Click += (sender, e) => { OnQrCodeClicked(); };

			// This is a child of the coordinator layout in the toolbar
			var text = Activity.FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);
            var scr = View.FindViewById<RelativeLayout>(Resource.Id.scroller);

            text.Threshold = 1;
            text.Adapter = new AddressListAdapter(Context);
            text.ItemClick += (sender, args) => { AddressChanged((AddressListAdapter)text.Adapter, args.Position); };
            scr.RequestFocus();
            text.Touch += (sender, args) =>
            {
                bool handled = false;
                if (args.Event.Action == MotionEventActions.Up)
                {
                    if (args.Event.GetX() >= (text.Width - text.TotalPaddingRight))
                    {
                        if (_googleApiClient != null)
                        {
                            RequestLocation();
                            handled = true;
                        }
                    }
                }

                args.Handled = handled;
            };

            var pumps = View.FindViewById<ListView>(Resource.Id.listPumps);
            pumps.ItemClick += (sender, args) => { OnItemClick((PumpListAdapter)pumps.Adapter, args.Position); };

            ViewCompat.SetNestedScrollingEnabled(pumps, true);

            if (ActivityCompat.CheckSelfPermission(Context, Manifest.Permission.AccessFineLocation) != Permission.Granted)
                RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation }, 0);
            else
                Initialise();

            if(savedInstanceState == null)
                CheckExistingChargeStatus();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnStop()
        {
            base.OnStop();
        }

        public void OnConnected(Bundle connectionHint)
        {
            var location = LocationServices.FusedLocationApi.GetLastLocation(_googleApiClient);
            if (location != null)
                OnLocationChanged(location);
            else
                RequestLocation();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
        }

        public void OnConnectionSuspended(int cause)
        {
        }

        public async void OnLocationChanged(Location location)
        {
            SharedData.lastLocation = new GoogleApi.LatLong() { lat = location.Latitude, lng = location.Longitude };

            if (_lastAddress == null)
            {
                try
                {
                    var addresses = await SharedData.googleApi.lookupLocationAsync(location.Latitude, location.Longitude);

                    if ((addresses != null) && (addresses.Count > 0))
                    {
                        var addr = new FoundAddress(addresses[0]);
                        SetToAddress(addr);
                    }
                }
                catch(GoogleApi.GoogleApiException e)
                {
                    Log.Debug(SharedData.APP, e.Message);
                }
            }
        }

        private void AddressChanged(AddressListAdapter adapter, int position)
        {
            var address = adapter.GetItem(position);

            SetToAddress(address);
        }

        private async void SetToAddress(FoundAddress address)
        {
            var text = Activity.FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);
            var scr = View.FindViewById<RelativeLayout>(Resource.Id.scroller);

            text.SetText(new Java.Lang.String(address.Title), false);
            scr.RequestFocus();
            _lastAddress = address;

            InputMethodManager input = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            if(input != null)
                input.HideSoftInputFromWindow(Activity.CurrentFocus.WindowToken, 0);

            if(address.Location == null)
            {
                try
                {
                    var addr = await SharedData.googleApi.lookupPlaceIdAsync(address.PlaceId);
                    if ((addr != null) && (addr.Count >= 0))
                        address.Location = addr[0].geometry.location;
                }
                catch(GoogleApi.GoogleApiException e)
                {
                    Log.Debug(SharedData.APP, e.Message);
                }
            }

            if(address.Location != null)
                UpdatePumpList(address.Location.lat, address.Location.lng);
        }

        private async void UpdatePumpList(double latitude, double longitude)
        {
            var view = View.FindViewById<ListView>(Resource.Id.listPumps);
            var progress = View.FindViewById<ProgressBar>(Resource.Id.progressBar);

            view.EmptyView = progress;

            view.Adapter = null;

            var eh = SharedData.api;

            try
            {
                var pumps = await eh.getPumpListAsync(latitude, longitude);
				view.Adapter = new PumpListAdapter(Context, pumps);
            }
            catch (EHApi.EHApiException e)
            {
                Log.Debug(SharedData.APP, "Couldn't get pump list: "+e.Message);
                Toast.MakeText(Activity.ApplicationContext, "Error communicating with Electric Highway Servers", ToastLength.Short).Show();
			}
            view.EmptyView = null;
        }

        private void OnItemClick(PumpListAdapter adapter, int position)
        {
            var pump = adapter.GetItem(position);

            var scr = View.FindViewById<RelativeLayout>(Resource.Id.scroller);
            scr.RequestFocus();

            Intent i = new Intent(Context, typeof(LocationActivity));
            i.PutExtra("locationId", Convert.ToInt32(pump.locationId));
            i.PutExtra("latitude", Convert.ToDouble(pump.latitude));
            i.PutExtra("longitude", Convert.ToDouble(pump.longitude));
            i.PutExtra("name", pump.name);
            StartActivity(i);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode != 0)
                return;

            if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                Initialise();
         
        }

        private void Initialise()
        {
            if (_googleApiClient == null)
            {
                _googleApiClient = new GoogleApiClient.Builder(Context)
                    .AddConnectionCallbacks(this)
                    .AddOnConnectionFailedListener(this)
                    .EnableAutoManage(Activity, this)
                    .AddApi(LocationServices.API)
                    .Build();
                _googleApiClient.Connect();
            }

            if (_lastAddress != null)
                SetToAddress(_lastAddress);
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            if (_lastAddress != null)
                outState.PutString("lastAddress", JsonConvert.SerializeObject(_lastAddress));
        }

        private void RequestLocation()
        {
            LocationRequest request = new LocationRequest();
            request.SetNumUpdates(1);
            request.SetPriority(LocationRequest.PriorityHighAccuracy);
            _lastAddress = null;
            LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, request, this);
        }

        private async void CheckExistingChargeStatus()
        {
            try
            {
                var status = await SharedData.api.getChargeStatusAsync();

                if (status.started != null && status.started != "" && !status.completed && status.status != "Retry")
                {
                    Intent i = new Intent(Context, typeof(ChargingActivity));
                    i.PutExtra("sessionId", status.sessionId);
                    i.PutExtra("pumpId", status.pumpId);
                    i.PutExtra("connectorId", status.pumpConnector);
                    StartActivity(i);
                }
            }
            catch(EHApi.EHApiException e)
            {
                Log.Debug(SharedData.APP, e.Message);
            }
        }

        private void OnQrCodeClicked()
        {
			Intent i = new Intent(Context, typeof(BarcodeActivity));
            i.AddFlags(ActivityFlags.NoHistory);
			StartActivityForResult(i,0);
		}

        public override async void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == 0 && data != null && data.HasExtra("pumpId"))
            {
                var pumpId = data.GetStringExtra("pumpId");

                var location = new EHApi.LocationDetails();

                if (pumpId != null)
                {
                    var pump = await SharedData.api.getPumpConnectorsAsync(Convert.ToInt32(pumpId));

                    if(pump != null)
                    {
	                    location.connector = pump.connector;
	                    location.name = pump.name;
	                    location.pumpId = pump.pumpId;

                        Intent i = new Intent(Context, typeof(StartChargeActivity));
	                    i.PutExtra("location", Newtonsoft.Json.JsonConvert.SerializeObject(location));
                        StartActivity(i);
                    }
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}

