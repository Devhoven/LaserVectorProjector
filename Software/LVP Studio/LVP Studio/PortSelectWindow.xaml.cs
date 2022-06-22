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

            // Got this from https://stackoverflow.com/a/46683622
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                var portList = portnames.Select(n => n + " - " + ports
                                        .FirstOrDefault(s => s.Contains(n)))
                                        .ToList();

                // Filtering out the duplicates
                portList = portList.GroupBy(x => x)
                                   .Select(g => g.First())
                                   .ToList();

                foreach (string s in portList)
                    PortPanel.Children.Add(new ComRecord(s.Substring(0, s.IndexOf(' ')), s.Substring(s.IndexOf(' '))));
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
            Content = PortName + Caption;
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
