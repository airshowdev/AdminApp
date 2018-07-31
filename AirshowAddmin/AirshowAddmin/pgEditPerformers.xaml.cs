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
            //Button myButton1 = new Button { Text = "Edit" };
            //Button myButton2 = new Button { Text = "Delete" };
            // myStackLayout.Children.Add(myButton1);
            // myStackLayout.Children.Add(myButton2);
        }

        Dictionary<string, object> Properties;
       

        /*********************************************************
         
         This stuff is all used to send the updates to firebase.
             
         *******************************************************/     
         
        List<Entry> entries = new List<Entry>();
        //This works because I created a list of entries^^. Everytime a new entry is made I add it to the list. Now we can Foreach them easily
        //I interate through the list and find all the entries that have the same autoID as the sender button
        //Then I create a string for every entry that matches and add the string to the JSONToPatch string
        //So the save buttons send a patch for every child of the specific performer that has the same index as their autoID... not best practice but it works
        private void sendPatch(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;

            string strIndex = btnSender.AutomationId;

            string JSONToPatch = "";

            foreach(Entry ent in entries)
            {
                if(ent.AutomationId.Split('.')[1] == strIndex)
                {
                    JSONToPatch +="\"" + ent.AutomationId.Split('.')[0] + "\":\"" + ent.Text.Trim() + "\",";
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


        private bool LoadPerformers(ScrollView sv)
        {
            try
            {
                return true;
               /* Properties = Database.getAirshowInfo(InfoStore.Selected);
                IReadOnlyList<Element> children = sv.Children;
                TableView tvProperties = children[0] as TableView;
                TableSection tsPropertires = new TableSection();

                sv.HeightRequest = App.Current.MainPage.Height;
                tvProperties.Margin = 5;

                ViewCell vcProperty = new ViewCell();
                StackLayout slProperty = new StackLayout();
                vcProperty.Height = slProperty.Height;
                slProperty.Orientation = StackOrientation.Horizontal;

                Button btnAdd = new Button();
                btnAdd.Text = "Add Performer";
                btnAdd.TextColor = Color.Black;
                slProperty.Children.Add(btnAdd);
                btnAdd.Clicked += btnAddClick;
                vcProperty.View = slProperty as View;
                tsPropertires.Add(vcProperty);

                foreach (KeyValuePair<string, object> prop in Properties)
                {
                    if(prop.Key == "Performers")
                    {
                        List<Performer> performers = prop.Value as List<Performer>;

                        for(int intCounter = 0; intCounter < performers.Count; intCounter++)
                        {
                            ViewCell vcPropertyDescription = new ViewCell();
                            StackLayout slPropertyDescription = new StackLayout();
                            vcPropertyDescription.Height = slPropertyDescription.Height;
                            slPropertyDescription.Orientation = StackOrientation.Horizontal;

                            Label lblDescription = new Label();
                            Entry txtDescription = new Entry();
                            
                            lblDescription.Text = "Description:";
                            lblDescription.TextColor = Color.Black;
                            lblDescription.VerticalTextAlignment = TextAlignment.Center;
                            lblDescription.HorizontalTextAlignment = TextAlignment.Start;
                            lblDescription.FontSize = 16;
                            lblDescription.Margin = 0;
                            lblDescription.WidthRequest = lblDescription.Text.Count() * lblDescription.FontSize * 0.66;

                            txtDescription.Text = performers[intCounter].Description as string;
                            txtDescription.TextColor = Color.Black;
                            txtDescription.AutomationId = "Description." + intCounter.ToString();
                            txtDescription.MinimumWidthRequest = 0;
                            txtDescription.HeightRequest = txtDescription.Text.Count() * txtDescription.FontSize * 1.2;
                            txtDescription.WidthRequest = (txtDescription.WidthRequest > App.Current.MainPage.Width - lblDescription.WidthRequest) ?
                                App.Current.MainPage.Width - lblDescription.WidthRequest : txtDescription.Text.Count() * lblDescription.FontSize * 0.72;

                            entries.Add(txtDescription);

                            slPropertyDescription.Children.Add(lblDescription);
                            slPropertyDescription.Children.Add(txtDescription);

                            vcPropertyDescription.View = slPropertyDescription as View;
                            tsPropertires.Add(vcPropertyDescription);
                
                            ViewCell vcPropertyImage = new ViewCell();
                            StackLayout slPropertyImage = new StackLayout();
                            vcPropertyImage.Height = slPropertyImage.Height;
                            slPropertyImage.Orientation = StackOrientation.Horizontal;

                            Label lblImage = new Label();
                            Entry txtImage = new Entry();

                            lblImage.Text = "Image:";
                            lblImage.TextColor = Color.Black;
                            lblImage.VerticalTextAlignment = TextAlignment.Center;
                            lblImage.HorizontalTextAlignment = TextAlignment.Start;
                            lblImage.FontSize = 16;
                            lblImage.Margin = 0;
                            lblImage.WidthRequest = lblImage.Text.Count() * lblImage.FontSize * 0.66;

                            txtImage.Text = performers[intCounter].Image as string;
                            txtImage.TextColor = Color.Black;
                            txtImage.AutomationId = "Image." + intCounter.ToString();
                            txtImage.MinimumWidthRequest = 0;
                            txtImage.HeightRequest = txtImage.Text.Count() * txtImage.FontSize * 1.2;
                            txtImage.WidthRequest = (txtImage.WidthRequest > App.Current.MainPage.Width - lblImage.WidthRequest) ?
                                App.Current.MainPage.Width - lblImage.WidthRequest : txtImage.Text.Count() * lblImage.FontSize * 0.72;

                            entries.Add(txtImage);

                            slPropertyImage.Children.Add(lblImage);
                            slPropertyImage.Children.Add(txtImage);
                            vcPropertyImage.View = slPropertyImage as View;
                            tsPropertires.Add(vcPropertyImage);
                

                            ViewCell vcPropertyInAir = new ViewCell();
                            StackLayout slPropertyInAir = new StackLayout();
                            vcPropertyInAir.Height = slPropertyInAir.Height;
                            slPropertyInAir.Orientation = StackOrientation.Horizontal;

                            Label lblInAir = new Label();
                            Entry txtInAir = new Entry();

                            lblInAir.Text = "In Air:";
                            lblInAir.TextColor = Color.Black;
                            lblInAir.VerticalTextAlignment = TextAlignment.Center;
                            lblInAir.HorizontalTextAlignment = TextAlignment.Start;
                            lblInAir.FontSize = 16;
                            lblInAir.Margin = 0;
                            lblInAir.WidthRequest = lblInAir.Text.Count() * lblInAir.FontSize * 0.66;

                            txtInAir.Text = performers[intCounter].InAir as string;
                            txtInAir.TextColor = Color.Black;
                            txtInAir.AutomationId = "In Air." + intCounter.ToString();
                            txtInAir.MinimumWidthRequest = 0;
                            txtInAir.HeightRequest = txtInAir.Text.Count() * txtInAir.FontSize * 1.2;
                            txtInAir.WidthRequest = (txtInAir.WidthRequest > App.Current.MainPage.Width - lblInAir.WidthRequest) ?
                                App.Current.MainPage.Width - lblInAir.WidthRequest : txtInAir.Text.Count() * lblInAir.FontSize * 0.72;

                            entries.Add(txtInAir);

                            slPropertyInAir.Children.Add(lblInAir);
                            slPropertyInAir.Children.Add(txtInAir);
                            vcPropertyInAir.View = slPropertyInAir as View;
                            tsPropertires.Add(vcPropertyInAir);
                
                            ViewCell vcPropertyName = new ViewCell();
                            StackLayout slPropertyName = new StackLayout();
                            vcPropertyName.Height = slPropertyName.Height;
                            slPropertyName.Orientation = StackOrientation.Horizontal;

                            Label lblName = new Label();
                            Entry txtName = new Entry();

                            lblName.Text = "Name:";
                            lblName.TextColor = Color.Black;
                            lblName.VerticalTextAlignment = TextAlignment.Center;
                            lblName.HorizontalTextAlignment = TextAlignment.Start;
                            lblName.FontSize = 16;
                            lblName.Margin = 0;
                            lblName.WidthRequest = lblName.Text.Count() * lblName.FontSize * 0.66;

                            txtName.Text = performers[intCounter].Name as string;
                            txtName.TextColor = Color.Black;
                            txtName.AutomationId = "Name." + intCounter.ToString();
                            txtName.MinimumWidthRequest = 0;
                            txtName.HeightRequest = txtName.Text.Count() * txtName.FontSize * 1.2;
                            txtName.WidthRequest = (txtName.WidthRequest > App.Current.MainPage.Width - lblName.WidthRequest) ?
                                App.Current.MainPage.Width - lblName.WidthRequest : txtName.Text.Count() * lblName.FontSize * 0.72;

                            entries.Add(txtName); 

                            slPropertyName.Children.Add(lblName);
                            slPropertyName.Children.Add(txtName);

                            Button btnSavePerformer = new Button();
                            btnSavePerformer.Text = "Save";
                            btnSavePerformer.AutomationId = intCounter.ToString(); 
                            btnSavePerformer.Clicked += sendPatch;

                            slPropertyName.Children.Add(btnSavePerformer);

                            vcPropertyName.View = slPropertyName as View;
                            tsPropertires.Add(vcPropertyName);
                        }              
                    }
                }
                
                TableRoot root = tvProperties.Root;
                root.Add(tsPropertires);
                tvProperties.Root = root;
                Console.WriteLine(tvProperties.ToString());
                return true;*/
            }
            catch (Exception E)
            {
                Console.Write(E.ToString());
                return false;
            }
        }

        bool bolLoaded = false;

        private void ScrollView_LayoutChanged(object sender, EventArgs e)
        {
            if (!bolLoaded)
                bolLoaded = LoadPerformers(sender as ScrollView);
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