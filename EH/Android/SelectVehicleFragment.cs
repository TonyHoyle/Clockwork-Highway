using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Content;
using System.Collections.Generic;
using Android.Support.V7.Preferences;

namespace EH.Android
{
    public class SelectVehicleFragment : DialogFragment
    {
        private int _selectedVehicle;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            List<string> vehicleList = new List<string>();

            foreach(var v in SharedData.login.Vehicles)
            {
                vehicleList.Add(v.registration.ToUpper() + " - " + v.make+ " " + v.model + " " + " " + v.specification);
            }

            _selectedVehicle = SharedData.login.DefaultVehicleIndex;

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.selectVehicle)
                .SetSingleChoiceItems(vehicleList.ToArray(), _selectedVehicle, VehicleClicked)
                .SetPositiveButton(Resource.String.select, (sender, args) => { SelectVehicle(); })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { });

            return builder.Create();
        }

        private void VehicleClicked(object sender, DialogClickEventArgs args)
        {
            _selectedVehicle = args.Which;
        }

        private void SelectVehicle()
        {
            SharedData.login.DefaultVehicleIndex = _selectedVehicle;

            var prefs = PreferenceManager.GetDefaultSharedPreferences(Context).Edit();
            prefs.PutInt("VehicleIndex", SharedData.login.DefaultVehicleIndex)
                .Commit();
        }
    }
}