using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Support.V7.Preferences;
using Android.Content;
using System;
using Android.Widget;

namespace EH.Android
{
    public class BaseActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private global::Android.Support.V7.App.ActionBarDrawerToggle _drawerToggle;
        private DrawerLayout _drawerLayout;
        public Fragment Fragment { get; private set; }

        public BaseActivity(Fragment fragment)
        {
            this.Fragment = fragment;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MainMenu);

            if(bundle == null)
            {
                global::Android.Support.V7.Widget.Toolbar toolbar = FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);

                SupportActionBar.SetDisplayHomeAsUpEnabled (true);
                SupportActionBar.SetHomeButtonEnabled(true);

                _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
                var navigationView = FindViewById<NavigationView>(Resource.Id.navigationView);

                navigationView.InflateMenu(Resource.Menu.drawerMenu);
                navigationView.InflateHeaderView(Resource.Layout.Header);
                navigationView.SetNavigationItemSelectedListener(this);

                var headerView = navigationView.GetHeaderView(0);
                var headerText = headerView.FindViewById<TextView>(Resource.Id.drawerHeaderTitle);

                headerText.Text = SharedData.login.Account.firstname + " " + SharedData.login.Account.lastname;

                _drawerToggle = new global::Android.Support.V7.App.ActionBarDrawerToggle(this, _drawerLayout, Resource.String.open, Resource.String.close);

                _drawerLayout.AddDrawerListener(_drawerToggle);

                Fragment.Arguments = Intent.Extras;

                SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, Fragment).Commit();       
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

        private void Logout()
        {
            SharedData.login.Logout();

            var prefs = PreferenceManager.GetDefaultSharedPreferences(this)
                .Edit()
                .Remove("password")
                .Commit();

            Intent intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.TaskOnHome | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }

        private void About()
        {
            DialogFragment about = new AboutFragment();
            about.Show(SupportFragmentManager, "AboutFragment");
        }

        private void AccountDetails()
        {
            Intent i = new Intent(this, typeof(AccountDetailsActivity));
            StartActivity(i);
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            _drawerLayout.CloseDrawers();
            switch (menuItem.ItemId)
            {
                case Resource.Id.about:  // About
                    About();
                    break;
//                case Resource.Id.preferences: // Preferences
//                    break;
                case Resource.Id.transactions: // Transactions
                    break;
                case Resource.Id.account: // Account
                    AccountDetails();
                    break;
                case Resource.Id.logout:
                    Logout();
                    return true;
            }
            return false;
        }
    }
}

