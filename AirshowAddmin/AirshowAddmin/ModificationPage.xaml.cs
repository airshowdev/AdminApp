using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AirshowAddmin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModificationPage : ContentPage
    {

        TableView tvProperties;
        TableSection tsPropertires = new TableSection();

        private List<string> badEntries = new List<string>();
        bool readyToSend = false;

        public ModificationPage()
        {
            InitializeComponent();
            
        }

        Dictionary<string, object> Properties;

        string JSONToPatch = "";

        private bool LoadBoxes(ScrollView sv)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }
            try
            {
                tvProperties = tblView;
                Properties = Database.getAirshowInfo(InfoStore.Selected);
                IReadOnlyList<Element> children = sv.Children;


                sv.HeightRequest = App.Current.MainPage.Height;
                tvProperties.Margin = 5;

                foreach (KeyValuePair<string, object> prop in Properties)
                {
                    if (prop.Value != null)
                    {
                        ViewCell vcProperty = new ViewCell();
                        StackLayout slProperty = new StackLayout();
                        vcProperty.Height = slProperty.Height;
                        slProperty.Orientation = StackOrientation.Horizontal;


                        if (prop.Value.GetType() != typeof(String))
                        {

                            Button btnProperty = new Button
                            {
                                Text = prop.Key as string,
                                TextColor = Color.Black,
                                HorizontalOptions = LayoutOptions.CenterAndExpand
                            };
                            slProperty.Children.Add(btnProperty);
                            btnProperty.Clicked += btnClick;
                        }
                        else
                        {
                            if (prop.Key == "Last Updated By" || prop.Key == "Last Update")
                            {
                                continue;
                            }
                        
                            Label lblProperty = new Label();
                            Entry txtProperty = new Entry();
                            lblProperty.Text = prop.Key as string + ":";
                            lblProperty.TextColor = Color.Black;
                            lblProperty.VerticalTextAlignment = TextAlignment.Center;
                            lblProperty.HorizontalTextAlignment = TextAlignment.Start;
                            lblProperty.FontSize = 16;
                            lblProperty.Margin = 0;
                            lblProperty.WidthRequest = lblProperty.Text.Count() * lblProperty.FontSize * 0.66;


                            txtProperty.Text = prop.Value as string;
                            txtProperty.TextColor = Color.Black;
                            txtProperty.AutomationId = prop.Key as string;
                            txtProperty.MinimumWidthRequest = 0;
                            txtProperty.HeightRequest = txtProperty.Text.Count() * txtProperty.FontSize * 1.2;
                            txtProperty.WidthRequest = (txtProperty.WidthRequest > App.Current.MainPage.Width - lblProperty.WidthRequest) ?
                                App.Current.MainPage.Width - lblProperty.WidthRequest : txtProperty.Text.Count() * lblProperty.FontSize * 0.72;

                            slProperty.Children.Add(lblProperty);
                            slProperty.Children.Add(txtProperty);
                            txtProperty.Unfocused += lostFocus;
                        }
                        vcProperty.View = slProperty as View;
                        if (tsPropertires.Count == 13)
                        {
                           break;
                        }
                        else
                        {
                            tsPropertires.Add(vcProperty);
                        }
                    }
                    else
                    {
                        continue;
                    }

                    
                }
                Button btnSave = new Button()
                {
                    Text = "Save",
                    TextColor = Color.Black,
                    WidthRequest = App.Current.MainPage.Width,
                };

                ViewCell vcSave = new ViewCell()
                {
                    View = btnSave
                };
                tsPropertires.Add(vcSave);
                btnSave.Clicked += btnSave_Clicked;
                TableRoot root = tvProperties.Root;
                root.Add(tsPropertires);
                tvProperties.Root = root;
                return true;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.StackTrace);
                return false;
            }

        }

        private void btnClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string strBtnName = btn.Text;

            //Each of these takes the user to a list of the respective type. This list is on its own page, which gets kind of messy...
            if (strBtnName == "Performers")
            {
                LoadPage(new pgEditPerformers());
            }
            else if (strBtnName == "Directions")
            {
                LoadPage(new pgEditDirections());
            }
            else if (strBtnName == "Foods")
            {
                LoadPage(new pgEditFoods());
            }
            else if (strBtnName == "Statics")
            {
                LoadPage(new pgEditStatics());
            }
        }

        bool bolLoaded = false;

        private void ScrollView_LayoutChanged(object sender, EventArgs e)
        {
            if (!bolLoaded)
                bolLoaded = LoadBoxes(sender as ScrollView);
        }
        
        private void sendPatch()
        {
            try
            {
                User currentUser = InfoStore.CurrentUser;
                string target = "https://airshowapp-d193b.firebaseio.com/Airshows/" + InfoStore.getAirshowIndex(InfoStore.Selected) +"/.json?auth=" + currentUser.token;

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string rawResponse = client.UploadString(new Uri(target), "PATCH", JSONToPatch);
                JObject response = JObject.Parse(rawResponse);

                Console.Write("Response is: " + rawResponse);
                readyToSend = false;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
                    
            }

        }
        User current = InfoStore.CurrentUser;
        private void btnSave_Clicked(object sender, EventArgs e)
        {
            if(badEntries.Count > 0)
            {
                DisplayAlert("Save Error", "Ensure all entries are valid", "OK");
            }
            else if (JSONToPatch != null && JSONToPatch.Length > 0)
            {
                JSONToPatch = "{" + JSONToPatch.Substring(0, JSONToPatch.LastIndexOf(','));
                JSONToPatch += ",\" Last Updated By \":\"" + current.localId + "\"" + "}";
                sendPatch();
                JSONToPatch = "";
                readyToSend = false;
                InfoStore.getDatabase();
            }
        }

        private bool validate(Entry snt)
        {
            try
            {
                if (snt.AutomationId == "Date")
                {
                    string[] date = snt.Text.Trim().Split('/');
                    DateTime result = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]));
                    snt.Text = result.ToString("MM/dd/yyyy");
                    return true;
                }
                else if (snt.AutomationId.EndsWith("Link"))
                {
                    Regex urlRegex = new Regex("^(http://www.|https://www.|http://|https://)?[a-z0-9]+([-.]{1}[a-z0-9]+)*.[a-z]{2,5}(:[0-9]{1,5})?(/.*)?$");
                    return urlRegex.IsMatch(snt.Text.Trim()) | snt.Text.Trim() == "None";
                }
                else
                {
                    return (snt.Text != null && snt.Text.Trim().Length > 0);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.StackTrace);
                return false;
            }
        }

        private void lostFocus(object sender, EventArgs e)
        {

            Entry snt = (Entry)sender;

            Console.WriteLine("Unfocusing: " + snt.AutomationId);
            int selectedIndex = InfoStore.getAirshowIndex(InfoStore.Selected);

            
            if (validate(snt))
            {
                JSONToPatch += "\"" + snt.AutomationId + "\":" + "\"" + snt.Text.Trim() + "\",";
                if (badEntries.Contains(snt.AutomationId))
                {
                    badEntries.Remove(snt.AutomationId);
                }
                readyToSend = true;
            }
            else if(!readyToSend)
            {

            }
            else
            {
                if (!badEntries.Contains(snt.AutomationId))
                {
                    badEntries.Add(snt.AutomationId);
                }
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
                base.OnBackButtonPressed();
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
            try
            {
                this.OnBackButtonPressed();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }

        }
    }
}