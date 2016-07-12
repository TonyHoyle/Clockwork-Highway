using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Support.V4.App;
using Android.Gms.Maps;
using Android.Provider;
using EH.Common;
using System.Collections.Generic;
using Android.Gms.Maps.Model;

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName")]
    public class LocationActivity : FragmentActivity, IOnMapReadyCallback
    {
        private double _locationLat;
        private double _locationLon;
        private string _locationName;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Location);

            Bundle extras = Intent.Extras;
            int locationId = extras.GetInt("locationId");
            _locationLat = extras.GetDouble("latitude");
            _locationLon = extras.GetDouble("longitude");
            _locationName = extras.GetString("name");

            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var list = FindViewById<ListView>(Resource.Id.connectors);
            var progress = FindViewById<ProgressBar>(Resource.Id.progressBar);

            this.Title = _locationName;

            list.EmptyView = progress;

            EHApi eh = new EHApi();
            var location = await eh.getLocationDetailsAsync(locationId, SharedData.login.Vehicle);
            var details = new List<EHApi.ConnectorDetails>();
            list.Adapter = new LocationAdapter(this, location);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var camera = new CameraPosition.Builder()
                        .Target(new LatLng(_locationLat, _locationLon))
                        .Zoom(15)
                        .Build();
            var marker = new MarkerOptions()
                        .SetPosition(new LatLng(_locationLat, _locationLon))
                        .SetTitle(_locationName);

            googleMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(camera));
            googleMap.AddMarker(marker);
            // Finding route is a pain involving web calls to google directions API
        }
    }
}
