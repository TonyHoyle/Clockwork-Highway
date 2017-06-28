using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System;

namespace ClockworkHighway.Android
{
    public class AccountDetailsFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.accountdetails, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            var accountList = View.FindViewById<ListView>(Resource.Id.accountDetails);
            var changePassword = View.FindViewById<Button>(Resource.Id.changePassword);

            var details = new List<AccountDetailsLine>();

            AddDetails(details, Resource.String.name, " ",
                SharedData.api.Login.Account.firstname,
                SharedData.api.Login.Account.lastname);
            AddDetails(details, Resource.String.address, "\n",
                SharedData.api.Login.Account.accountDetails.street,
                SharedData.api.Login.Account.accountDetails.village,
                SharedData.api.Login.Account.accountDetails.city,
                SharedData.api.Login.Account.accountDetails.postcode);
            AddDetails(details, Resource.String.email, " ",
                SharedData.api.Login.Account.email);
            AddDetails(details, Resource.String.phone, " ",
                SharedData.api.Login.Account.phone);
            if(SharedData.api.Login.Vehicle != null)
                AddDetails(details, Resource.String.car, " ",
                    SharedData.api.Login.Vehicle.registration.ToUpper() + " -",
                    SharedData.api.Login.Vehicle.make,
                    SharedData.api.Login.Vehicle.model,
                    SharedData.api.Login.Vehicle.specification);
            if(SharedData.api.Login.Card != null)
                AddDetails(details, Resource.String.card, " ",
                    SharedData.api.Login.Card.cardType,
                    SharedData.api.Login.Card.lastDigits);
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