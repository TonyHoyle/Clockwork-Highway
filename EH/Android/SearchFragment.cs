using System;
using System.Collections.Generic;
using System.Text;
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
using EH.Common;

namespace EH.Android
{
    public class SearchFragment : Fragment,
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener,
        global::Android.Gms.Location.ILocationListener
    {
        private GoogleApiClient _googleApiClient;
        private Address _lastAddress;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Search, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            if(savedInstanceState != null)
                _lastAddress = (Address)savedInstanceState.GetParcelable("lastAddress");

            var text = View.FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);

            text.Threshold = 1;
            text.Adapter = new AddressListAdapter(Context);
            text.ItemClick += (sender, args) => { AddressChanged((AddressListAdapter)text.Adapter, args.Position); };
            text.ClearFocus();

            var pumps = View.FindViewById<ListView>(Resource.Id.listPumps);
            pumps.ItemClick += (sender, args) => { OnItemClick((PumpListAdapter)pumps.Adapter, args.Position); };

            if (ActivityCompat.CheckSelfPermission(Context, Manifest.Permission.AccessFineLocation) != Permission.Granted)
                ActivityCompat.RequestPermissions(Activity, new string[] { Manifest.Permission.AccessFineLocation }, 0);
            else
                Initialise();
        }

        public override void OnStart()
        {
            base.OnStart();
            if(_googleApiClient != null)
                _googleApiClient.Connect();
        }

        public override void OnStop()
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
            var text = View.FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);

            LocationServices.FusedLocationApi.RemoveLocationUpdates(_googleApiClient, this);

            SharedData.lastLocation = new LatLon() { Lat = location.Latitude, Lon = location.Longitude };

            if (_lastAddress == null)
            {
                var geocoder = new Geocoder(Context);
                var addresses = new List<Address>(geocoder.GetFromLocation(location.Latitude, location.Longitude, 1)).ToArray();

                if (addresses.Length > 0)
                    SetToAddress(addresses[0]);
            }
        }

        private void AddressChanged(AddressListAdapter adapter, int position)
        {
            var address = adapter.GetItem(position);

            SetToAddress(address);
        }

        private void SetToAddress(Address address)
        {
            var text = View.FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);

            text.SetText(new Java.Lang.String(DescribeAddress(address)), false);
            text.ClearFocus();
            _lastAddress = address;

            InputMethodManager input = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            if(input != null)
                input.HideSoftInputFromWindow(Activity.CurrentFocus.WindowToken, 0);

            UpdatePumpList(address.Latitude, address.Longitude);
        }

        private async void UpdatePumpList(double latitude, double longitude)
        {
            var view = View.FindViewById<ListView>(Resource.Id.listPumps);
            var progress = View.FindViewById<ProgressBar>(Resource.Id.progressBar);

            view.EmptyView = progress;

            view.Adapter = null;

            var eh = new EHApi();

            try
            {
                var pumps = await eh.getPumpListAsync(latitude, longitude, SharedData.login.Vehicle);
                view.Adapter = new PumpListAdapter(Context, pumps);
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

            var text = View.FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);
            text.ClearFocus();

            Intent i = new Intent(Context, typeof(LocationActivity));
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
                Initialise();
         
        }

        private void Initialise()
        {
            if (_googleApiClient == null)
            {
                _googleApiClient = new GoogleApiClient.Builder(Context)
                    .AddConnectionCallbacks(this)
                    .AddOnConnectionFailedListener(this)
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

            outState.PutParcelable("lastAddress", _lastAddress);
        }
    }

}

