﻿using HFPMApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
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
using System.Windows.Media;
using System.Windows.Navigation;

namespace HFPMApp
{
    public partial class Edit_Phones : PhoneApplicationPage
    {
        
        public string downloadedText;
        WebClient client;
        WebClient client_up;
        string url;
        string url_post;
        string uri;
        string json_to_send = null;
        String server_ip;
        string given_username;


        public Edit_Phones()
        {
            InitializeComponent();

            try
            {
                using (StreamReader sr = new StreamReader("server_ip.txt"))
                {
                    server_ip = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("The file could not be read:");
                MessageBox.Show(e.Message);
            }


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



            // CLIENTS
            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;

            client_up = new WebClient();
            client_up.UploadStringCompleted += client_UploadStringCompleted;
        }







        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            loadingProgressBar.IsVisible = true;

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                page_title.Text = "Επεξεργασία";


                phone.Text = "Σταθερό";
                mobile.Text = "Κινητό";
                fax.Text = "Φαξ";

                send_phones.Content = "Αποθήκευση";


                

            }



            

            // pairnw ta parameters (username kai password) apo tin MainPage.xaml
            given_username = PhoneApplicationService.Current.State["Username"].ToString();

            
            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);

            url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/phones/username/" + given_username + "/randomnum/" + rand;
            client.DownloadStringAsync(new Uri(url));

        }

        private void send_phones_Click(object sender, RoutedEventArgs e)
        {

            // REST Call

            loadingProgressBar.IsVisible = true;

            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);


            url_post = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/phones/username/" + given_username + "/randomnum/" + rand;



            client_up.Headers["Method"] = "POST";
            client_up.Headers[HttpRequestHeader.ContentType] = "application/json";


            // encode JSON
            RootObject jsonObject = new RootObject();
            // put fields' values to json object
            jsonObject.phone = edit_phone.Text;
            jsonObject.mobile = edit_mobile.Text;
            jsonObject.fax = edit_fax.Text;
            
            this.json_to_send = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, new JsonSerializerSettings { });


            client_up.UploadStringAsync(new Uri(url_post), this.json_to_send);



        }



        // function that sends json data to web server
        void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {

                try
                {

                    this.downloadedText = e.Result;
                    RootObject jsonObject_res = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);

                    if (jsonObject_res.message == "Updated")
                    {
                        
                        uri = "/Edit.xaml";
                        NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Κάτι δεν πήγε καθόλου καλά. Προσπαθήστε ξανά.");
                        else MessageBox.Show("Something went terribly wrong. Please try again.");
                    }


                }
                catch (WebException ex)
                {
                    MessageBox.Show("WebException: " + ex.Message);
                }

            }
            catch (TargetInvocationException ex)
            {
                MessageBox.Show("TargetInvocationException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }

            loadingProgressBar.IsVisible = false;

        }



        // function that retreives json data from web server
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {

                this.downloadedText = e.Result;

                // decode JSON
                RootObject jsonObject = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);

                string phone = jsonObject.phone;
                string mobile = jsonObject.mobile;
                string fax = jsonObject.fax;


                // fill boxes with new (updated) data
                edit_phone.Text = phone;
                edit_mobile.Text = mobile;
                edit_fax.Text = fax;



            }
            catch (TargetInvocationException ex)
            {
                MessageBox.Show("TargetInvocationException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }

            loadingProgressBar.IsVisible = false;

        }




        private void main_menu_Click(object sender, EventArgs e)
        {

            uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }


        private void back_Click(object sender, EventArgs e)
        {

            uri = "/Edit.xaml";
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





        // the class which contains the properties of the specific JSON response
        public class RootObject
        {
            public string phone { get; set; }
            public string mobile { get; set; }
            public string fax { get; set; }
            public string error { get; set; }
            public string message { get; set; }
        }

        



    }
}