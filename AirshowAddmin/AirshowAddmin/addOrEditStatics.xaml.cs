using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AirshowAddmin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class addOrEditStatics : ContentView
	{
        private int index;
        public addOrEditStatics()
        {
            InitializeComponent();
        }
        public addOrEditStatics(string selectedStaticName, int staticIndex)
        {
            InitializeComponent();
            index = staticIndex;
            Static selected = InfoStore.getStaticByName(selectedStaticName);
            txtDesc.Text = selected.Description;
            txtName.Text = selected.Name;
            txtImage.Text = selected.Image;
        }

        private void performDelete()
        {
            string toWrite = "";
            Static statToDelete = new Static("", "", "");
            foreach (Static stat in Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Statics)
            {

                if (stat.Name == txtName.Text.Trim())
                {
                    statToDelete = stat;
                }
            }
            try
            {
                Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Statics.Remove(statToDelete);
                toWrite = JsonConvert.SerializeObject(Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Statics);
                Console.WriteLine(toWrite);
            }
            catch (Exception E)
            {
                App.Current.MainPage.DisplayAlert("Delete Error", "There was an error deleting this entry.  Make sure that no fields were changed and try again", "OK");
                Console.WriteLine(E.ToString());
                LoadPage(new pgEditStatics());
            }

            try
            {
                User currentUser = InfoStore.CurrentUser;
                string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Statics/.json?auth=" + currentUser.token;

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string rawResponse = client.UploadString(new Uri(target), "PUT", toWrite);

                Console.Write("Response is: " + rawResponse);
                InfoStore.getDatabase();
                LoadPage(new pgEditStatics());
            }
            catch (Exception E)
            {

                App.Current.MainPage.DisplayAlert("Delete Error", "There was an error deleting this entry.  Make sure that no fields were changed and try again", "OK");

                Console.WriteLine(E.ToString());
            }
        }

        private void deleteStatics(object sender, EventArgs e)
        {
            InfoStore.getDatabase();

            if(Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Statics.Count > 1)
            {
                performDelete();
            }
            else
            {
                App.Current.MainPage = new pgEditStatics();
                App.Current.MainPage.DisplayAlert("Delete Error", "There must be at least one static display stored. Please add another before attempting to delete one.", "OK");
            }
            
        }

        private async void LoadPage(ContentPage page)
        {
            NavigationPage navPage = new NavigationPage(page);
            page.Parent = null;
            await navPage.Navigation.PushAsync(page);
            App.Current.MainPage = page;
        }
        User current = InfoStore.CurrentUser;
        private void performSave()
        {
            string JSONToPatch = "{";
            //string JSONToPatch = JsonConvert.SerializeObject(new Static(txtName.Text.Trim(), txtDesc.Text.Trim(), txtImage.Text.Trim()));

            foreach (View v in stkLayout.Children)
            {
                if (v.GetType() == typeof(Entry))
                {
                    Entry txtBox = v as Entry;
                    JSONToPatch += "\"" + txtBox.AutomationId + "\":\"" + txtBox.Text.Trim() + "\",";
                }
                else if (v.GetType() == typeof(Editor))
                {
                    Editor txtBox = v as Editor;
                    JSONToPatch += "\"" + txtBox.AutomationId + "\":\"" + txtBox.Text.Trim() + "\",";
                }
            }

            JSONToPatch += "\" Last Updated By \":\"" + current.localId + "\"}";

            if (index > InfoStore.getStaticNames().Count)
            {
                try
                {
                    User currentUser = InfoStore.CurrentUser;
                    string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Statics/" + index.ToString() + ".json?auth=" + currentUser.token;

                    ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                    WebClient client = new WebClient();
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string rawResponse = client.UploadString(new Uri(target), "PUT", JSONToPatch);

                    Console.Write("Response is: " + rawResponse);

                    InfoStore.getDatabase();

                    LoadPage(new pgEditStatics());

                }
                catch (Exception E)
                {
                    Console.WriteLine(E.ToString());
                }
            }
            else
            {
                try
                {
                    JSONToPatch = String.Concat(" ", JSONToPatch);
                    User currentUser = InfoStore.CurrentUser;
                    string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Statics/" + index.ToString() + ".json?auth=" + currentUser.token;

                    ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                    WebClient client = new WebClient();
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string rawResponse = client.UploadString(new Uri(target), "PUT", JSONToPatch);
                    JObject response = JObject.Parse(rawResponse);

                    Console.Write("Response is: " + rawResponse);

                    InfoStore.getDatabase();
                    LoadPage(new pgEditStatics());
                }
                catch (Exception E)
                {
                    App.Current.MainPage.DisplayAlert("Save Error", "There was an error saving this entry.  Make sure that all fields are valid and try again", "OK");
                    Console.WriteLine(E.ToString());
                }
            }
            
        }

        private void saveStatic(object sender, EventArgs e)
        {
            if(!Helper.ValidateEntry(txtName))
            {
                App.Current.MainPage.DisplayAlert("Save Error", "There was an error saving this entry.  Enter a valid name and try again", "OK");
                txtName.Focus();
            }
            else if (!Helper.ValidateImageFromEntry(txtImage))
            {
                App.Current.MainPage.DisplayAlert("Save Error", "There was an error saving this entry.  Enter a valid image url and try again", "OK");
                txtImage.Focus();
            }
            else if(!Helper.ValidateEditor(txtDesc))
            {
                App.Current.MainPage.DisplayAlert("Save Error", "There was an error saving this entry. Enter a valid description and try again", "OK");
                txtDesc.Focus();
            }
            else
            {
                performSave();
            }
        }
    }
}