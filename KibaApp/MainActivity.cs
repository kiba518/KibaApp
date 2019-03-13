using Android.App;
using Android.Widget;
using Android.OS;
 
using Android.Views;
using ZXing.Mobile;
using System.Threading.Tasks;
using Android.Views.Animations;

namespace KibaApp
{
    [Activity(Label = "KibaApp", MainLauncher = true)]
    public class MainActivity : BaseActivity
    {
        View zxingOverlay;
        MobileBarcodeScanner scanner;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainActivity);

            Button btnScan = this.FindControl<Button>("btnScan");
            btnScan.Click += (s, e) =>
            {
                scanner = new MobileBarcodeScanner();
                Task t = new Task(AutoScan);
                t.Start();
            };
        }
        async void AutoScan()
        {
            scanner.UseCustomOverlay = true;
            zxingOverlay = LayoutInflater.FromContext(this).Inflate(Resource.Layout.ZxingOverlay, null);

            ImageView ivScanning = zxingOverlay.FindViewById<ImageView>(Resource.Id.ivScanning);
            Button btnCancelScan = zxingOverlay.FindViewById<Button>(Resource.Id.btnCancelScan);
            btnCancelScan.Click += (s, e) =>
            {
                if (scanner != null)
                {
                    scanner.Cancel();
                }
            };

            zxingOverlay.Measure(MeasureSpecMode.Unspecified.GetHashCode(), MeasureSpecMode.Unspecified.GetHashCode());
            int width = zxingOverlay.MeasuredWidth;
            int height = zxingOverlay.MeasuredHeight;

            // 从上到下的平移动画
            Animation verticalAnimation = new TranslateAnimation(0, 0, 0, height);
            verticalAnimation.Duration = 3000; // 动画持续时间
            verticalAnimation.RepeatCount = Animation.Infinite; // 无限循环

            // 播放动画
            ivScanning.Animation = verticalAnimation;
            verticalAnimation.StartNow();

            scanner.CustomOverlay = zxingOverlay;
            var mbs = MobileBarcodeScanningOptions.Default;
            mbs.AssumeGS1 = true;
            mbs.AutoRotate = true;
            mbs.DisableAutofocus = false;
            mbs.PureBarcode = false;
            mbs.TryInverted = true;
            mbs.TryHarder = true;
            mbs.UseCode39ExtendedMode = true;
            mbs.UseFrontCameraIfAvailable = false;
            mbs.UseNativeScanning = true;

            var result = await scanner.Scan(this, mbs);
            HandleScanResult(result);

        }
        void HandleScanResult(ZXing.Result result)
        {
            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                if (result.Text != null && result.Text.Trim().Length > 5)
                {
                    this.RunOnUi(() => { this.ShowToast(result.Text); });

                }
                else
                {
                    this.RunOnUi(() => { this.ShowToast("扫描无数据"); });
                }
            }
            else
            {
                this.RunOnUi(() => { this.ShowToast("扫描取消"); });
            }
            scanner.Cancel();
        }

    }
}

