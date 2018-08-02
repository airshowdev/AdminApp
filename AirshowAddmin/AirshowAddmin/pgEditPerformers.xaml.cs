using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public partial class pgEditPerformers : ContentPage
	{
        Button btnBack;
		public pgEditPerformers ()
		{
			InitializeComponent ();
            btnBack = (Button)myStackLayout.Children[2];
            foreach(string name in InfoStore.getPerformerNames())
            {
                pckPerformers.Items.Add(name);
            }
            pckPerformers.Items.Add("Add New Performer");
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }
        }
       

        /*********************************************************
         
         This stuff is all used to send the updates to firebase.
             
         *******************************************************/     
         
        List<View> entries = new List<View>();
        //This works because I created a list of entries^^. Everytime a new entry is made I add it to the list. Now we can Foreach them easily
        //I interate through the list and find all the entries that have the same autoID as the sender button
        //Then I create a string for every entry that matches and add the string to the JSONToPatch string
        //So the save buttons send a patch for every child of the specific performer that has the same index as their autoID... not best practice but it works
        private void sendPatch(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;

            string strIndex = btnSender.AutomationId;

            string JSONToPatch = "";

            foreach(View v in entries)
            {
                if (v.GetType() == typeof(Entry))
                {
                    Entry ent = v as Entry;
                    if (ent.AutomationId.Split('.')[1] == strIndex)
                    {
                        JSONToPatch += "\"" + ent.AutomationId.Split('.')[0] + "\":\"" + ent.Text.Trim() + "\",";
                    }
                }
                else if (v.GetType() == typeof(Editor))
                {
                    Editor edt = v as Editor;
                    if (edt.AutomationId.Split('.')[1] == strIndex)
                    {
                        JSONToPatch += "\"" + edt.AutomationId.Split('.')[0] + "\":\"" + edt.Text.Trim() + "\",";
                    }
                }
                else if (v.GetType() == typeof(Picker))
                {
                    Picker pck = v as Picker;
                    if (pck.AutomationId.Split('.')[1] == strIndex)
                    {
                        JSONToPatch += "\"" + pck.AutomationId.Split('.')[0] + "\":\"" + pck.SelectedItem.ToString() + "\",";
                    }
                }
                
            }
            JSONToPatch = "{" + JSONToPatch.Substring(0, JSONToPatch.LastIndexOf(',')) + "}";

            try
            {
                User currentUser = InfoStore.CurrentUser;
                string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Performers/" + btnSender.AutomationId + ".json?auth=" + currentUser.token;

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string rawResponse = client.UploadString(new Uri(target), "PATCH", JSONToPatch);
                JObject response = JObject.Parse(rawResponse);

                Console.Write("Response is: " + rawResponse);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }
        }
        /*********************************************************

        This is the end of the firebase update stuff.

        *******************************************************/
        
        private string strSelectedPerformer = "";
        
        private void storeSelected(object sender, EventArgs e)
        {
            if (myStackLayout.Children.Count > 3)
            {
                myStackLayout.Children.RemoveAt(2);
            }
            strSelectedPerformer = pckPerformers.SelectedItem.ToString();
            myStackLayout.Children.Add(new AddOrEditPerformerView(strSelectedPerformer, pckPerformers.SelectedIndex));

            if (!myStackLayout.Children.Contains(btnBack))
            { myStackLayout.Children.Add(btnBack); }
            else
            {
                myStackLayout.Children.Remove(btnBack);
                myStackLayout.Children.Add(btnBack);
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
                App.Current.MainPage = new ModificationPage();
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
    }
}