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
	public partial class pgAddStatic : ContentPage
	{
		public pgAddStatic ()
		{
			InitializeComponent ();
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
            else if (!Helper.ValidateImageFromEntry(txtImage))
            {
                DisplayAlert("Image Error", "Please enter correctly formatted image URL", "OK");
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
                newShow.Statics = newShow.Statics ?? new List<Static>();
                newShow.Statics.Add(new Static(txtName.Text.Trim(), txtDesc.Text.Trim(), txtImage.Text.Trim()));
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
                LoadPage( new pgCreate());
                base.OnBackButtonPressed();
                return true;
            }
            catch (Exception E)
            {
                E.ToString();
                return false;
            }
        }

        private async void LoadPage(ContentPage page)
        {
            NavigationPage navPage = new NavigationPage(page);
            page.Parent = null;
            await navPage.Navigation.PushAsync(page);
            App.Current.MainPage = page;
        }
    }


}