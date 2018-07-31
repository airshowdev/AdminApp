using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Firebase.Auth;
using Firebase;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AirshowAddmin.Droid
{
    [Activity(Label = "AirshowAddmin", Icon = "@mipmap/icon", Theme = "@style/Theme.Splash", MainLauncher =true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            SetTheme(Resource.Style.MainTheme);
            

            base.OnCreate(bundle);


            global::Xamarin.Forms.Forms.Init(this, bundle);

            FirebaseApp.InitializeApp(Application.Context);
            LoadApplication(new App());

           



        }
        
    }
}

