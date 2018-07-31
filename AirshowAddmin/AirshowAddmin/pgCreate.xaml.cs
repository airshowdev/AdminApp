using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AirshowAddmin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class pgCreate : ContentPage
    {
        public pgCreate()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }

            txtDate.Placeholder = "MM/DD/YYYY";

            if (isNew)
            {
                newAirshow = new Airshow()
                {
                    Name = "New"
                };
                Database.Airshows.Add(newAirshow);
                InfoStore.Selected = "New";
                isNew = false;
            }
            else {

                newAirshow = Database.Airshows[Database.Airshows.Count - 1];
                LoadBoxData(newAirshow);
                InfoStore.database = new Database();
            }
            newAirshowIndex = Database.Airshows.IndexOf(newAirshow);

        }

        static bool isNew = true;
        int newAirshowIndex;
        Airshow newAirshow;
        

        private bool PostAirshow()
        {

            if (Validate())
            {

                InfoStore.database = new Database();
                try
                {

                    newAirshow.LastUpdate = InfoStore.CurrentUser.localId;
                    User currentUser = InfoStore.CurrentUser;

                    string target = "https://airshowapp-d193b.firebaseio.com/Airshows/"+Convert.ToString(newAirshowIndex)+"/.json?auth=" + currentUser.token;

                    string payload = JsonConvert.SerializeObject(newAirshow);


                    ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                    WebClient client = new WebClient();
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string rawResponse = client.UploadString(new Uri(target), "PUT", payload);
                    JObject response = JObject.Parse(rawResponse);

                    Console.Write(rawResponse);
                    App.Current.MainPage = new pgSelectOrCreate();
                    isNew = true;
                    return true;
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.ToString());
                    return false;
                }
            }
            else {
                return false;
            }
        }

        private void viewUnfocused(object sender, FocusEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(Entry))
                {
                    Entry ent = sender as Entry;
                    switch (ent.AutomationId)
                    {
                        case "Base":

                            newAirshow.Base = ent.Text.Trim();
                            break;

                        case "Date":
                            newAirshow.Date = ent.Text.Trim();
                            break;
                        case "Facebook Link":
                            newAirshow.FacebookLink = ent.Text.Trim();
                            break;
                        case "Instagram Link":
                            newAirshow.InstagramLink = ent.Text.Trim();
                            break;
                        case "Name":
                            newAirshow.Name = ent.Text.Trim();
                            break;
                        case "Sponsors":
                            newAirshow.Sponsors = ent.Text.Trim();
                            break;
                        case "Twitter Link":
                            newAirshow.TwitterLink = ent.Text.Trim();
                            break;
                        case "Website Link":
                            newAirshow.WebsiteLink = ent.Text.Trim();
                            break;
                    }
                }
                else if (sender.GetType() == typeof(Editor))
                {
                    Editor edt = sender as Editor;
                    newAirshow.Description = edt.Text.Trim();
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString() + E.StackTrace);
            }
        }


        protected override bool OnBackButtonPressed()
        {
            try
            {
                if (Database.Airshows[Database.Airshows.Count - 1] == new Airshow() { Name = "New" })
                {
                    Database.Airshows.Remove(newAirshow);
                }
                App.Current.MainPage = new pgSelectOrCreate();
                isNew = true;
                return true;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
                return false;
            }

        }

        private void performersClicked(object sender, EventArgs e)
        {
            LoadPage(new pgAddPerformer());
        }

        private void directionsClicked(object sender, EventArgs e)
        {
            LoadPage(new pgAddDirections());
        }

        private void foodsClicked(object sender, EventArgs e)
        {
            LoadPage(new pgAddFood());
        }

        private void staticsClicked(object sender, EventArgs e)
        {
            LoadPage(new pgAddStatic());
        }

        private void backButton_Pressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        private void saveClicked(object sender, EventArgs e)
        {
            PostAirshow();
        }

        private async void LoadPage(ContentPage page)
        {
            NavigationPage navPage = new NavigationPage(page);
            page.Parent = null;
            await navPage.Navigation.PushAsync(page);
            App.Current.MainPage = page;
        }


        private bool Validate()
        {
            if (!Helper.ValidateEntry(txtBase))
            {
                DisplayAlert("Invalid Base", "Please enter a base name between 1 and 60 characters long", "OK");
                txtBase.Focus();
            }
            else if (!ValidateDate(txtDate))
            {
                DisplayAlert("Invalid Date", "Please enter a date in the format (MM/DD/YYYY)", "OK");
                txtDate.Focus();
            }
            else if (!Helper.ValidateEditor(txtDesc))
            {
                DisplayAlert("Invalid Description", "Please enter a description", "OK");
                txtDesc.Focus();
            }
            else if (!Helper.ValidateLink(txtFBLink) && txtFBLink.Text != "None")
            {
                DisplayAlert("Invalid Facebook URL", "Please enter a valid URL", "OK");
                txtFBLink.Focus();
            }
            else if (!Helper.ValidateEntry(txtName))
            {
                DisplayAlert("Invalid Name", "Please enter a name between 1 and 60 characters", "OK");
                txtName.Focus();
            }
            else if (!Helper.ValidateLink(txtSponsors) && txtSponsors.Text != "None")
            {
                DisplayAlert("Invalid Sponsor URL", "Please enter a valid URL", "OK");
                txtSponsors.Focus();
            }
            else if (!Helper.ValidateLink(txtTwitterLink) && txtTwitterLink.Text != "None")
            {
                DisplayAlert("Invalid Twitter URL", "Please enter a valid URL", "OK");
                txtTwitterLink.Focus();
            }

            else if (!Helper.ValidateLink(txtWebsiteLink) && txtWebsiteLink.Text != "None")
            {
                DisplayAlert("Invalid Website URL", "Please enter a valid URL", "OK");
                txtWebsiteLink.Focus();
            }

            else if (!Helper.ValidateLink(txtIGLink) && txtIGLink.Text != "None")
            {
                DisplayAlert("Invalid Instagram URL", "Please enter a valid URL", "OK");
                txtIGLink.Focus();
            }
            else
            {
                if (Database.Airshows[newAirshowIndex].Directions == null)
                {
                    Database.Airshows[newAirshowIndex].Directions = new List<Direction>() {new Direction(false, "Coming Soon", "Drive", 12,12)};
                }
                if (Database.Airshows[newAirshowIndex].Performers == null)
                {
                    Database.Airshows[newAirshowIndex].Performers = new List<Performer>() { new Performer("Coming Soon","Coming Soon", "Preparing", "None", 1) };
                }
                if (Database.Airshows[newAirshowIndex].Statics == null)
                {
                    Database.Airshows[newAirshowIndex].Statics = new List<Static>() { new Static("Coming Soon", "Coming Soon", "None") };
                }
                if (Database.Airshows[newAirshowIndex].Foods == null)
                {
                    Database.Airshows[newAirshowIndex].Foods = new List<Food>() {new Food( "Coming Soon", "Coming Soon")};
                }
                newAirshow.Base = txtBase.Text.Trim();
                newAirshow.Date = txtDate.Text.Trim();
                newAirshow.Description = txtDesc.Text.Trim();
                newAirshow.FacebookLink = txtFBLink.Text.Trim();
                newAirshow.Name = txtName.Text.Trim();
                newAirshow.Sponsors = txtSponsors.Text.Trim();
                newAirshow.TwitterLink = txtTwitterLink.Text.Trim();
                newAirshow.WebsiteLink = txtWebsiteLink.Text.Trim();
                newAirshow.InstagramLink = txtIGLink.Text.Trim();
                return true;
            }
            return false;
        }

        private bool ValidateDate(Entry entDate)
        {
            try
            {

                String[] date = entDate.Text.Trim().Split('/');
                int day = Convert.ToByte(date[1]);
                int month = Convert.ToByte(date[0]);
                int year = Convert.ToInt16(date[2]);

                string dateOut = String.Format("{0}/{1}/{2}", month, day, year);

                DateTime dt = new DateTime();

                if (DateTime.TryParse(dateOut, out dt))
                {
                    entDate.Text = dateOut;
                    return true;
                }
                else {
                    return false;
                }

            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
                return false;
            }
        }

        private void LoadBoxData(Airshow airshow)
        {
            txtBase.Text = airshow.Base ?? "";
            txtDate.Text = airshow.Date ?? "";
            txtDesc.Text = airshow.Description ?? "";
            txtFBLink.Text = airshow.FacebookLink ?? "None";
            txtTwitterLink.Text = airshow.TwitterLink ?? "None";
            txtWebsiteLink.Text = airshow.WebsiteLink ?? "None";
            txtIGLink.Text = airshow.InstagramLink ?? "None";
            txtName.Text = airshow.Name ?? "";

        }
    }

}