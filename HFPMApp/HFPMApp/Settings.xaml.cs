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
    public partial class Settings : PhoneApplicationPage
    {

        public string downloadedText;
        WebClient client;
        string url;
        string uri;

        String server_ip;
        string file_contents = null;
        RootObjectPop jsonObject;


        public Settings()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                settings.Text = "Ρυθμίσεις";

                submit_button.Content = "Αποθήκευση";


            }



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
            button1.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.Relative);
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") button1.Text = "κυρίως μενού";
            else button1.Text = "main menu";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(main_menu_Click);

            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
                menuItem1.Text = "Έξοδος (" + PhoneApplicationService.Current.State["Username"] + ")";
            else
                menuItem1.Text = "Logout (" + PhoneApplicationService.Current.State["Username"] + ")";

            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(logout_Click);


            // CLIENT
            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;



        }







        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            loadingProgressBar.IsVisible = true;

            ListPickerItem new_item1 = new ListPickerItem();
            types.Items.Add(new_item1);
            new_item1.Content = "All types";

            ListPickerItem new_item2 = new ListPickerItem();
            departments.Items.Add(new_item2);
            new_item2.Content = "All departments";


            if (Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
            {

                Random rnd = new Random();
                int rand = rnd.Next(1, 10000);

                // edw kalw kai pairw apo ton server ola ta tmimata kai olous tous typous ka8ikontwn

                // REST Call
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/populatesettings/randomnum/" + rand;
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







        private void selectionSubmitted(object sender, RoutedEventArgs e)
        {

            // pairnw ta indices apo tin lista kai pairnw to antistoixo onoma 

            int type_index = types.SelectedIndex;
            string type_selected = null;
            if (type_index == 0)
            {
                type_selected = "All types";
            }
            else
            {
                type_selected = jsonObject.duties[type_index - 1].duty_name;
            }

            int department_index = departments.SelectedIndex;
            string department_selected = null;
            if (department_index == 0)
            {
                department_selected = "All departments";
            }
            else
            {
                department_selected = jsonObject.departments[department_index - 1].department_name;
            }

            //MessageBox.Show(type_index + ":" + type_selected + ", " + department_index + ":" + department_selected);



            // twra ta grafw sto arxeio gia na ta exw kai stin selida Declared.xaml
            using (StreamWriter writer = new StreamWriter("settings.txt"))
            {
                writer.Write(type_index.ToString() + "," + department_index.ToString() + "," + type_selected + "," + department_selected);
                writer.Close();
            }


            uri = "/MainMenuPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));


        }// end selectionSubmitted





        // function that retreives json data from web server
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {



                this.downloadedText = e.Result;

                // decode JSON
                jsonObject = JsonConvert.DeserializeObject<RootObjectPop>(this.downloadedText);

                int departments_count = jsonObject.departments.Count;
                int duties_count = jsonObject.duties.Count;


                for (int i = 0; i < duties_count; i++)
                {


                    string duty = jsonObject.duties[i].duty_name;


                    // populate list
                    ListPickerItem new_item = new ListPickerItem();
                    types.Items.Add(new_item);
                    new_item.Content = duty;

                }// for


                for (int i = 0; i < departments_count; i++)
                {


                    string department = jsonObject.departments[i].department_name;


                    // populate list
                    ListPickerItem new_item = new ListPickerItem();
                    departments.Items.Add(new_item);
                    new_item.Content = department;

                }// for




                

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



                string[] words = file_contents.Split(',');
                types.SelectedIndex = Convert.ToInt32(words[0]);
                departments.SelectedIndex = Convert.ToInt32(words[1]);



            }
            catch (TargetInvocationException ex)
            {

                if (Convert.ToString(PhoneApplicationService.Current.State["Language"]) == "GR") MessageBox.Show("Ανεπιτυχής προσπάθεια σύνδεσης στον εξυπηρέτη ή δεν υπάρχει αυτός ο χρήστης.");
                else MessageBox.Show("Could not connect to server or no such user.");
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }


            loadingProgressBar.IsVisible = false;

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





        // the classes which contain the properties of the specific JSON response
        public class RootObjectPop
        {
            public List<Departments> departments { get; set; }
            public List<Duties> duties { get; set; }
        }






        public class Duties
        {
            public string duty_name;
        }

        public class Departments
        {
            public string department_name;
        }

    }
}