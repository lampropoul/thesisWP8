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
            }



            // CLIENTS
            client_up = new WebClient();
            client_up.UploadStringCompleted += client_UploadStringCompleted;




        }






        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            


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
                    jsonObject.request_start_time = timepicked.ValueString;

                    this.json_to_send = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, new JsonSerializerSettings { });


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

                    MessageBox.Show("You are currently offline. The next time you connect to the Internet your request you just did will be sent.");
                    uri = "/Personal_Program.xaml";
                    NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

                }



            }// using


        }





        // function that sends json data to web server
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
                        MessageBox.Show("Request sent to server.");
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

            NavigationService.GoBack();
        }



        private void settings_Click(object sender, EventArgs e)
        {
            uri = "/Settings.xaml";
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