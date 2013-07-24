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
        public string downloadedText;
        WebClient client;
        WebClient client_up;
        string url;
        string url_post;
        string uri;
        string json_to_send = null;
        String server_ip;
        string given_username;
        

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

            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem2.Text = "Εκκαθάριση περασμένων καθηκόντων";
            else menuItem2.Text = "Clear old entries";
            ApplicationBar.MenuItems.Add(menuItem2);

            ApplicationBarMenuItem menuItem3 = new ApplicationBarMenuItem();
            if (PhoneApplicationService.Current.State["Language"].ToString() == "GR") menuItem3.Text = "Ρυθμίσεις για τα αιτήματα.";
            else menuItem3.Text = "Settings for declared";
            ApplicationBar.MenuItems.Add(menuItem3);
            menuItem3.Click += new EventHandler(settings_Click);


            // CLIENTS
            client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;

            //client_up = new WebClient();
            //client_up.UploadStringCompleted += client_UploadStringCompleted;


        }

        

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            
            // REST CALL

            // pairnw ta parameters (username kai password) apo tin MainPage.xaml
            given_username = PhoneApplicationService.Current.State["Username"].ToString();


            Random rnd = new Random();
            int rand = rnd.Next(1, 1000);

            url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/program/username/" + given_username + "/randomnum/" + rand;
            client.DownloadStringAsync(new Uri(url));



            //If we return to this page after Tomstoning, then get the previous state from
            // PhoneApplicationService
            if (PhoneApplicationService.Current.State.ContainsKey(CurrentEntryDateKey))
                _entryDate = (DateTime)PhoneApplicationService.Current.State[CurrentEntryDateKey];

            InitializeCalendar(_entryDate.Value);

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
                    border.Margin = new Thickness(0, 0, 5, 5);
                    border.Width = 99;
                    border.Height = 99;
                    border.CornerRadius = new CornerRadius(20);

                    Button btn = new Button();
                    btn.Name = "Day" + i;
                    btn.Content = i.ToString();
                    btn.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    btn.Width = 99;
                    btn.Height = 99;
                    btn.FontSize = 32;
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
            if (!_dummyRepository.ContainsKey(selectedDate.Date))
                _dummyRepository.Add(selectedDate.Date, "data");
            
            //NOTE: In real scenarios in OnButtonClick we would launch a new Page to accept data for the selected
            // date. And when we return back from the data entry page to this page, the "OnNavigatedTo()" method
            // will get called. And in "OnNavigatedTo() method we are calling InitializeCalendar() which takes care of redrawing the
            // calendar.
            // Since this just a sample app...I'm calling InitializeCalendar() in "OnButtonClick" to refresh the view.
            // If you are launching a page OnButtonClick, you don't have to call InitializeCalendar() here.

            MessageBox.Show(selectedDate.ToString());

            InitializeCalendar(_entryDate.Value);
        }






        // function that retreives json data from web server
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {

                this.downloadedText = e.Result;

                // decode JSON
                RootObject jsonObject = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);


                int length = jsonObject.programs.Count;

                for (int i = 0; i < length; i++)
                {
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
                        _dummyRepository.Add(selectedDate.Date, "data");

                    //MessageBox.Show(selectedDate + "------" + words[0] + "-" + words[1] + "-" + words[2]);

                    InitializeCalendar(selectedDate);

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

            NavigationService.GoBack();
        }



        private void settings_Click(object sender, EventArgs e)
        {
            uri = "/Settings.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }






        // the classes which contain the properties of the specific JSON response
        public class RootObject
        {
            public List<Programs> programs { get; set; }
        }


        public class Programs
        {
            public string date { get; set; }
            public string duty_type { get; set; }
            public string duty_start_time { get; set; }
            public string duty_end_time { get; set; }
            public string location { get; set; }
            public string program_name { get; set; }
            public string error { get; set; }
            public string message { get; set; }
        }





    }
}