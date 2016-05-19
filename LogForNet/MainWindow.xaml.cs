using System;
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

using log4net;
using log4net.Config;

namespace LogForNet
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("D:\\LogConfig.xml"));

            ILog iLog = log4net.LogManager.GetLogger("Logger");
            iLog.Info("LogMessage");


        }
    }
}
