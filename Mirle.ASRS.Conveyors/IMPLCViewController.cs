using Mirle.MPLC.DataType;
using Mirle.MPLC.FileData;
using System.Windows.Forms;
using Mirle.MPLC;

namespace Mirle.ASRS.Converyor
{
    public interface IMPLCViewController
    {
        string Title { get; }
        IDataType CurrentFocusedSignal { get; }

        Form DefaultView();
        FileReader GetFileReader();
        void SetMPLCSource(IMPLCProvider provider);
    }
}