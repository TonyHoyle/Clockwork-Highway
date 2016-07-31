using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using EH.Common;
using System.Collections.Generic;
using System.Linq;

namespace EH.Android
{
    public class TransactionsFragment : DialogFragment
    {
        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Calling GetLayoutInflator for the dialog here causes a recursive loop as DialogFragment.GetLayoutInflator
            // contains a call to OnCreateDialog (which seems bogus but unfixed in latest android).
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.transactions, null);
            var list = view.FindViewById<ListView>(Resource.Id.transactions);
            var progress = view.FindViewById<ProgressBar>(Resource.Id.loadingProgress);

            var adapter = new TransactionItemAdapter(Context);
            list.Adapter = adapter;

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.transactions)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { });
            var dlg = builder.Create();
            loadTransactions(dlg, list, adapter, progress);
            return dlg;
        }

        private async void loadTransactions(AlertDialog dlg, ListView list, TransactionItemAdapter adapter, ProgressBar progress)
        {
            try
            {
                var transactions = await SharedData.login.Api.getTransactionListAsync(SharedData.login.Username, SharedData.login.Password);

                if (transactions != null)
                {
                    progress.Visibility = global::Android.Views.ViewStates.Gone;

                    var trans = transactions.contractAccount.OrderByDescending(o => o.date).ThenByDescending(o => o.sessionId).ToList();
                    trans.RemoveAll(item => item.date == null); // Bogus transactions
                    adapter.Transactions = trans;
                }
                    
            }
            catch(EHApi.EHApiException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}