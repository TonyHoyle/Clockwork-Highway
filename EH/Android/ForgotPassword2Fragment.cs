using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using TonyHoyle.EH;
using Android.Content;
using Android.Widget;
using Android.Util;

namespace ClockworkHighway.Android
{
    public class ForgotPassword2Fragment : DialogFragment
    {
        public string HashKey { get; set; }

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            if(savedInstanceState != null)
            {
                HashKey = savedInstanceState.GetString("HashKey");
            }

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
               .SetTitle(Resource.String.forgotPasswordTitle)
               .SetMessage(Resource.String.forgotPassword2)
               .SetPositiveButton(Resource.String.continu, (sender, args) => { })
               .SetNegativeButton(Resource.String.cancel, (sender, args) => { Dismiss(); });
            return builder.Create();
        }

        private async void DoForgot2()
        {
            var eh = new EHApi(SharedData.httpClient);

            try
            {
                var hashkey2 = await eh.getPasswordTokenAsync("android", HashKey);

                if(!hashkey2.success)
                {
                    var toast = Toast.MakeText(Context.ApplicationContext, Resource.String.tokenNotReady, ToastLength.Long);
                    toast.Show();
                    return;
                }

                var dlg = new ForgotPassword3Fragment();
                dlg.HashKey = HashKey;
                dlg.HashKey2 = hashkey2.hashkey;
                dlg.Show(FragmentManager, "ForgotPassword3Fragment");
                Dismiss();
            }
            catch (EHApi.EHApiException e)
            {
                Log.Debug(SharedData.APP, "Failed to do forget password: " + e.Message);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("HashKey", HashKey);
        }

        public override void OnStart()
        {
            base.OnStart();

            AlertDialog dlg = (AlertDialog)Dialog;
            if (dlg != null)
            {
                Button positivePutton = dlg.GetButton((int)DialogButtonType.Positive);
                positivePutton.Click += (sender, args) => { DoForgot2(); };
            }
        }
    }
}