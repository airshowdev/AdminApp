using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AirshowAddmin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class pgEditOrNotify : ContentPage
	{
		public pgEditOrNotify ()
		{
			InitializeComponent ();
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }
        }

        IList<View> children;

        private void StackLayout_LayoutChanged(object sender, EventArgs e)
        {
            StackLayout slNavigation = sender as StackLayout;
            children = slNavigation.Children;
        }

        private void btnEdit_Clicked(object sender, EventArgs e)
        {
            LoadPage(new ModificationPage());
        }

        private void btnNotify_Clicked(object sender, EventArgs e)
        {
            LoadPage(new pgDraftNotification());
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
                LoadPage(new pgSelectOrCreate());
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