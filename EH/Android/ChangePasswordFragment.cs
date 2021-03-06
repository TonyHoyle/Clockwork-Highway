using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Support.Design.Widget;
using TonyHoyle.EH;
using Android.Content;
using Android.Support.V7.Preferences;
using Android.Util;

namespace ClockworkHighway.Android
{
    public class ChangePasswordFragment : DialogFragment
    {
        private TextInputEditText _oldPassword;
        private TextInputEditText _newPassword;
        private TextInputEditText _newPassword2;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Calling GetLayoutInflator for the dialog here causes a recursive loop as DialogFragment.GetLayoutInflator
            // contains a call to OnCreateDialog (which seems bogus but unfixed in latest android).
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.changepassword, null);
            var username = view.FindViewById<TextView>(Resource.Id.username);
            _oldPassword = view.FindViewById<TextInputEditText>(Resource.Id.oldPassword);
            _newPassword = view.FindViewById<TextInputEditText>(Resource.Id.newPassword);
            _newPassword2 = view.FindViewById<TextInputEditText>(Resource.Id.newPassword2);

            username.Text = "Changing password for " + SharedData.api.Login.Account.firstname + " " + SharedData.api.Login.Account.lastname;

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.changePassword)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { Dismiss(); });

            return builder.Create();

        }

        private async void ChangePassword(string oldPassword, string newPassword, string newPassword2)
        {
            var eh = SharedData.api;

            if(newPassword != newPassword2)
            {
                _oldPassword.Error = null;
                _newPassword2.Error = Context.Resources.GetString(Resource.String.passwordsnomatch);
                return;
            }
            try
            {
                var result = await eh.changePasswordAsync(oldPassword, newPassword);
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
                    if(string.IsNullOrEmpty(result.message))
                        _oldPassword.Error = Context.Resources.GetString(Resource.String.badpassword);
                    else
                        _oldPassword.Error = result.message;
                    _newPassword2.Error = null;
                }
            }
            catch (EHApi.EHApiException e)
            {
                Log.Debug(SharedData.APP, "Couldn't change password: " + e.Message);
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            AlertDialog dlg = (AlertDialog)Dialog;
            if(dlg != null)
            {
                Button positivePutton = dlg.GetButton((int)DialogButtonType.Positive);
                positivePutton.Click += (sender, args) => { ChangePassword(_oldPassword.Text, _newPassword.Text, _newPassword2.Text); };
            }
        }
    }
}