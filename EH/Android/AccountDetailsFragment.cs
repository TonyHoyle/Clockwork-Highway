using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System;

namespace EH.Android
{
    public class AccountDetailsFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.AccountDetails, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            var accountList = View.FindViewById<ListView>(Resource.Id.accountDetails);
            var changePassword = View.FindViewById<Button>(Resource.Id.changePassword);

            var details = new List<AccountDetailsLine>();

            AddDetails(details, Resource.String.name, " ",
                SharedData.login.Account.firstname,
                SharedData.login.Account.lastname);
            AddDetails(details, Resource.String.address, "\n",
                SharedData.login.Account.accountDetails.street,
                SharedData.login.Account.accountDetails.village,
                SharedData.login.Account.accountDetails.city,
                SharedData.login.Account.accountDetails.postcode);
            AddDetails(details, Resource.String.email, " ",
                SharedData.login.Account.email);
            AddDetails(details, Resource.String.phone, " ",
                SharedData.login.Account.phone);
            AddDetails(details, Resource.String.car, " ",
                SharedData.login.Vehicle.registration.ToUpper() + " -",
                SharedData.login.Vehicle.make,
                SharedData.login.Vehicle.model,
                SharedData.login.Vehicle.specification);
            AddDetails(details, Resource.String.card, " ",
                SharedData.login.Card.cardType,
                SharedData.login.Card.lastDigits);
            accountList.Adapter = new AccountDetailsAdapter(Context, details);
            accountList.ItemClick += (sender, args) => { OnAccountDetailsClicked(((AccountDetailsAdapter)accountList.Adapter).GetItem(args.Position)); };

            changePassword.Click += OnChangePassword;
        }

        private void OnAccountDetailsClicked(AccountDetailsLine line)
        {
            switch(line.Id)
            {
                case Resource.String.car:
                    SelectCar();
                    break;
                case Resource.String.card:
                    SelectCard();
                    break;
                case Resource.String.email:
                    ChangeEmail();
                    break;
            }
        }

        private void OnChangePassword(object sender, EventArgs e)
        {
            ChangePassword();
        }

        private void AddDetails(List<AccountDetailsLine> details, int title, string separator, params string[] text)
        {
            details.Add(new AccountDetailsLine() { Id = title, Title = Context.Resources.GetString(title), Text = String.Join(separator, text) });
        }

        private void SelectCar()
        {
            var frag = new SelectVehicleFragment();
            frag.Show(FragmentManager, "SelectVehicleFragment");
        }

        private void SelectCard()
        {
            var frag = new SelectCardFragment();
            frag.Show(FragmentManager, "SelectCardFragment");
        }

        private void ChangeEmail()
        {
            var frag = new ChangeEmailFragment();
            frag.Show(FragmentManager, "ChangeEmailFragment");
        }

        private void ChangePassword()
        {
            var frag = new ChangePasswordFragment();
            frag.Show(FragmentManager, "ChangePasswordFragment");
        }
    }
}