using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace EH.Android
{
    public interface ILoginResult
    {
        void OnLogin(string username, string password);
        void OnLoginCancelled();
    }

    public class LoginFragment : DialogFragment
    {
        private ILoginResult _owner;

        public LoginFragment(ILoginResult owner)
        {
            _owner = owner;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            LayoutInflater inflater = Activity.LayoutInflater;

            builder.SetView(inflater.Inflate(Resource.Layout.LoginDialog, null))
                   .SetPositiveButton(Resource.String.login, (sender, args) =>
                   {
                       var username = Dialog.FindViewById<EditText>(Resource.Id.username);
                       var password = Dialog.FindViewById<EditText>(Resource.Id.password);

                       Dialog.Dismiss();
                       _owner.OnLogin(username.Text, password.Text);
                   })
                   .SetNegativeButton(Resource.String.cancel, (sender, args) =>
                    {
                        Dialog.Cancel();
                        _owner.OnLoginCancelled();
                    });
            return builder.Create();
        }
    }
}