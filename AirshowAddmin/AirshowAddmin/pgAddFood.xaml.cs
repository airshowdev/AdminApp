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
	public partial class pgAddFood : ContentPage
	{
		public pgAddFood ()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }
        }

        private bool validateAll()
        {
            if (!Helper.ValidateEntry(txtDesc))
            {
                DisplayAlert("Description Error", "Please enter a description between 1-60 characters long", "OK");
                return false;
            }
            else if (!Helper.ValidateEntry(txtName))
            {
                DisplayAlert("Name Error", "Please enter a name between 1-60 characters long", "OK");
                return false;
            }
            return true;
        }
        private void btnSave_Pressed(object sender, EventArgs e)
        {
            if (validateAll())
            {
                Airshow newShow = Database.Airshows[Database.Airshows.Count - 1];
                newShow.Foods = newShow.Foods ?? new List<Food>();
                newShow.Foods.Add(new Food(txtName.Text.Trim(), txtDesc.Text.Trim()));
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