using Android;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Locations;
using System.Collections.Generic;
using System.Text;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using EH.Common;
using Android.Views.InputMethods;
using Android.Content;
using System;

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName")]
    public class SearchActivity : Activity,
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener,
        global::Android.Gms.Location.ILocationListener
    {
        private GoogleApiClient _googleApiClient;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Search);

            var text = FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);

            text.Threshold = 1;
            text.Adapter = new AddressListAdapter(this);
            text.ItemClick += (sender, args) => { AddressChanged((AddressListAdapter)text.Adapter, args.Position); };

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessFineLocation }, 0);
            }
            else
                InitGoogleApi();
        }

        protected override void OnStart()
        {
            base.OnStart();
            if(_googleApiClient != null)
                _googleApiClient.Connect();
        }

        protected override void OnStop()
        {
            base.OnStop();
            if(_googleApiClient != null)
                _googleApiClient.Disconnect();
        }

        public void OnConnected(Bundle connectionHint)
        {
            var location = LocationServices.FusedLocationApi.GetLastLocation(_googleApiClient);
            if (location != null)
            {
                OnLocationChanged(location);
            }
            else
            {
                LocationRequest request = new LocationRequest();
                request.SetNumUpdates(1);
                request.SetPriority(LocationRequest.PriorityHighAccuracy);
                LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, request, this);
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
        }

        public void OnConnectionSuspended(int cause)
        {
        }

        public void OnLocationChanged(Location location)
        {
            var text = FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);

            LocationServices.FusedLocationApi.RemoveLocationUpdates(_googleApiClient, this);

            var geocoder = new Geocoder(this);
            var addresses = new List<Address>(geocoder.GetFromLocation(location.Latitude, location.Longitude, 1)).ToArray();

            if (addresses.Length > 0)
                text.Hint = DescribeAddress(addresses[0]);

            InputMethodManager input = (InputMethodManager)GetSystemService(Context.InputMethodService);
            input.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);

            SharedData.lastLocation = new LatLon() { Lat = location.Latitude, Lon = location.Longitude };
            UpdatePumpList(location.Latitude, location.Longitude);
        }

        private void AddressChanged(AddressListAdapter adapter, int position)
        {
            var text = FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);
            var address = adapter.GetItem(position);

            text.Text = DescribeAddress(address);

            UpdatePumpList(address.Latitude, address.Longitude);
        }

        private async void UpdatePumpList(double latitude, double longitude)
        {
            var view = FindViewById<ListView>(Resource.Id.listPumps);
            var progress = FindViewById<ProgressBar>(Resource.Id.progressBar);

            view.EmptyView = progress;

            view.Adapter = null;

            var eh = new EHApi();

            try
            {
                var pumps = await eh.getPumpListAsync(latitude, longitude, SharedData.vehicle);
                view.Adapter = new PumpListAdapter(this, pumps);
                view.ItemClick += (sender, args) => { OnItemClick((PumpListAdapter)view.Adapter, args.Position); };
            }
            catch (EHApi.EHApiException e)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't get pump list: "+e.Message);
            }
            view.EmptyView = null;
        }

        private void OnItemClick(PumpListAdapter adapter, int position)
        {
            var pump = adapter.GetItem(position);

            Intent i = new Intent(this, typeof(LocationActivity));
            i.PutExtra("locationId", Convert.ToInt32(pump.locationId));
            i.PutExtra("latitude", Convert.ToDouble(pump.latitude));
            i.PutExtra("longitude", Convert.ToDouble(pump.longitude));
            i.PutExtra("name", pump.name);
            StartActivity(i);
        }

        private string DescribeAddress(Address addr)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < addr.MaxAddressLineIndex; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(addr.GetAddressLine(i));
            }
            return sb.ToString();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode != 0)
                return;

            if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                InitGoogleApi();
         
        }

        private void InitGoogleApi()
        {
            if (_googleApiClient == null)
            {
                _googleApiClient = new GoogleApiClient.Builder(this)
                    .AddConnectionCallbacks(this)
                    .AddOnConnectionFailedListener(this)
                    .AddApi(LocationServices.API)
                    .Build();
                _googleApiClient.Connect();
            }
        }
    }

}

