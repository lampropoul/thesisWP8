using HFPMApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace HFPMApp
{
    public partial class Edit : PhoneApplicationPage
    {


        string uri;


        public Edit()
        {
            InitializeComponent();


        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // APP BAR
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Images/YourImage.png", UriKind.Relative);
            button1.Text = "Main Menu";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(main_menu_Click);

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                page_title.Text = "Επεξεργασία";


            }


            bool hasInternet = NetworkInterface.GetIsNetworkAvailable();

            // test
            //hasInternet = false;

            if (!hasInternet)
            {
                edit_user.Visibility = Visibility.Collapsed;
                edit_address.Visibility = Visibility.Collapsed;
                edit_phones.Visibility = Visibility.Collapsed;

                MessageBox.Show("Sorry, no internet!");

                uri = "/MainMenuPage.xaml";
                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                
            }
            
            

        } // OnNavigatedTo end




        private void edit_user_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Edit_User.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }



        private void edit_address_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Edit_Address.xaml";
            MessageBox.Show(sender.ToString());
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void edit_phones_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Edit_Phones.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        } 


        private void main_menu_Click(object sender, EventArgs e)
        {

            uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }
        

    } // class Edit : PhoneApplicationPage end


    
} // namespace end