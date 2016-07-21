using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using EH.Common;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Content;
using Android.Widget;

namespace EH.Android
{
    public class ForgotPasswordFragment : DialogFragment
    {
        private TextInputEditText _email;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            View view = Activity.LayoutInflater.Inflate(Resource.Layout.ForgotPassword, null);
            _email = view.FindViewById<TextInputEditText>(Resource.Id.email);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.forgotPasswordTitle)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { Dismiss(); });
            return builder.Create();
        }

        private async void DoForgot()
        {
            var eh = new EHApi();

            try
            {
                var hashkey = await eh.forgottenPasswordAsync(_email.Text);

                if(hashkey == null)
                {
                    _email.Error = Context.Resources.GetString(Resource.String.invalidEmail);
                    return;
                }

                var dlg = new ForgotPassword2Fragment();
                dlg.HashKey = hashkey.hashkey;
                dlg.Show(FragmentManager, "ForgotPassword2Fragment");
                Dismiss();
            }
            catch (EHApi.EHApiException e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to do forget password: " + e.Message);
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            AlertDialog dlg = (AlertDialog)Dialog;
            if (dlg != null)
            {
                Button positivePutton = dlg.GetButton((int)DialogButtonType.Positive);
                positivePutton.Click += (sender, args) => { DoForgot(); };
            }
        }
    }
}