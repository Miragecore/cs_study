using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace Vaisala_SurfaceSensor
{
   
    class SurfaceData_ViewModel : INotifyPropertyChanged
    {
        public void Update(SurfaceData data)
        {
            DSC111_Hw_Status = data.DSC111_HwStatus;
            DSC111_Window_Status = data.DSC111_WindowStatus;
            DST_Hw_Status = data.DST_HwStatus;
            SurfaceStatus = data.SurfaceStatus;
            GripStatus = data.GripStatus;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DSC111_HW_STATUS_TYPE       DSC111_Hw_Status 
        { 
            get { return DSC111_Hw_Status; }
            set { OnPropertyUpdate("DSC11_Hw_Status"); }
        }

        public DSC111_WINDOW_STATUS_TYPE   DSC111_Window_Status 
        {
            get { return DSC111_Window_Status; }
            set { OnPropertyUpdate("DSC111_Window_Status"); }
        }
        public DST_HW_STATUS_TYPE DST_Hw_Status { get; set; }
        public SURFACE_STATUS_TYPE SurfaceStatus 
        { 
            get { return SurfaceStatus; } 
            set { OnPropertyUpdate("SurfaceStatus"); }
        }

        public GRIP_STATUS_TYPE GripStatus 
        {
            get { return GripStatus; }
            set { OnPropertyUpdate("GripStatus"); }
        }

        private void OnPropertyUpdate(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
