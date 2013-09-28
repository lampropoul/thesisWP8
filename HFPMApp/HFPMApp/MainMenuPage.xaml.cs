using HFPMApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Scheduler;
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
        WebClient client2; //logout
        WebClient client3;
        string url;
        string uri;
        string server_ip = String.Empty;


        public MainMenuPage()
        {
            InitializeComponent();



            pers_prog_button.Content = "Personal\nProgram";
            search_button.Content = "Search";
            declared_button.Content = "Declared\nDuties";
            edit_button.Content = "Edit\nPersonal\nInfo";
            camera_button.Content = "Take\nPhoto";
            background_button.Content = "Run\nin BG";
            tile.Message = "Nothing new!";
            //tile.Title = "Notifi-\ncations";
            //tile_next_duty.Title = "Next\nDuties";


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

                pers_prog_button.Content = "Προσωπικό\nΠρόγραμμα";
                search_button.Content = "Αναζήτηση";
                declared_button.Content = "Αιτήματα\nΆλλων\nΧρηστών";
                edit_button.Content = "Επεξεργασία\nΣτοιχείων";
                camera_button.Content = "Τραβήξτε\nΦωτογραφία";
                background_button.Content = "Στο\nπαρασκήνιο";
                tile.Message = "Κανένα νέο!";
                //tile.Title = "Ειδοποι-\nήσεις";
                //tile_next_duty.Title = "Επόμενα\nΚαθήκοντα";
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

            client2 = new WebClient();
            client2.DownloadStringCompleted += client2_DownloadStringCompleted;

            client3 = new WebClient();
            client3.DownloadStringCompleted += client3_DownloadStringCompleted;
            
        }




        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            

            string user_id = String.Empty;
            string given_username = PhoneApplicationService.Current.State["Username"].ToString();

            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {
                IEnumerable<Users> query =
                    from user in db.Users
                    where user.Username == given_username
                    select user;
                

                foreach (Users us in query)
                {
                    user_id = us.Userid.ToString();
                }
            }

            PhoneApplicationService.Current.State["UserId"] = user_id;
            // TODO

            // edw erxomai otan kanw logout kai pataw to hardware back
            // 8elei dior8wsi

            

            Random rnd = new Random();
            int rand = rnd.Next(1, 10000);


            if (Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
            {


                loadingProgressBar.IsVisible = true;

                // REST Call
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/notifications/id/" + user_id + "/randomnum/" + rand;
                client.DownloadStringAsync(new Uri(url));

                // REST Call
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/populatedeclared/id/" + user_id + "/randomnum/" + rand;
                client3.DownloadStringAsync(new Uri(url));

            }

            // ---- NEXT 2 DUTIES ---- //

            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {

                // -------------------------------------------------------------------//
                // -------------------------- LOCAL DATABASE -------------------------//
                // -------------------------------------------------------------------//

                db.CreateIfNotExists();
                db.LogDebug = true;


                IEnumerable<Program> query =
                        from prog in db.Program
                        where prog.Userid == PhoneApplicationService.Current.State["UserId"].ToString()
                        orderby prog.Date ascending
                        select prog;


                string[] date = new String[2];
                int i = 0;

                foreach (Program pr in query)
                {
                    date[i] = pr.Date;
                    //MessageBox.Show(date[i]);

                    i++;
                    if (i == 2) break;
                }


                if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
                {
                    if (i == 0) tile_next_duty.Message = "Δεν υπάρχουν\nκαθήκοντα!";
                    if (i == 1) tile_next_duty.Message = "Το επόμενο\nκαθήκον είναι\nτην " + date[0];
                    if (i == 2) tile_next_duty.Message = "Επόμενα καθήκοντα:\n" + date[0] + "\nκαι\n" + date[1];
                }
                else
                {
                    if (i == 0) tile_next_duty.Message = "No duties\nin program!";
                    if (i == 1) tile_next_duty.Message = "Next duty\nis on\n" + date[0];
                    if (i == 2) tile_next_duty.Message = "Next duties\nare on\n" + date[0] + "\nand\n" + date[1];
                }

                VisualStateManager.GoToState(tile_next_duty, "Flipped", true);

            }

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


                        //string req_date = String.Empty;
                        //string req_time = String.Empty;


                        using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                        {

                            // -------------------------------------------------------------------//
                            // -------------------------- LOCAL DATABASE -------------------------//
                            // -------------------------------------------------------------------//

                            db.CreateIfNotExists();
                            db.LogDebug = true;







                            // ***************** GET DATE & TIME FROM CHANGE_LIST *************** //

                            //IEnumerable<Change_list> query1 =
                            //                from change in db.Change_list
                            //                where change.Programid == jsonObject.program_id[i]
                            //                select change;

                            //foreach (Change_list cl in query1)
                            //{
                            //    req_date = cl.RequestDate;
                            //    req_time = cl.RequestStartTime;
                            //}







                            // **************** UPDATE PROGRAM *************** //

                            IEnumerable<Program> query2 =
                                            from programs in db.Program
                                            where programs.Programid == jsonObject.program_id[i]
                                            select programs;


                            foreach (Program p in query2)
                            {
                                // retreive difference
                                /*
                                string[] words1 = p.Start.Split(':');
                                int hour_start = Convert.ToInt32(words1[0]);
                                string[] words2 = p.End.Split(':');
                                int hour_end = Convert.ToInt32(words2[0]);

                                int diff = hour_end - hour_start;
                                if (diff < 0) diff += 24;


                                string[] words3 = req_time.Split(':');
                                int hour_start_new = Convert.ToInt32(words3[0]);
                                int hour_end_new = hour_start_new + diff;
                                */
                                p.Date = jsonObject.date[i];
                                p.Start = jsonObject.start_time[i];
                                p.End = jsonObject.end_time[i];

                                /*
                                if (hour_end_new < 10) p.End = "0" + hour_end_new.ToString() + ":00:00";
                                else p.End = hour_end_new.ToString() + ":00:00";
                                 * */

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


                        loadingProgressBar.IsVisible = false;

                        tile.Message = jsonObject.description[i];
                        //MessageBox.Show(jsonObject.description[i]);


                    }// for

                }// if
                else
                {
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") tile.Message = "Κανένα νέο!";
                    else tile.Message = "Nothing new!";
                }

                VisualStateManager.GoToState( tile , "Flipped", true );

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

            loadingProgressBar.IsVisible = false;

        }






        
        private void logout_Click(object sender, EventArgs e)
        {

            loadingProgressBar.IsVisible = true;


            string user_id = PhoneApplicationService.Current.State["UserId"].ToString();

            string given_username = PhoneApplicationService.Current.State["Username"].ToString();
            Random rnd = new Random();
            int rand = rnd.Next(1, 10000);

            if (Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
            {
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/logout/id/" + user_id + "/randomnum/" + rand;
                client2.DownloadStringAsync(new Uri(url));
            }

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
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Εκκαθάριση ΟΚ.");
                    else MessageBox.Show("Cleared.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }


        }








        void client2_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            loadingProgressBar.IsVisible = false;
        }





        private void camera_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not yet implemented!");
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
            public List<string> date { get; set; }
            public List<string> start_time { get; set; }
            public List<string> end_time { get; set; }
            public string error { get; set; }
        }







        private void background_button_Click(object sender, RoutedEventArgs e)
        {


            // ************  TILE  ************* //

            IconicTileData oIcontile = new IconicTileData();
            oIcontile.Title = "HFPM Notifications";
            oIcontile.Count = 1;

            oIcontile.IconImage = new Uri("Assets/Tiles/110x110.png", UriKind.Relative);
            oIcontile.SmallIconImage = new Uri("Assets/Tiles/220x220.png", UriKind.Relative);

            oIcontile.WideContent1 = "windows phone 8 Live tile";
            oIcontile.WideContent2 = "Icon tile";
            oIcontile.WideContent3 = "All about Live tiles By WmDev";

            oIcontile.BackgroundColor = System.Windows.Media.Colors.Orange;

            // find the tile object for the application tile that using "Iconic" contains string in it.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("Iconic".ToString()));

            if (TileToFind != null && TileToFind.NavigationUri.ToString().Contains("Iconic"))
            {
                TileToFind.Delete();
                ShellTile.Create(new Uri("/MainPage.xaml?id=Iconic", UriKind.Relative), oIcontile, true);
            }
            else
            {
                ShellTile.Create(new Uri("/MainPage.xaml?id=Iconic", UriKind.Relative), oIcontile, true);
            }



        }








        private void tile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            Random rnd = new Random();
            int rand = rnd.Next(1, 10000);

            string user_id = PhoneApplicationService.Current.State["UserId"].ToString();


            if (Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
            {

                loadingProgressBar.IsVisible = true;

                // REST Call
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/notifications/id/" + user_id + "/randomnum/" + rand;
                client.DownloadStringAsync(new Uri(url));


                // REST Call
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/populatedeclared/id/" + user_id + "/randomnum/" + rand;
                client3.DownloadStringAsync(new Uri(url));

            }

            VisualStateManager.GoToState(tile, "Flipped", true);
        }






        void client3_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {

                this.downloadedText = e.Result;

                // decode JSON
                RootObject2 jsonObject = JsonConvert.DeserializeObject<RootObject2>(this.downloadedText);

                int duties_count = jsonObject.duties.Count;

                if (duties_count != 0)
                {
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") tile.Message = "Υπάρχουν\nκαθήκοντα\nπρος ανταλλαγή.";
                    else tile.Message = "There are\nexchangeable\nduties.";
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



        public class RootObject2
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






        private void tile_next_duty_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            VisualStateManager.GoToState(tile_next_duty, "Flipped", true);
        }





    }
}