using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Content;
using System.Collections.Generic;
using Android.Support.V7.Preferences;

namespace ClockworkHighway.Android
{
    public class SelectCardFragment : DialogFragment
    {
        private int _selectedCard;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            List<string> cardList = new List<string>();

            foreach(var c in SharedData.api.Login.Cards)
            {
                cardList.Add(c.cardType + " " + c.lastDigits);
            }

            _selectedCard = SharedData.api.Login.DefaultCardIndex;

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.selectCard)
                .SetSingleChoiceItems(cardList.ToArray(), _selectedCard, CardClicked)
                .SetPositiveButton(Resource.String.select, (sender, args) => { SelectCard(); })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { });

            return builder.Create();
        }

        private void CardClicked(object sender, DialogClickEventArgs args)
        {
            _selectedCard = args.Which;
        }

        private void SelectCard()
        {
            SharedData.api.Login.DefaultCardIndex = _selectedCard;

            var prefs = PreferenceManager.GetDefaultSharedPreferences(Context).Edit();
            prefs.PutInt("CardIndex", SharedData.api.Login.DefaultCardIndex)
                .Commit();
        }
    }
}