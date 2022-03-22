using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using Mirle.MPLC;
using Mirle.MPLC.MCProtocol;
using Mirle.ASRS.Conveyor.U2NMMA30.Service;
using Mirle.ASRS.Converyor.View;

namespace Mirle.ASRS.Conveyors.View
{
    public partial class MainView : Form
    {
        private readonly Conveyor _conveyor;
        private readonly Conveyor _conveyor2;
        //private LoggerService _loggerService;
        private static int bufferCount = 10;

        public MainView(Conveyor conveyor,Conveyor conveyor2)
        {
            InitializeComponent();
            _conveyor = conveyor;
            _conveyor2 = conveyor2;
        }
        private void MainView_Load(object sender, EventArgs e)
        {
            pnlHP.Visible = true;
            pnlUpper.Visible = false;
            pnlLower.Visible = false;

            timerMainProc.Enabled = true;
            //Start Timer
            timerMainProc.Interval = 300;
            timerMainProc.Enabled = true;
        }

        private void timerMainProc_Tick(object sender, EventArgs e)
        {
            timerMainProc.Enabled = false;
            try
            {
                if (!_conveyor.IsConnected)
                {
                    for (int index = 1; index <= bufferCount; index++)
                    {
                        if (splitContainer1.Panel1.Controls.Find("A" + index, true).FirstOrDefault() is BufferView bufferView)
                        {
                            if (_conveyor.TryGetBuffer(bufferView.BufferIndex, out var buffer))
                            {
                                bufferView.Refresh_BufferPLCError(buffer);
                            }
                        }
                    }
                }
                else
                {
                    if (pnlHP.Visible)
                    {
                        for (int index = 0; index < pnlHP.Controls.Count; index++)
                        {
                            if (pnlHP.Controls[index] is BufferView bufferView)
                            {
                                if (_conveyor.TryGetBuffer(bufferView.BufferIndex, out var buffer))
                                {
                                    bufferView.Refresh_Buffer(buffer);
                                }
                            }

                            if (pnlHP.Controls[index] is BarcodeDataView barcodeDataView)
                            {
                                _conveyor.TryGetBuffer(43, out var bcrbuffer);
                                barcodeDataView1.RefreshBCR(bcrbuffer.Item_No, bcrbuffer.Lot_ID, bcrbuffer.Plt_Id);
                                _conveyor.TryGetBuffer(48, out var bcrbuffer1);
                                barcodeDataView2.RefreshBCR(bcrbuffer1.Item_No, bcrbuffer1.Lot_ID, bcrbuffer1.Plt_Id);
                            }
                         }
                    }
                    else if (pnlLower.Visible)
                    {
                        for (int index = 0; index < pnlLower.Controls.Count; index++)
                        {
                            if (pnlLower.Controls[index] is BufferView bufferView)
                            {
                                if (_conveyor.TryGetBuffer(bufferView.BufferIndex, out var buffer))
                                {
                                    bufferView.Refresh_Buffer(buffer);
                                }
                            }
                        }
                    }
                    else if (pnlUpper.Visible)
                    {
                        for (int index = 0; index < pnlUpper.Controls.Count; index++)
                        {
                            if (pnlUpper.Controls[index] is BufferView bufferView)
                            {
                                if (_conveyor2.TryGetBuffer(bufferView.BufferIndex, out var buffer))
                                {
                                    bufferView.Refresh_Buffer(buffer);
                                }
                            }
                            if (pnlUpper.Controls[index] is BarcodeDataView barcodeDataView)
                            {
                                _conveyor2.TryGetBuffer(25, out var bcrbuffer3);
                                barcodeDataView3.RefreshBCR(bcrbuffer3.Item_No, bcrbuffer3.Lot_ID, bcrbuffer3.Plt_Id);
                            }
                        }
                        _conveyor.TryGetBuffer(42, out var buffer2);
                        bufferView77.Refresh_Buffer(buffer2);
                        _conveyor.TryGetBuffer(48, out var buffer4);
                        bufferView76.Refresh_Buffer(buffer4);
                    }
                }
            }
            catch (Exception ex)
            {
                //_loggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                timerMainProc.Enabled = true;
            }
        }
        private void btnHP_Click(object sender, EventArgs e)
        {
            pnlHP.Visible = true;
            pnlUpper.Visible = false;
            pnlLower.Visible = false;
        }

        private void btnLower_Click(object sender, EventArgs e)
        {
            pnlHP.Visible = false;
            pnlUpper.Visible = false;
            pnlLower.Visible = true;
        }

        private void btnUpper_Click(object sender, EventArgs e)
        {
            pnlHP.Visible = false;
            pnlUpper.Visible = true;
            pnlLower.Visible = false;
        }
    }
}
