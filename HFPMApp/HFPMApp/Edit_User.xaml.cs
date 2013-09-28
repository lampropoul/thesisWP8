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
    public partial class Edit_User : PhoneApplicationPage
    {
       
        public string downloadedText;
        WebClient client;
        WebClient client_up;
        string url;
        string url_post;
        string uri;
        string json_to_send = null;
        String server_ip;


        public Edit_User()
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


                new_pass.Text = "Νέος κωδικός";
                new_pass2.Text = "Νέος κωδικός (ξανά)";
                new_name.Text = "Όνομα";
                new_surname.Text = "Επώνυμο";
                new_email.Text = "E-mail";
                new_amka.Text = "ΑΜΚΑ";
                new_userteam.Text = "Ομάδα χρήστη";
                new_status.Text = "Κατάσταση";
                new_department.Text = "Τμήμα";

                edit_completed_button.Content = "Αποθήκευση";

                

            }


            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") edit_username.Text = "Λεπτομέρειες του χρήστη " + PhoneApplicationService.Current.State["Username"] + ":";
            else edit_username.Text = PhoneApplicationService.Current.State["Username"] + "'s account details:";


            //CycleTileData tileData = new CycleTileData()
            //{
            //    Title = "dsdas",
            //    SmallBackgroundImage = new Uri("FlipCycleTileSmall.png",
            //      UriKind.RelativeOrAbsolute),
            //    CycleImages = { }
            //};

            //ShellTile.Create(new Uri("/Settings.xaml", UriKind.Relative), tileData, true);


            

            // pairnw ta parameters (username kai password) apo tin MainPage.xaml
            string given_username = PhoneApplicationService.Current.State["Username"].ToString();

            edit_username.Text = given_username;

            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);

            url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/user/username/" + given_username + "/randomnum/" + rand;
            client.DownloadStringAsync(new Uri(url));

        }

        private void edit_button_pressed(object sender, RoutedEventArgs e)
        {

            // REST Call

            string email_encoded = edit_email.Text.Replace("@", "%40");

            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);

            url_post = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/user" + "/username/" + edit_username.Text + "/randomnum/" + rand;

            client_up.Headers["Method"] = "POST";
            client_up.Headers[HttpRequestHeader.ContentType] = "application/json";
            
            
            // encode JSON
            RootObject jsonObject = new RootObject();
            // put fields' values to json object
            jsonObject.user_team = Int32.Parse(edit_userteam.Text);
            jsonObject.name_user = edit_name.Text;
            jsonObject.surname_user = edit_surname.Text;
            jsonObject.username = edit_username.Text;
            jsonObject.password = edit_password.Password;
            jsonObject.email = edit_email.Text;
            jsonObject.amka = edit_amka.Text;
            jsonObject.status = edit_status.Text;
            jsonObject.department = edit_department.Text;

            this.json_to_send = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, new JsonSerializerSettings { });

            

            // if passwords match
            if (edit_password.Password == edit_password2.Password && edit_password.Password != String.Empty)
            {
                loadingProgressBar.IsVisible = true;
                client_up.UploadStringAsync(new Uri(url_post), this.json_to_send);
            }
            else
            {
                if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Οι κωδικοί δεν ταιριάζουν ή δεν έχετε εισάγει κωδικό. Προσπαθήστε ξανά.");
                else MessageBox.Show("Passwords don't match or you have specified no password at all. Try again.");
            }



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

                        using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                        {

                            // -------------------------------------------------------------------//
                            // -------------------------- LOCAL DATABASE -------------------------//
                            // -------------------------------------------------------------------//

                            db.CreateIfNotExists();
                            db.LogDebug = true;


                            // retreive user id from table users

                            IEnumerable<Users> query =
                                        from user in db.Users
                                        where user.Username == PhoneApplicationService.Current.State["Username"].ToString()
                                        select user;

                            // delete
                            foreach (Users us in query)
                            {
                                db.Users.DeleteOnSubmit(us);
                            }



                            db.Users.InsertOnSubmit(new Users
                            {
                                Userid = jsonObject_res.id,
                                Username = jsonObject_res.username,
                                Password = jsonObject_res.password,
                                Nameuser = jsonObject_res.name_user,
                                Surnameuser = jsonObject_res.surname_user,
                                Amka = jsonObject_res.amka,
                                Department = jsonObject_res.department,
                                Userteam = jsonObject_res.user_team
                            });



                            // changes do not take place until SubmitChanges method is called
                            try
                            {
                                db.SubmitChanges();
                                uri = "/MainMenuPage.xaml";
                                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }


                        }

                        
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

                int id = jsonObject.id;
                int user_team = jsonObject.user_team;
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
                edit_status.Text = status;
                edit_userteam.Text = user_team.ToString();



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
            public int id { get; set; }
            public int user_team { get; set; }
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
    }
}