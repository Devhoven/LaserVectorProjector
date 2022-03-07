using ProjectorInterface.GalvoInterface;
using ProjectorInterface.Helpler;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectorInterface
{
    public partial class PortSelectWindow : Window
    {
        public PortSelectWindow(Window owner)
        {
            InitializeComponent();

            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Got this from https://stackoverflow.com/a/2876126/9241163
            // Retreives all of the port names and their caption and appends them to the PortPanel
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
            {
                string[] portNames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                var captions = (from n in portNames
                                join p in ports on n equals p["DeviceID"].ToString()
                                select p["Caption"]).ToList();
                for (int i = 0; i < portNames.Length; i++)
                    PortPanel.Children.Add(new ComRecord(portNames[i], (string)captions[i]));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.C)
                Close();
        }
    }

    class ComRecord : Button
    {
        string PortName;
        string Caption;

        public ComRecord(string portName, string caption)
        {
            PortName = portName;
            Caption = caption;

            BorderBrush = PortName == SerialManager.PortName ? Brushes.LightBlue : Brushes.Black;
            Content = PortName + " - " + Caption;
            FontSize = 20;
            HorizontalContentAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(0, 5, 0, 5);
            Background = (Brush)Application.Current.Resources["ForegroundColor"];
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            foreach (ComRecord record in ((Panel)Parent).Children)
                record.BorderBrush = Brushes.Black;
            BorderBrush = Brushes.LightBlue;

            SerialManager.Initialize(PortName);
            RegistryManager.SetValue("PortName", PortName);
        }
    }
}
