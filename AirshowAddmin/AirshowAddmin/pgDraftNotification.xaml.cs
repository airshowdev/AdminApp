using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Firebase.Database;
using Firebase;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;

namespace AirshowAddmin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class pgDraftNotification : ContentPage
    {
        public pgDraftNotification()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }
        }
        private void DraftNote(object sender, EventArgs e)
        {
            try
            {
                SendPushNotification(txtHeader.Text.Trim(), txtBody.Text.Trim(), "Enabled");
                DisplayAlert("Sent Message", "The notification was sent successfully", "OK");
                txtBody.Text = "";
                txtHeader.Text = "";
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
                DisplayAlert("Message Error", "There was an error while sending the message, please try again", "OK");
            }
        }

        private async void LoadPage(ContentPage page)
        {
            NavigationPage navPage = new NavigationPage(page);
            page.Parent = null;
            await navPage.Navigation.PushAsync(page);
            App.Current.MainPage = page;
        }
        protected override bool OnBackButtonPressed()
        {
            try
            {
                LoadPage(new pgEditOrNotify());
                return true;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
                return false;
            }

        }

        private void backButton_Pressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        private void SendPushNotification(string strTitle, string strBody, string strSound)
        {
            try
            {
                string apiKey = "AIzaSyBStVTKLxZC21zf0UbQlm7KinAOuc3slJ4";
                string target = "https://fcm.googleapis.com/fcm/send";

                string message = JsonConvert.SerializeObject(new
                {
                    to = "/topics/" + InfoStore.Selected.Replace(" ", "").Replace("&", "And"),
                    title = strTitle,
                    body = strBody
                });

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers.Add("Authorization:key=" + apiKey);
                string rawResponse = client.UploadString(new Uri(target), "POST", message);
                JObject response = JObject.Parse(rawResponse);
                Console.WriteLine(rawResponse);

                string messageid = rawResponse.Replace("{\"message_id\":", "").Replace("}", "");

                LogMessage(message, messageid);
            }
            catch (Exception E)
            {
                throw E;
            }
        }


        private void LogMessage(string messageJson, string messageid)
        {
            User currentUser = InfoStore.CurrentUser;
            JObject message = JObject.Parse(messageJson);
            string target = "https://airshowapp-d193b.firebaseio.com/Messages/" + Convert.ToString((Database.Messages == null)?0:Database.Messages.Count) + "/.json?auth=" + currentUser.token;
            ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };
            string dateNow = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            string messageLog = JsonConvert.SerializeObject(new
            {
                uid = currentUser.localId,
                body = message.GetValue("body"),
                title = message.GetValue("title"),
                id = messageid,
                date = dateNow,
                topic = InfoStore.Selected
            });

            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string rawResponse = client.UploadString(new Uri(target), "PUT", messageLog);
            JObject response = JObject.Parse(rawResponse);
            Console.WriteLine(rawResponse);
            InfoStore.database = Database.FromJson(rawResponse);
        }
    }

}