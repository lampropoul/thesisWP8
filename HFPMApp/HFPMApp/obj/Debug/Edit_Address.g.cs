﻿#pragma checksum "C:\Users\Basilis\documents\visual studio 2012\Projects\HFPMApp\HFPMApp\Edit_Address.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "867DDC66440C18996898FDF7D6A87EF1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace HFPMApp {
    
    
    public partial class Edit_Address : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ProgressIndicator loadingProgressBar;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock app_title;
        
        internal System.Windows.Controls.TextBlock page_title;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBox edit_nomos;
        
        internal System.Windows.Controls.TextBlock nomos;
        
        internal System.Windows.Controls.TextBox edit_dimos;
        
        internal System.Windows.Controls.TextBlock dimos;
        
        internal System.Windows.Controls.TextBox edit_city;
        
        internal System.Windows.Controls.TextBlock city;
        
        internal System.Windows.Controls.TextBox edit_address;
        
        internal System.Windows.Controls.TextBlock address;
        
        internal System.Windows.Controls.TextBox edit_postal_code;
        
        internal System.Windows.Controls.TextBlock postal_code;
        
        internal System.Windows.Controls.TextBox edit_country;
        
        internal System.Windows.Controls.TextBlock nomos_Copy4;
        
        internal System.Windows.Controls.TextBox edit_area;
        
        internal System.Windows.Controls.TextBlock area;
        
        internal System.Windows.Controls.TextBlock country;
        
        internal System.Windows.Controls.Button send_address_info;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/HFPMApp;component/Edit_Address.xaml", System.UriKind.Relative));
            this.loadingProgressBar = ((Microsoft.Phone.Shell.ProgressIndicator)(this.FindName("loadingProgressBar")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.app_title = ((System.Windows.Controls.TextBlock)(this.FindName("app_title")));
            this.page_title = ((System.Windows.Controls.TextBlock)(this.FindName("page_title")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.edit_nomos = ((System.Windows.Controls.TextBox)(this.FindName("edit_nomos")));
            this.nomos = ((System.Windows.Controls.TextBlock)(this.FindName("nomos")));
            this.edit_dimos = ((System.Windows.Controls.TextBox)(this.FindName("edit_dimos")));
            this.dimos = ((System.Windows.Controls.TextBlock)(this.FindName("dimos")));
            this.edit_city = ((System.Windows.Controls.TextBox)(this.FindName("edit_city")));
            this.city = ((System.Windows.Controls.TextBlock)(this.FindName("city")));
            this.edit_address = ((System.Windows.Controls.TextBox)(this.FindName("edit_address")));
            this.address = ((System.Windows.Controls.TextBlock)(this.FindName("address")));
            this.edit_postal_code = ((System.Windows.Controls.TextBox)(this.FindName("edit_postal_code")));
            this.postal_code = ((System.Windows.Controls.TextBlock)(this.FindName("postal_code")));
            this.edit_country = ((System.Windows.Controls.TextBox)(this.FindName("edit_country")));
            this.nomos_Copy4 = ((System.Windows.Controls.TextBlock)(this.FindName("nomos_Copy4")));
            this.edit_area = ((System.Windows.Controls.TextBox)(this.FindName("edit_area")));
            this.area = ((System.Windows.Controls.TextBlock)(this.FindName("area")));
            this.country = ((System.Windows.Controls.TextBlock)(this.FindName("country")));
            this.send_address_info = ((System.Windows.Controls.Button)(this.FindName("send_address_info")));
        }
    }
}

