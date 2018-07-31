using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using Newtonsoft.Json.Linq;

namespace AirshowAddmin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class pgEditDirections : ContentPage
	{
        private string strSelectedDirection;
        Button btnBack;
		public pgEditDirections ()
		{
			InitializeComponent ();
            btnBack = (Button)myStackLayout.Children[2];
            pckDirections.Items.Clear();
            foreach(string name in InfoStore.getDirectionNames())
            {
                pckDirections.Items.Add(name);
            }
            pckDirections.Items.Add("Add new directions");
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }
        }

        private void storeSelected(object sender, EventArgs e)
        {
            if (myStackLayout.Children.Count > 3)
            {
                myStackLayout.Children.RemoveAt(2);
            }
            strSelectedDirection = pckDirections.SelectedItem.ToString();
            myStackLayout.Children.Add(new AddOrEditDirectionsView(strSelectedDirection, pckDirections.SelectedIndex));
          
            if (!myStackLayout.Children.Contains(btnBack))
            { myStackLayout.Children.Add(btnBack); }
            else
            {
                myStackLayout.Children.Remove(btnBack);
                myStackLayout.Children.Add(btnBack);
            }
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
                App.Current.MainPage.DisplayAlert("Error", "A fatal error has occured.  Details: " + E.InnerException.ToString(), "OK");
                App.Current.MainPage = new pgSelectOrCreate();
                return false;
            }

        }

        private void backButton_Pressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }
}