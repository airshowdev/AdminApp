using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AirshowAddmin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddOrEditFoodView : ContentView
    {
        private int index;
        public AddOrEditFoodView ()
		{
			InitializeComponent ();
        }
        public AddOrEditFoodView(string selectedFoodName, int fooIndex)
        {
            InitializeComponent();
            index = fooIndex;
            Food selected = InfoStore.getFoodByName(selectedFoodName);
            txtDesc.Text = selected.Description;
            txtName.Text = selected.Name;
        }
        User current = InfoStore.CurrentUser;
        private void performSave()
        {
            string JSONToPatch = "";

            JSONToPatch += "\"" + txtName.AutomationId + "\":\"" + txtName.Text.Trim() + "\",";
            JSONToPatch += "\"" + txtDesc.AutomationId + "\":\"" + txtDesc.Text.Trim() + "\",";

            JSONToPatch = "{" + JSONToPatch.Substring(0, JSONToPatch.LastIndexOf(','));

            JSONToPatch += ",\" Last Updated By \":\"" + current.localId + "\"}";

            if (index > InfoStore.getFoodNames().Count)
            {

                try
                {
                    User currentUser = InfoStore.CurrentUser;
                    string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Foods/" + index.ToString() + ".json?auth=" + currentUser.token;

                    ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                    WebClient client = new WebClient();
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string rawResponse = client.UploadString(new Uri(target), "PUT", JSONToPatch);

                    Console.Write("Response is: " + rawResponse);

                    InfoStore.getDatabase();
                    LoadPage(new pgEditFoods());

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
                    // JSONToPatch = String.Concat(" ", JSONToPatch);
                    User currentUser = InfoStore.CurrentUser;
                    string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Foods/" + index.ToString() + ".json?auth=" + currentUser.token;

                    ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                    WebClient client = new WebClient();
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string rawResponse = client.UploadString(new Uri(target), "PUT", JSONToPatch);

                    Console.Write("Response is: " + rawResponse);

                    InfoStore.getDatabase();
                    LoadPage(new pgEditFoods());
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.ToString());
                }
            }
        }

        private void performDelete()
        {
            string toWrite = "";
            Food fooToDelete = new Food("", "");
            foreach (Food foo in Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Foods)
            {

                if (foo.Name == txtName.Text.Trim())
                {
                    fooToDelete = foo;
                }
            }
            try
            {
                Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Foods.Remove(fooToDelete);
                toWrite = JsonConvert.SerializeObject(Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Foods);
            }
            catch (Exception E)
            {

                Console.WriteLine(E.ToString());
                LoadPage(new pgEditFoods());
            }

            try
            {
                User currentUser = InfoStore.CurrentUser;
                string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Foods/.json?auth=" + currentUser.token;

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string rawResponse = client.UploadString(new Uri(target), "PUT", toWrite);
                Console.Write("Response is: " + rawResponse);
                InfoStore.getDatabase();
                LoadPage(new pgEditFoods());
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
                App.Current.MainPage.DisplayAlert("Delete Error", "There was an error deleting this entry.  Please try again", "OK");
                LoadPage(new ModificationPage());
            }
        }

        private void deleteFood(object sender, EventArgs e)
        {
            InfoStore.getDatabase();
            if (Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Foods.Count > 1)
            {
                performDelete();
            }
            else if (!InfoStore.getFoodNames().Contains(txtName.Text.Trim()))
            {
                LoadPage(new pgEditFoods());
                App.Current.MainPage.DisplayAlert("Delete Food Error", "Error deleting food set. Please make sure the \"Name\" entry has not been changed", "OK");
            }
            else
            {
                LoadPage(new pgEditFoods());
                App.Current.MainPage.DisplayAlert("Delete Food Error", "There must be at least food set. Please add another before attempting to delete one", "OK");
            }
        }

        private async void LoadPage(ContentPage page)
        {
            NavigationPage navPage = new NavigationPage(page);
            page.Parent = null;
            await navPage.Navigation.PushAsync(page);
            App.Current.MainPage = page;
        }



        private void saveFood(object sender, EventArgs e)
        {

            if (!Helper.ValidateEntry(txtName))
            {
                App.Current.MainPage.DisplayAlert("Name Error", "Please enter a valid name", "OK");
                txtName.Focus();
            }
            else if (!Helper.ValidateEditor(txtDesc))
            {
                App.Current.MainPage.DisplayAlert("Description Error", "Please enter a valid description", "OK");
                txtDesc.Focus();
            }
            else
            {
                performSave();
            }
        }
    }
}