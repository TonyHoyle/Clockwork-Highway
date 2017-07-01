using System;
using Android;
using Android.OS;
using Android.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Gms.Vision.Barcodes;
using Android.Gms.Vision;
using Android.Graphics;
using Android.Runtime;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Widget;
using TonyHoyle.EH;
using Android.Content;

namespace ClockworkHighway.Android
{
	[Activity(Label = "@string/ApplicationName")]
	public class BarcodeActivity : AppCompatActivity, ISurfaceHolderCallback, Detector.IProcessor
	{
        CameraSource mCameraSource;
        SurfaceView mCameraView;
        BarcodeDetector mBarcodeDetector;

        public BarcodeActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.barcode);

			mCameraView = FindViewById<SurfaceView>(Resource.Id.camera_view);
			mCameraView.Holder.AddCallback(this);

			if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
                RequestPermissions(new string[] { Manifest.Permission.Camera }, 0);
            else
                Initialise();
        }

        protected void Initialise()
        {

			mBarcodeDetector = new BarcodeDetector.Builder(this)
                                                     .SetBarcodeFormats(BarcodeFormat.QrCode)
                                                     .Build();
            if(!mBarcodeDetector.IsOperational)
            {
                Toast.MakeText(this, "Barcode detector is not functional", ToastLength.Long);
                Finish();
            }

            mCameraSource = new CameraSource.Builder(this, mBarcodeDetector)
                                            .SetAutoFocusEnabled(true)
                                            .Build();

            mBarcodeDetector.SetProcessor(this);
        }

		public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
		{
		}

		public void SurfaceCreated(ISurfaceHolder holder)
		{
            if(mCameraSource != null)
                mCameraSource.Start(mCameraView.Holder); 
		}

		public void SurfaceDestroyed(ISurfaceHolder holder)
		{
            if(mCameraSource != null)
                mCameraSource.Stop();
		}

        public void ReceiveDetections(Detector.Detections detections)
        {
            var items = detections.DetectedItems;
            if (items.Size() > 0)
            {
                var barcode = (Barcode)items.ValueAt(0);
                string pumpId = barcode.DisplayValue;

                var intent = new Intent(this, typeof(BarcodeActivity));
                intent.PutExtra("pumpId", pumpId);

                SetResult(Result.Ok, intent);
                Finish();
            }
		}

        public void Release()
        {
        }

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (requestCode != 0)
				return;

            if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
            {
                Initialise();
                mCameraSource.Start(mCameraView.Holder);
            }
		}
	}
}
