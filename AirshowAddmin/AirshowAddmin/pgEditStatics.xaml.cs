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
	public partial class pgEditStatics : ContentPage
	{
        private string strSelectedStatic = "";
        Button btnBack;
        public pgEditStatics()
        {
            InitializeComponent();
            btnBack = (Button)myStackLayout.Children[2];
            pckStatics.Items.Clear();
            foreach (string name in InfoStore.getStaticNames())
            {
                pckStatics.Items.Add(name);
            }
            pckStatics.Items.Add("Add New Statics");
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
            strSelectedStatic = pckStatics.SelectedItem.ToString();
            myStackLayout.Children.Add(new addOrEditStatics(strSelectedStatic, pckStatics.SelectedIndex));

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
                return false;
            }
        }

        private void backButton_Pressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }
}