﻿#pragma checksum "C:\Users\Basilis\documents\visual studio 2012\Projects\HFPMApp\HFPMApp\Settings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9B9A0F9CABDE8076B13A0FFFEB170217"
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
    
    
    public partial class Settings : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock title;
        
        internal System.Windows.Controls.TextBlock settings;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.ListPicker types;
        
        internal Microsoft.Phone.Controls.ListPickerItem list_all;
        
        internal Microsoft.Phone.Controls.ListPickerItem list_call;
        
        internal Microsoft.Phone.Controls.ListPickerItem list_shift;
        
        internal Microsoft.Phone.Controls.ListPickerItem list_everyday;
        
        internal Microsoft.Phone.Controls.ListPicker departments;
        
        internal Microsoft.Phone.Controls.ListPickerItem list_swtiria;
        
        internal Microsoft.Phone.Controls.ListPickerItem list_ges;
        
        internal System.Windows.Controls.Button submit_button;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/HFPMApp;component/Settings.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.title = ((System.Windows.Controls.TextBlock)(this.FindName("title")));
            this.settings = ((System.Windows.Controls.TextBlock)(this.FindName("settings")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.types = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("types")));
            this.list_all = ((Microsoft.Phone.Controls.ListPickerItem)(this.FindName("list_all")));
            this.list_call = ((Microsoft.Phone.Controls.ListPickerItem)(this.FindName("list_call")));
            this.list_shift = ((Microsoft.Phone.Controls.ListPickerItem)(this.FindName("list_shift")));
            this.list_everyday = ((Microsoft.Phone.Controls.ListPickerItem)(this.FindName("list_everyday")));
            this.departments = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("departments")));
            this.list_swtiria = ((Microsoft.Phone.Controls.ListPickerItem)(this.FindName("list_swtiria")));
            this.list_ges = ((Microsoft.Phone.Controls.ListPickerItem)(this.FindName("list_ges")));
            this.submit_button = ((System.Windows.Controls.Button)(this.FindName("submit_button")));
        }
    }
}

