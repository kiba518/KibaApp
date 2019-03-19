using Android.App;
using Android.Widget;
using Android.OS;
 
using Android.Views;
using ZXing.Mobile;
using System.Threading.Tasks;
using Android.Views.Animations;

namespace KibaApp
{
    [Activity(Label = "KibaApp")]
    public class ParamActivity : BaseActivity
    {
       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ParamActivity);
            string para1 =  this.Intent.GetStringExtra("para1");
            int para2 = this.Intent.GetIntExtra("para2",-1);
            this.ShowToast("para1:" + para1 + "===para2:" + para2);
        }
       

    }
}

