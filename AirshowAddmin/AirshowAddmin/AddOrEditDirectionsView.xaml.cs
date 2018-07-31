using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;

namespace AirshowAddmin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddOrEditDirectionsView : ContentView
	{
        private int index = 0;
		public AddOrEditDirectionsView ()
		{
			InitializeComponent ();
            fullPick();
            typePick();
		}
        public AddOrEditDirectionsView(string selectedDirectionName, int directIndex)
        {
            InitializeComponent();
            fullPick();
            typePick();
            index = directIndex;
            Direction selected = InfoStore.getDirectionByName(selectedDirectionName);
            pckFull.SelectedItem = selected.Full.ToString();
            pckType.SelectedItem = selected.Type.ToString();
            txtName.Text = selected.Name;
            txtXCoord.Text = selected.XCoord.ToString();
            txtYCoord.Text = selected.YCoord.ToString();
            
        }

        private void typePick()
        {
            pckType.Items.Clear();
            pckType.Items.Add("Drive");
            pckType.Items.Add("Bike");
        }

        private void fullPick()
        {
            pckFull.Items.Clear();
            pckFull.Items.Add("True");
            pckFull.Items.Add("False");
        }
        private void performDelete()
        {
            string toWrite = "";
            Direction dirToDelete = new Direction(false,"","",0,0);
            foreach (Direction dir in Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Directions)
            {

                if (dir.Name == txtName.Text.Trim())
                {
                    dirToDelete = dir;
                }
            }
            try
            {
                Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Directions.Remove(dirToDelete);
                toWrite = JsonConvert.SerializeObject(Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected)].Directions);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }

            try
            {
                User currentUser = InfoStore.CurrentUser;
                string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Directions/.json?auth=" + currentUser.token;

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string rawResponse = client.UploadString(new Uri(target), "PUT", toWrite);

                Console.Write("Response is: " + rawResponse);
                InfoStore.getDatabase();
                LoadPage(new pgEditDirections());
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
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
            string JSONToPatch = "";

            JSONToPatch += "\"" + txtName.AutomationId + "\":\"" + txtName.Text.Trim() + "\",";
            JSONToPatch += "\"" + pckFull.AutomationId + "\":" + pckFull.SelectedItem.ToString().ToLower() + ",";
            JSONToPatch += "\"" + txtXCoord.AutomationId + "\":" + Convert.ToDouble(txtXCoord.Text.Trim()) + ",";
            JSONToPatch += "\"" + txtYCoord.AutomationId + "\":" + Convert.ToDouble(txtYCoord.Text.Trim()) + ",";
            JSONToPatch += "\"" + pckType.AutomationId + "\":\"" + pckType.SelectedItem.ToString() + "\",";
            JSONToPatch += "\" Last Updated By \":\"" + current.localId + "\",";

            JSONToPatch = "{" + JSONToPatch.Substring(0, JSONToPatch.LastIndexOf(',')) + "}";

            if (index > InfoStore.getDirectionNames().Count)
            {
                try
                {
                    User currentUser = InfoStore.CurrentUser;
                    string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Directions/" + index.ToString() + ".json?auth=" + currentUser.token;

                    ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };
                    Console.WriteLine("Patch note: " + JSONToPatch);
                    WebClient client = new WebClient();
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string rawResponse = client.UploadString(new Uri(target), "PUT", JSONToPatch);

                    InfoStore.getDatabase();
                    LoadPage(new pgEditDirections());

                }
                catch (Exception E)
                {
                    Console.WriteLine(E.ToString());
                    App.Current.MainPage.DisplayAlert("Save Error", "There was an error saving this entry.  Please try again", "OK");
                    LoadPage(new pgEditDirections());
                }
            }
            else
            {
                try
                {
                    // JSONToPatch = String.Concat(" ", JSONToPatch);
                    User currentUser = InfoStore.CurrentUser;
                    string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Directions/" + index.ToString() + ".json?auth=" + currentUser.token;

                    ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                    WebClient client = new WebClient();
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    Console.WriteLine("Patched: " + JSONToPatch);
                    string rawResponse = client.UploadString(new Uri(target), "PATCH", JSONToPatch);
                    Console.Write("Response is: " + rawResponse);

                    InfoStore.getDatabase();
                    LoadPage(new pgEditDirections());
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.ToString());
                    LoadPage(new pgEditDirections());
                }
            }
        }

        private void btnSave_Pressed(object sender, EventArgs e)
        {
            if (!Helper.ValidateEntry(txtName))
            {
                App.Current.MainPage.DisplayAlert("Name Error", "Please enter a valid name", "OK");
                txtName.Focus();
            }
            else if(!Helper.ValidatePickerSelected(pckFull))
            {
                App.Current.MainPage.DisplayAlert("Full Picker Error", "Please select a value in the Full picker", "OK");
            }
            else if (!Helper.ValidatePickerSelected(pckType))
            {
                App.Current.MainPage.DisplayAlert("Type Picker Error", "Please select a value in the Type picker", "OK");
            }
            else if (!Helper.validateCoordinateFromEntry(txtXCoord))
            {
                App.Current.MainPage.DisplayAlert("X Coordinate Error", "Please entrer a valid value in the X coordinate box", "OK");
                txtXCoord.Focus();
            }
            else if (!Helper.validateCoordinateFromEntry(txtYCoord))
            {
                App.Current.MainPage.DisplayAlert("Y Coordinate Error", "Please entrer a valid value in the Y coordinate box", "OK");
                txtYCoord.Focus();
            }
            else
            {
                performSave();
            }
        }

       
        private void btnDelete_Pressed(object sender, EventArgs e)
        {
            InfoStore.getDatabase();
            if(Database.Airshows[InfoStore.getAirshowIndex(InfoStore.Selected+1)].Directions.Count > 1)
            {
                performDelete();
            }
            else if(!InfoStore.getDirectionNames().Contains(txtName.Text.Trim()))
            {
                LoadPage(new pgEditDirections());
                App.Current.MainPage.DisplayAlert("Delete Directions Error", "Error deleting direction set. Please make sure the \"Name\" entry has not been changed", "OK");
            }
            else
            {
                LoadPage(new pgEditDirections());
                App.Current.MainPage.DisplayAlert("Delete Directions Error", "There must be at least one direction set. Please add another before attempting to delete one", "OK");
            }
        }
    }
}