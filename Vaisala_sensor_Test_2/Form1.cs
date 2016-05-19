using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Vaisala_SurfaceSensor;

namespace TestProjects
{
  
    
    public partial class Form1 : Form
    {
        //public static Queue<string> recvMsgBuf;
        VaisalaSurfaceSensor SurfaceSensor = new VaisalaSurfaceSensor();
        SurfaceData_ViewModel ViewModel = new SurfaceData_ViewModel();

        public Form1()
        {
            InitializeComponent();
            
            //
            //recvCharBuf = new char[102-4];
            //recvMsgBuf = new Queue<string>();

            //readThread = new Thread(Read);
            //_continue = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SurfaceSensor.Start();
            
            //System.IO.Ports.SerialPort sp = new System.IO.Ports.SerialPort("COM5", 9600, System.IO.Ports.Parity.None, 8);
            //sp.ReadTimeout = 500;
            //sp.WriteTimeout = 500;

            //sp.Open();            

            //if (sp.IsOpen)
            //{
            //    System.Console.WriteLine("is Opened");

            //    readThread.Start();

            //    Thread.Sleep(100000);  //100sec

            //    System.Console.WriteLine("End");

            //    _continue = false;
            //    readThread.Join();                
            //    sp.Close();
            //}
            //else
            //    System.Console.WriteLine("Can't Open");
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            SurfaceSensor.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = SurfaceSensor.CurrentSurfaceData.SurfaceStatus.ToString();
            
        }

    }
}
