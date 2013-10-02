//using HFPMApp;
//using HFPMApp.Scheduler;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;

namespace HFMPApp.Scheduler
{
    public class ScheduledAgent : ScheduledTaskAgent
    {

        String downloadedText;
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background

            //ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(10));






            
            String server_ip = String.Empty;
            /*
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

            MessageBox.Show(server_ip);

            String logged = String.Empty;
            try
            {
                using (StreamReader sr2 = new StreamReader("already_logged.txt"))
                {
                    logged = sr2.ReadToEnd();
                    sr2.Close();
                }
            }
            catch (FileNotFoundException fnf)
            {
                MessageBox.Show(fnf.Message);
            }
            catch (UnauthorizedAccessException ua)
            {
                MessageBox.Show(ua.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("The file could not be read:");
                MessageBox.Show(e.Message);
            }


            
            String[] words = logged.Split(',');
            PhoneApplicationService.Current.State["Username"] = words[0];
            */
            
            String username = String.Empty;
            //username = PhoneApplicationService.Current.State["Username"].ToString();
            Random rnd = new Random();
            int rand = rnd.Next(1, 10000);


            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;

            // REST Call
            server_ip = "192.168.1.5";
            username = "q";
            String url = "http://" + server_ip + "/HFPM_Server_CI/index.php/restful/api/notificationsnodelete/username/" + username + "/randomnum/" + rand;
            MessageBox.Show(url);
            client.DownloadStringAsync(new Uri(url));









            String toastMessage = "";

            // If your application uses both PeriodicTask and ResourceIntensiveTask
            // you can branch your application code here. Otherwise, you don't need to.
            if (task is PeriodicTask)
            {
                // Execute periodic task actions here.
                toastMessage = "Periodic task running.";
            }
            else
            {
                // Execute resource-intensive task actions here.
                toastMessage = "Resource-intensive task running.";
            }

            

            // If debugging is enabled, launch the agent again in one minute.
            #if DEBUG_AGENT
              ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(10));
            #endif

            NotifyComplete();
        }






        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {


                this.downloadedText = e.Result;
                MessageBox.Show(downloadedText);
                // decode JSON
                RootObject jsonObject = JsonConvert.DeserializeObject<RootObject>(this.downloadedText);


                if (jsonObject.error == "")
                {

                    // Launch a toast to show that the agent is running.
                    // The toast will not be shown if the foreground application is running.
                    ShellToast toast = new ShellToast();
                    toast.Title = "HFPM";
                    toast.Content = jsonObject.description[0];
                    toast.Show();

                }

            }
            catch (TargetInvocationException ex)
            {
                System.Diagnostics.Debug.WriteLine("TargetInvocationException: " + ex.Message);
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("WebException: " + ex.Message);
            }


            

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





    }
}