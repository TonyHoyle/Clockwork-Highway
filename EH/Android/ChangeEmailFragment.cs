using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Support.Design.Widget;
using TonyHoyle.EH;
using Android.Content;
using Android.Util;

namespace ClockworkHighway.Android
{
    public class ChangeEmailFragment : DialogFragment
    {
        private TextInputEditText _newEmail;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Calling GetLayoutInflator for the dialog here causes a recursive loop as DialogFragment.GetLayoutInflator
            // contains a call to OnCreateDialog (which seems bogus but unfixed in latest android).
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.changeemail, null);
            var username = view.FindViewById<TextView>(Resource.Id.username);
            _newEmail = view.FindViewById<TextInputEditText>(Resource.Id.email);

            username.Text = "Changing email for " + SharedData.api.Login.Account.firstname + " " + SharedData.api.Login.Account.lastname;

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.changeEmail)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { Dismiss(); });

            return builder.Create();

        }

        private async void ChangeEmail(string newEmail)
        {
            var eh = SharedData.api;

            try
            {
                var result = await eh.changeEmailAsync(newEmail);
                if (result.result)
                {
                    Dismiss();
                }
                else
                {
                    if (string.IsNullOrEmpty(result.message))
                        _newEmail.Error = Context.Resources.GetString(Resource.String.invalidEmail);
                    else
                        _newEmail.Error = result.message;
                }
            }
            catch (EHApi.EHApiException e)
            {
                Log.Debug(SharedData.APP, "Couldn't change email: " + e.Message);
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