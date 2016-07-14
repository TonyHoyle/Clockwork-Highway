using Android.Support.V7.App;
using Android.OS;
using Android.App;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Views;

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName")]
    public class SearchActivity : AppCompatActivity
    {
        private ActionBarDrawerToggle _drawerToggle;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MainMenu);

            if(bundle == null)
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);

                SupportActionBar.SetDisplayHomeAsUpEnabled (true);
                SupportActionBar.SetHomeButtonEnabled(true);

                var drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
                var navigationView = FindViewById<NavigationView>(Resource.Id.navigationView);

                navigationView.InflateMenu(Resource.Menu.drawerMenu);

                _drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, Resource.String.open, Resource.String.close);

                drawerLayout.AddDrawerListener(_drawerToggle);

                var fragment = new SearchFragment();
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, fragment).Commit();
         
            }
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            // Sync the toggle state after onRestoreInstanceState has occurred.
            _drawerToggle.SyncState();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (_drawerToggle.OnOptionsItemSelected(item))
                return true;

            return base.OnOptionsItemSelected(item);
        }
    }

}

