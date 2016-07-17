using Android.OS;
using Android.Gms.Common.Apis;
using Android.Provider;
using Android.Preferences;
using Android.Content;
using Android.Widget;
using Android.Views;
using Android.Support.V4.App;
using EH.Common;

namespace EH.Android
{
    public class MainFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Main, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            var login = View.FindViewById<Button>(Resource.Id.login);
            var forgotPassword = View.FindViewById<TextView>(Resource.Id.forgotPassword);

            login.Click += OnLoginClick;
            forgotPassword.Click += OnPasswordClick;

            SharedData.login = new EHLogin();
            SharedData.deviceId = Settings.Secure.GetString(Context.ContentResolver, Settings.Secure.AndroidId);
            SharedData.googleApi = new GoogleApi(Context.GetString(Resource.String.google_maps_key));

            var prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            var username = prefs.GetString("username", "");
            var password = prefs.GetString("password", "");
            var usernamePrompt = View.FindViewById<EditText>(Resource.Id.username);
            var passwordPrompt = View.FindViewById<EditText>(Resource.Id.password);
            usernamePrompt.Text = username;
            passwordPrompt.Text = password;

            DoLogin(username, password);
    }

    private void OnPasswordClick(object sender, System.EventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OnLoginClick(object sender, System.EventArgs e)
    {
        var username = View.FindViewById<EditText>(Resource.Id.username);
        var password = View.FindViewById<EditText>(Resource.Id.password);

        DoLogin(username.Text, password.Text);
    }

    public void ShowProgress(bool visible, string message = "", bool isError = false)
    {
        var progress = View.FindViewById<ProgressBar>(Resource.Id.loadingProgress);
        var loading = View.FindViewById<TextView>(Resource.Id.loadingText);
        var button = View.FindViewById<Button>(Resource.Id.login);

        loading.Text = message;
        if (isError) loading.SetTextColor(global::Android.Graphics.Color.Red);
        else loading.SetTextColor(global::Android.Graphics.Color.Black);

        progress.Visibility = visible ? global::Android.Views.ViewStates.Visible : global::Android.Views.ViewStates.Gone;
        button.Visibility = visible ? global::Android.Views.ViewStates.Gone : global::Android.Views.ViewStates.Visible;
    }

    public async void DoLogin(string username, string password)
    {
        try
        {
            SharedData.login.Logout();

            if (username == "" || password == "")
            {
                ShowProgress(false);
                return;
            }

            ShowProgress(true, "Logging in..");
            if (!await SharedData.login.Login(username, password))
            {
                ShowProgress(false, "Unknown username or password", true);
            }
            if (SharedData.login.IsLoggedIn)
            {
                var prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
                var editor = prefs.Edit();
                editor.PutString("username", username);
                editor.PutString("password", password);
                editor.Apply();

                Intent intent = new Intent(Context, typeof(SearchActivity));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.TaskOnHome);
                StartActivity(intent);
                Activity.Finish();
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