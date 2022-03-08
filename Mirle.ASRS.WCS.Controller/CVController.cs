using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mirle.ASRS.Conveyors;
using Mirle.ASRS.Conveyors.View;
using Mirle.MPLC;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.DataBlocks.DeviceRange;
using Mirle.MPLC.MCProtocol;
using Mirle.MPLC.SharedMemory;

namespace Mirle.ASRS.WCS.Controller
{
    public class CVController : IDisposable
    {
        private readonly PLCHost _plcHost;
        private readonly MainView _mainView;
        private readonly Form1 _form;
        private readonly Conveyors.Conveyor _converyor;
        private readonly bool _InMemorySimulator;
       

        public CVController(string ipAddress, int tcpPort, bool InMemorySimulator,int PLCNo)
        {
            if (InMemorySimulator)
            {
                var smWriter = new SMReadWriter();
                var blockInfos = GetBlockInfos(PLCNo);
                foreach (var block in blockInfos)
                {
                    smWriter.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{block.SharedMemoryName}"));
                }
                _converyor = new Conveyors.Conveyor(smWriter,PLCNo);
                _InMemorySimulator = InMemorySimulator;
            }
            else
            {
                var plcHostInfo = new PLCHostInfo("VITALON", ipAddress, tcpPort, GetBlockInfos(PLCNo));
                _plcHost = new PLCHost(plcHostInfo);
                _plcHost.Interval = 200;
                _plcHost.MPLCTimeout = 600;
                _plcHost.EnableWriteRawData = false;
                _plcHost.EnableWriteShareMemory = true;
                //var smReader = new SMReadOnlyCachedReader();
                //var blockInfos = GetBlockInfos();
                //foreach (var block in blockInfos)
                //{
                //    smReader.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{block.SharedMemoryName}"));
                //}
                _converyor = new Conveyors.Conveyor(_plcHost,PLCNo);
                _plcHost.Start();
            }

            foreach (var buffer in _converyor.Buffers)
            {
                buffer.OnIniatlNotice += Buffer_OnIniatlNotice;
            }

            _converyor.Start();
            _mainView = new MainView(_converyor);
        }

        private IEnumerable<BlockInfo> GetBlockInfos(int PLCNo)
        {
            if (PLCNo == 1)
            {
                yield return new BlockInfo(new DDeviceRange("D101", "D600"), "Read", 0);
                yield return new BlockInfo(new DDeviceRange("D3101", "D3590"), "Write", 1);
            }
            else if(PLCNo == 2)
            {
                yield return new BlockInfo(new DDeviceRange("D101", "D365"), "Read", 0);
                yield return new BlockInfo(new DDeviceRange("D3101", "D3360"), "Write", 1);
            }
        }




        private void Buffer_OnIniatlNotice(object sender, BufferEventArgs e)
        {
        }

      

        public Conveyors.Conveyor GetConveryor()
        {
            return _converyor;
        }

        public bool GetConnect()
        {
            if (_InMemorySimulator)
            {
                return true;
            }
            else
            {
                return _plcHost.IsConnected;
            }
        }

        public Form GetMainView()
        {
            return _mainView;
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _converyor.Dispose();
                    _plcHost.Dispose();
                }

                disposedValue = true;
            }
        }

        ~CVController()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
