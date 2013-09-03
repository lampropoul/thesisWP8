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
        string server_ip = String.Empty;


        public MainMenuPage()
        {
            InitializeComponent();

            // APP BAR
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;



            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem1.Text = "Έξοδος (" + PhoneApplicationService.Current.State["Username"] + ")";
            else menuItem1.Text = "Logout (" + PhoneApplicationService.Current.State["Username"] + ")";
            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(logout_Click);

            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem2.Text = "Εκκαθάριση καθηκόντων";
            else menuItem2.Text = "Clear old entries";
            ApplicationBar.MenuItems.Add(menuItem2);
            menuItem2.Click += new EventHandler(clear_old_entries_Click);

            ApplicationBarMenuItem menuItem3 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem3.Text = "Ρυθμίσεις για τα αιτήματα.";
            else menuItem3.Text = "Settings for declared";
            ApplicationBar.MenuItems.Add(menuItem3);
            menuItem3.Click += new EventHandler(settings_Click);


            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                page_title.Text = "Κεντρικό Μενού";

                pers_prog_button.Content = "Πρόγραμμα";
                search_button.Content = "Αναζήτηση";
                declared_button.Content = "Αιτήματα";
                edit_button.Content = "Επεξεργασία";
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



            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            
        }




        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            
            // TODO

            // edw erxomai otan kanw logout kai pataw to hardware back
            // 8elei dior8wsi

            // pairnw ta parameters (username kai password) apo tin MainPage.xaml
            string given_username = PhoneApplicationService.Current.State["Username"].ToString();
            string user_id = PhoneApplicationService.Current.State["UserId"].ToString();

            Random rnd = new Random();
            int rand = rnd.Next(1, 10000);

            // REST Call
            url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/notifications/id/" + user_id + "/randomnum/" + rand;
            client.DownloadStringAsync(new Uri(url));

        }







        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {


            try
            {



                this.downloadedText = e.Result;
                // decode JSON
                RootObject jsonObject = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);

                
                if (jsonObject.error == "")
                {

                    int count = jsonObject.program_id.Count;
                    
                    for (int i = 0; i < count; i++)
                    {


                        string req_date = String.Empty;
                        string req_time = String.Empty;


                        using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                        {

                            // -------------------------------------------------------------------//
                            // -------------------------- LOCAL DATABASE -------------------------//
                            // -------------------------------------------------------------------//

                            db.CreateIfNotExists();
                            db.LogDebug = true;







                            // ***************** GET DATE & TIME FROM CHANGE_LIST *************** //

                            IEnumerable<Change_list> query1 =
                                            from change in db.Change_list
                                            where change.Programid == jsonObject.program_id[i]
                                            select change;

                            foreach (Change_list cl in query1)
                            {
                                req_date = cl.RequestDate;
                                req_time = cl.RequestStartTime;
                            }







                            // **************** UPDATE PROGRAM *************** //

                            IEnumerable<Program> query2 =
                                            from programs in db.Program
                                            where programs.Programid == jsonObject.program_id[i]
                                            select programs;


                            foreach (Program p in query2)
                            {
                                // retreive difference

                                string[] words1 = p.Start.Split(':');
                                int hour_start = Convert.ToInt32(words1[0]);
                                string[] words2 = p.End.Split(':');
                                int hour_end = Convert.ToInt32(words2[0]);

                                int diff = hour_end - hour_start;
                                if (diff < 0) diff += 24;


                                string[] words3 = req_time.Split(':');
                                int hour_start_new = Convert.ToInt32(words3[0]);
                                int hour_end_new = hour_start_new + diff;

                                p.Date = req_date;
                                p.Start = req_time;

                                if (hour_end_new < 10) p.End = "0" + hour_end_new.ToString() + ":00:00";
                                else p.End = hour_end_new.ToString() + ":00:00";

                            }


                            try
                            {
                                db.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }







                            // ************* DELETE FROM CHANGE_LIST *************** //


                            IEnumerable<Change_list> query3 =
                                            from change in db.Change_list
                                            where change.Programid == jsonObject.program_id[i]
                                            select change;

                            foreach (Change_list cl in query3)
                            {
                                db.Change_list.DeleteOnSubmit(cl);
                            }


                            try
                            {
                                db.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }




                        }



                        MessageBox.Show(jsonObject.description[i]);


                    }// for

                }// if


            }
            catch (TargetInvocationException ex)
            {



                if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Σφάλμα τοπικής βάσης.");
                else MessageBox.Show("Local DB fault.");

                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }


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




        /// <summary>
        /// ka8arizw ton program kai ton change_list tis topikis basis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clear_old_entries_Click(object sender, EventArgs e)
        {


            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {

                // -------------------------------------------------------------------//
                // -------------------------- LOCAL DATABASE -------------------------//
                // -------------------------------------------------------------------//

                db.CreateIfNotExists();
                db.LogDebug = true;



                // === PROGRAM ===

                IEnumerable<Program> query =
                        from prog in db.Program
                        where prog.Userid == PhoneApplicationService.Current.State["UserId"].ToString() && Convert.ToDateTime(prog.Date) < DateTime.Now
                        select prog;

                // delete
                foreach (Program pr in query)
                {
                    db.Program.DeleteOnSubmit(pr);
                }

                // changes do not take place until SubmitChanges method is called
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }




                // === CHANGE_LIST ===
                
                IEnumerable<Change_list> query2 =
                        from change in db.Change_list
                        where change.Userid == Convert.ToInt32(PhoneApplicationService.Current.State["UserId"]) && Convert.ToDateTime(change.RequestDate) < DateTime.Now
                        select change;

                // delete
                foreach (Change_list ch in query2)
                {
                    db.Change_list.DeleteOnSubmit(ch);
                }

                // changes do not take place until SubmitChanges method is called
                try
                {
                    db.SubmitChanges();
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Διαγράφηκαν.");
                    else MessageBox.Show("Cleared.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }


        }



       

        private void pers_prog_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Personal_Program.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void search_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Search.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void declared_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Declared.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void edit_button_Click(object sender, RoutedEventArgs e)
        {
            uri = "/Edit.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }




        public class RootObject
        {
            public List<int> program_id { get; set; }
            public List<int> isSecretary { get; set; }
            public List<string> description { get; set; }
            public string error { get; set; }
        }

    }
}