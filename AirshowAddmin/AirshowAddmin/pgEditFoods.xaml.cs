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
    public partial class pgEditFoods : ContentPage
    {
        private string strSelectedFood = "";
        Button btnBack;
        public pgEditFoods()
        {
            InitializeComponent();
            btnBack = (Button)myStackLayout.Children[2];
            foreach (string name in InfoStore.getFoodNames())
            {
                pckFoods.Items.Add(name);
            }
            pckFoods.Items.Add("Add New Food");
            if (Device.RuntimePlatform == Device.Android)
            {
                backButton.IsVisible = false;
            }
        }

        private void storeSelected(object sender, EventArgs e)
        {

            //Removes the AddOrEditFoodView from the stackLayout if it is there,
            //if not, the Back button is moved to the end after adding the view

            if (myStackLayout.Children.Count > 3)
            {
                myStackLayout.Children.RemoveAt(2);
            }
            strSelectedFood = pckFoods.SelectedItem.ToString();
            myStackLayout.Children.Add(new AddOrEditFoodView(strSelectedFood, pckFoods.SelectedIndex));

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
        
        //Handles back button being pressed
        private void backButton_Pressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }
}