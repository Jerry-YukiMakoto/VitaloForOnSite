using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirle.DataBase;
using Config.Net;
using System.Windows.Forms;
using Mirle.Def.T1JUMAK0;
using Mirle.Def;
using Mirle.ASRS.Conveyors.View;

namespace Mirle.ASRS.WCS.Controller
{
    public class ControllerReader : IDisposable
    {
        
        private static CVController _cvController;
        private static LoggerManager _loggerManager;
        

        public static void FunGetController(clsPlcConfig CVConfig,clsPlcConfig CV_Config2,bool OnlyMonitor,clsDbConfig dbConfig) 
        {
            _loggerManager = new LoggerManager();
            _cvController = new CVController(CVConfig,CV_Config2,OnlyMonitor,dbConfig);
        }

        #region Get_Manager
        public static LoggerManager GetLoggerManager()
        {
            return _loggerManager;
        }

        public static CVController GetCVControllerr()
        {
            return _cvController;
        }
        #endregion Get_Manager

        #region Dispose
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cvController.Dispose();
                    _loggerManager.Dispose();
                }

                disposedValue = true;
            }
        }
        ~ControllerReader()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Dispose
    }
}
