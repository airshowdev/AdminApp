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
	public partial class AddOrEditPerformerView : ContentView
	{
        int aSIndex = InfoStore.getAirshowIndex(InfoStore.Selected);

        private int index;
        Performer selected = new Performer("", "", "Preparing", "", InfoStore.getPerformerNames().Count);
		public AddOrEditPerformerView ()
		{
			InitializeComponent ();
            pckInAir.ItemsSource = InfoStore.statusOptions;
        }

        public AddOrEditPerformerView(string selectedPerformerName, int perfIndex)
        {
            InitializeComponent();
            index = perfIndex;
            pckInAir.ItemsSource = InfoStore.statusOptions;
            selected = InfoStore.getPerformerByName(selectedPerformerName);
            txtDesc.Text = selected.Description;
            txtImage.Text = selected.Image;
            txtName.Text = selected.Name;
            txtSchedule.Text = selected.OrderNumber.ToString();
            pckInAir.SelectedItem = selected.InAir;
        }

        private async void LoadPage(ContentPage page)
        {
            NavigationPage navPage = new NavigationPage(page);
            page.Parent = null;
            await navPage.Navigation.PushAsync(page);
            App.Current.MainPage = page;
        }

        private void performDelete()
        {
            string toWrite = "";
            Performer perfToDelete = new Performer("", "", "", "", 1);
            foreach (Performer perf in Database.Airshows[aSIndex].Performers)
            {
                if (perf.Name == txtName.Text.Trim())
                {
                    perfToDelete = perf;
                }
            }
            try
            {
                Database.Airshows[aSIndex].Performers.Remove(perfToDelete);
                toWrite = JsonConvert.SerializeObject(Database.Airshows[aSIndex].Performers);
                Console.WriteLine(toWrite);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }

            try
            {
                User currentUser = InfoStore.CurrentUser;
                string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) + "/Performers/.json?auth=" + currentUser.token;

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string rawResponse = client.UploadString(new Uri(target), "PUT", toWrite);
                JObject response = JObject.Parse(rawResponse);

                Console.Write("Response is: " + rawResponse);
                InfoStore.getDatabase();
                LoadPage(new pgEditPerformers());
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }
        }

        private void deletePerformer(object sender, EventArgs e)
        {
           if(Database.Airshows[aSIndex].Performers.Count > 1)
            {
                performDelete();
            }
           else
            {
                App.Current.MainPage = new pgEditPerformers();
                App.Current.MainPage.DisplayAlert("Performer Delete Error", "There must be at least one performer.  Please add another before attempting to delete one", "OK");
            }
        }

        User current = InfoStore.CurrentUser;

        private void performSave()
        {
            try
            {
                string JSONToPatch = "{";

                OrderPerformers();

                //JSONToPatch = JsonConvert.SerializeObject(Database.Airshows[aSIndex].Performers);

                IList<Entry> entries = new List<Entry>();
                foreach (View v in stkLayout.Children)
                {
                    if (v.GetType() == typeof(Entry))
                    {
                        Entry txtBox = v as Entry;
                        JSONToPatch += "\"" + txtBox.AutomationId + "\":\"" + txtBox.Text.Trim() + "\",";
                    }
                }

                JSONToPatch += "\" Last Updated By \":\"" + current.localId + "\"}";

                User currentUser = InfoStore.CurrentUser;
                string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + aSIndex.ToString() + "/Performers/" + index +"/.json?auth=" + currentUser.token;

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string rawResponse = client.UploadString(new Uri(target), "PATCH", JSONToPatch);

                Console.Write("Response is: " + rawResponse);
            }
            catch (Exception E) {
                Console.Write(E.ToString());
            }

                InfoStore.getDatabase();

                LoadPage(new pgEditPerformers());
        }


        private void OrderPerformers()
        {
            Performer changed = InfoStore.getPerformerByName(selected.Name);
            bool newisHigher = (changed.OrderNumber < Convert.ToInt16(txtSchedule.Text.Trim()));
            changed.OrderNumber =   Convert.ToInt16(txtSchedule.Text.Trim());
            changed.Name        =   txtName.Text.Trim();
            changed.Description =   txtName.Text.Trim();
            changed.Image       =   txtImage.Text.Trim();
            changed.InAir       =   pckInAir.SelectedItem.ToString();

            if (index >= InfoStore.getPerformerNames().Count)
            {
                Database.Airshows[aSIndex].Performers.Add(changed);
            }
            else
            {
                Database.Airshows[aSIndex].Performers[index] = changed;
            }

            Database.Airshows[aSIndex].Performers = Database.Airshows[aSIndex].Performers.OrderBy(x => x.OrderNumber).ToList();
            

            if (!newisHigher)
            {
                for (int i =1; i <= Database.Airshows[aSIndex].Performers.Count; i++)
                {
                    if (changed.OrderNumber == Database.Airshows[aSIndex].Performers.Count && Database.Airshows[aSIndex].Performers[Database.Airshows[aSIndex].Performers.Count - 1] != changed)
                    {
                        Database.Airshows[aSIndex].Performers[Database.Airshows[aSIndex].Performers.Count - 2] = Database.Airshows[aSIndex].Performers[Database.Airshows[aSIndex].Performers.Count - 1];
                        Database.Airshows[aSIndex].Performers[Database.Airshows[aSIndex].Performers.Count - 2].OrderNumber = Database.Airshows[aSIndex].Performers.Count - 1;
                        Database.Airshows[aSIndex].Performers[Database.Airshows[aSIndex].Performers.Count - 1] = changed;
                    }
                    else if (!(changed.OrderNumber == Database.Airshows[aSIndex].Performers[i - 1].OrderNumber))
                    {
                        Database.Airshows[aSIndex].Performers[i - 1].OrderNumber = i;
                    }
                    else if (Database.Airshows[aSIndex].Performers[i - 1] == changed)
                    {
                        Database.Airshows[aSIndex].Performers[i - 1].OrderNumber = changed.OrderNumber;
                    }
                    else
                    {
                        Database.Airshows[aSIndex].Performers[i - 1].OrderNumber = ++i;
                    }

                }
            }
            else
            {
                for (int i = 1; i <= Database.Airshows[aSIndex].Performers.Count; i++)
                {
                    if (!(changed.OrderNumber == Database.Airshows[aSIndex].Performers[i - 1].OrderNumber))
                    {
                        Database.Airshows[aSIndex].Performers[i - 1].OrderNumber = i;
                    }
                    else if (Database.Airshows[aSIndex].Performers[i - 1].Name == changed.Name)
                    {
                        Database.Airshows[aSIndex].Performers[i - 1].OrderNumber = changed.OrderNumber;
                    }
                    else
                    {
                        Database.Airshows[aSIndex].Performers[i - 1].OrderNumber = i - 1;
                    }
                }
            }
            Database.Airshows[aSIndex].Performers = Database.Airshows[aSIndex].Performers.OrderBy(x => x.OrderNumber).ToList();

        }


        private void savePerformer(object sender, EventArgs e)
        {
            if (!Helper.ValidateEntry(txtName))
            {
                App.Current.MainPage.DisplayAlert("Name error", "Please enter a valid name in the Name box", "OK");
                txtName.Focus();
            }
            else if(!int.TryParse(txtSchedule.Text.Trim(), out int intOrder))
            {
                App.Current.MainPage.DisplayAlert("Schedule error", "Please enter a valid number in the Schedule box", "OK");
                txtSchedule.Focus();
            }
            else if (!Helper.ValidateEditor(txtDesc))
            {
                App.Current.MainPage.DisplayAlert("Description error", "Please enter a valid number in the Description box", "OK");
                txtDesc.Focus();
            }
            else if(!Helper.ValidateImageFromEntry(txtImage))
            {
                App.Current.MainPage.DisplayAlert("Image error", "Please enter a valid image URL in the Schedule box", "OK");
                txtImage.Focus();
            }
            else if(!Helper.ValidatePickerSelected(pckInAir))
            {
                App.Current.MainPage.DisplayAlert("Status error", "Please make a selection in the In Air picker", "OK");
            }
            else
            {
                performSave();
            }
        }
    }
}