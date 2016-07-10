using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName", Theme = "@android:style/Theme.DeviceDefault.Light")]
    public class LocationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                int locationId = extras.GetInt("locationId");
            }
        }
    }
}