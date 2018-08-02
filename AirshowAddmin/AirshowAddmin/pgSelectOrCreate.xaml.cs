using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AirshowAddmin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class pgSelectOrCreate : ContentPage
	{
        public pgSelectOrCreate()
        {
            InitializeComponent();
            InfoStore.Selected = "";
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            try
            {
                App.Current.MainPage =App.Current.MainPage;
                return true;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
                return false;
            }

        }
    
        private void StackLayout_LayoutChanged(object sender, EventArgs e)
        {
                Database database = InfoStore.database;
                TableView tvSelectAirshow;
                IList<View> children;
                TableRoot Airshows;

                IList<String> airshowNames;
            try
            {
                StackLayout slModify = sender as StackLayout;
                children = slModify.Children;
                if (children[0].GetType() == typeof(TableView))
                {
                    airshowNames = Database.AirshowNames;
                    tvSelectAirshow = children[0] as TableView;
                    tvSelectAirshow.Root.Clear();
                    Airshows = tvSelectAirshow.Root;
                    TableSection Airshow = new TableSection();
                    Airshow.Clear();

                    foreach (String name in airshowNames)
                    {
                        TextCell AirshowName = new TextCell();
                        AirshowName.Text = name;
                        AirshowName.TextColor = Color.Black;
                        Airshow.Add(AirshowName);
                        AirshowName.Tapped += AirshowClicked;
                    }
                    Airshows.Clear();
                    ViewCell vc = new ViewCell();

                    Button Create = new Button()
                    {
                        Text = "Create New Airshow"
                    };
                    Create.Clicked += CreateClicked;
                    vc.View = Create;

                    Airshow.Add(vc);
                    Airshows.Add(Airshow);
                    
                    tvSelectAirshow.Root = Airshows;

                }

            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }
        }

        private void CreateClicked(object sender, EventArgs e)
        {
            LoadPage(new pgCreate());
        }

        private void AirshowClicked(object sender, EventArgs e)
        {
            try
            {
                InfoStore.Selected = (sender as TextCell).Text;
                App.Current.MainPage = new pgEditOrNotify();
            }
            catch (Exception E)
            {
                E.ToString();
            }
            

        }

        private async void LoadPage(ContentPage page) 
        {
            
                NavigationPage navPage = new NavigationPage(page);
                page.Parent = null;
                await navPage.Navigation.PushAsync(page);
                App.Current.MainPage = page;
        }

        private void backButton_Pressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }
}