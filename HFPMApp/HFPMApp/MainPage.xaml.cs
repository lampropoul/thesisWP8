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


namespace HFPMApp
{
    public partial class MainPage : PhoneApplicationPage
    {

        public string downloadedText;
        WebClient client;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            username.Focus();
            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;
        }

        

        // login
        private void login_btn_click1(object sender, RoutedEventArgs e)
        {

            SolidColorBrush Brush = new SolidColorBrush();
            Brush.Color = Colors.Blue;

            string uri;
            
            // edw upoti8etai oti 8a ginetai to erwtima stin topiki basi
            
            

            if (username.Text != String.Empty)
            {
                
                using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                {
                    db.CreateIfNotExists();
                    db.LogDebug = true;


                    string uname = username.Text;
                    string pass = password.Password;



                    // REST Call

                    string url = "http://192.168.42.236/HFPM_Server_CI/index.php/restful/api/user/username/" + uname;

                    client.DownloadStringAsync(new Uri(url));
                    //MessageBox.Show("JSON downloaded.");
                    //var client = new WebClient();
                    //string response = await client.DownloadStringTaskAsync(new Uri(url));


                    //try
                    //{
                    //    this.downloadedText = response;
                    //    MessageBox.Show(response);
                    //}
                    //catch (ArgumentNullException ex)
                    //{
                    //    MessageBox.Show("ArgumentNullException: " + ex.Message);
                    //    System.Diagnostics.Debug.WriteLine("ArgumentNullException: " + ex.Message);
                    //}
                    //catch (WebException ex)
                    //{
                    //    MessageBox.Show("WebException: " + ex.Message);
                    //    System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
                    //}

                    //var query = from Users users in db.Users
                    //            select users;

                    //ObservableCollection<Users> result = new ObservableCollection<Users>(query);

                    
                    // this is if(1)
                    if (username.Text == uname && password.Password == pass)
                    {

                        uri = "/MainMenuPage.xaml?username=" + uname + "&password=" + this.downloadedText;
                        NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

                    }
                    else
                    {
                        
                        ShellToast toast = new ShellToast();
                        toast.Title = "Warning";
                        toast.Content = "User not found on local database.";
                        toast.Show();
                    }

                }

            }
            else
            {
                uri = "/ErrorPage.xaml";
                //NavigationService.Navigate(new Uri(uri, UriKind.Relative));

                MessageBox.Show("Please specify username.");
                username.Focus();
            }

        }


        // register
        private void login_btn_click2(object sender, RoutedEventArgs e)
        {


            string uri = "/RegisterPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }


        private void ResponseCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asyncResult);

            MemoryStream tempStream = new MemoryStream();
            webResponse.GetResponseStream().CopyTo(tempStream);

            MessageBox.Show(tempStream.ToString());


        }



        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                this.downloadedText = e.Result;
                MessageBox.Show(this.downloadedText.ToString());
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

        
    }


}