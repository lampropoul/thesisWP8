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

namespace HFPMApp
{
    public partial class Personal_Program : PhoneApplicationPage
    {
        
        
        const string CurrentEntryDateKey = "CurrentEntryDateKey";
        DateTime? _entryDate = DateTime.Now;
        Dictionary<DateTime, string> _dummyRepository = new Dictionary<DateTime, string>();
        Dictionary<DateTime, string> _requestRepository = new Dictionary<DateTime, string>();

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
        

        public Personal_Program()
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
                page_title.Text = "Πρόγραμμα";

                please_wait.Text = "Παρακαλώ περιμένετε...";
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

            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem2.Text = "Εκκαθάριση  καθηκόντων";
            else menuItem2.Text = "Clear old entries";
            ApplicationBar.MenuItems.Add(menuItem2);
            menuItem2.Click += new EventHandler(clear_old_entries_Click);


            // CLIENTS
            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;

            client_up = new WebClient();
            client_up.UploadStringCompleted += client_UploadStringCompleted;


        }

        




        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            loadingProgressBar.IsVisible = true;

            // pairnw ta parameters (username)
            given_username = PhoneApplicationService.Current.State["Username"].ToString();


            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);


            

            
            // ean exw internet proxwraw kai kanw to REST call
            if (Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
            {

                

                // REST CALL
                url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/program/username/" + given_username + "/randomnum/" + rand;
                client.DownloadStringAsync(new Uri(url));


                // kai twra stelnw tuxon requests poy ekane o xristis offline

                // prwta select apo tin local
                using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                {

                    // -------------------------------------------------------------------//
                    // -------------------------- LOCAL DATABASE -------------------------//
                    // -------------------------------------------------------------------//

                    db.CreateIfNotExists();
                    db.LogDebug = true;


                    IEnumerable<Change_list> query =
                                from change in db.Change_list
                                where change.Userid == Convert.ToInt32(PhoneApplicationService.Current.State["UserId"])
                                select change;

                    int count = 0;
                    //int[] ids = {0,0,0,0,0,0,0,0,0,0};
                    //int[] user_ids = {0,0,0,0,0,0,0,0,0,0};
                    //String[] dates = new String[20];
                    //String[] start_times = new String[20];


                    // encode JSON
                    RootObjectRequest jsonObject = new RootObjectRequest();
                    Changes[] changes_array = new Changes[10];

                    foreach (Change_list ch in query)
                    {

                        // put fields' values to json object
                        
                        //jsonObject.changes[count].user_id = Convert.ToInt32(PhoneApplicationService.Current.State["UserId"]);
                        //jsonObject.changes[count].id = ch.Programid;
                        //jsonObject.changes[count].request_date = ch.RequestDate;
                        //jsonObject.changes[count].request_start_time = ch.RequestStartTime;

                        //changes["user_id"].Add(Convert.ToInt32(PhoneApplicationService.Current.State["UserId"]);

                        changes_array[count] = new Changes();

                        changes_array[count].user_id = Convert.ToInt32(PhoneApplicationService.Current.State["UserId"]);
                        changes_array[count].id = ch.Programid;
                        changes_array[count].request_date = ch.RequestDate;
                        changes_array[count].request_start_time = ch.RequestStartTime;
                        
                        //ids[count] = ch.Programid;
                        //user_ids[count] = ch.Userid;
                        //dates[count] = ch.RequestDate;
                        //start_times[count] = ch.RequestStartTime;

                        count++;
                    }

                    //MessageBox.Show(ids[0].ToString() + "," + ids[1].ToString() + "," + ids[2].ToString() + "," + ids[3].ToString() + "," + ids[4].ToString());



                    if (count != 0)
                    {

                        // stlenw ston server tis aithseiw pou ekane o xristis otan htan offline

                        this.json_to_send = JsonConvert.SerializeObject(new { changes = changes_array, size = count }, Formatting.Indented, new JsonSerializerSettings { });

                        //MessageBox.Show(json_to_send);

                        // REST CALL
                        url_post = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/changes/randomnum/" + rand;
                        client_up.Headers["Method"] = "POST";
                        client_up.Headers[HttpRequestHeader.ContentType] = "application/json";
                        client_up.UploadStringAsync(new Uri(url_post), json_to_send);


                        //// kai svinw tin change list

                        //IEnumerable<Change_list> query2 =
                        //            from change in db.Change_list
                        //            where change.Userid == Convert.ToInt32(PhoneApplicationService.Current.State["UserId"])
                        //            select change;


                        //// delete
                        //foreach (Change_list ch in query2)
                        //{
                        //    db.Change_list.DeleteOnSubmit(ch);
                        //}


                        //try
                        //{
                        //    db.SubmitChanges();
                        //}
                        //catch (Exception ex)
                        //{
                        //    MessageBox.Show(ex.Message);
                        //}



                    }

                }


                
                

            }
            // ean den exw internet, kalw thn topiki bash --PROGRAM--
            else
            {

                

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
                            orderby prog.Date descending
                            select prog;



                    foreach (Program pr in query)
                    {

                        string[] words = pr.Date.Split('-');
                        DateTime selectedDate = new DateTime(Int32.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2]));
                        if (!_dummyRepository.ContainsKey(selectedDate.Date))
                            _dummyRepository.Add(selectedDate.Date, pr.Programid.ToString());

                        //MessageBox.Show(selectedDate + "------" + words[0] + "-" + words[1] + "-" + words[2]);

                        InitializeCalendar(selectedDate);

                    }

                    

                }

                loadingProgressBar.IsVisible = false;

            }


            // request data on calendar

            using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
            {

                // -------------------------------------------------------------------//
                // -------------------------- LOCAL DATABASE -------------------------//
                // -------------------------------------------------------------------//

                db.CreateIfNotExists();
                db.LogDebug = true;


                IEnumerable<Change_list> query =
                        from change in db.Change_list
                        join program in db.Program on change.Programid equals program.Programid
                        where change.Userid == Convert.ToInt32(PhoneApplicationService.Current.State["UserId"])
                        select change;



                foreach (Change_list ch in query)
                {

                    string[] words = ch.RequestDate.Split('-');
                    DateTime selectedDate = new DateTime(Int32.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2]));
                    if (!_requestRepository.ContainsKey(selectedDate.Date))
                        _requestRepository.Add(selectedDate.Date, ch.Programid.ToString());

                    //MessageBox.Show(selectedDate + "------" + words[0] + "-" + words[1] + "-" + words[2]);

                    InitializeCalendar(selectedDate);

                }


                IEnumerable<Program> query2 =
                        from program in db.Program
                        join change in db.Change_list on program.Programid equals change.Programid
                        where change.Userid == Convert.ToInt32(PhoneApplicationService.Current.State["UserId"])
                        select program;



                foreach (Program ch in query2)
                {

                    string[] words = ch.Date.Split('-');
                    DateTime selectedDate = new DateTime(Int32.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2]));
                    if (!_requestRepository.ContainsKey(selectedDate.Date))
                        _requestRepository.Add(selectedDate.Date, ch.Programid.ToString());

                    //MessageBox.Show(selectedDate + "------" + words[0] + "-" + words[1] + "-" + words[2]);

                    InitializeCalendar(selectedDate);

                }

            }





            //If we return to this page after Tomstoning, then get the previous state from
            // PhoneApplicationService
            if (PhoneApplicationService.Current.State.ContainsKey(CurrentEntryDateKey))
                _entryDate = (DateTime)PhoneApplicationService.Current.State[CurrentEntryDateKey];

            InitializeCalendar(_entryDate.Value);

            if (!Convert.ToBoolean(PhoneApplicationService.Current.State["hasInternet"]))
            {
                please_wait.Visibility = Visibility.Collapsed;
            }


            

        }

        /// <summary>
        /// In this method we redraw the calendar when the month is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChangeMonth(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "NextBtn")
                _entryDate = _entryDate.Value.AddMonths(1);
            else
                _entryDate = _entryDate.Value.AddMonths(-1);

            //saving the entry date to restore the state after Tombstoning
            PhoneApplicationService.Current.State[CurrentEntryDateKey] = _entryDate;

            CalendarListBox.Visibility = Visibility.Collapsed;

            //Redraw the calendar
            InitializeCalendar(_entryDate.Value);
        }

        /// <summary>
        /// This method creates the actual calendar view. The control used to display "day" is made up
        /// Rectangle shape with rounded borders + button control (See XAML).
        /// Current Date is indicated by Orange color.
        /// A Date with no data is indicated by while color.
        /// A Date with data is indicated by yellow color
        /// </summary>
        /// <param name="entryDate"></param>
        protected void InitializeCalendar(DateTime entryDate)
        {
            MonthYear.Text = String.Format("{0:MMMM yyyy}", _entryDate.Value);

            string[] english = MonthYear.Text.Split(' ');

            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR")
            {
                if (english[0] == "January") MonthYear.Text = "Ιανουάριος " + english[1];
                if (english[0] == "February") MonthYear.Text = "Φεβρουάριος " + english[1];
                if (english[0] == "March") MonthYear.Text = "Μάρτιος " + english[1];
                if (english[0] == "April") MonthYear.Text = "Απρίλιος " + english[1];
                if (english[0] == "May") MonthYear.Text = "Μάιος " + english[1];
                if (english[0] == "June") MonthYear.Text = "Ιούνιος " + english[1];
                if (english[0] == "July") MonthYear.Text = "Ιούλιος " + english[1];
                if (english[0] == "August") MonthYear.Text = "Αύγουστος " + english[1];
                if (english[0] == "September") MonthYear.Text = "Σεπτέμβριος " + english[1];
                if (english[0] == "October") MonthYear.Text = "Οκτώβριος " + english[1];
                if (english[0] == "November") MonthYear.Text = "Νοέμβριος " + english[1];
                if (english[0] == "December") MonthYear.Text = "Δεκέμβριος " + english[1];
            }

            DateTime todaysDate = DateTime.Now;
            bool isTodaysDate = false;
            
            //if entryDate month = today's month, then disable the next button
            if (todaysDate.Month == entryDate.Month && todaysDate.Year == entryDate.Year)
            {
                //NextBtn.Visibility = System.Windows.Visibility.Collapsed;

                isTodaysDate = true;
            }
            else
                NextBtn.Visibility = System.Windows.Visibility.Visible;


            //The following code is an optimization, such that instead of recreating all the days
            // of a month it just adds/removes controls depending on how many days are in a month.
            //Example: The initial view is for December; when user clicks the previous button to browse to
            // November, the code below will remove the last (31st) control. Likewise when the user
            // browses back to December, it'll add new control to the end.
            //
            //This code greatly improves the performance of your app when switching between months
            //
            int numDays = DateTime.DaysInMonth(entryDate.Year, entryDate.Month);
            //check if the day buttons are already added
            int count = CalendarWrapPanel.Children.Count;
            if (count > numDays)
            {
                //remove days from the end
                for (int i = 1; i <= count - numDays; i++)
                    CalendarWrapPanel.Children.RemoveAt(count - i);
            }
            else
            {
                //calculate number of days to add
                int start = count + 1;
                for (int i = start; i <= numDays; i++)
                {
                    Border border = new Border();
                    border.Background = new SolidColorBrush(Color.FromArgb(255, 103, 183, 212));
                    border.Margin = new Thickness(0, 0, 2, 2);
                    border.Width = 60;
                    border.Height = 60;
                    border.CornerRadius = new CornerRadius(10);

                    Button btn = new Button();
                    btn.Name = "Day" + i;
                    btn.Content = i.ToString();
                    btn.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    btn.Width = 70;
                    btn.Height = 70;
                    btn.FontSize = 18;
                    border.Child = btn;
                    btn.Style = this.Resources["ButtonStyle1"] as Style;

                    //btn.Margin = new Thickness(0, 0, 5, 5);
                    btn.Click += new RoutedEventHandler(OnDayButtonClick);

                    CalendarWrapPanel.Children.Add(border);
                }
            }

            //NOTE: To change the button foreground color I'm using custom styles instead of setting the Foreground color.
            // This is because there's an issue with the Silverlight button (don't know if its an issue or just the default behavior)
            // where even after setting the Foreground color, it would revert back to the default color.
            // So the workaround is to define different styles in XAML (for example in MainPage.xaml I defined ButtonStyle1, HasDataButtonStyle
            // and TodayHasDataButtonStyle and set the style as
            // Button.Style = Resources["ButtonStyle1"] as Style

            //reset the backgrounds as necessary
            for (int i = 0; i < numDays; i++)
            {
                Border border = (Border)CalendarWrapPanel.Children[i];
                if (border != null)
                {
                    Button btn = (Button)border.Child;
                    //check if user has entered data for this day
                    //if (isTodaysDate && (i + 1) > todaysDate.Day)
                    //{
                    //    //disable future days
                    //    btn.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    //    btn.IsEnabled = false;
                    //}
                    //else
                        btn.IsEnabled = true;

                    bool isToday = false;
                    DateTime currDate = new DateTime(entryDate.Year, entryDate.Month, i + 1);
                    //if this is the current date, set the background color to orange
                    if (currDate.Date.CompareTo(DateTime.Now.Date) == 0)
                    {
                        border.Background = new SolidColorBrush(Color.FromArgb(255, 255, 165, 0));
                        isToday = true;
                    }
                    else
                    {
                        border.Background = new SolidColorBrush(Color.FromArgb(255, 103, 183, 212));
                    }

                    //check if there's any data available for this day
                    string data;
                    _dummyRepository.TryGetValue(new DateTime(entryDate.Year, entryDate.Month, i + 1), out data);

                    string req_data;
                    _requestRepository.TryGetValue(new DateTime(entryDate.Year, entryDate.Month, i + 1), out req_data);




                    if (data != null)
                    {
                        //if there's data for this day, set the button fore ground color to Orange
                        if (isToday)
                            btn.Style = this.Resources["TodayHasDataButtonStyle"] as Style;
                        else
                            btn.Style = this.Resources["HasDataButtonStyle"] as Style;
                    }
                    else
                    {
                        //there's no data for this day, set the button foreground to White
                        btn.Style = this.Resources["ButtonStyle1"] as Style;
                    }


                    if (req_data != null)
                    {
                        //if there's data for this day, set the button fore ground color to Orange
                        if (isToday)
                            btn.Style = this.Resources["TodayHasReqDataButtonStyle"] as Style;
                        else
                            btn.Style = this.Resources["HasReqDataButtonStyle"] as Style;
                    }

                }
            }
            CalendarWrapPanel.UpdateLayout();
            CalendarListBox.Visibility = Visibility.Visible;

            

        }



        private void OnDayButtonClick(object sender, RoutedEventArgs e)
        {
            //Handle button click event
            //On click adding some dummy data to the repository
            DateTime selectedDate = new DateTime(_entryDate.Value.Year,_entryDate.Value.Month,Int32.Parse((string)((Button)sender).Content));
            //if (!_dummyRepository.ContainsKey(selectedDate.Date))
            //    _dummyRepository.Add(selectedDate.Date, "data");
            
            //NOTE: In real scenarios in OnButtonClick we would launch a new Page to accept data for the selected
            // date. And when we return back from the data entry page to this page, the "OnNavigatedTo()" method
            // will get called. And in "OnNavigatedTo() method we are calling InitializeCalendar() which takes care of redrawing the
            // calendar.
            // Since this just a sample app...I'm calling InitializeCalendar() in "OnButtonClick" to refresh the view.
            // If you are launching a page OnButtonClick, you don't have to call InitializeCalendar() here.

            //MessageBox.Show(selectedDate.ToString());

            //InitializeCalendar(_entryDate.Value);


            if (_dummyRepository.ContainsKey(selectedDate.Date))
            {

                uri = "/Change_Duty_Page.xaml?progid=" + _dummyRepository[selectedDate.Date];
                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
            }
            else
            {
                if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Δεν υπάρχει κάποιο καθήκον αυτήν την ημέρα.");
                else MessageBox.Show("There's no duty for this day.");
            }
            
            
        }






        // function that retreives json data from web server
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            loadingProgressBar.IsVisible = true;
            
            try
            {

                this.downloadedText = e.Result;

                // decode JSON
                RootObject jsonObject = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);


                int length = jsonObject.programs.Count;


                // logged via internet. now delete and insert to local db
                using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                {

                    // -------------------------------------------------------------------//
                    // -------------------------- LOCAL DATABASE -------------------------//
                    // -------------------------------------------------------------------//

                    db.CreateIfNotExists();
                    db.LogDebug = true;

                    

                    // delete all programs just to fetch the (possibly) updated ones
                    IEnumerable<Program> query =
                                    from prog in db.Program
                                    select prog;

                    // delete
                    foreach (Program pr in query)
                    {
                        db.Program.DeleteOnSubmit(pr);
                        //MessageBox.Show("Program with id=" + program_id + " deleted.");
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


                    


                    for (int i = 0; i < length; i++)
                    {

                        int user_id = jsonObject.programs[i].user_id;
                        int program_id = jsonObject.programs[i].id;
                        //MessageBox.Show(program_id.ToString());
                        string date = jsonObject.programs[i].date;
                        string duty_type = jsonObject.programs[i].duty_type;
                        string duty_start_time = jsonObject.programs[i].duty_start_time;
                        string duty_end_time = jsonObject.programs[i].duty_end_time;
                        string location = jsonObject.programs[i].location;
                        string program_name = jsonObject.programs[i].program_name;


                        // fill calendar with program data

                        string[] words = date.Split('-');
                        DateTime selectedDate = new DateTime(Int32.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2]));
                        if (!_dummyRepository.ContainsKey(selectedDate.Date))
                            _dummyRepository.Add(selectedDate.Date, program_id.ToString());

                        //MessageBox.Show(selectedDate + "------" + words[0] + "-" + words[1] + "-" + words[2]);

                        InitializeCalendar(selectedDate);




                        db.Program.InsertOnSubmit(new Program
                        {
                            Userid = user_id.ToString(),
                            Programid = program_id,
                            Date = date,
                            Start = duty_start_time,
                            End = duty_end_time,
                            Dutytype = duty_type,
                            Location = location,
                            Progname = program_name
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




                    }// for

                    //MessageBox.Show("Loaded.");




                }// using

                loadingProgressBar.IsVisible = false;

            }
            catch (TargetInvocationException ex)
            {
                loadingProgressBar.IsVisible = false;

                if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Δεν υπάρχουν καθήκοντα");
                else MessageBox.Show("No duties");
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }


            please_wait.Visibility = Visibility.Collapsed;


        }





        
        // function that sends json data to web server
        // send request to server
        void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {

                try
                {

                    this.downloadedText = e.Result;
                    //MessageBox.Show(this.downloadedText);
                    RootObjectRequest jsonObject_res = JsonConvert.DeserializeObject<RootObjectRequest>(this.downloadedText);


                    if (jsonObject_res.message == "InsertedOrUpdated")
                    {


                        try
                        {
                            using (StreamReader sr = new StreamReader("previous_requests.txt"))
                            {
                                previous_requests = sr.ReadToEnd();

                                if (previous_requests == "pending")
                                {

                                    loadingProgressBar.IsVisible = false;

                                    if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") MessageBox.Show("Οι εκκρεμείς αιτήσεις εστάλησαν στην γραμματεία.");
                                    else MessageBox.Show("Your previous requests are sent to server.");
                                }

                                sr.Close();
                            }

                            using (StreamWriter sw = new StreamWriter("previous_requests.txt"))
                            {

                                if (previous_requests == "pending") sw.Write("ok");

                                sw.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("The file could not be read:");
                            MessageBox.Show(ex.Message);
                        }


                        uri = "/Personal_Program.xaml";
                        NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

                    }
                    else
                    {

                        loadingProgressBar.IsVisible = false;

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

                loadingProgressBar.IsVisible = false;

                MessageBox.Show("TargetInvocationException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }

            please_wait.Visibility = Visibility.Collapsed;
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




        // the classes which contain the properties of the specific JSON response
        public class RootObject
        {
            public List<Programs> programs { get; set; }
        }


        public class Programs
        {
            public int user_id { get; set; }
            public int id { get; set; }
            public string date { get; set; }
            public string duty_type { get; set; }
            public string duty_start_time { get; set; }
            public string duty_end_time { get; set; }
            public string location { get; set; }
            public string program_name { get; set; }
            public string error { get; set; }
            public string message { get; set; }
        }



        public class RootObjectRequest
        {
            public List<Changes> changes { get; set; }
            public string error { get; set; }
            public string message { get; set; }
        }


        public class Changes
        {
            public int user_id { get; set; }
            public int id { get; set; }
            public string request_date { get; set; }
            public string request_start_time { get; set; }
        }



    }
}