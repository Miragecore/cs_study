using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

using log4net;


namespace Vaisala_SurfaceSensor
{
   
    class SurfaceData_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Update(SurfaceData data, string _msg)
        {
            DSC111_Hw_Status = data.DSC111_HwStatus;
            DSC111_Window_Status = data.DSC111_WindowStatus;
            DST_Hw_Status = data.DST_HwStatus;
            SurfaceStatus = data.SurfaceStatus;
            GripStatus = data.GripStatus;
            ReceivedMsg = _msg;
        }

        protected string _ReceivedMsg;

        public string ReceivedMsg
        {
            get { return _ReceivedMsg; }
            set { _ReceivedMsg = value; OnPropertyUpdate("ReceivedMsg"); }
        }

        protected DSC111_HW_STATUS_TYPE _DSC111_Hw_Status;
        public DSC111_HW_STATUS_TYPE       DSC111_Hw_Status 
        {
            get { return _DSC111_Hw_Status; }
            set { _DSC111_Hw_Status = value; OnPropertyUpdate("DSC111_Hw_Status"); }
        }

        protected DSC111_WINDOW_STATUS_TYPE _DSC111_Window_Status;
        public DSC111_WINDOW_STATUS_TYPE   DSC111_Window_Status 
        {
            get { return _DSC111_Window_Status; }
            set { _DSC111_Window_Status = value;  OnPropertyUpdate("DSC111_Window_Status"); }
        }

        protected DST_HW_STATUS_TYPE _DST_Hw_Status;
        public DST_HW_STATUS_TYPE DST_Hw_Status 
        { 
            get {return _DST_Hw_Status;}
            set { _DST_Hw_Status = value; OnPropertyUpdate("DST_Hw_Status");} 
        }

        protected SURFACE_STATUS_TYPE _SurfaceStatus;
        public SURFACE_STATUS_TYPE SurfaceStatus 
        {
            get { return _SurfaceStatus; }
            set { _SurfaceStatus = value;  OnPropertyUpdate("SurfaceStatus"); }
        }

        protected GRIP_STATUS_TYPE _GripStatus;
        public GRIP_STATUS_TYPE GripStatus 
        {
            get { return _GripStatus; }
            set { _GripStatus = value;OnPropertyUpdate("GripStatus"); }
        }

        private void OnPropertyUpdate(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
