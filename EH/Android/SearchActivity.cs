using Android.App;
using Android.Widget;
using Android.OS;
using static Android.Manifest;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Locations;
using System.Collections.Generic;
using System.Text;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using EH.Common;

namespace EH.Android
{
    [Activity(Theme = "@android:style/Theme.DeviceDefault.Light")]
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

            if (ContextCompat.CheckSelfPermission(this, Permission.AccessFineLocation) != global::Android.Content.PM.Permission.Granted)
                ActivityCompat.RequestPermissions(this, new string[] { Permission.AccessFineLocation }, 0);

            if (_googleApiClient == null)
            {
                _googleApiClient = new GoogleApiClient.Builder(this)
                    .AddConnectionCallbacks(this)
                    .AddOnConnectionFailedListener(this)
                    .AddApi(LocationServices.API)
                    .Build();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            _googleApiClient.Connect();
        }

        protected override void OnStop()
        {
            base.OnStop();
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
            {
                text.Text = DescribeAddress(addresses[0]);
            }

            SharedData.lastLocation = new LatLon() { Lat = location.Latitude, Lon = location.Longitude };
            UpdatePumpList();
        }

        private void AddressChanged(AddressListAdapter adapter, int position)
        {
            var text = FindViewById<AutoCompleteTextView>(Resource.Id.editLocation);
            var address = adapter.GetItem(position);

            text.Text = DescribeAddress(address);

            SharedData.lastLocation = new LatLon() { Lat = address.Latitude, Lon = address.Longitude };
            UpdatePumpList();
        }

        private async void UpdatePumpList()
        {
            var view = FindViewById<ListView>(Resource.Id.listPumps);
            var progress = FindViewById<ProgressBar>(Resource.Id.progressBar);

            view.EmptyView = progress;

            view.Adapter = null;

            var eh = new EHApi();

            var pumps = await eh.getPumpListAsync(SharedData.lastLocation.Lat, SharedData.lastLocation.Lon, SharedData.vehicle);

            view.Adapter = new PumpListAdapter(this, pumps);
            view.EmptyView = null;
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
    }

}

