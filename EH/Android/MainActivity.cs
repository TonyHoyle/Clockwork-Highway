using Android.App;
using Android.OS;
using static Android.Manifest;
using Android.Gms.Common.Apis;
using Android.Support.V4.App;
using EH.Common;
using Android.Preferences;
using Android.Content;

namespace EH.Android
{
    [Activity(Label = "Clockwork Highway", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.DeviceDefault.Light")]
    public class MainActivity : Activity, ILoginResult
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            var username = prefs.GetString("username","");
            var password = prefs.GetString("password","");
            OnLogin(username, password);
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        private void ShowLoginDialog()
        {
            var transaction = FragmentManager.BeginTransaction();
            var dialogFragment = new LoginFragment(this);
            dialogFragment.Show(transaction, "login");
        }

        public async void OnLogin(string username, string password)
        {
            EHApi.AccountData account;
            var eh = new EHApi();

            try
            {

                SharedData.accountData = null;
                SharedData.vehicle = null;

                if (username == "" || password == "")
                    account = null;
                else
                    account = await eh.loginAsync(username, password);
                if (account == null)
                {
                    ShowLoginDialog();
                }
                else
                {
                    SharedData.accountData = account;
                    var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                    var editor = prefs.Edit();
                    editor.PutString("username", username);
                    editor.PutString("password", password);
                    editor.Apply();

                    var vehicles = await eh.getUserVehicleListAsync(username, password);
                    if (vehicles == null || vehicles.Count < 1)
                        SharedData.vehicle = new EHApi.Vehicle();
                    else
                        SharedData.vehicle = vehicles[0];

                    Intent intent = new Intent(this, typeof(SearchActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.TaskOnHome);
                    StartActivity(intent);
                    Finish();
    }
            }
            catch (EHApi.EHApiException e)
            {
                System.Diagnostics.Debug.WriteLine(e.reason);
            }
        }

        public void OnLoginCancelled()
        {
            Finish();
        }
    }

}

