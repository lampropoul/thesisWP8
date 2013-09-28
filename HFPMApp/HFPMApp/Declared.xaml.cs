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
    public partial class Declared : PhoneApplicationPage
    {

        public string downloadedText;
        WebClient client;
        string url;
        string uri;
        RootObject jsonObject;
        String server_ip;
        string file_contents = null;
        string settings_type = null;
        string settings_department = null;


        
        public Declared()
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


            try
            {
                using (StreamReader sr = new StreamReader("settings.txt"))
                {
                    file_contents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception esr)
            {
                MessageBox.Show("The file could not be read:");
                MessageBox.Show(esr.Message);
            }


            // -- GET SETTINGS -- //

            string[] words = file_contents.Split(',');
            settings_type = words[2];
            settings_department = words[3];



            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                page_title.Text = "Αιτήματα";
            }



            // CLIENT
            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;



        }














        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            loadingProgressBar.IsVisible = true;

            if (Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
            {

                Random rnd = new Random();
                int rand = rnd.Next(1, 10000);
                string user_id = PhoneApplicationService.Current.State["UserId"].ToString();

                // edw kalw kai pairw apo ton server ola ta tmimata kai olous tous typous ka8ikontwn

                // REST Call
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/populatedeclared/id/" + user_id +"/randomnum/" + rand;
                client.DownloadStringAsync(new Uri(url));



            }
            else
            {
                if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Δεν υπάρχει σύνδεση στο Internet.");
                else MessageBox.Show("Sorry, no internet connectivity.");

                uri = "/MainMenuPage.xaml";
                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
            }

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

                if (duties_count == 0)
                {
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Δεν βρέθηκε κάτι.");
                    else MessageBox.Show("No results");

                    uri = "/MainMenuPage.xaml";
                    NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                }

                List<string> StringsList = new List<string> { };

                for (int i = 0; i < duties_count; i++)
                {

                    int program_id = jsonObject.duties[i].program_id;
                    string name = jsonObject.duties[i].name;
                    string surname = jsonObject.duties[i].surname;
                    string type = jsonObject.duties[i].type;
                    string department = jsonObject.duties[i].department;
                    string date = jsonObject.duties[i].date;
                    string start = jsonObject.duties[i].start;
                    string end = jsonObject.duties[i].end;
                    string req_date = jsonObject.duties[i].req_date;
                    string req_time = jsonObject.duties[i].req_time;

                    int r = i+1;
                    string item = String.Empty;

                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
                    {
                        item = r.ToString() + ".  Ο χρήστης " + name + " " + surname + "\n    (" + type + ")\n    την ημέρα " + date + "\n    με έναρξη στις " + start + "\n    και λήξη στις " + end + "\n    αιτήθηκε αλλαγής την ημέρα " + req_date + "\n    με έναρξη στις " + req_time;
                    }
                    else
                    {
                        item = r.ToString() + ".  User " + name + " " + surname + "\n    (" + type + ")\n    on date " + date + "\n    with start time " + start + "\n    and end time " + end + "\n    requested change at " + req_date + "\n    and start time " + req_time;
                    }

                    if (i != duties_count - 1) item += "\n    -----------------------------------";


                    // check settings from file
                    if ( (settings_type == type || settings_type == "All types" )  &&  (settings_department == department || settings_department == "All departments") )
                    {
                        StringsList.Add(item);
                    }




                }// for


                declared_duties.ItemsSource = StringsList;






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






        private void declared_duties_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            string[] words = declared_duties.SelectedItem.ToString().Split('.');
            int index = Convert.ToInt32(words[0]) - 1;
            int program_id = jsonObject.duties[index].program_id;


            uri = "/Duties_Exchange.xaml?duty_id=" + program_id.ToString();
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
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
            public string name { get; set; }
            public string surname { get; set; }
            public string type { get; set; }
            public string department { get; set; }
            public string date { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string req_date { get; set; }
            public string req_time { get; set; }
        }


    }
}