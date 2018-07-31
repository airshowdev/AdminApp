using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AirshowAddmin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class pgAddDirections : ContentPage
	{
		public pgAddDirections ()
		{
			InitializeComponent ();
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }
            pckFull.ItemsSource = new List<String>() { "True", "False" };
            pckType.ItemsSource = new List<String>() { "Drive", "Park" };
        }

        private void savePressed(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
                Airshow newAirshow = Database.Airshows[Database.Airshows.Count - 1];
                newAirshow.Directions = new List<Direction>();
                newAirshow.Directions.Add(new Direction(Convert.ToBoolean(pckFull.SelectedItem.ToString()), txtName.Text.Trim(),pckType.SelectedItem.ToString(), Convert.ToDouble(txtXCoord.Text.Trim()), Convert.ToDouble(txtYCoord.Text.Trim())));

                OnBackButtonPressed();
            }
        }

        private bool ValidateAll()
        {
            if (!Helper.ValidateEntry(txtName))
            {
                DisplayAlert("Name Error", "Please enter a name between 1-60 characters long", "OK");
                return false;
            }
            else if (!Helper.validateCoordinateFromEntry(txtXCoord))
            {
                DisplayAlert("X-Coordinate Error", "Please enter a valid X-Coordinate", "OK");
                return false;
            }
            else if (!Helper.validateCoordinateFromEntry(txtYCoord))
            {
                DisplayAlert("Y-Coordinate Error", "Please enter a valid Y-Coordinate", "OK");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void backButton_Pressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        protected override bool OnBackButtonPressed()
        {
            try
            {
                App.Current.MainPage = new pgCreate();
                base.OnBackButtonPressed();
                return true;
            }
            catch (Exception E)
            {
                E.ToString();
                return false;
            }
        }
    }
}