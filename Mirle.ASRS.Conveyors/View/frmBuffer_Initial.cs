using Mirle.ASRS.Conveyors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WCS
{
    public partial class frmBuffer_Initial : Form
    {

        private Timer timRefresh = new Timer();
        private readonly Conveyor _conveyor;
        private readonly int _PLCNo;

        public int StnIdx { get; private set; }

        public frmBuffer_Initial(Conveyor conveyor,int PLCNo)
        {

            InitializeComponent();
            _conveyor = conveyor;
            _PLCNo = PLCNo;
            timRefresh.Stop();
            timRefresh.Tick += new EventHandler(timRefresh_Tick);
            timRefresh.Interval = 100;
            timRefresh.Start();
        }

        private void timRefresh_Tick(object sender, EventArgs e)
        {
            StnIdx = 0;

            if (!string.IsNullOrWhiteSpace(cboBuffer_Name.Text.Trim()))
            {
                StnIdx = cboBuffer_Name.SelectedIndex+1;

                txtCmdSno_Plc.Text=_conveyor.GetBuffer(StnIdx).CommandId.ToString();
                txtCmdSno_Pc.Text = _conveyor.GetBuffer(StnIdx).PCCommandId.ToString();
                txtCmdMode_Plc.Text = _conveyor.GetBuffer(StnIdx).CmdMode.ToString();
                txtCmdMode_Pc.Text = _conveyor.GetBuffer(StnIdx).PC_CmdMode.ToString();
                txtInitial_Plc.Text = _conveyor.GetBuffer(StnIdx).InitialNotice.ToString();
                txtInitial_Pc.Text = _conveyor.GetBuffer(StnIdx).PC_InitialNotice.ToString();

            }
            else
            {
                txtCmdSno_Plc.Text = "";
                txtCmdSno_Pc.Text = "";
                txtCmdMode_Plc.Text = "0";
                txtCmdMode_Pc.Text = "0";
                txtInitial_Pc.Text = "0";
                txtInitial_Plc.Text = "0";
                txtPath_Pc.Text = "0";
                txtPath_Plc.Text = "0";
                txtRead_Ack.Text = "0";
                txtReceive.Text = "0";
            }



        }

        private void frmBuffer_Initial_Load(object sender, EventArgs e)
        {
            if (_PLCNo == 1)
            {
                cboBuffer_Name.Items.Clear();
                cboBuffer_Name.Items.Add("A01-01");
                cboBuffer_Name.Items.Add("A01-02");
                cboBuffer_Name.Items.Add("A01-03");
                cboBuffer_Name.Items.Add("A01-04");
                cboBuffer_Name.Items.Add("A01-05");
                cboBuffer_Name.Items.Add("A01-06");
                cboBuffer_Name.Items.Add("A01-07");
                cboBuffer_Name.Items.Add("A01-08");
                cboBuffer_Name.Items.Add("A01-09");
                cboBuffer_Name.Items.Add("A01-10");
                cboBuffer_Name.Items.Add("A01-11");
                cboBuffer_Name.Items.Add("A01-12");
                cboBuffer_Name.Items.Add("A02-01");
                cboBuffer_Name.Items.Add("A02-02");
                cboBuffer_Name.Items.Add("A02-03");
                cboBuffer_Name.Items.Add("A02-04");
                cboBuffer_Name.Items.Add("A02-05");
                cboBuffer_Name.Items.Add("A02-06");
                cboBuffer_Name.Items.Add("A02-07");
                cboBuffer_Name.Items.Add("A02-08");
                cboBuffer_Name.Items.Add("A02-09");
                cboBuffer_Name.Items.Add("A02-10");
                cboBuffer_Name.Items.Add("A02-11");
                cboBuffer_Name.Items.Add("A02-12");
                cboBuffer_Name.Items.Add("A04");
                cboBuffer_Name.Items.Add("A05-1");
                cboBuffer_Name.Items.Add("A05-2");
                cboBuffer_Name.Items.Add("A06-1");
                cboBuffer_Name.Items.Add("A06-2");
                cboBuffer_Name.Items.Add("A11-01");
                cboBuffer_Name.Items.Add("A11-02");
                cboBuffer_Name.Items.Add("A05-3");
                cboBuffer_Name.Items.Add("A05-4");
                cboBuffer_Name.Items.Add("A05-5");
                cboBuffer_Name.Items.Add("A05-6");
                cboBuffer_Name.Items.Add("A05-7");
                cboBuffer_Name.Items.Add("A05-8");
                cboBuffer_Name.Items.Add("A06-3");
                cboBuffer_Name.Items.Add("A06-4");
                cboBuffer_Name.Items.Add("A11-3");
                cboBuffer_Name.Items.Add("A11-4");
                cboBuffer_Name.Items.Add("A07");
                cboBuffer_Name.Items.Add("A08");
                cboBuffer_Name.Items.Add("A09");
                cboBuffer_Name.Items.Add("A010");
                cboBuffer_Name.Items.Add("A01-12");
                cboBuffer_Name.Items.Add("A11-5");
                cboBuffer_Name.Items.Add("A11-6");
            }
            else
            {
                cboBuffer_Name.Items.Clear();
                cboBuffer_Name.Items.Add("B01-1");
                cboBuffer_Name.Items.Add("B01-2");
                cboBuffer_Name.Items.Add("B01-3");
                cboBuffer_Name.Items.Add("B01-4");
                cboBuffer_Name.Items.Add("B01-5");
                cboBuffer_Name.Items.Add("B01-6");
                cboBuffer_Name.Items.Add("B01-7");
                cboBuffer_Name.Items.Add("B01-8");
                cboBuffer_Name.Items.Add("B01-9");
                cboBuffer_Name.Items.Add("B1-10");
                cboBuffer_Name.Items.Add("B1-11");
                cboBuffer_Name.Items.Add("B1-12");
                cboBuffer_Name.Items.Add("B02-1");
                cboBuffer_Name.Items.Add("B02-2");
                cboBuffer_Name.Items.Add("B02-3");
                cboBuffer_Name.Items.Add("B02-4");
                cboBuffer_Name.Items.Add("B02-5");
                cboBuffer_Name.Items.Add("B02-6");
                cboBuffer_Name.Items.Add("B02-7");
                cboBuffer_Name.Items.Add("B02-8");
                cboBuffer_Name.Items.Add("B02-9");
                cboBuffer_Name.Items.Add("B2-10");
                cboBuffer_Name.Items.Add("B2-11");
                cboBuffer_Name.Items.Add("B2-12");
                cboBuffer_Name.Items.Add("B03");
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInitial_Pc_Click(object sender, EventArgs e)//清PC->PLC的值
        {
            _conveyor.GetBuffer(StnIdx).WriteCommandIdAsync("0", 0);
            _conveyor.GetBuffer(StnIdx).WritePathChabgeNotice(0);
        }

        private void btnInitial_Plc_Click(object sender, EventArgs e)
        {
            _conveyor.GetBuffer(StnIdx).InitialNoticeTrigger();
        }
    }
}
