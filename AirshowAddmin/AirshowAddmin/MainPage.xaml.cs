using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Firebase.Database;
using Firebase;

namespace AirshowAddmin
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        User currentUser = null;

        

        private void slLogin_ChildAdded(object sender, EventArgs e)
        {
            IList<View> slChildren = loginStackLayout.Children;
            foreach (View v in slChildren)
            {
                if (v.GetType() != typeof(Button))
                {
                    v.WidthRequest = loginStackLayout.Width;
                }
            }
        }
        private void btnLogin_Clicked(object sender, EventArgs e)
        {

            NavigationPage navPage = new NavigationPage(this);
            IList<View> children = loginStackLayout.Children;

            
            if (txtUsername.Text.Trim() != "" && txtPass.Text.Trim() != "")
            {
                if (Login(txtUsername.Text, txtPass.Text))
                {
                    InfoStore.getDatabase();
                    App.Current.MainPage = new pgSelectOrCreate();
                }
                else
                {
                    DisplayAlert("Login Invalid", "Input correct credentials", "OK");
                    txtPass.Text = "";
                    txtUsername.Text = "";
                }
            }



        }
        HttpClient client = new HttpClient();
        
        
        private bool Login(string Email, string Password)
        {
            try
            {
                string apiKey = "AIzaSyCLZKa2KoR0rkmhnLm1rSGAHvS19cApY1A";
                string target = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + apiKey;
                string payload = JsonConvert.SerializeObject(new
                {
                    email = Email,
                    password = Password,
                    returnSecureToken = true
                });

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string rawResponse = client.UploadString(new Uri(target), "POST", payload);
                JObject response = JObject.Parse(rawResponse);

                currentUser = new User(response.Value<object>("idToken").ToString(), response.Value<object>("email").ToString(), response.Value<object>("localId").ToString(), response.Value<object>("refreshToken").ToString());
                
                InfoStore.CurrentUser = currentUser;
                return true;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
                return false;
            }
        }
    }
}
