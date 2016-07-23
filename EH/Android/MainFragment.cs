using Android.OS;
using Android.Gms.Common.Apis;
using Android.Provider;
using Android.Preferences;
using Android.Content;
using Android.Widget;
using Android.Views;
using Android.Support.V4.App;
using EH.Common;
using Android.Support.Design.Widget;

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
            var forgotUsername = View.FindViewById<TextView>(Resource.Id.forgotUsername);
            var forgotPassword = View.FindViewById<TextView>(Resource.Id.forgotPassword);

            login.Click += OnLoginClick;
            forgotUsername.Click += OnUsernameClick;
            forgotPassword.Click += OnPasswordClick;

            SharedData.login = new EHLogin();
            SharedData.deviceId = Settings.Secure.GetString(Context.ContentResolver, Settings.Secure.AndroidId);
            SharedData.googleApi = new GoogleApi(Context.GetString(Resource.String.google_maps_key));

            var prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            var username = prefs.GetString("username", "");
            var password = prefs.GetString("password", "");
            var usernamePrompt = View.FindViewById<TextInputEditText>(Resource.Id.username);
            var passwordPrompt = View.FindViewById<TextInputEditText>(Resource.Id.password);
            usernamePrompt.Text = username;
            passwordPrompt.Text = password;

            SharedData.login.DefaultCardIndex = prefs.GetInt("CardIndex", 0);
            SharedData.login.DefaultVehicleIndex = prefs.GetInt("VehicleIndex", 0);

            DoLogin(username, password);
        }

        private void OnUsernameClick(object sender, System.EventArgs e)
        {
            ForgotUsername();
        }

        private void OnPasswordClick(object sender, System.EventArgs e)
        {
            ForgotPassword();
        }

        private void OnLoginClick(object sender, System.EventArgs e)
        {
            var username = View.FindViewById<TextInputEditText>(Resource.Id.username);
            var password = View.FindViewById<TextInputEditText>(Resource.Id.password);

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

        private void ForgotUsername()
        {
            DialogFragment dlg = new ForgotUsernameFragment();
            dlg.Show(FragmentManager, "ForgotUsernameFragment");

            var usernamePrompt = View.FindViewById<TextInputEditText>(Resource.Id.username);
            var passwordPrompt = View.FindViewById<TextInputEditText>(Resource.Id.password);
            usernamePrompt.Text = "";
            passwordPrompt.Text = "";
            ShowProgress(false);
        }

        private void ForgotPassword()
        {
            DialogFragment dlg = new ForgotPasswordFragment();
            dlg.Show(FragmentManager, "ForgotPasswordFragment");

            var passwordPrompt = View.FindViewById<TextInputEditText>(Resource.Id.password);
            passwordPrompt.Text = "";
            ShowProgress(false);
        }
    }
}
