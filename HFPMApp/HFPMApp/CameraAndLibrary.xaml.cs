using HFPMApp.Resources;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HFPMApp
{
    public partial class CameraAndLibrary : PhoneApplicationPage
    {

        string uri;


        public CameraAndLibrary()
        {
            InitializeComponent();

            // APP BAR
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Images/appbar.back.png", UriKind.Relative);
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") button2.Text = "πίσω";
            else button2.Text = "go back";
            ApplicationBar.Buttons.Add(button2);
            button2.Click += new EventHandler(back_Click);

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            //button1.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.Relative);
            button1.IconUri = new Uri("/Content/images/main_menu.png", UriKind.Relative);
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") button1.Text = "μενού";
            else button1.Text = "main menu";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(main_menu_Click);


            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem1.Text = "Έξοδος (" + PhoneApplicationService.Current.State["Username"] + ")";
            else menuItem1.Text = "Logout (" + PhoneApplicationService.Current.State["Username"] + ")";
            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(logout_Click);


            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                page_title.Text = "Φωτογραφίες";

                shoot_photo_button.Content = "Τραβήξτε\nΦωτογραφία";
                view_saved_button.Content = "Άνοιγμα\nΣυλλογής";
            }
            else
            {
                shoot_photo_button.Content = "Shoot\nPhoto";
                view_saved_button.Content = "View\nSaved\nPhotos";
            }


        }










        private void main_menu_Click(object sender, EventArgs e)
        {

            uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }


        private void back_Click(object sender, EventArgs e)
        {

            uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void logout_Click(object sender, EventArgs e)
        {

            PhoneApplicationService.Current.State["Username"] = null;


            using (StreamWriter writer = new StreamWriter("already_logged.txt"))
            {
                writer.Write(PhoneApplicationService.Current.State["Username"]);
                writer.Close();
            }

            uri = "/MainPage.xaml?logout=true";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }





        // ------ TAKE PHOTO ------ //
        private void shoot_photo_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Camera.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }



        // ------ MEDIA LIBRARY ------ //
        private void view_saved_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Library.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }



    }
}