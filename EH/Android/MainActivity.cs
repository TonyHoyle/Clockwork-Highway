using Android.App;
using Android.OS;
using Android.Gms.Common.Apis;
using EH.Common;
using Android.Preferences;
using Android.Content;
using Android.Widget;

// 'Car with cog' logo by Benjamin STAWARZ licensed under CC BY 3.0

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var login = FindViewById<Button>(Resource.Id.login);
            var forgotPassword = FindViewById<TextView>(Resource.Id.forgotPassword);

            login.Click += OnLoginClick;
            forgotPassword.Click += OnPasswordClick;

            SharedData.login = new EHLogin();

            var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            var username = prefs.GetString("username","");
            var password = prefs.GetString("password","");
            DoLogin(username, password);
        }

        private void OnPasswordClick(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnLoginClick(object sender, System.EventArgs e)
        {
            var username = FindViewById<EditText>(Resource.Id.username);
            var password = FindViewById<EditText>(Resource.Id.password);

            DoLogin(username.Text, password.Text);
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        public void ShowProgress(bool visible, string message = "", bool isError = false)
        {
            var progress = FindViewById<ProgressBar>(Resource.Id.loadingProgress);
            var loading = FindViewById<TextView>(Resource.Id.loadingText);
            var button = FindViewById<Button>(Resource.Id.login);

            loading.Text = message;
            if (isError) loading.SetTextColor(global::Android.Graphics.Color.Red);
            else loading.SetTextColor(global::Android.Graphics.Color.Black);
      
            progress.Visibility = visible?global::Android.Views.ViewStates.Visible: global::Android.Views.ViewStates.Gone;
            button.Visibility = visible ? global::Android.Views.ViewStates.Gone : global::Android.Views.ViewStates.Visible;
        }

        public async void DoLogin(string username, string password)
        {
            try
            {

                SharedData.login.Logout();

                if(username == "" || password == "")
                {
                    ShowProgress(false);
                    return;
                }

                ShowProgress(true, "Logging in..");
                if(!await SharedData.login.Login(username, password))
                {
                    ShowProgress(false, "Unknown username or password", true);
                }
                if (SharedData.login.IsLoggedIn)
                {
                    var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                    var editor = prefs.Edit();
                    editor.PutString("username", username);
                    editor.PutString("password", password);
                    editor.Apply();

                    Intent intent = new Intent(this, typeof(SearchActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.TaskOnHome);
                    StartActivity(intent);
                    Finish();
    }
            }
            catch (EHApi.EHApiException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                ShowProgress(false, e.Message, true);
            }
        }
    }

}

