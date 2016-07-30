using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System;
using System.Timers;
using EH.Common;
using Android.Support.V7.App;

namespace EH.Android
{
    public class ChargingFragment : Fragment
    {
        private string _sessionId;
        private int _pumpId;
        private int _connectorId;

        private TextView _chargeStatus;
        private TextView _chargeTime;
        private TextView _chargePower;
        private ProgressBar _progressBar;
        private Button _chargeStop;
        private Timer _timer;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Charging, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            _sessionId = Arguments.GetString("sessionId");
            _pumpId = Arguments.GetInt("pumpId");
            _connectorId = Arguments.GetInt("connectorId");

            _chargeStatus = View.FindViewById<TextView>(Resource.Id.chargeStatus);
            _chargeTime = View.FindViewById<TextView>(Resource.Id.chargeTime);
            _chargePower = View.FindViewById<TextView>(Resource.Id.chargePower);
            _progressBar = View.FindViewById<ProgressBar>(Resource.Id.progressBar);
            _chargeStop = View.FindViewById<Button>(Resource.Id.chargeStop);

            _chargeStop.Click += OnStopCharge;

            _timer = new Timer();
            _timer.Interval = 5000;
            _timer.Elapsed += new ElapsedEventHandler(OnTimer);
            _timer.AutoReset = false;
            _timer.Enabled = true;

            OnTimer(this, null);
        }

        private void OnStopCharge(object sender, EventArgs e)
        {
            var dlg = new AlertDialog.Builder(Context)
                .SetTitle(Resource.String.stopCharge)
                .SetMessage(Resource.String.areYouSureStop)
                .SetPositiveButton("Yes", (obj, args) => { OnStopChargeYes(); })
                .SetNegativeButton("No", (obj, args) => { })
                .Create();

            dlg.Show();
        }

        private async void OnStopChargeYes()
        {
            EHApi.BoolResult res = await SharedData.login.Api.stopChargeSessionAsync(SharedData.login.Username, SharedData.login.Password, SharedData.deviceId, _pumpId, _connectorId, _sessionId);
            if (!res.result)
            {
                var t = Toast.MakeText(Context, res.message!=null?res.message:"Unable to stop charge", ToastLength.Long);
                t.Show();
            }
        }

        private async void OnTimer(object source, ElapsedEventArgs e)
        {
            var status = await SharedData.login.Api.getChargeStatusAsync(SharedData.deviceId, _sessionId, _pumpId, _connectorId, SharedData.login.Vehicle);

            _chargeStatus.Text = status.message;
            _chargePower.Text = String.Format(Context.GetString(Resource.String.powerSupplied), ((double)status.energyConsumption) / 100);
            long mins = (UnixNow() - status.started) / 60;
            _chargeTime.Text = String.Format(Context.GetString(Resource.String.chargingMinutes), mins);
            _progressBar.Max = 30;
            _progressBar.Progress = (int)Math.Max(30, mins);

            if (!status.completed)
                _timer.Start();
            else
            {
                _chargeStop.Visibility = ViewStates.Gone;
            }
        }

        private static readonly DateTime _reference = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private long UnixNow()
        {
            return (long)(DateTime.Now - _reference).TotalSeconds;
        }
    }
}