using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using TonyHoyle.EH;
using System.Linq;
using Android.Views;

namespace ClockworkHighway.Android
{
    public class TransactionsFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.transactions, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            // Calling GetLayoutInflator for the dialog here causes a recursive loop as DialogFragment.GetLayoutInflator
            // contains a call to OnCreateDialog (which seems bogus but unfixed in latest android).
            var list = View.FindViewById<ListView>(Resource.Id.transactions);
            var progress = View.FindViewById<ProgressBar>(Resource.Id.loadingProgress);

            var adapter = new TransactionItemAdapter(Context);
            list.Adapter = adapter;

            loadTransactions(list, adapter, progress);
        }

        private async void loadTransactions(ListView list, TransactionItemAdapter adapter, ProgressBar progress)
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