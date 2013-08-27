using HFPMApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Scheduler;



namespace HFPMApp
{
    public partial class Change_Duty_Page : PhoneApplicationPage
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
        string previous_requests;




        public Change_Duty_Page()
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


            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
                page_title.Text = "Λεπτομέρειες";

                to.Text = "μέχρι";

                newdateblock.Text = "Νέα Ημ/νία";
                newtimeblock.Text = "Νέα Ώρα";
                request_change_button.Content = "Αίτημα Αλλαγής";

                remind.Text = "Υπενθύμιση";
                min30.Content = "30 λεπτά πριν";
                min60.Content = "1 ώρα πριν";
                min120.Content = "2 ώρες πριν";
                alarm_button.Content = "Ορισμός";
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
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem1.Text = "Έξοδος (" + PhoneApplicationService.Current.State["Username"] + ")";
            else menuItem1.Text = "Logout (" + PhoneApplicationService.Current.State["Username"] + ")";
            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(logout_Click);




            // CLIENT
            client_up = new WebClient();
            client_up.UploadStringCompleted += client_UploadStringCompleted;


            any_time.Checked += any_time_Checked;
            any_time.Unchecked += any_time_Unchecked;

        }

        void any_time_Unchecked(object sender, RoutedEventArgs e)
        {
            timepicked.Visibility = Visibility.Visible;
        }

        void any_time_Checked(object sender, RoutedEventArgs e)
        {
            timepicked.Visibility = Visibility.Collapsed;
        }






        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string progid = String.Empty;
            NavigationContext.QueryString.TryGetValue("progid", out progid);


            // retreive info about the selected program
            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {


                // -------------------------------------------------------------------//
                // -------------------------- LOCAL DATABASE -------------------------//
                // -------------------------------------------------------------------//

                db.CreateIfNotExists();
                db.LogDebug = true;



                IEnumerable<Program> query =
                                    from program in db.Program
                                    where program.Programid == Convert.ToInt32(progid)
                                    select program;



                foreach (Program pr in query)
                {
                    page_title.Text = pr.Date;
                    cur_start_time.Text = pr.Start;
                    cur_end_time.Text = pr.End;
                    type.Text = pr.Dutytype;
                    location.Text = pr.Location;
                }


            }
            


        }





        private void request_change_button_Click(object sender, RoutedEventArgs e)
        {




            // pairnw ta parameters (username)
            given_username = PhoneApplicationService.Current.State["Username"].ToString();


            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);

            datepicked.ValueStringFormat = "{0:yyyy-MM-dd}";
            timepicked.ValueStringFormat = "{0:HH:mm:ss}";

            string progid = String.Empty;
            NavigationContext.QueryString.TryGetValue("progid", out progid);


            // check internet connectivity
            bool hasInternet = NetworkInterface.GetIsNetworkAvailable();
            //hasInternet = false;



            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {


                // -------------------------------------------------------------------//
                // -------------------------- LOCAL DATABASE -------------------------//
                // -------------------------------------------------------------------//

                db.CreateIfNotExists();
                db.LogDebug = true;



                IEnumerable<Change_list> query =
                                    from change in db.Change_list
                                    where change.Programid == Int32.Parse(progid)
                                    select change;


                // delete
                foreach (Change_list ch in query)
                {
                    db.Change_list.DeleteOnSubmit(ch);
                }


                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }





                db.Change_list.InsertOnSubmit(new Change_list
                {
                    Userid = Int32.Parse(PhoneApplicationService.Current.State["UserId"].ToString()),
                    Programid = Int32.Parse(progid),
                    RequestDate = datepicked.ValueString,
                    RequestStartTime = timepicked.ValueString
                });



                // changes do not take place until SubmitChanges method is called
                try
                {
                    db.SubmitChanges();
                    //MessageBox.Show("Program with id=" + program_id + " inserted.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }




                



                // ean exw internet proxwraw kai kanw to REST call
                if (Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
                {


                    // encode JSON
                    RootObject jsonObject = new RootObject();
                    // put fields' values to json object
                    jsonObject.user_id = Int32.Parse(PhoneApplicationService.Current.State["UserId"].ToString());
                    jsonObject.id = Int32.Parse(progid);
                    jsonObject.request_date = datepicked.ValueString;
                    if (Convert.ToBoolean(any_time.IsChecked))
                    {
                        jsonObject.request_start_time = "Any";
                    }
                    else
                    {
                        jsonObject.request_start_time = timepicked.ValueString;
                    }

                    this.json_to_send = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, new JsonSerializerSettings { });

                    MessageBox.Show(json_to_send);
                    // REST CALL
                    client_up.Headers["Method"] = "POST";
                    client_up.Headers[HttpRequestHeader.ContentType] = "application/json";

                    url_post = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/change/id/" + progid + "/randomnum/" + rand;
                    client_up.UploadStringAsync(new Uri(url_post), json_to_send);

                }
                // ean den exw internet
                else
                {

                    try
                    {
                        using (StreamWriter sw = new StreamWriter("previous_requests.txt"))
                        {
                            sw.Write("pending");
                            sw.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("The file could not be read:");
                        MessageBox.Show(ex.Message);
                    }



                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Δεν εντοπίστηκε σύνδεση στο Internet. Την επόμενη φορά που θα εντοπιστεί το αίτημά σας θα σταλεί.");
                    else MessageBox.Show("You are currently offline. The next time you connect to the Internet your request you just did will be sent.");
                    
                    
                    uri = "/Personal_Program.xaml";
                    NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

                }



            }// using


        }





        /// <summary>
        /// --- ALARM ---
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alarm_button_Click(object sender, RoutedEventArgs e)
        {

            Int32 year;
            Int32 month;
            Int32 day;
            Int32 hour;
            Int32 minute;
            Int32 second;


            string[] words1 = page_title.Text.Split('-');
            string[] words2 = cur_start_time.Text.Split(':');

            year = Convert.ToInt32(words1[0]);
            month = Convert.ToInt32(words1[1]);
            day = Convert.ToInt32(words1[2]);
            hour = Convert.ToInt32(words2[0]);
            minute = Convert.ToInt32(words2[1]);
            second = Convert.ToInt32(words2[2]);




            // 30 mins
            if (min_before_list.SelectedIndex == 0)
            {
                
                DateTime bt = new DateTime(year, month, day, hour, minute, second);

                bt.AddMinutes(-30);

                if (ScheduledActionService.Find("My Alarm") != null)
                    ScheduledActionService.Remove("My Alarm");

                Alarm a = new Alarm("My Alarm")
                {
                    Content = "Alarm",
                    BeginTime = bt.Date
                };


                if (bt.Date >= DateTime.Now)
                {
                    ScheduledActionService.Add(a);
                    MessageBox.Show("Alarm added " + min_before_list.SelectedItem.ToString() + ".");
                }
                else
                {
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Αυτό το καθήκον ανήκει στο παρελθόν. Παρακαλώ εκκαθαρίστε τις παλιές εγγραφές μέσω του popup μενού.");
                    else MessageBox.Show("This duty has passed. Please clear old entries (popup menu selection).");
                }

                uri = "/Personal_Program.xaml";
                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                

            }
            // 60 mins
            else if (min_before_list.SelectedIndex == 1)
            {
                DateTime bt = new DateTime(year, month, day, hour, minute, second);

                bt.AddHours(-1);

                if (ScheduledActionService.Find("My Alarm") != null)
                    ScheduledActionService.Remove("My Alarm");

                Alarm a = new Alarm("My Alarm")
                {
                    Content = "Alarm",
                    BeginTime = bt.Date
                };


                if (bt.Date >= DateTime.Now)
                {
                    ScheduledActionService.Add(a);
                    MessageBox.Show("Alarm added " + min_before_list.SelectedItem.ToString() + ".");
                }
                else
                {
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Αυτό το καθήκον ανήκει στο παρελθόν. Παρακαλώ εκκαθαρίστε τις παλιές εγγραφές μέσω του popup μενού.");
                    else MessageBox.Show("This duty has passed. Please clear old entries (popup menu selection).");
                }

                uri = "/Personal_Program.xaml";
                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

            }
            // 120 mins
            else if (min_before_list.SelectedIndex == 2)
            {
                DateTime bt = new DateTime(year, month, day, hour, minute, second);

                bt.AddHours(-2);

                if (ScheduledActionService.Find("My Alarm") != null)
                    ScheduledActionService.Remove("My Alarm");

                Alarm a = new Alarm("My Alarm")
                {
                    Content = "Alarm",
                    BeginTime = bt.Date
                };


                if (bt.Date >= DateTime.Now)
                {
                    ScheduledActionService.Add(a);
                    MessageBox.Show("Alarm added " + min_before_list.SelectedItem.ToString() + ".");
                }
                else
                {
                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Αυτό το καθήκον ανήκει στο παρελθόν. Παρακαλώ εκκαθαρίστε τις παλιές εγγραφές μέσω του popup μενού.");
                    else MessageBox.Show("This duty has passed. Please clear old entries (popup menu selection).");
                }

                uri = "/Personal_Program.xaml";
                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

            }




            


        }






        /// <summary>
        /// function that sends json data to web server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {

                try
                {

                    this.downloadedText = e.Result;
                    RootObject jsonObject_res = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);


                    if (jsonObject_res.message == "InsertedOrUpdated")
                    {

                        //using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                        //{

                        //    // -------------------------------------------------------------------//
                        //    // -------------------------- LOCAL DATABASE -------------------------//
                        //    // -------------------------------------------------------------------//

                        //    db.CreateIfNotExists();
                        //    db.LogDebug = true;


                            

                        //    IEnumerable<Change_list> query =
                        //                from change in db.Change_list
                        //                select change;

                        //    // delete
                        //    foreach (Change_list ch in query)
                        //    {
                        //        db.Change_list.DeleteOnSubmit(ch);
                        //    }


                        //    try
                        //    {
                        //        db.SubmitChanges();
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        MessageBox.Show(ex.Message);
                        //    }


                        //    db.Change_list.InsertOnSubmit(new Change_list
                        //    {
                        //        Programid = jsonObject_res.id,
                        //        Userid = jsonObject_res.user_id,
                        //        RequestDate = jsonObject_res.request_date,
                        //        RequestStartTime = jsonObject_res.request_start_time
                        //    });




                        //    // changes do not take place until SubmitChanges method is called
                        //    try
                        //    {
                        //        db.SubmitChanges();
                        //        MessageBox.Show("Request sent to server.");
                        //        uri = "/Personal_Program.xaml";
                        //        NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        MessageBox.Show(ex.Message);
                        //    }


                        //}

                        if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Το αίτημα εστάλη στην γραμματεία.");
                        else MessageBox.Show("Request sent to server.");
                        uri = "/Personal_Program.xaml";
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

            uri = "/Personal_Program.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }





        public class RootObject
        {
            public int user_id { get; set; }
            public int id { get; set; }
            public string request_date { get; set; }
            public string request_start_time { get; set; }
            public string error { get; set; }
            public string message { get; set; }
        }





    }
}