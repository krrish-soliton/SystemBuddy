﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SystemBuddy;

namespace SystemBuddyUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //monitorManager.ValueUpdated += OnValueUpdated;
        }

        private void OnValueUpdated(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                //MainLabel.Text = monitorManager.Info;
                
            }));
        }

        //HardwareHandler monitorManager = new HardwareHandler();

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }
    }
}
