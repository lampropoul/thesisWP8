using HFPMApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Reflection;
using Newtonsoft.Json;


namespace HFPMApp
{
    public partial class MainPage : PhoneApplicationPage
    {

        public string downloadedText;
        WebClient client;
        string url;
        string uri;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //bool isNetwork = NetworkInterface.GetIsNetworkAvailable();

            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            //url = "http://192.168.42.236/HFPM_Server_CI/index.php/restful/api/user/username/" + uname;
            //var request = HttpWebRequest.Create(url) as HttpWebRequest;
            //request.Accept = "application/json;odata=verbose"; // Must pass the HttpWebRequest object in the state attached to this call // Begin the request…
            //request.BeginGetResponse(new AsyncCallback(GotResponse), request);
        }

        

        // login
        private void login_btn_click1(object sender, RoutedEventArgs e)
        {

            SolidColorBrush Brush = new SolidColorBrush();
            Brush.Color = Colors.Blue;

            
            
            // edw upoti8etai oti 8a ginetai to erwtima stin topiki basi
            string uname = username_box.Text;
            string pass = password_box.Password;
            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);


            if (uname != String.Empty && pass != String.Empty && !Convert.ToBoolean(gr.IsChecked))
            {
                
                using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                {
                    db.CreateIfNotExists();
                    db.LogDebug = true;

                    
                    // elegxw an exw internet

                    // ean exw proxwraw kai kanw to REST call
                    // TODO: check internet connectivity


                    // REST Call
                    url = "http://192.168.42.236/HFPM_Server_CI/index.php/restful/api/user/username/" + uname + "/password/" + pass + "/randomnum/" + rand;
                    client.DownloadStringAsync(new Uri(url));
                    




                    // ean oxi, kalw thn topiki bash
                    // CALL TO LOCAL DB

                    // TODO: call local db


                }

            }
            else if (Convert.ToBoolean(gr.IsChecked))
            {
                // do something in GREEK
            }
            else if (uname == String.Empty && pass != String.Empty)
            {
                MessageBox.Show("Please specify username.");
                username_box.Focus();
            }
            else if (pass == String.Empty && uname != String.Empty)
            {
                MessageBox.Show("Please specify password.");
                password_box.Focus();
            }
            else
            {
                MessageBox.Show("Please fill in all the fields.");
                username_box.Focus();
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

                try
                {

                    if (password == password_box.Password)
                    {
                        uri = "/MainMenuPage.xaml?username=" + username;
                        //MessageBox.Show("User " + username + " (" + email + ") logged in successfully.");
                        this.downloadedText = null;
                        NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        MessageBox.Show("Wrong password. Please try again.");
                        password_box.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //username_box.Focus();
                }

            }
            catch (TargetInvocationException ex)
            {
                MessageBox.Show("No such user. Please try again.");
                username_box.Focus();
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }
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
    }


}