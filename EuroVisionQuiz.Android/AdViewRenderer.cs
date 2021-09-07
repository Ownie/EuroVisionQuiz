using Android.Gms.Ads;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EuroVisionQuiz.Ad.AdControlView), typeof(EuroVisionQuiz.Droid.PlatformSpecific.AdViewRenderer))]

namespace EuroVisionQuiz.Droid.PlatformSpecific
{
    public class AdViewRenderer : ViewRenderer<Ad.AdControlView, AdView>
    {
        public AdViewRenderer() : base(Android.App.Application.Context)
        {
        }

        //string adUnitId = string.Empty;
        //private string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //test
        private string adUnitId = "ca-app-pub-7647345657299803/8858527135"; //real

        //Note you may want to adjust this, see further down.
        private AdSize adSize = AdSize.SmartBanner;

        private AdView adView;

        private AdView CreateNativeAdControl()
        {
            if (adView != null)
                return adView;

            // This is a string in the Resources/values/strings.xml that I added or you can modify it here. This comes from admob and contains a / in it
            //adUnitId = Forms.Context.Resources.GetString(Resource.String.banner_ad_unit_id);
            adView = new AdView(Context)
            {
                AdSize = adSize,
                AdUnitId = adUnitId
            };
            //adView.SetBackgroundColor(new Android.Graphics.Color(110,20,78));

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            adView.LayoutParameters = adParams;

            adView.LoadAd(new AdRequest
                            .Builder()
                            .AddTestDevice(AdRequest.DeviceIdEmulator)
                            .AddTestDevice("35824005111110")
                            .Build());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Ad.AdControlView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                CreateNativeAdControl();
                SetNativeControl(adView);
            }
        }
    }
}