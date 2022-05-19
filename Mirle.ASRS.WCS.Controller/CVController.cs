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
using Mirle.Def;
using Mirle.DataBase;
using Mirle.MPLC.DataBase;

namespace Mirle.ASRS.WCS.Controller
{
    public class CVController : IDisposable
    {
        private readonly PLCHost _plcHost;
        private readonly PLCHost _plcHost2;
        private readonly MainView _mainView;
        private readonly Conveyors.Conveyor _converyor;
        private readonly Conveyors.Conveyor _converyor2;
        private readonly bool _InMemorySimulator;
        private readonly bool _OnlyMonitor;
        private Dictionary<string, string> _CVCalarm = new Dictionary<string, string>();



        public CVController(clsPlcConfig CVConfig, clsPlcConfig CV_Config2,bool OnlyMonitor, clsDbConfig dbConfig)
        {
            if (CVConfig.InMemorySimulator)
            {
                var smWriter = new SMReadWriter();
                var blockInfos = GetBlockInfos(CVConfig.MPLCNo);
                foreach (var block in blockInfos)
                {
                    smWriter.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{block.SharedMemoryName}"));
                }
                _converyor = new Conveyors.Conveyor(smWriter,CVConfig.MPLCNo);
                _InMemorySimulator = CVConfig.InMemorySimulator;

                var smWriter1 = new SMReadWriter();
                var blockInfos1 = GetBlockInfos(CV_Config2.MPLCNo);
                foreach (var block in blockInfos1)
                {
                    smWriter.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{block.SharedMemoryName}"));
                }
                _converyor2 = new Conveyors.Conveyor(smWriter, CV_Config2.MPLCNo);
            }
            else if(OnlyMonitor)
            {
                _OnlyMonitor = true;
                DBReadOnlyReader dBReadOnlyReader = new DBReadOnlyReader(dbConfig);
                foreach (var blockInfos in GetBlockInfos2(CVConfig.MPLCNo))
                {
                    dBReadOnlyReader.AddDataBlock(blockInfos);
                }
                _converyor = new Conveyors.Conveyor(dBReadOnlyReader, CVConfig.MPLCNo);
                _converyor.Start();

                DBReadOnlyReader dBReadOnlyReader2 = new DBReadOnlyReader(dbConfig);
                foreach (var blockInfos in GetBlockInfos2(CV_Config2.MPLCNo))
                {
                    dBReadOnlyReader2.AddDataBlock(blockInfos);
                }
                _converyor2 = new Conveyors.Conveyor(dBReadOnlyReader2, CV_Config2.MPLCNo);
                _converyor2.Start();
            }
            else
            {
                var plcHostInfo = new PLCHostInfo("VITALON1", CVConfig.MPLCIP, CVConfig.MPLCPort, GetBlockInfos(CVConfig.MPLCNo));
                _plcHost = new PLCHost(plcHostInfo);
                _plcHost.Interval = 200;
                _plcHost.MPLCTimeout = 600;
                _plcHost.EnableWriteRawData = true;
                _plcHost.EnableWriteShareMemory = false;
                _plcHost.SetWriteRawDataToDB(dbConfig);
                _plcHost.PLCHostNo = 1;

                var plcHostInfo2 = new PLCHostInfo("VITALON2", CV_Config2.MPLCIP, CV_Config2.MPLCPort, GetBlockInfos(CV_Config2.MPLCNo));
                _plcHost2 = new PLCHost(plcHostInfo2);
                _plcHost2.Interval = 200;
                _plcHost2.MPLCTimeout = 600;
                _plcHost2.EnableWriteRawData = true;
                _plcHost2.EnableWriteShareMemory = false;
                _plcHost2.SetWriteRawDataToDB(dbConfig);
                _plcHost2.PLCHostNo = 2;
                //var smReader = new SMReadOnlyCachedReader();
                //var blockInfos = GetBlockInfos();
                //foreach (var block in blockInfos)
                //{
                //    smReader.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{block.SharedMemoryName}"));
                //}
                _converyor = new Conveyors.Conveyor(_plcHost,CVConfig.MPLCNo);
                _converyor2 = new Conveyors.Conveyor(_plcHost2, CV_Config2.MPLCNo);
                _plcHost.Start();
                _plcHost2.Start();
            }

            foreach (var buffer in _converyor.Buffers)
            {
                buffer.OnIniatlNotice += Buffer_OnIniatlNotice;
            }

            _converyor.Start();
            _converyor2.Start();
            _mainView = new MainView(_converyor, _converyor2);
        }

        //public CVControllertest(string ipAddress, int tcpPort, bool InMemorySimulator, int PLCNo)
        //{
        //    if (InMemorySimulator)
        //    {
        //        var smWriter = new SMReadWriter();
        //        var blockInfos = GetBlockInfos(PLCNo);
        //        foreach (var block in blockInfos)
        //        {
        //            smWriter.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{block.SharedMemoryName}"));
        //        }
        //        _converyor = new Conveyors.Conveyor(smWriter, PLCNo);
        //        _InMemorySimulator = InMemorySimulator;
        //    }
        //    else
        //    {
        //        var plcHostInfo = new PLCHostInfo("VITALON", ipAddress, tcpPort, GetBlockInfos(PLCNo));
        //        _plcHost = new PLCHost(plcHostInfo);
        //        _plcHost.Interval = 200;
        //        _plcHost.MPLCTimeout = 600;
        //        _plcHost.EnableWriteRawData = false;
        //        _plcHost.EnableWriteShareMemory = true;
        //        //var smReader = new SMReadOnlyCachedReader();
        //        //var blockInfos = GetBlockInfos();
        //        //foreach (var block in blockInfos)
        //        //{
        //        //    smReader.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{block.SharedMemoryName}"));
        //        //}
        //        _converyor = new Conveyors.Conveyor(_plcHost, PLCNo);
        //        _plcHost.Start();
        //    }

        //    foreach (var buffer in _converyor.Buffers)
        //    {
        //        buffer.OnIniatlNotice += Buffer_OnIniatlNotice;
        //    }

        //    _converyor.Start();
        //    _mainView = new MainView(_converyor);
        //}

        private IEnumerable<BlockInfo> GetBlockInfos(int PLCNo)
        {
            if (PLCNo == 1)
            {
                yield return new BlockInfo(new DDeviceRange("D101", "D630"), "Read", 0);
                yield return new BlockInfo(new DDeviceRange("D3101", "D3590"), "Write", 1);
            }
            else if(PLCNo == 2)
            {
                yield return new BlockInfo(new DDeviceRange("D101", "D380"), "Read2", 2);
                yield return new BlockInfo(new DDeviceRange("D3101", "D3360"), "Write2", 3);
            }
        }

        private IEnumerable<FileDataBlock> GetBlockInfos2(int PLCNo)
        {
            if (PLCNo == 1)
            {
                yield return new FileDataBlock(new DDeviceRange("D101", "D630"), 0);
                yield return new FileDataBlock(new DDeviceRange("D3101", "D3590"), 1);
            }
            else if (PLCNo == 2)
            {
                yield return new FileDataBlock(new DDeviceRange("D101", "D380"), 2);
                yield return new FileDataBlock(new DDeviceRange("D3101", "D3360"), 3);
            }
        }




        private void Buffer_OnIniatlNotice(object sender, BufferEventArgs e)
        {
        }

        public Conveyors.Conveyor GetConveryor()
        {
            return _converyor;
        }

        public Conveyors.Conveyor GetConveryor2()
        {
            return _converyor2;
        }

        public bool GetConnect()
        {
            if (_InMemorySimulator)
            {
                return true;
            }
            else if(_OnlyMonitor)
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
