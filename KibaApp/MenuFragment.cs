using Android.App;
using Android.OS;
using Android.Views;

namespace KibaApp
{
    [Activity(Label = "KibaApp")]
    public class MenuFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        { 
            View view = inflater.Inflate(Resource.Layout.MenuFragment, container, false); 
            return view; 
        }
    }

   

}

