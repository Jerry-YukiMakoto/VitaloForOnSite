using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mirle.MPLC;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.DataBlocks.DeviceRange;
using Mirle.MPLC.DataType;
using Mirle.MPLC.FileData;
using Mirle.ASRS.Converyor;
using Mirle.ASRS.Conveyors.MPLCView;

namespace Mirle.ASRS.Conveyors
{
    public class MPLCViewController : IMPLCViewController
    {
        public string Title { get; } = "Log viewer";
        private FileReader _fReader = new FileReader();
        private Form _defaultView;

        private IDataType _currentFocusedSignal = null;

        public MPLCViewController(int type)
        {

                _defaultView = new MainView(_fReader, this);
                Title = "Vitalo";
                var _dlist = GetBlockInfos(0);
                foreach (var data in _dlist)
                {
                    _fReader.AddDataBlock(data);
                }
            
        }

        public IDataType CurrentFocusedSignal
        {
            internal set
            {
                _currentFocusedSignal = value;
            }
            get
            {
                var signal = _currentFocusedSignal;
                _currentFocusedSignal = null;
                return signal;
            }
        }

        public Form DefaultView()
        {
            return _defaultView;
        }

        public FileReader GetFileReader()
        {
            return _fReader;
        }

        public void SetMPLCSource(IMPLCProvider provider)
        {
        }
        private IEnumerable<FileDataBlock> GetBlockInfos(int signalGroup)
        {
                yield return new FileDataBlock(new DDeviceRange("D101", "D630"), 0);
                yield return new FileDataBlock(new DDeviceRange("D3101", "D3590"),  1);
                yield return new FileDataBlock(new DDeviceRange("D101", "D380"), 2);
                yield return new FileDataBlock(new DDeviceRange("D3101", "D3360"), 3);
        }

    }
}
