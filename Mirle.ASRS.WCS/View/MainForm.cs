using System;
using System.Drawing;
using System.Windows.Forms;
using Mirle.Grid.T1JUMAK0;
using Mirle.DB.Object;
using Mirle.Logger;
using Mirle.ASRS.Close.Program;
using System.Threading;
using Mirle.ASRS.WCS.Library;
using Mirle.ASRS.WCS.Controller;
using Mirle.Def;
using Mirle.DataBase;
using Mirle.ASRS.Conveyors.View;
using Mirle.ASRS.AWCS.View;

namespace Mirle.ASRS.WCS.View
{
    public partial class MainForm : Form
    {
        //public static clsCheckPathIsWork CheckPathIsWork = new clsCheckPathIsWork();
        

        private DB.ClearCmd.Proc.clsHost clearCmd;
        private static WCSManager _wcsManager;
        private static System.Timers.Timer timRead = new System.Timers.Timer();
        private AlarmView _alarmView;

        public MainForm()
        {
            InitializeComponent();
            timRead.Elapsed += new System.Timers.ElapsedEventHandler(timRead_Elapsed);
            timRead.Enabled = false; 
            timRead.Interval = 500;
            _alarmView = new AlarmView();
            _alarmView.Show();
            _alarmView.Visible = false;
        }

        #region Event
        private void MainForm_Load(object sender, EventArgs e)
        {
            ChkAppIsAlreadyRunning();
            this.Text = this.Text + "  v " + ProductVersion;
            clInitSys.FunLoadIniSys();
            FunInit();
            FunEventInit();
            GridInit();
            Library.clsWriLog.Log.FunWriTraceLog_CV("WCS程式已開啟");
            timRead.Start();
            timer1.Start();
        }

        private void FunEventInit()
        {
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmCloseProgram objCloseProgram;
            try
            {
                e.Cancel = true;

                objCloseProgram = new frmCloseProgram();

                if (objCloseProgram.ShowDialog() == DialogResult.OK)
                {
                    chkOnline.Checked = false;
                    SpinWait.SpinUntil(() => false, 1000);
                    Library.clsWriLog.Log.FunWriTraceLog_CV("WCS程式已關閉！");
                    throw new Exception();
                }
            }
            catch
            {
                Environment.Exit(0);
            }
            finally
            {
                objCloseProgram = null;
            }
        }

        private void chkOnline_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOnline.Checked)
                Library.clsWriLog.Log.FunWriTraceLog_CV("WCS OnLine.");
            else
                Library.clsWriLog.Log.FunWriTraceLog_CV("WCS OffLine.");
        }

        #endregion Event



        #region 側邊欄buttons


        private CraneSpeedMaintainView StockerSpeed;
        private void btnCraneSpeedMaintain_Click(object sender, EventArgs e)
        {
            if (StockerSpeed == null)
            {
                StockerSpeed = new CraneSpeedMaintainView();
                StockerSpeed.TopMost = true;
                StockerSpeed.FormClosed += new FormClosedEventHandler(funCraneSpeedMaintain_FormClosed);
                StockerSpeed.Show();
            }
            else
            {
                StockerSpeed.BringToFront();
            }
        }

        private void funCraneSpeedMaintain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (StockerSpeed != null)
                StockerSpeed = null;
        }

        private frmCmdMaintance cmdMaintance;
        private void btnCmdMaintain_Click(object sender, EventArgs e)
        {
            if (cmdMaintance == null)
            {
                cmdMaintance = new frmCmdMaintance();
                cmdMaintance.TopMost = true;
                cmdMaintance.FormClosed += new FormClosedEventHandler(funCmdMaintain_FormClosed);
                cmdMaintance.Show();
            }
            else
            {
                cmdMaintance.BringToFront();
            }
        }

        private frmEquCmdMaintance equcmdMaintance;
        private void btnEquMaintain_Click(object sender, EventArgs e)
        {
            if (equcmdMaintance == null)
            {
                equcmdMaintance = new frmEquCmdMaintance();
                equcmdMaintance.TopMost = true;
                equcmdMaintance.FormClosed += new FormClosedEventHandler(funEquMaintain_FormClosed);
                equcmdMaintance.Show();
            }
            else
            {
                equcmdMaintance.BringToFront();
            }
        }

        private Form test;
        private void test_Click(object sender, EventArgs e)
        {
            if (test == null)
            {
                test = new Form1();
                test.FormClosed += new FormClosedEventHandler(funtest_FormClosed);
                test.Show();
                
            }
            else
            {
                test.BringToFront();
            }
        }

        private void funCmdMaintain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (cmdMaintance != null)
                cmdMaintance = null;
        }

        private void funEquMaintain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (equcmdMaintance != null)
                equcmdMaintance = null;
        }

        private void funtest_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (test != null)
                test = null;
        }

        #endregion 側邊欄buttons

        #region Timer

        private void timRead_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            timRead.Enabled = false;
            try
            {
                SubShowCmdtoGrid(ref GridCmd);
                EqustsShow();
                AlarmDataShow();
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                Library.clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                timRead.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            try
            {
                lblDBConn.BackColor = DB.Proc.clsHost.IsConn ? Color.Lime : Color.Red;
                lblPLCConn.BackColor = ControllerReader.GetCVControllerr().GetConnect() ? Color.Lime : Color.Red;
                lblTimer.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                Library.clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                timer1.Start();
            }
        }

        #endregion Timer

       
        private void FunInit()
        {
            var archive = new AutoArchive();
            archive.Start();
            clsDB_Proc.Initial(clInitSys.DbConfig,clInitSys.OnlyMonitor); //原DataAccessController功能
            ControllerReader.FunGetController(clInitSys.CV_Config,clInitSys.CV_Config2,clInitSys.OnlyMonitor, clInitSys.DbConfig);

            _wcsManager = new WCSManager();
            if (clInitSys.OnlyMonitor == false)//是否是監控模式
            {
                _wcsManager.Start();
            }
            clearCmd = new DB.ClearCmd.Proc.clsHost();
            ChangeSubForm(ControllerReader.GetCVControllerr().GetMainView()); 
        }

        #region Grid顯示
        private void GridInit()
        {
            Grid.clInitSys.GridSysInit(ref GridCmd);
            ColumnDef.CMD_MST.GridSetLocRange(ref GridCmd);
        }

        delegate void degShowCmdtoGrid(ref DataGridView oGrid);
        private void SubShowCmdtoGrid(ref DataGridView oGrid)
        {
            degShowCmdtoGrid obj;
            try
            {
                if (InvokeRequired)
                {
                    obj = new degShowCmdtoGrid(SubShowCmdtoGrid);
                    Invoke(obj, oGrid);
                }
                else
                {
                    Grid.IGrid grid;
                    grid = new DB.Object.GridData.CmdMst();
                    grid.SubShowCmdtoGrid(ref oGrid);
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                Library.clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }
        #endregion Grid顯示

        /// <summary>
        /// 檢查程式是否重複開啟
        /// </summary>
        private void ChkAppIsAlreadyRunning()
        {
            try
            {
                string aFormName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
                string aProcName = System.IO.Path.GetFileNameWithoutExtension(aFormName);
                if (System.Diagnostics.Process.GetProcessesByName(aProcName).Length > 1)
                {
                    MessageBox.Show("程式已開啟", "Communication System", MessageBoxButtons.OK);
                    //Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                Library.clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                Environment.Exit(0);
            }
        }

        private void ChangeSubForm(Form subForm)
        {
            try
            {
                var children = spcMainView.Panel1.Controls;
                foreach (Control c in children)
                {
                    if (c is Form)
                    {
                        var thisChild = c as Form;
                        //thisChild.Hide();
                        spcMainView.Panel1.Controls.Remove(thisChild);
                        thisChild.Width = 0;
                    }
                }

                if (subForm != null)
                {
                    subForm.TopLevel = false;
                    subForm.Dock = DockStyle.Fill;//適應窗體大小
                    subForm.FormBorderStyle = FormBorderStyle.None;//隱藏右上角的按鈕
                    subForm.Parent = spcMainView.Panel1;
                    spcMainView.Panel1.Controls.Add(subForm);
                    subForm.Show();
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                Library.clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        private void EqustsShow()
        {
            try
            {
                string[] sCrnMode = new string[7];

                if (listBox1.InvokeRequired)
                {
                    listBox1.Invoke(new Action((EqustsShow)));
                }

                for(int i = 1;i<=6;i++)
                {
                    if (clsDB_Proc.GetDB_Object().GetEqu_Cmd().FunGetEquStsMode(i, out var dataObject) == DBResult.Success)
                    {
                        sCrnMode[i] = dataObject[0].EquSts;
                    }
                    else
                    {
                        sCrnMode[i] = "X";
                    }
                }

                for (int i=1;i<=6;i++)
                {
                    if(clsDB_Proc.GetDB_Object().GetEqu_Cmd().FunGetEquSts(i, out var dataObject2)==DBResult.Success);

                    if(sCrnMode[i]=="E")
                    {
                        string errorcode= (i).ToString().PadLeft(2, '0') + ":" + dataObject2[0].AlarmDesc;
                        if(!listBox1.Items.Contains(errorcode))
                        {
                            listBox1.Items.Add(errorcode);
                        }
                    }
                    else
                    {
                        if(listBox1.Items.Count>0)//異常解除通知
                        {
                            string sErrorCode = string.Empty;
                            bool bFlag = false;
                            for (int iRow = 0; iRow < listBox1.Items.Count; iRow++)
                            {
                                if (listBox1.Items[iRow].ToString().Substring(0, 2) == (i).ToString().PadLeft(2, '0'))
                                {
                                    bFlag = true;
                                    sErrorCode = listBox1.Items[iRow].ToString();
                                    break;
                                }
                            }
                            if(bFlag==true)
                            {
                                listBox1.Items.Remove(sErrorCode);
                            }
                        }
                    }

                }
                
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                Library.clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        delegate void degAlarmDataShow();

        private void AlarmDataShow()
        {
            try
            {
                degAlarmDataShow obj;
                if (_alarmView.InvokeRequired)
                {
                    _alarmView.Invoke(new Action((AlarmDataShow)));
                    obj = new degAlarmDataShow(AlarmDataShow);
                    Invoke(obj);
                }
                else
                {
                    if (clsDB_Proc.GetDB_Object().GeterrorReport().isAlarm())
                    {
                        _alarmView.Visible = true;
                        _alarmView.UpdateListData(clsDB_Proc.GetDB_Object().GeterrorReport().GetAlarmData());
                    }
                    else
                    {
                        _alarmView.Visible = false;
                        _alarmView.ClearListData();
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                Library.clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public AlarmView GetAlarmView()
        {
            return _alarmView;
        }
    }
}
