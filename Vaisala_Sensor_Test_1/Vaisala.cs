using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.IO.Ports;

using log4net;

namespace Vaisala_SurfaceSensor
{

    public enum GRIP_STATUS_TYPE
    {
        GRIP_OK = 0,
        GRIP_WARNING = 1,
        GRIP_ALARM = 2
    }
    public enum SURFACE_STATUS_TYPE
    {
        SENSOR_ERROR = 0,
        DRY = 1,
        MOIST = 2,
        WET = 3,
        FROSTY1 = 5,
        SNOWY = 6,
        ICY = 7,
        SLUSHY = 8
    }

    public enum DSC111_WINDOW_STATUS_TYPE
    {
        CLEAR = 0,
        CONTAMINATED = 1,
        HEAVY_CONTAMINATED = 2
    }

    public enum DSC111_HW_STATUS_TYPE
    {
        OK = 0,
        CPU_WARNING = 1,
        TRANSMITTER_WARNING = 2,
        DATA_MISSING = 3,  //Data temporarily missing due to excessive ambient light
        LOW_VISIBILITY = 4  //Road surface data uncertain due to low visibility            
    }
    public enum DST_HW_STATUS_TYPE
    {
        OK = 0,
        CPU_WARNING = 1,
        DETECTOR_WARNING = 2,
        MEASUREMENT_FAIL = 3 //Humidity and air temperature measurement failure
    }

    //TODO : SingleTon으로 구현
    public class SurfaceData
    {
        public int ID { get; set; }
        public DateTime time;
        public float AirTemperature { get; set; }
        public float RelativeHumidity { get; set; }
        public float DewPoint { get; set; }
        public int Visibility { get; set; }
        public float InputVoltage { get; set; }
        public float SurfaceTemperature { get; set; }
        public int DST_StatusCode
        {
            get { return DST_StatusCode; }
            set { UpdateDSTHWStatus(value); }
        }
        public uint SurfaceStatusWRS
        {
            get { return SurfaceStatusWRS;}
            set { UpdateSurfaceStatus(value); } 
        }
        public float LevelOfGrip { get; set; }
        public int DSC_StatusCode 
        {
            get { return DSC_StatusCode; }

            set
            {
                UpdateDSC11WindowStatus(value);
                UpdateDSC111HwStatus(value);
            }
        }
        public float AmoutOfWater { get; set; }
        public float AmoutOfIce { get; set; }
        public float AmountOfSnow { get; set; }

        public GRIP_STATUS_TYPE GripStatus { get; set; }         
        public SURFACE_STATUS_TYPE SurfaceStatus { get; set; }

        public DSC111_WINDOW_STATUS_TYPE    DSC111_WindowStatus { get; set; }
        public DSC111_HW_STATUS_TYPE DSC111_HwStatus { get; set; }

        public DST_HW_STATUS_TYPE DST_HwStatus { get; set; }
        protected void UpdateDSTHWStatus(int _statusCode)
        {
            switch(_statusCode)
            {
                case 0:
                    DST_HwStatus = DST_HW_STATUS_TYPE.OK;
                    break;
                case 1:
                    DST_HwStatus = DST_HW_STATUS_TYPE.CPU_WARNING;
                    break;
                case 2:
                    DST_HwStatus = DST_HW_STATUS_TYPE.DETECTOR_WARNING;
                    break;
                case 3:
                    DST_HwStatus = DST_HW_STATUS_TYPE.MEASUREMENT_FAIL;
                    break;
            }
        }
        protected void UpdateDSC11WindowStatus(int _statusCode)
        {
            int windowstatusCode = _statusCode / 10;

            if (windowstatusCode == 0)
                DSC111_WindowStatus = DSC111_WINDOW_STATUS_TYPE.CLEAR;
            else if (windowstatusCode == 1)
                DSC111_WindowStatus = DSC111_WINDOW_STATUS_TYPE.CONTAMINATED;
            else if (windowstatusCode == 2)
                DSC111_WindowStatus = DSC111_WINDOW_STATUS_TYPE.HEAVY_CONTAMINATED;

        }

        protected void UpdateDSC111HwStatus(int _statusCode)
        {
            int HwStatus = _statusCode % 10;

            switch(HwStatus)
            {
                case 0:
                    DSC111_HwStatus = DSC111_HW_STATUS_TYPE.OK;
                    break;
                case 1:
                    DSC111_HwStatus = DSC111_HW_STATUS_TYPE.CPU_WARNING;
                    break;
                case 2:
                    DSC111_HwStatus = DSC111_HW_STATUS_TYPE.TRANSMITTER_WARNING;
                    break;
                case 3:
                    DSC111_HwStatus = DSC111_HW_STATUS_TYPE.DATA_MISSING;
                    break;
                case 4:
                    DSC111_HwStatus = DSC111_HW_STATUS_TYPE.LOW_VISIBILITY;
                    break;
            }
        }

        protected void UpdateSurfaceStatus(uint SurfaceStatusWRS)
        {
            uint gripCode = SurfaceStatusWRS / 100;
            uint surfaceCode = SurfaceStatusWRS % 10;

            if (gripCode == 0)
            {
                //none
                GripStatus = GRIP_STATUS_TYPE.GRIP_OK;
                System.Console.WriteLine("Grip OK");
            }
            else if (gripCode == 1)
            {
                //1       Grip warning = surface slightly slippery                                
                GripStatus = GRIP_STATUS_TYPE.GRIP_WARNING;
                System.Console.WriteLine("Grip Warning");
            }
            else if (gripCode == 2)
            {
                //2       Grip alarm = surface slippery
                GripStatus = GRIP_STATUS_TYPE.GRIP_ALARM;
                System.Console.WriteLine("Grip Alarm");
            }
        
            switch (surfaceCode)
            {
                case 0:
                    //Error
                    SurfaceStatus = SURFACE_STATUS_TYPE.SENSOR_ERROR;
                    break;
                case 1:
                    //Dry
                    SurfaceStatus = SURFACE_STATUS_TYPE.DRY;
                    break;
                case 2:
                    //Moist
                    SurfaceStatus = SURFACE_STATUS_TYPE.MOIST;
                    break;
                case 3:
                    //Wet
                    SurfaceStatus = SURFACE_STATUS_TYPE.WET;
                    break;
                case 4:
                    //(reserved)
                    break;
                case 5:
                    //Forsty1
                    SurfaceStatus = SURFACE_STATUS_TYPE.FROSTY1;
                    break;
                case 6:
                    //Snowy
                    SurfaceStatus = SURFACE_STATUS_TYPE.SNOWY;
                    break;
                case 7:
                    //icy
                    SurfaceStatus = SURFACE_STATUS_TYPE.ICY;
                    break;
                case 8:
                    //reserved
                    break;
                case 9:
                    //Slushy       
                    SurfaceStatus = SURFACE_STATUS_TYPE.SLUSHY;
                    break;
            }
        }
    };

    public class VaisalaSurfaceSensor
    {

        public event EventHandler<DataReceivedEventArgs> DataReceived = null;

        public SurfaceData CurrentSurfaceData { get; set; }
        public string _PortName { get; set; }
        public int _BaudRate { get; set; }
        public Parity _Parity { get; set; }
        public int _DataBits { get; set; }
        public StopBits _StopBits { get; set; }

        SerialPort sp;
        Thread readThread;
        bool _continue;
        string readBuf;

        string[] testMsgs = {"01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 00;66 1;68 0.15;71 00;72 0.00;73 0.05;74 0.35;\n",
                            "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 01;66 2;68 0.15;71 01;72 0.00;73 0.05;74 0.35;\n",
                            "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 02;66 3;68 0.15;71 02;72 0.00;73 0.05;74 0.35;\n",
                            "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 03;66 4;68 0.15;71 13;72 0.00;73 0.05;74 0.35;\n",
                            "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 01;66 5;68 0.15;71 14;72 0.00;73 0.05;74 0.35;\n",
                            "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 02;66 6;68 0.15;71 21;72 0.00;73 0.05;74 0.35;\n",
                            "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 03;66 7;68 0.15;71 22;72 0.00;73 0.05;74 0.35;\n",
                            "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 01;66 8;68 0.15;71 13;72 0.00;73 0.05;74 0.35;\n",
                            "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 02;66 9;68 0.15;71 14;72 0.00;73 0.05;74 0.35;\n"};
        int testCount=0;

        Queue<string> msgQueue;

        ILog logger;

        int Count { get; set; }

        public VaisalaSurfaceSensor()
        {
            CurrentSurfaceData = new SurfaceData();
            readBuf = "";
            readThread = null;
            msgQueue = new Queue<string>();

            _PortName = "COM5";
            _BaudRate = 9600;
            _Parity = Parity.None;
            _DataBits = 8;
            _StopBits = StopBits.One;

            logger = log4net.LogManager.GetLogger("Logger");
        }

        public void Start()
        {
            Count = 0;

            if (readThread != null)
            {
                _continue = false;
                readThread.Join();
            }

            _continue = true;
            readThread = new Thread(Read);

            readThread.Start();
        }

        public void Stop()
        {
            _continue = false;
            readThread.Join();
            Console.WriteLine("Join");
        }
        public void SetComPort(string _portName, int _baudRate, Parity _parity, int _dataBis, StopBits _stopBits)
        {
            _PortName = _portName;
            _BaudRate = _baudRate;
            _Parity = _parity;
            _DataBits = _dataBis;
            _StopBits = _stopBits;
            
        }
        protected void Read()
        {
            if (sp != null)
            {
                sp.Close();
                sp.Dispose();
            }

            sp = new SerialPort(_PortName, _BaudRate, _Parity, _DataBits, _StopBits);

            Thread _readThread = new Thread(ReadMsg);
            _readThread.Start();

            if (sp.IsOpen)
            {
                sp.WriteLine("@1 AMES 16 1");

                while (_continue)
                {   
                    Thread.Sleep(100);
                    string szRead = sp.ReadExisting();
                    lock (readBuf)
                    {
                        readBuf += szRead;
                        readBuf = ReceiveTokenize(readBuf);
                    }
                    //Parse(tBuf);
                    Thread.Sleep(1000);
                }
            }
            else
            {
                //Thread _readThread = new Thread(Read);
                while (_continue)
                {
                    lock (readBuf)
                    {
                        
                        {
                            //test Message 16
                            //2005-06-08 14:05,01,M16,DSC
                            //01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 00;66 206;68 0.15;71 00;72 0.00;73 0.05;74 0.35;
                            readBuf += "2005-06-08 14:05,01,M16,DSC\n";
                            //readBuf += "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 00;66 206;68 0.15;71 00;72 0.00;73 0.05;74 0.35;\n";
                            //readBuf += "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 00;66 1;68 0.15;71 00;72 0.00;73 0.05;74 0.35;\n";
                            readBuf += testMsgs[testCount%9];
                            testCount++;
                            //readBuf += "01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 00;66 106;68 0.15;71 00;72 0.00;73 0.05;74 0.35;\n";
                            //readBuf += string.Format("TestCount - {0}\n Header - ", Count++);
                        }
                        readBuf = ReceiveTokenize(readBuf);
                    }
                    //Parse(tBuf);
                    //Console.WriteLine("{0}", Count++);
                    Thread.Sleep(5000);
                }

            }
            _readThread.Join();
            Console.WriteLine("Comm end");
        }
        protected string ReceiveTokenize(string buf)
        {
            //char[] delimiterChars = { '\n',' ', ',', '[', ']', ':' };
            char[] delimiterChars = { '\n' };
            string[] tokens = buf.Split(delimiterChars);
            lock (msgQueue)
            {
                //foreach(string token in tokens)
                //    msgQue.Enqueue(token);
                for (int i = 0; i < tokens.Length - 1; i++)
                    msgQueue.Enqueue(tokens[i]);
            }
            return tokens[tokens.Length - 1];
        }
       
        protected void MsgParse(string msg)
        {
            char[] delimiterChars = { ' ', ',', ';' };
            string[] tokens = msg.Split(delimiterChars);

            if (tokens.Length < 1)
                return;

            if (tokens.Length == 1)
                CurrentSurfaceData.ID = Int16.Parse(tokens[0]);
            else if (tokens[4].CompareTo("DSC") == 0)
            {
                //01
                //2005-06-08 14:05,01,M16,DSC
                //01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 00;66 206;68 0.15;71 00;72 0.00;73 0.05;74 0.35;
                DateTime.TryParse(tokens[0] + " " + tokens[1], out CurrentSurfaceData.time);
                CurrentSurfaceData.ID = Int16.Parse(tokens[2]);
            }
            else
            {
                if (tokens.Length > 25)
                {
                    int offset = 1;
                    CurrentSurfaceData.AirTemperature       = float.Parse(tokens[offset]);
                    CurrentSurfaceData.RelativeHumidity     = float.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.DewPoint             = float.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.Visibility           = Int16.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.InputVoltage         = float.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.SurfaceTemperature   = float.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.DST_StatusCode       = Int16.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.SurfaceStatusWRS     = uint.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.LevelOfGrip          = float.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.DSC_StatusCode       = Int16.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.AmoutOfWater         = float.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.AmoutOfIce           = float.Parse(tokens[offset += 2]);
                    CurrentSurfaceData.AmountOfSnow         = float.Parse(tokens[offset += 2]);
                }
            }            
        }

        protected void ReadMsg()
        {
            while (_continue)
            {
                string szReadMsg;
                lock (msgQueue)
                {
                    while (msgQueue.Count > 0)
                    {
                        //2005-06-08 14:05,01,M16,DSC
                        //01 -1.5;02 20;03 -3.0;11 2000;14 23.8;60 -2.0;61 00;66 206;68 0.15;71 00;72 0.00;73 0.05;74 0.35;

                        /** Contents of Message 16
                         *                        
                         * Source   Parameter Number    Parameter Name          Sid         Range, Resolution and Unit                        
                         * DSC111   01                  Air temperature         T1          -40.0 ... +60.0 °C
                         *          02                  Relative humidity       RH1         000.0 ... 100.0 %
                         *          03                  Dew point               TD1         -40.0 ... +60.0 °C
                         *          11                  Visibility              VI          0 ... 2000 m\
                         *          14                  Input voltage           BT1         9.0 ... 30.0 V
                         *          60                  Surface temperature     TS3         -40.0 ... +60.0 °C
                         *          61                  DST111 hardware status  HTS         00 ... 99
                         *          66                  Surface status          ST3         WRS
                         *          68                  Level of grip           GR3         0.00 ... 1.00
                         *          71                  DSC111 hardware status  HCS         00 ... 99
                         *          72                  Amount of water         WL3         00.00 ... 99.00 mm
                         *          73                  Amount of ice           IL3         00.00 ... 99.00 mm
                         *          74                  Amount of snow          SL3         00.00 ... 99.00 mm
                         * */

                        /** ROSA-Compatible Surface Status WRS Code
                         *  W       Warning                                     R           Rain        S   Surface state
                         *  Sp      No warning                                  Sp or 0     Not used    0   Error
                         *  1       Grip warning = surface slightly slippery                            1   Dry
                         *  2       Grip alarm = surface slippery                                       2   Moist
                         *                                                                              3   Wet
                         *                                                                              4   (Reserved)
                         *                                                                              5   Frosty1
                         *                                                                              6   Snowy
                         *                                                                              7   Icy
                         *                                                                              8   (Reserved)
                         *                                                                              9   Slushy                                                                                                     
                         * */

                        /*For example, according to Table 8, if WRS = 206,
                         * there is a grip alarm (W=2) and the surface state is snowy (S=6). 
                         * If WRS = 1, then the surface state is dry (S=1). If there is no warning, 
                         * the WRS code only consists of one character that is the surface state.
                         * */

                        szReadMsg = msgQueue.Dequeue();
                        MsgParse(szReadMsg);
                        if (DataReceived != null)
                            DataReceived(this, new DataReceivedEventArgs(CurrentSurfaceData, szReadMsg));
                        //Console.WriteLine(szReadMsg + string.Format(" -- Queue_Count: {0}", msgQueue.Count));
                        
                        logger.Info(szReadMsg); ;
                    }
                    //Make Event szReadMsg
                }
                Thread.Sleep(1000);
            }
            Console.WriteLine("Read End");
            //while (_continue)
            //{
            //    try
            //    {
            //        string message = sp.ReadLine();

            //        Console.WriteLine(message);
            //        Console.WriteLine(".");
            //    }
            //    catch (TimeoutException) { }
            //}
        }
    }

    public class DataReceivedEventArgs : EventArgs
    {
        public SurfaceData Data { get; set; }
        public string ReceivedMsg { get; set; }


        public DataReceivedEventArgs(SurfaceData _data, string _msg)
        {
            Data = _data;
            ReceivedMsg = _msg;
            
        }
    }
}
