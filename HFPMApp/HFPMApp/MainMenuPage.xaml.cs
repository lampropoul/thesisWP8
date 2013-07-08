using HFPMApp.Resources;
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

    


    public partial class MainMenuPage : PhoneApplicationPage
    {

        public string downloadedText;
        WebClient client;
        WebClient client_up;
        string url;
        string url_post;
        string uri;
        string json_to_send = "";


        public MainMenuPage()
        {
            InitializeComponent();


            

            // CLIENTS
            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;

            client_up = new WebClient();
            client_up.UploadStringCompleted += client_UploadStringCompleted;

        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (PhoneApplicationService.Current.State["Language"] == "GR")
            {
                title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                welcome.Text = "Μενού";
                
                
                pers_prog_pitem.Header = "Πρόγραμμα";
                search_pitem.Header = "Αναζήτηση";
                declared_pitem.Header = "Αιτήματα";
                edit_pitem.Header = "Επεξεργασία";


                new_pass.Text = "Νέος κωδικός";
                new_pass2.Text = "Νέος κωδικός (ξανά)";
                new_name.Text = "Όνομα";
                new_surname.Text = "Επώνυμο";
                new_email.Text = "E-mail";
                new_amka.Text = "ΑΜΚΑ";
                new_userteam.Text = "Ομάδα χρήστη";
                new_status.Text = "Κατάσταση";
                new_department.Text = "Τμήμα";
                edit_completed_button.Content = "Ολοκλήρωση";


            }


            if (PhoneApplicationService.Current.State["Language"] == "GR")
                edit_username.Text = "Λεπτομέρειες του χρήστη " + PhoneApplicationService.Current.State["Username"] + ":";
            else
                edit_username.Text = PhoneApplicationService.Current.State["Username"] + "'s account details:";

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

            ApplicationBarMenuItem menuItem3 = new ApplicationBarMenuItem();

            if (PhoneApplicationService.Current.State["Language"] == "GR")
                menuItem3.Text = "Ρυθμίσεις για τα αιτήματα.";
            else
                menuItem3.Text = "Settings for declared";

            ApplicationBar.MenuItems.Add(menuItem3);
            menuItem3.Click += new EventHandler(settings_Click);

            


            // pairnw ta parameters (username kai password) apo tin MainPage.xaml
            string given_username = PhoneApplicationService.Current.State["Username"].ToString();

            edit_username.Text = given_username;

            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);

            url = "http://192.168.42.236/HFPM_Server_CI/index.php/restful/api/user/username/" + given_username + "/randomnum/" + rand;
            client.DownloadStringAsync(new Uri(url));

        }


        private void edit_button_pressed(object sender, RoutedEventArgs e)
        {

            // REST Call

            string email_encoded = edit_email.Text.Replace("@", "%40");

            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);

            url_post = "http://192.168.42.236/HFPM_Server_CI/index.php/restful/api/user" +
                "/username/" + edit_username.Text +
                "/password/" + edit_password.Password +
                "/amka/" + edit_amka.Text +
                "/email/" + email_encoded + 
                "/userteam/" + edit_userteam.Text + 
                "/name/" + edit_name.Text + 
                "/surname/" + edit_surname.Text + 
                "/status/" + edit_status.Text +
                "/department/" + edit_department.Text +
                "/randomnum/" + rand;
            
            client_up.Headers["Method"] = "POST";
            
            // if passwords match
            if (edit_password.Password == edit_password2.Password && edit_password.Password != String.Empty)
            {
                client_up.UploadStringAsync(new Uri(url_post), this.json_to_send);
            }
            else
            {
                if (PhoneApplicationService.Current.State["Language"] == "GR")
                    MessageBox.Show("Οι κωδικοί δεν ταιριάζουν ή δεν έχετε εισάγει κωδικό. Προσπαθήστε ξανά.");
                else
                    MessageBox.Show("Passwords don't match or you have specified no password at all. Try again.");
            }



        }


        // function that sends json data to web server
        void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                
                

                try
                {

                    //// encode JSON
                    //RootObject jsonObject = new RootObject();

                    //// put fields' values to json object
                    //jsonObject.user_team = edit_userteam.Text;
                    //jsonObject.name_user = edit_name.Text;
                    //jsonObject.surname_user = edit_surname.Text;
                    //jsonObject.username = edit_username.Text;
                    //jsonObject.password = edit_password.Password;
                    //jsonObject.email = edit_email.Text;
                    //jsonObject.amka = edit_amka.Text;
                    //jsonObject.status = edit_status.Text;
                    //jsonObject.department = edit_department.Text;

                    //this.json_to_send = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, new JsonSerializerSettings { });

                    this.downloadedText = e.Result;
                    RootObject jsonObject = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);

                    if (jsonObject.message == "Updated") MessageBox.Show("Edit OK.");
                    else
                    {
                        if (PhoneApplicationService.Current.State["Language"] == "GR")
                            MessageBox.Show("Κάτι δεν πήγε καθόλου καλά. Προσπαθήστε ξανά.");
                        else
                            MessageBox.Show("Something went terribly wrong. Please try again.");
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
        }



        // function that retreives json data from web server
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                
               this.downloadedText = e.Result;

                // decode JSON
                RootObject jsonObject = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);

                int id = jsonObject.id;
                string user_team = jsonObject.user_team;
                string name_user = jsonObject.name_user;
                string surname_user = jsonObject.surname_user;
                string username = jsonObject.username;
                string password = jsonObject.password;
                string email = jsonObject.email;
                string amka = jsonObject.amka;
                string status = jsonObject.status;
                string department = jsonObject.department;


                

                // fill boxes with new (updated) data
                edit_amka.Text = amka;
                edit_department.Text = department;
                edit_email.Text = email;
                edit_name.Text = name_user;
                edit_surname.Text = surname_user;
                //edit_password.Password = "new pass";
                //edit_password2.Password = "new pass again";
                edit_status.Text = status;
                edit_userteam.Text = user_team;


                
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
        }

        // the class which contains the properties of the specific json response
        public class RootObject
        {
            public int id { get; set; }
            public string user_team { get; set; }
            public string name_user { get; set; }
            public string surname_user { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public string amka { get; set; }
            public string status { get; set; }
            public string department { get; set; }
            public string error { get; set; }
            public string message { get; set; }
        }

        
        
        
        
        
        
        private void logout_Click(object sender, EventArgs e)
        {

            PhoneApplicationService.Current.State["Username"] = null;
            uri = "/MainPage.xaml?logout=true";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }


        private void settings_Click(object sender, EventArgs e)
        {
            uri = "/Settings.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        
    }
}