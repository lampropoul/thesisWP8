﻿#pragma checksum "C:\Users\Basilis\documents\visual studio 2012\Projects\HFPMApp\HFPMApp\Search.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C6261CED8CFE9F8E9184D7CD3110E0F3"
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
    
    
    public partial class Search : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ProgressIndicator loadingProgressBar;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock app_title;
        
        internal System.Windows.Controls.TextBlock page_title;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.StackPanel container;
        
        internal System.Windows.Controls.CheckBox any_date;
        
        internal Microsoft.Phone.Controls.DatePicker pick_date;
        
        internal Microsoft.Phone.Controls.ListPicker list_duties;
        
        internal Microsoft.Phone.Controls.ListPicker list_locations;
        
        internal Microsoft.Phone.Controls.ListPicker list_program_names;
        
        internal System.Windows.Controls.Button search;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/HFPMApp;component/Search.xaml", System.UriKind.Relative));
            this.loadingProgressBar = ((Microsoft.Phone.Shell.ProgressIndicator)(this.FindName("loadingProgressBar")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.app_title = ((System.Windows.Controls.TextBlock)(this.FindName("app_title")));
            this.page_title = ((System.Windows.Controls.TextBlock)(this.FindName("page_title")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.container = ((System.Windows.Controls.StackPanel)(this.FindName("container")));
            this.any_date = ((System.Windows.Controls.CheckBox)(this.FindName("any_date")));
            this.pick_date = ((Microsoft.Phone.Controls.DatePicker)(this.FindName("pick_date")));
            this.list_duties = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("list_duties")));
            this.list_locations = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("list_locations")));
            this.list_program_names = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("list_program_names")));
            this.search = ((System.Windows.Controls.Button)(this.FindName("search")));
        }
    }
}

