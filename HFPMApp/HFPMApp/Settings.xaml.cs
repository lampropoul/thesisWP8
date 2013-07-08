using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace HFPMApp
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (PhoneApplicationService.Current.State["Language"] == "GR")
            {
                title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                settings.Text = "Ρυθμίσεις";

                list_all.Content = "Όλα";
                list_call.Content = "Εφημερία";
                list_shift.Content = "Βάρδια";
                list_everyday.Content = "Καθημερινά Ιατρεία";

                list_swtiria.Content = "ΣΩΤΗΡΙΑ";
                list_ges.Content = "ΓΕΣ";

                submit_button.Content = "Ολοκλήρωση";
                
            }


            // APP BAR
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            //ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            //button1.IconUri = new Uri("/Images/YourImage.png", UriKind.Relative);
            //button1.Text = "button 1";
            //ApplicationBar.Buttons.Add(button1);

            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();

            if (PhoneApplicationService.Current.State["Language"] == "GR")
                menuItem1.Text = "Έξοδος";
            else
                menuItem1.Text = "Logout";

            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(logout_Click);

            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem();

            if (PhoneApplicationService.Current.State["Language"] == "GR")
                menuItem2.Text = "Εκκαθάριση περασμένων καθηκόντων";
            else
                menuItem2.Text = "Clear old entries";

            ApplicationBar.MenuItems.Add(menuItem2);

        }




        private void selectionSubmitted(object sender, RoutedEventArgs e)
        {
            //TODO: POST
            MessageBox.Show("OK.");
            string uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }



        private void logout_Click(object sender, EventArgs e)
        {

            PhoneApplicationService.Current.State["Username"] = null;
            string uri = "/MainPage.xaml?logout=true";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }
        

    }
}