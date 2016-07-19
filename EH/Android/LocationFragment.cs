using System.Collections.Generic;
using Android.Support.V4.App;
using Android.OS;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Views;
using Android.Support.V7.App;
using EH.Common;

namespace EH.Android
{
    public class LocationFragment : Fragment, IOnMapReadyCallback
    {
        private double _locationLat;
        private double _locationLon;
        private string _locationName;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Location, container, false);
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            int locationId = Arguments.GetInt("locationId");
            _locationLat = Arguments.GetDouble("latitude");
            _locationLon = Arguments.GetDouble("longitude");
            _locationName = Arguments.GetString("name");

            var mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var list = View.FindViewById<ListView>(Resource.Id.connectors);
            var progress = View.FindViewById<ProgressBar>(Resource.Id.progressBar);

            ((AppCompatActivity)Activity).SupportActionBar.Title = _locationName;

            list.EmptyView = progress;

            EHApi eh = new EHApi();
            var location = await eh.getLocationDetailsAsync(locationId, SharedData.login.Vehicle);
            var details = new List<EHApi.ConnectorDetails>();
            list.Adapter = new LocationAdapter(Context, location);
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