﻿using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using Mirle.MPLC;
using Mirle.MPLC.MCProtocol;
using Mirle.ASRS.Conveyor.U2NMMA30.Service;

namespace Mirle.ASRS.Conveyors.View
{
    public partial class MainView : Form
    {
        private readonly Conveyor _conveyor;
        private LoggerService _loggerService;
        private static int bufferCount = 10;

        public MainView(Conveyor conveyor)
        {
            InitializeComponent();
            _conveyor = conveyor;
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
                if(!_conveyor.IsConnected)
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
                        }
                    }
                    else if (pnlLower.Visible)
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
                        }
                    }
                    else if (pnlUpper.Visible) 
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
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                _loggerService.WriteExceptionLog(MethodBase.GetCurrentMethod(), $"{ex.Message}\n{ex.StackTrace}");
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
