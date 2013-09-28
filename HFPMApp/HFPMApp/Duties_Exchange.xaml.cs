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
    public partial class Duties_Exchange : PhoneApplicationPage
    {


        public string downloadedText;
        WebClient client;
        WebClient client2;
        string url;
        string uri;
        RootObject jsonObject;
        RootObject2 jsonObject2;
        string program_id_from = string.Empty;
        int program_id_to = 0;



        String server_ip;
        public Duties_Exchange()
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

            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem2.Text = "Εκκαθάριση  καθηκόντων";
            else menuItem2.Text = "Clear old entries";
            ApplicationBar.MenuItems.Add(menuItem2);

            ApplicationBarMenuItem menuItem3 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem3.Text = "Ρυθμίσεις για τα αιτήματα.";
            else menuItem3.Text = "Settings for declared";
            ApplicationBar.MenuItems.Add(menuItem3);
            menuItem3.Click += new EventHandler(settings_Click);



            try
            {
                using (StreamReader sr = new StreamReader("server_ip.txt"))
                {
                    server_ip = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("The file could not be read:");
                MessageBox.Show(e.Message);
            }




            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                page_title.Text = "Αιτήματα";
            }



            // CLIENTS
            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;

            client2 = new WebClient();
            client2.DownloadStringCompleted += client_DownloadStringCompleted2;

        }









        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            loadingProgressBar.IsVisible = true;


            NavigationContext.QueryString.TryGetValue("duty_id", out program_id_from);



            Random rnd = new Random();
            int rand = rnd.Next(1, 10000);
            string user_id = PhoneApplicationService.Current.State["UserId"].ToString();

            // edw kalw kai pairw apo ton server ola ta tmimata kai olous tous typous ka8ikontwn

            // REST Call
            url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/populatedutiesavail/dutyid/" + program_id_from + "/randomnum/" + rand;
            client.DownloadStringAsync(new Uri(url));



        }







        // function that retreives json data from web server
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {



                this.downloadedText = e.Result;

                // decode JSON
                jsonObject = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);

                int duties_count = jsonObject.duties.Count;


                List<string> StringsList = new List<string> { };

                if (jsonObject.duties[0].program_id != 0)
                {

                    for (int i = 0; i < duties_count; i++)
                    {

                        program_id_to = jsonObject.duties[i].program_id;
                        string type = jsonObject.duties[i].type;
                        string date = jsonObject.duties[i].date;
                        string start = jsonObject.duties[i].start;
                        string end = jsonObject.duties[i].end;

                        string item = String.Empty;

                        if (Convert.ToString(PhoneApplicationService.Current.State["Language"]) == "GR")
                        {
                            item = "Ανταλλάξτε με το καθήκον σας:\n    (" + type + ")\n    την ημέρα " + date + "\n    με ώρα έναρξης " + start + "\n    και ώρα λήξης " + end + "\n    (" + program_id_from + "->" + program_id_to + ")";
                        }
                        else
                        {
                            item = "Exchange with your duty:\n    (" + type + ")\n    on date " + date + "\n    with start time " + start + "\n    and end time " + end + "\n    (" + program_id_from + "->" + program_id_to + ")";
                        }
                        
                        StringsList.Add(item);


                    }// for


                    duties_avail.ItemsSource = StringsList;

                }
                else
                {
                    if (Convert.ToString(PhoneApplicationService.Current.State["Language"]) == "GR") MessageBox.Show("Δεν υπάρχουν δικά σας καθήκοντα προς ανταλλαγή.");
                    else MessageBox.Show("No duties to exchange.");
                }




            }
            catch (TargetInvocationException ex)
            {

                if (Convert.ToString(PhoneApplicationService.Current.State["Language"]) == "GR") MessageBox.Show("Ανεπιτυχής προσπάθεια σύνδεσης στον εξυπηρέτη ή δεν υπάρχουν αιτήματα από άλλους χρήστες.");
                else MessageBox.Show("Could not connect to server or no declared duties from other users.");
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }


            loadingProgressBar.IsVisible = false;

        }








        private void duties_avail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            Random rnd = new Random();
            int rand = rnd.Next(1, 10000);
            string user_id = PhoneApplicationService.Current.State["UserId"].ToString();

            // edw kalw kai pairw apo ton server ola ta tmimata kai olous tous typous ka8ikontwn

            // REST Call
            url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/exchangeduties/from/" + program_id_from + "/to/" + program_id_to + "/randomnum/" + rand;
            client2.DownloadStringAsync(new Uri(url));

        }






        // function that retreives json data from web server
        void client_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {


                this.downloadedText = e.Result;

                // decode JSON
                jsonObject2 = JsonConvert.DeserializeObject<RootObject2>(this.downloadedText);

                if (jsonObject2.error == "none")
                {
                    MessageBox.Show("OK");
                    uri = "/Declared.xaml";
                    NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                }

            }
            catch (TargetInvocationException ex)
            {

                if (Convert.ToString(PhoneApplicationService.Current.State["Language"]) == "GR") MessageBox.Show("Ανεπιτυχής προσπάθεια σύνδεσης στον εξυπηρέτη ή δεν υπάρχουν αιτήματα από άλλους χρήστες.");
                else MessageBox.Show("Could not connect to server or no declared duties from other users.");
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

            uri = "/Declared.xaml";
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


        private void settings_Click(object sender, EventArgs e)
        {
            uri = "/Settings.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }




        // the classes which contain the properties of the specific JSON response
        public class RootObject
        {
            public List<Duties> duties { get; set; }
        }



        public class Duties
        {
            public int program_id { get; set; }
            public string type { get; set; }
            public string date { get; set; }
            public string start { get; set; }
            public string end { get; set; }
        }


        public class RootObject2
        {
            public string error { get; set; }
        }


    }
}