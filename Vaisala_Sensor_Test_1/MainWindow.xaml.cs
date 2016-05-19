using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

using log4net.Config;
using log4net;

using System.IO.Ports;
using Vaisala_SurfaceSensor;

namespace Vaisala_Sensor_Test
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        SurfaceData_ViewModel surfaceData_ViewModel = null;
        
        VaisalaSurfaceSensor surfaceSensor = null;
        public MainWindow()
        {
            InitializeComponent();

            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("D:\\LogConfig.xml"));            

        }

        void SurfaceData_Received(object sender, DataReceivedEventArgs e)
        {
            surfaceData_ViewModel.Update(e.Data, e.ReceivedMsg);
            Application.Current.Dispatcher.BeginInvoke((Action)(() => { rtbMsgList.AppendText(e.ReceivedMsg + "\n"); }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Parity _parity = Parity.None;
            StopBits _stopBits = StopBits.One;
            switch(cbParity.SelectedIndex)
            {
                case 0:
                    _parity = Parity.None;
                    break;
                case 1:
                    _parity = Parity.Even;
                    break;
                case 2:
                    _parity = Parity.Odd;
                    break;
                case 3:
                    _parity = Parity.Mark;
                    break;
                case 4:
                    _parity = Parity.Space;
                    break;
            }

            switch (cbStopBits.SelectedIndex)
            {
                case 0:
                    _stopBits = StopBits.None;
                    break;
                case 1:
                    _stopBits = StopBits.One;
                    break;
                case 2:
                    _stopBits = StopBits.OnePointFive;
                    break;
                case 3:
                    _stopBits = StopBits.Two;
                    break;
            }

            surfaceSensor.SetComPort(txtPortName.Text, Int16.Parse(txtBaudRate.Text), _parity, Int16.Parse(txtDataBits.Text), _stopBits);
            surfaceSensor.Start();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            surfaceData_ViewModel = new SurfaceData_ViewModel();
            surfaceSensor = new VaisalaSurfaceSensor();

            this.DataContext = surfaceData_ViewModel;
            surfaceSensor.DataReceived += SurfaceData_Received;    
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            surfaceSensor.Stop();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
