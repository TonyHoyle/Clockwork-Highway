using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using EH.Common;
using Android.Support.Design.Widget;
using Android.Widget;
using Android.Content;
using Android.Support.V7.Preferences;

namespace EH.Android
{
    public class ForgotPassword3Fragment : DialogFragment
    {
        public string HashKey { get; set; }
        public string HashKey2 { get; set; }

        private TextInputEditText _newPassword;
        private TextInputEditText _newPassword2;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            if(savedInstanceState != null)
            {
                HashKey = savedInstanceState.GetString("HashKey");
                HashKey2 = savedInstanceState.GetString("HashKey2");
            }

            var view = Activity.LayoutInflater.Inflate(Resource.Layout.forgotpassword3, null);
            _newPassword = view.FindViewById<TextInputEditText>(Resource.Id.newPassword);
            _newPassword2 = view.FindViewById<TextInputEditText>(Resource.Id.newPassword2);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.changePassword)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { Dismiss(); });

            return builder.Create();
        }

        private async void ChangePassword(string newPassword, string newPassword2)
        {
            var eh = new EHApi(SharedData.httpClient);

            if (newPassword != newPassword2)
            {
                _newPassword2.Error = Context.Resources.GetString(Resource.String.passwordsnomatch);
                return;
            }
            try
            {
                var result = await eh.usePasswordTokenAsync("android", HashKey, HashKey2, newPassword);
                if (result.result)
                {
                    // Store the new password for next time we login
                    var prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
                    var editor = prefs.Edit();
                    editor
                        .PutString("password", newPassword)
                        .Apply();
                    Dismiss();
                }
                else
                {
                    string text;

                    if (string.IsNullOrEmpty(result.message))
                        text = "Unable to set new password";
                    else
                        text = result.message;

                    var t = Toast.MakeText(Context, text, ToastLength.Long);
                    t.Show();
                }
            }
            catch (EHApi.EHApiException e)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't change password: " + e.Message);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("HashKey", HashKey);
            outState.PutString("HashKey2", HashKey);
        }

        public override void OnStart()
        {
            base.OnStart();

            AlertDialog dlg = (AlertDialog)Dialog;
            if (dlg != null)
            {
                Button positivePutton = dlg.GetButton((int)DialogButtonType.Positive);
                positivePutton.Click += (sender, args) => { ChangePassword(_newPassword.Text, _newPassword2.Text); };
            }
        }
    }
}