using HFPMApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
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
    public partial class MainPage : PhoneApplicationPage
    {

        public string downloadedText;
        WebClient client;
        string url;
        string uri;

        String server_ip, logged, previous_remember, remember;





        // Constructor
        public MainPage()
        {
            InitializeComponent();

            

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
                using (StreamReader sr2 = new StreamReader("already_logged.txt"))
                {
                    logged = sr2.ReadToEnd();
                    sr2.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("The file could not be read:");
                MessageBox.Show(e.Message);
            }


            try
            {
                using (StreamReader sr3 = new StreamReader("remember.txt"))
                {
                    remember = sr3.ReadToEnd();
                    
                    if (remember != String.Empty)
                    {
                        string[] words = remember.Split(',');
                        username_box.Text = words[0];
                        password_box.Password = words[1];
                        remember_me.IsChecked = true;
                    }
                    sr3.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("The file could not be read:");
                MessageBox.Show(e.Message);
            }

            

            


        }







        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (logged == String.Empty)
            {

                client = new WebClient();
                client.DownloadStringCompleted += client_DownloadStringCompleted;

            }
            else
            {

                string[] words = logged.Split(',');
                PhoneApplicationService.Current.State["Username"] = words[0];
                PhoneApplicationService.Current.State["Language"] = words[1];

                uri = "/MainMenuPage.xaml";
                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

            }

            gr.Checked += gr_Checked;
            gr.Unchecked += gr_Unchecked;
            password_box.GotFocus += password_box_GotFocus;

        }

        void gr_Unchecked(object sender, RoutedEventArgs e)
        {

            app_title.Text = "Health Facilities Personnel Management App";
            page_title.Text = "Login";

            uname.Text = "Username";
            pwd.Text = "Password";

            remember_me.Content = "Remember me";
            login_btn.Content = "Login";

            gr.Content = "Greek";

        }

        void gr_Checked(object sender, RoutedEventArgs e)
        {

            app_title.Text = "Εφαρμογή Διαχείρισης Μονάδων Υγείας";
            page_title.Text = "Είσοδος";

            uname.Text = "Όνομα";
            pwd.Text = "Κωδικός";

            remember_me.Content = "Θυμήσου με";
            login_btn.Content = "Είσοδος";

            gr.Content = "Ελληνικά";

        }



        void password_box_GotFocus(object sender, RoutedEventArgs e)
        {

            if (username_box.Text != String.Empty)
            {

                string[] words = remember.Split(',');
                for (int i = 0; i < words.Length; i++)
                {
                    if (username_box.Text == words[i])
                    {
                        password_box.Password = words[i+1];
                        remember_me.IsChecked = true;
                        break;
                    }
                }

            }

        }








        // login
        private void login_btn_click1(object sender, RoutedEventArgs e)
        {

            if (Convert.ToBoolean(gr.IsChecked)) PhoneApplicationService.Current.State["Language"] = "GR";
            else PhoneApplicationService.Current.State["Language"] = "EN";

            // edw upoti8etai oti 8a ginetai to erwtima stin topiki basi
            string uname = username_box.Text;
            string pass = password_box.Password;
            Random rnd = new Random();
            int rand = rnd.Next(1, 10000);


            if (uname != String.Empty && pass != String.Empty)
            {

                int user_id = -1;

                // check internet connectivity

                bool hasInternet = NetworkInterface.GetIsNetworkAvailable();
                //hasInternet = false;

                // ean exw internet proxwraw kai kanw to REST call
                if (hasInternet)
                {

                    // REST Call
                    url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/user/username/" + uname + "/randomnum/" + rand;
                    client.DownloadStringAsync(new Uri(url));

                }
                // ean den exw internet, kalw thn topiki bash
                else
                {


                    using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                    {

                        // -------------------------------------------------------------------//
                        // -------------------------- LOCAL DATABASE -------------------------//
                        // -------------------------------------------------------------------//

                        db.CreateIfNotExists();
                        db.LogDebug = true;


                        
                        // changes do not take place until SubmitChanges method is called
                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }


                        IEnumerable<Users> query =
                            from user in db.Users
                            //where student.Scores[0] > 90
                            select user;


                        //MessageBox.Show("Query 1: " + query);
                        bool found = false;
                        string password = null;
                        

                        foreach (Users us in query)
                        {
                            
                            //MessageBox.Show(us.Userid + ", " + us.Username + ", " + us.Password + ", " + us.Nameuser + ", " + us.Surnameuser + ", " + us.Amka + ", " + us.Department + ", " + us.Userteam);

                            if (us.Username == uname)
                            {
                                found = true;
                                password = us.Password;
                                user_id = us.Userid;
                            }

                        }


                        if (found)
                        {
                            try
                            {

                                if (password == password_box.Password)
                                {
                                    PhoneApplicationService.Current.State["Username"] = uname;
                                    PhoneApplicationService.Current.State["UserId"] = user_id;

                                    if (Convert.ToBoolean(gr.IsChecked)) PhoneApplicationService.Current.State["Language"] = "GR";
                                    else PhoneApplicationService.Current.State["Language"] = "EN";


                                    using (StreamWriter writer = new StreamWriter("already_logged.txt"))
                                    {
                                        writer.Write(PhoneApplicationService.Current.State["Username"] + "," + PhoneApplicationService.Current.State["Language"]);
                                        writer.Close();
                                    }



                                    if (Convert.ToBoolean(remember_me.IsChecked))
                                    {
                                        using (StreamReader sr3 = new StreamReader("remember.txt"))
                                        {
                                            previous_remember = sr3.ReadToEnd();
                                            sr3.Close();
                                        }
                                        using (StreamWriter writer = new StreamWriter("remember.txt"))
                                        {
                                            string[] words = remember.Split(',');
                                            for (int i = 0; i < words.Length; i++)
                                            {
                                                if (username_box.Text == words[i])
                                                {
                                                    if (words[i + 1] != password_box.Password)
                                                    {
                                                        if (PhoneApplicationService.Current.State["Language"].ToString() == "EN") MessageBox.Show("Stored password for this user changed.");
                                                        else MessageBox.Show("Ο αποθηκευμένος κωδικός για αυτόν τον χρήστη άλλαξε.");
                                                        words[i + 1] = password_box.Password;
                                                    }
                                                    break;
                                                }
                                            }
                                            writer.Write(PhoneApplicationService.Current.State["Username"] + "," + password_box.Password + "," + previous_remember);
                                            writer.Close();
                                        }
                                    }



                                    uri = "/MainMenuPage.xaml";
                                    NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                                }
                                else
                                {
                                    if (Convert.ToBoolean(gr.IsChecked)) MessageBox.Show("Λάθος κωδικός. Προσπαθήστε ξανά.");
                                    else MessageBox.Show("Wrong password. Please try again.");

                                    password_box.Focus();
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        else
                        {
                            
                            if (Convert.ToBoolean(gr.IsChecked)) MessageBox.Show("Δεν υπάρχει αυτός ο χρήστης. Προσπαθήστε ξανά.");
                            else MessageBox.Show("No such user. Try again.");

                            username_box.Focus();
                        }


                        
                    }

                } // end internet if

            }
            else if (uname == String.Empty && pass != String.Empty)
            {

                if (Convert.ToBoolean(gr.IsChecked))
                    MessageBox.Show("Παρακαλώ εισάγετε όνομα χρήστη.");
                else
                    MessageBox.Show("Please specify username.");
                
                username_box.Focus();
            }
            else if (pass == String.Empty && uname != String.Empty)
            {
                if (Convert.ToBoolean(gr.IsChecked))
                    MessageBox.Show("Παρακαλώ εισάγετε κωδικό.");
                else
                    MessageBox.Show("Please specify password.");

                password_box.Focus();
            }
            else
            {
                if (Convert.ToBoolean(gr.IsChecked))
                    MessageBox.Show("Παρακαλώ συμπληρώστε όλα τα πεδία.");
                else
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
                int user_team = jsonObject.user_team;
                string name_user = jsonObject.name_user;
                string surname_user = jsonObject.surname_user;
                string username = jsonObject.username;
                string password = jsonObject.password;
                string email = jsonObject.email;
                string amka = jsonObject.amka;
                string status = jsonObject.status;
                string department = jsonObject.department;
                string error = jsonObject.error;

                try
                {

                    if (password == password_box.Password)
                    {
                        PhoneApplicationService.Current.State["Username"] = username;
                        PhoneApplicationService.Current.State["UserId"] = id;

                        if (Convert.ToBoolean(gr.IsChecked)) PhoneApplicationService.Current.State["Language"] = "GR";
                        else PhoneApplicationService.Current.State["Language"] = "EN";




                        using (StreamWriter writer = new StreamWriter("already_logged.txt"))
                        {
                            writer.Write(PhoneApplicationService.Current.State["Username"] + "," + PhoneApplicationService.Current.State["Language"]);
                            writer.Close();
                        }


                        if (Convert.ToBoolean(remember_me.IsChecked))
                        {
                            using (StreamReader sr3 = new StreamReader("remember.txt"))
                            {
                                previous_remember = sr3.ReadToEnd();
                                sr3.Close();
                            }
                            using (StreamWriter writer = new StreamWriter("remember.txt"))
                            {
                                string[] words = remember.Split(',');
                                for (int i = 0; i < words.Length; i++)
                                {
                                    if (username_box.Text == words[i])
                                    {
                                        if (words[i + 1] != password_box.Password)
                                        {
                                            if (PhoneApplicationService.Current.State["Language"].ToString() == "EN") MessageBox.Show("Stored password for this user changed.");
                                            else MessageBox.Show("Ο αποθηκευμένος κωδικός για αυτόν τον χρήστη άλλαξε.");
                                            words[i + 1] = password_box.Password;
                                        }
                                        break;
                                    }
                                }
                                writer.Write(PhoneApplicationService.Current.State["Username"] + "," + password_box.Password + "," + previous_remember);
                                writer.Close();
                            }
                        }


                        // logged via internet. now delete and insert to local db
                        using (HospitalContext db = new HospitalContext(HospitalContext.ConnectionString))
                        {

                            // -------------------------------------------------------------------//
                            // -------------------------- LOCAL DATABASE -------------------------//
                            // -------------------------------------------------------------------//

                            db.CreateIfNotExists();
                            db.LogDebug = true;


                            // retreive user id from table users

                            IEnumerable<Users> query =
                                        from user in db.Users
                                        where user.Username == PhoneApplicationService.Current.State["Username"].ToString()
                                        select user;

                            // delete
                            foreach (Users us in query)
                            {
                                db.Users.DeleteOnSubmit(us);
                            }



                            db.Users.InsertOnSubmit(new Users
                            {
                                Userid = id,
                                Username = username,
                                Password = password,
                                Nameuser = name_user,
                                Surnameuser = surname_user,
                                Amka = amka,
                                Department = department,
                                Userteam = user_team
                            });



                            // changes do not take place until SubmitChanges method is called
                            try
                            {
                                db.SubmitChanges();
                                uri = "/MainMenuPage.xaml";
                                NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }





                        }


                        
                    }
                    else
                    {
                        if (Convert.ToBoolean(gr.IsChecked)) MessageBox.Show("Λάθος κωδικός. Προσπαθήστε ξανά.");
                        else MessageBox.Show("Wrong password. Please try again.");

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
                if (Convert.ToBoolean(gr.IsChecked)) MessageBox.Show("Δεν υπάρχει αυτός ο χρήστης. Προσπαθήστε ξανά.");
                else MessageBox.Show("No such user. Please try again.");

                username_box.Focus();
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                MessageBox.Show("WebException: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }
        }



        // the class which contains the properties of the specific JSON response
        public class RootObject
        {
            public int id { get; set; }
            public int user_team { get; set; }
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


}