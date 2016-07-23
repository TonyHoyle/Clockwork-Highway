using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Support.Design.Widget;
using EH.Common;
using Android.Content;
using Android.Support.V7.Preferences;

namespace EH.Android
{
    public class ChangeEmailFragment : DialogFragment
    {
        private TextInputEditText _newEmail;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Calling GetLayoutInflator for the dialog here causes a recursive loop as DialogFragment.GetLayoutInflator
            // contains a call to OnCreateDialog (which seems bogus but unfixed in latest android).
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.ChangeEmail, null);
            var username = view.FindViewById<TextView>(Resource.Id.username);
            _newEmail = view.FindViewById<TextInputEditText>(Resource.Id.email);

            username.Text = "Changing email for " + SharedData.login.Account.firstname + " " + SharedData.login.Account.lastname;

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.changePassword)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { Dismiss(); });

            return builder.Create();

        }

        private async void ChangeEmail(string newEmail)
        {
            var eh = new EHApi();

            try
            {
                if (await eh.changeEmailAsync(SharedData.login.Username, SharedData.login.Password, newEmail))
                {
                    Dismiss();
                }
                else
                {
                    _newEmail.Error = Context.Resources.GetString(Resource.String.invalidEmail);
                }
            }
            catch (EHApi.EHApiException e)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't change email: " + e.Message);
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            AlertDialog dlg = (AlertDialog)Dialog;
            if(dlg != null)
            {
                Button positivePutton = dlg.GetButton((int)DialogButtonType.Positive);
                positivePutton.Click += (sender, args) => { ChangeEmail(_newEmail.Text); };
            }
        }
    }
}