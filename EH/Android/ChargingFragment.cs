using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System;
using System.Timers;
using TonyHoyle.EH;
using Android.Support.V7.App;
using System.Globalization;

namespace ClockworkHighway.Android
{
    public class ChargingFragment : Fragment
    {
        private string _sessionId;
        private int _pumpId;
        private int _connectorId;
        private bool _charging;

        private TextView _chargeStatus;
        private TextView _chargeTime;
        private TextView _chargePower;
        private ProgressBar _progressBar;
        private Button _chargeStop;
        private Timer _timer;
        private TextView _messageStop;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.charging, container, false);
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
            _messageStop = View.FindViewById<TextView>(Resource.Id.messageStop);

            _chargeStop.Click += OnStopCharge;
            _chargeStop.LongClick += OnTerminateCharge;

            _timer = new Timer();
            _timer.Interval = 5000;
            _timer.Elapsed += new ElapsedEventHandler(OnTimer);
            _timer.AutoReset = true;
            _timer.Enabled = true;

            OnTimer(this, null);
        }

        private void OnStopCharge(object sender, EventArgs e)
        {
            if (!_charging)
            {
                _timer.Enabled = false;
                Activity.Finish();
                return;
            }

            OnTerminateCharge(sender, e);
        }

        private void OnTerminateCharge(object sender, EventArgs e)
        {
            int msg;

            if (_charging)
                msg = Resource.String.areYouSureStop;
            else
                msg = Resource.String.areYouSureStop2;

            var dlg = new AlertDialog.Builder(Context)
                .SetTitle(Resource.String.stopCharge)
                .SetMessage(msg)
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

            if (status == null)
                return;

            Activity.RunOnUiThread(() =>
            {
                _chargeStatus.Text = status.message;
                _chargePower.Text = String.Format(Context.GetString(Resource.String.powerSupplied), ((double)status.energyConsumption) / 1000);
                long mins;
                TimeSpan diff;
                DateTime started = DateTime.ParseExact(status.started, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                if(string.IsNullOrEmpty(status.finished))
                    diff = DateTime.Now - started;
                else
                {
                    DateTime finished = DateTime.ParseExact(status.finished, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    diff = finished - started;
                };
 
                mins = (long)diff.TotalMinutes;
                _chargeTime.Text = String.Format(Context.GetString(Resource.String.chargingMinutes), mins);
                _progressBar.Max = 30;
                _progressBar.Progress = (int)Math.Max(30, mins);

                if (status.completed)
                {
                    Activity.SetTitle(Resource.String.lastCharge);
                    _chargeStop.SetText(Resource.String.chargeFinished);                    
                }
                _charging = !status.completed;
            });

            if (status.completed)
                _timer.Enabled = false;
        }

        public override void OnStop()
        {
            base.OnStop();

            if(_timer != null)
                _timer.Stop();
        }
    }
}