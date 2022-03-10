using System;
using System.Collections.Generic;
using Mirle.ASRS.WCS.Controller;
using Mirle.DB.Fun;
using Mirle.Def;
using Mirle.DataBase;
using Mirle.Def.U0NXMA30;
using Mirle.Grid.U0NXMA30;
using Mirle.Structure;
using Mirle.Structure.Info;
using System.Linq;
using WCS_API_Client.ReportInfo;
using System.Data;
using Mirle.ASRS.WCS.Model.PLCDefinitions;
using Mirle.ASRS.WCS.Model.DataAccess;
using Mirle.CENS.U0NXMA30;

namespace Mirle.DB.Proc
{
    public class clsProc
    {
        private clsPortDef PortDef = new clsPortDef();
        private Fun.clsCmd_Mst CMD_MST = new Fun.clsCmd_Mst();
        private Fun.clsEqu_Cmd EQU_CMD = new Fun.clsEqu_Cmd();
        private Fun.clsLocMst Loc_Mst = new Fun.clsLocMst();
        private Fun.clsItmMst Itm_Mst = new Fun.clsItmMst();
        private Fun.clsSno SNO = new Fun.clsSno();
        private Fun.clsLocMst LocMst = new Fun.clsLocMst();
        private Fun.clsProc proc;
        private Fun.clsAlarmData alarmData = new Fun.clsAlarmData();
        private Fun.clsCmd_Mst_His CMD_MST_HIS = new Fun.clsCmd_Mst_His();
        private Fun.clsUnitStsLog unitStsLog = new Fun.clsUnitStsLog();

        public List<Element_Port>[] GetLstPort()
        {
            return PortDef.GetLstPort();
        }

        private clsDbConfig _config = new clsDbConfig();
        private clsDbConfig _config_WMS = new clsDbConfig();
        private clsDbConfig _config_Sqlite = new clsDbConfig();
        public clsProc(clsDbConfig config, clsDbConfig config_WMS, clsDbConfig config_Sqlite)
        {
            _config = config;
            _config_WMS = config_WMS;
            _config_Sqlite = config_Sqlite;
            proc = new Fun.clsProc(_config_WMS);
        }

        public Fun.clsProc GetFunProcess()
        {
            return proc;
        }

        #region StoreIn
        public bool FunOutsourceStoreInWriPlc(string sStnNo, int bufferIndex)
        {
            string Item_No="";
            string Plt_Id="";
            string Lot_No="";
            string New_Loc = "";
            string Equ_No = "";
            int IsHigh = 0;
            string[] Cranests = new string[6];
            try
            {
                var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                if (_conveyor.GetBuffer(bufferIndex).BcrNotice == 1)
                {

                    using (var db = clsGetDB.GetDB(_config))
                    {
                        int iRet = clsGetDB.FunDbOpen(db);
                        if (iRet == DBResult.Success)
                        {
                            if (CMD_MST.GetCmdMstByStoreInStart(sStnNo, Item_No, Lot_No, out var dataObject, db).ResultCode == DBResult.Success) //讀取CMD_MST
                            {

                                string cmdSno = dataObject[0].Cmd_Sno;
                                int CmdMode = Convert.ToInt32(dataObject[0].Cmd_Mode);
                                int IOType = Convert.ToInt32(dataObject[0].IO_Type);

                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Get StoreIn Command => {cmdSno}, " +
                                        $"{CmdMode}");

                                #region//根據buffer狀態更新命令
                                if (_conveyor.GetBuffer(bufferIndex).Auto != true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotAutoMode, db);
                                    return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).InMode != true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotInMode, db);
                                    return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).Error == true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.BufferError, db);
                                    return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).CommandId > 0)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.CmdLeftOver, db);
                                    return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).Presence == true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.PresenceExist, db);
                                    return false;
                                }
                                #endregion

                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready Receive StoreIn Command=> {cmdSno}");

                                if (EQU_CMD.GetEquStatus(out var dataObject1, db).ResultCode == DBResult.Success)
                                {
                                    for (int i = 0; i < dataObject1.Count; i++)
                                    {
                                        Cranests[i] = dataObject1[i].EquNo;
                                    }
                                }

                                IsHigh=_conveyor.GetBuffer(bufferIndex).LoadHeight;//根據荷高去選儲位位置

                                if (IsHigh == 1)
                                {
                                    if (Loc_Mst.GetLocMst_EmptyLochigh(Equ_No, out var dataObject2, db).ResultCode == DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find Loc succeess => {cmdSno}");
                                        New_Loc = dataObject2[0].New_Loc;
                                    }
                                }
                                else
                                {
                                    if (Loc_Mst.GetLocMst_EmptyLoc(Equ_No, out var dataObject2, db).ResultCode == DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find Loc succeess => {cmdSno}");
                                        New_Loc = dataObject2[0].New_Loc;
                                    }
                                }

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "begin fail");
                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMstTransferring(cmdSno, Trace.StoreInWriteCmdToCV, New_Loc, Plt_Id, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd_mst succeess => {cmdSno}");
                                }
                                else
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd_mst fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (CMD_MST.UpdateCmdDtlTransferring(cmdSno, Plt_Id, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd_dtl succeess => {cmdSno}");
                                }
                                else
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd_dtl fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (Loc_Mst.UpdateStoreInLocMst(New_Loc, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find Loc succeess => {cmdSno}");
                                }
                                var WritePlccheck = _conveyor.GetBuffer(bufferIndex).WriteCommandIdAsync(cmdSno, CmdMode).Result;//確認寫入PLC的方法是否正常運作，傳回結果和有異常的時候的訊息
                                bool Result = WritePlccheck;
                                if (Result != true)//寫入命令和模式
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).BCRNoticeComplete(1).Result;
                                Result = WritePlccheck;
                                if (Result != true)//通知讀取完成
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).BCRNoticeComplete(0).Result;
                                Result = WritePlccheck;
                                if (Result != true)//通知讀取完成
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "Commit Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                return true;

                            }
                            else return false;

                        }
                        else
                        {
                            string strEM = "Error: 開啟DB失敗！";
                            clsWriLog.Log.FunWriTraceLog_CV(strEM);
                            return false;
                        }
                    }
                }
                else return false;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunProduceStoreInWriPlc(string sStnNo, int bufferIndex)
        {
            string Item_No = "";
            string Plt_Id = "";
            string Lot_No = ""; //效期
            string New_Loc = "";
            string Equ_No = "";
            int IsHigh = 0;
            string cmdSno="";
            int CmdMode=0;
            string Item_Desc = "";
            string Item_Unit = "";
            string Item_Type = "";
            Double Qty_Plt = 0;
            string[] Cranests = new string[6];
            try
            {
                var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                if (_conveyor.GetBuffer(bufferIndex).BcrNotice == 1)
                {

                    using (var db = clsGetDB.GetDB(_config))
                    {
                        int iRet = clsGetDB.FunDbOpen(db);
                        if (iRet == DBResult.Success)
                        {
                            
                            #region//根據buffer狀態更新命令
                            if (_conveyor.GetBuffer(bufferIndex).Auto != true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotAutoMode, db);
                                    return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).InMode != true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotInMode, db);
                                    return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).Error == true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.BufferError, db);
                                    return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).CommandId > 0)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.CmdLeftOver, db);
                                    return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).Presence == true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.PresenceExist, db);
                                    return false;
                                }
                                #endregion

                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready Receive StoreIn Command=> {cmdSno}");

                            if (Itm_Mst.GetItmMstDtl(Item_No, out var dataObject3, db).ResultCode == DBResult.Success)//當搜尋不到料號資料，執行退版動作
                            {
                                Item_Desc = dataObject3[0].Item_Desc;
                                Item_Unit = dataObject3[0].Item_Unit;
                                Item_Type = dataObject3[0].Item_Type;
                                Qty_Plt = dataObject3[0].Qty_Plt;

                                if (EQU_CMD.GetEquStatus(out var dataObject1, db).ResultCode == DBResult.Success)
                                {
                                    for (int i = 0; i < dataObject1.Count; i++)
                                    {
                                        Cranests[i] = dataObject1[i].EquNo;
                                    }
                                }

                                IsHigh = _conveyor.GetBuffer(bufferIndex).LoadHeight;//根據荷高去選儲位位置

                                if (IsHigh == 1)
                                {
                                    if (Loc_Mst.GetLocMst_EmptyLochigh(Equ_No, out var dataObject2, db).ResultCode == DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find High Loc success");
                                        New_Loc = dataObject2[0].New_Loc;
                                    }
                                    else
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find High Loc fail");
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (Loc_Mst.GetLocMst_EmptyLoc(Equ_No, out var dataObject2, db).ResultCode == DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find Loc success");
                                        New_Loc = dataObject2[0].New_Loc;
                                    }
                                    else
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find Loc fail");
                                        return false;
                                    }
                                }

                                cmdSno = SNO.FunGetSeqNo(clsEnum.enuSnoType.CMDSNO, db);
                                if (cmdSno == "" || cmdSno == "00000")
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find cmdSno fail");
                                    return false;
                                }

                                struCmdMst stuCmdMst = new struCmdMst();
                                stuCmdMst.CmdSno = cmdSno;
                                stuCmdMst.CmdSts = "1";
                                stuCmdMst.CmdAbnormal = "NA";
                                stuCmdMst.Prty = "5";
                                stuCmdMst.StnNo = sStnNo;
                                stuCmdMst.CmdMode = "1";
                                stuCmdMst.IoType = "1";
                                stuCmdMst.WhId = "ASRS";
                                stuCmdMst.EquNo = Equ_No;
                                stuCmdMst.Loc = sStnNo;
                                stuCmdMst.NewLoc = New_Loc;
                                stuCmdMst.CrtDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                stuCmdMst.TrnUser = "WCS";
                                stuCmdMst.HostName = System.Environment.MachineName;
                                stuCmdMst.Trace = Trace.StoreInWriteCmdToCV;
                                stuCmdMst.Plt_Id = Plt_Id;

                                struCmdDtl stuCmdDtl = new struCmdDtl();   //命令交易編號 = 儲位交易編號
                                stuCmdDtl.Cmd_Txno = SNO.FunGetSeqNo(clsEnum.enuSnoType.LOCTXNO, db);
                                if (stuCmdDtl.Cmd_Txno == "" || stuCmdDtl.Cmd_Txno == "00000")
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find cmd_Txno fail");
                                    return false;
                                }
                                stuCmdDtl.Cmd_Sno = stuCmdMst.CmdSno;
                                stuCmdDtl.Plt_Qty = Qty_Plt;
                                stuCmdDtl.Trn_Qty = 0;
                                stuCmdDtl.Loc = "";
                                stuCmdDtl.In_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                stuCmdDtl.Item_No = Item_No;
                                stuCmdDtl.Lot_No = "";
                                stuCmdDtl.Plt_Id = Plt_Id;
                                stuCmdDtl.Company_ID = "";
                                stuCmdDtl.Item_Desc = Item_Desc;
                                stuCmdDtl.Uom = Item_Unit;
                                stuCmdDtl.Created_by = "WCS";
                                stuCmdDtl.Created_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "begin fail");
                                    return false;
                                }
                                if (CMD_MST.FunInsCmdMst(stuCmdMst, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Insert cmd_mst succeess => {cmdSno}");
                                }
                                else
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Insert cmd_mst fail => {cmdSno}");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (CMD_MST.FunInsCmdDtl(stuCmdDtl, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Insert cmd_dtl succeess => {cmdSno}");
                                }
                                else
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Insert cmd_dtl fail => {cmdSno}");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (Loc_Mst.UpdateStoreInLocMst(New_Loc, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find Loc succeess => {cmdSno}");
                                }
                                var WritePlccheck = _conveyor.GetBuffer(bufferIndex).WriteCommandIdAsync(cmdSno, CmdMode).Result;//確認寫入PLC的方法是否正常運作，傳回結果和有異常的時候的訊息
                                bool Result = WritePlccheck;
                                if (Result != true)//寫入命令和模式
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).BCRNoticeComplete(1).Result;
                                Result = WritePlccheck;
                                if (Result != true)//通知讀取完成
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).BCRNoticeComplete(0).Result;
                                Result = WritePlccheck;
                                if (Result != true)//通知讀取完成
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "Commit Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                return true;

                            }
                            else //當搜尋不到料號資料，執行退版動作
                            {
                                cmdSno = SNO.FunGetSeqNo(clsEnum.enuSnoType.CMDSNO, db);
                                if (cmdSno == "" || cmdSno == "00000")
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Abnormal Produce process find cmdSno fail");
                                    return false;
                                }

                                struCmdMst stuCmdMst = new struCmdMst();
                                stuCmdMst.CmdSno = cmdSno;
                                stuCmdMst.CmdSts = "1";
                                stuCmdMst.CmdAbnormal = "NA";
                                stuCmdMst.Prty = "5";
                                stuCmdMst.StnNo = sStnNo;
                                stuCmdMst.CmdMode = "9";
                                stuCmdMst.IoType = "9";
                                stuCmdMst.WhId = "ASRS";
                                stuCmdMst.EquNo = "";
                                stuCmdMst.Loc = sStnNo;
                                stuCmdMst.NewLoc = "";
                                stuCmdMst.CrtDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                stuCmdMst.TrnUser = "WCS";
                                stuCmdMst.HostName = System.Environment.MachineName;
                                stuCmdMst.Trace = "0";
                                stuCmdMst.Plt_Id = Plt_Id;

                                struCmdDtl stuCmdDtl = new struCmdDtl();   //命令交易編號 = 儲位交易編號
                                stuCmdDtl.Cmd_Txno = SNO.FunGetSeqNo(clsEnum.enuSnoType.LOCTXNO, db);
                                if (stuCmdDtl.Cmd_Txno == "" || stuCmdDtl.Cmd_Txno == "00000")
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find cmd_Txno fail");
                                    return false;
                                }
                                stuCmdDtl.Cmd_Sno = stuCmdMst.CmdSno;
                                stuCmdDtl.Plt_Qty = 0;
                                stuCmdDtl.Trn_Qty = 0;
                                stuCmdDtl.Loc = "";
                                stuCmdDtl.In_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                stuCmdDtl.Item_No = Item_No;
                                stuCmdDtl.Lot_No = "";
                                stuCmdDtl.Plt_Id = Plt_Id;
                                stuCmdDtl.Company_ID = "";
                                stuCmdDtl.Item_Desc = Item_Desc;
                                stuCmdDtl.Uom = Item_Unit;
                                stuCmdDtl.Created_by = "WCS";
                                stuCmdDtl.Created_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                return false;
                            }

                        }
                        else
                        {
                            string strEM = "Error: 開啟DB失敗！";
                            clsWriLog.Log.FunWriTraceLog_CV(strEM);
                            return false;
                        }
                    }
                }
                else return false;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunStoreIn_CreateEquCmd(int bufferIndex)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr2().GetConveryor();
                        
                        string cmdSno = (_conveyor.GetBuffer(bufferIndex).CommandId).ToString();
                        if (CMD_MST.GetCmdMstByStoreInCrane(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)
                        {

                            #region//根據buffer狀態更新命令
                            if (_conveyor.GetBuffer(bufferIndex).Auto != true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotAutoMode, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).InMode != true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotInMode, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).Error == true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.BufferError, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).Presence != true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.PresenceNotExist, db);
                                return false;
                            }
                            #endregion

                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready StoreIn=> {cmdSno}");

                            string source = "";
                            string IOType = dataObject[0].IOType;
                            string dest = "";
                            string Cmd_mode = dataObject[0].Cmd_Mode;

                            if (Cmd_mode == "3")//如果是撿料，入庫儲位欄位是LOC，一般入庫是NewLoc
                            {
                                dest = $"{dataObject[0].Loc}";
                            }
                            else
                            {
                                dest = $"{dataObject[0].NewLoc}";
                            }

                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "Create Crane StoreIn Command, Begin Fail");

                                return false;
                            }
                            if (CMD_MST.UpdateCmdMst(cmdSno, Trace.StoreInCreateCraneCmd, db).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane StoreIn Command, Update CmdMst Fail => {cmdSno}");

                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            if (EQU_CMD.InsertStoreInEquCmd(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, 1, cmdSno, "0000002", dest, 5, db) == false)
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane StoreIn Command, Insert EquCmd Fail => {cmdSno}");

                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane StoreIn Command, Commit Fail => {cmdSno}");

                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            else return true;
                            
                        }
                        else return false;
                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunStoreInEquCmdFinish()
        {
            try
            {
                var stn1 = new List<string>()
                {
                    StnNo.A11_06,
                    StnNo.A08,
                };
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {

                        if (CMD_MST.GetCmdMstByStoreInFinish(stn1, out var dataObject,db).ResultCode == DBResult.Success)
                        {
                            foreach (var cmdMst in dataObject.Data)
                            {

                                if (EQU_CMD.GetEquCmd(cmdMst.Cmd_Sno, out var equCmd,db).ResultCode == DBResult.Success)
                                {
                                    if (equCmd[0].ReNeqFlag != "F" && equCmd[0].CmdSts == "9")
                                    {
                                        string cmdsts = "";
                                        string cmdabnormal = "";
                                        string remark = "";
                                        bool bflag = false;

                                        if (equCmd[0].CompleteCode == "92")//正常完成
                                        {
                                            cmdsts = CmdSts.CompleteWaitUpdate;
                                            cmdabnormal = "NA";
                                            remark = "存取車搬送命令完成";
                                            bflag = true;
                                        }
                                        else if (equCmd[0].CompleteCode.StartsWith("W"))
                                        {
                                            if (EQU_CMD.UpdateEquCmdRetry(equCmd[0].CmdSno, db).ResultCode != DBResult.Success)
                                            {
                                                return false;
                                            }
                                            bflag = false;
                                        }
                                        else if (equCmd[0].CompleteCode == clsEnum.Cmd_Abnormal.EF.ToString()) //地上盤強制取消 EF
                                        {
                                            cmdsts = CmdSts.CmdCancel;
                                            cmdabnormal = clsEnum.Cmd_Abnormal.EF.ToString();
                                            remark = "存取車地上盤強制取消命令";
                                            bflag = true;
                                        }
                                        else if (equCmd[0].CompleteCode == clsEnum.Cmd_Abnormal.FF.ToString()) //地上盤強制完成 FF
                                        {
                                            cmdsts = CmdSts.CompleteWaitUpdate;
                                            cmdabnormal = clsEnum.Cmd_Abnormal.FF.ToString();
                                            remark = "存取車地上盤強制完成命令";
                                            bflag = true;
                                        }
                                        if (bflag == true)
                                        {
                                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                            {
                                                return false;
                                            }
                                            if (CMD_MST.UpdateCmdMst(equCmd[0].CmdSno, cmdsts, Trace.StoreInCraneCmdFinish, db).ResultCode != DBResult.Success)
                                            {
                                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            if (CMD_MST.UpdateCmdMstRemarkandAbnormal(equCmd[0].CmdSno, remark, cmdabnormal, db).ResultCode != DBResult.Success)
                                            {
                                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            if (EQU_CMD.DeleteEquCmd(equCmd[0].CmdSno, db).ResultCode != DBResult.Success)
                                            {
                                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                            return true;
                        }
                        else return true;
                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        #endregion StoreIn

        #region StoreOut

        public bool FunStoreOut_A01_01ToA01_11_WriPlc(string sStnNo, int bufferIndex) //出庫與盤點共用
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        if (CMD_MST.GetCmdMstByStoreOutStart(sStnNo, out var dataObject, db).ResultCode == DBResult.Success)
                        {
                            string cmd_Sno = dataObject[0].Cmd_Sno;
                            int Cmd_Mode = Convert.ToInt32(dataObject[0].Cmd_Mode);
                            string io_type = dataObject[0].IO_Type;
                            string Stn_No = dataObject[0].Stn_No;
                            var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();

                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Get StoreOut Command => {cmd_Sno}, " +
                                    $"{Cmd_Mode}");

                                #region//確認站口狀態
                            if (_conveyor.GetBuffer(bufferIndex).Auto != true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmd_Sno, Remark.NotAutoMode, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).OutMode != true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmd_Sno, Remark.NotOutMode, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).Error == true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmd_Sno, Remark.BufferError, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).CommandId > 0)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmd_Sno, Remark.CmdLeftOver, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).Presence == true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmd_Sno, Remark.PresenceExist, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).Ready != Ready.StoreOutReady)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmd_Sno, Remark.NotStoreOutReady, db);
                                return false;
                            }
                            #endregion


                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready Receive StoreOut Command => {cmd_Sno}, " +
                                    $"{Cmd_Mode}");

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Begin Fail => {cmd_Sno}");
                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMstTransferring(cmd_Sno, Trace.StoreOutWriteCraneCmdToCV, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd Success => {cmd_Sno}, " +
                                    $"{Cmd_Mode}");
                                }
                                else
                                {
                                    clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd fail => {cmd_Sno}, " +
                                    $"{Cmd_Mode}");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                var WritePlccheck = _conveyor.GetBuffer(bufferIndex).WriteCommandIdAsync(cmd_Sno, Cmd_Mode).Result;//寫入命令和模式//確認寫入PLC的方法是否正常運作，傳回結果和有異常的時候的訊息
                                bool Result = WritePlccheck;
                                if (Result != true)//寫入命令和模式
                                {
                                    clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }

                                #region 根據站號選擇路徑編號
                                int path =99;
                                if (Stn_No == "A11_04")
                                {
                                    path = PathNotice.OutPath1_toA11_04;
                                }
                                else if(Stn_No == "A11_02")
                                {
                                    path = PathNotice.OutPath2_toA11_02;
                                }
                                #endregion

                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).WritePathChabgeNotice(path).Result;//錯誤時回傳exmessage
                                Result = WritePlccheck;
                                if (Result != true)
                                {
                                    clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Path2_toA3 Fail");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "Commit Fail");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                return true;
                                
                        }
                        else return false;
                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunStoreOut_A01_01ToA01_11_CreateEquCmd(int bufferIndex)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        string cmdSno = _conveyor.GetBuffer(bufferIndex).CommandId.ToString();

                        if (CMD_MST.GetCmdMstByStoreOutCrane(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)
                        {
                            cmdSno = dataObject[0].Cmd_Sno;
                            string source = dataObject[0].Loc;
                            int Equ_No = Convert.ToInt32(dataObject[0].Equ_No); 

                            #region//站口狀態確認
                            if (_conveyor.GetBuffer(bufferIndex).Auto != true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotAutoMode, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).OutMode != true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotOutMode, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).Error == true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.BufferError, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).Presence == true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.PresenceExist, db);
                                return false;
                            }
                        #endregion

                                
                            clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready StoreOut => {cmdSno}");

                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane StoreOut Command, Begin Fail => {cmdSno}");
                                return false;
                            }
                            if (CMD_MST.UpdateCmdMst(cmdSno, Trace.StoreOutCreateCraneCmd, db).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane StoreOut Command, Update CmdMst Fail => {cmdSno}");
                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            if (EQU_CMD.InsertStoreOutEquCmd(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, Equ_No, cmdSno, source, "000001" , 5, db) == false)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane StoreOut Command, Insert EquCmd Fail => {cmdSno}");
                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane StoreOut Command, Commit Fail => {cmdSno}");
                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            return true;
                            
                        }
                            return true;
                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunStoreOutEquCmdFinish(IEnumerable<string> stations) 
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        if (CMD_MST.GetCmdMstByStoreOutFinish(stations, out var dataObject, db).ResultCode == DBResult.Success)
                        {
                            foreach (var cmdMst in dataObject.Data)
                            {
                                if (EQU_CMD.GetEquCmd(cmdMst.Cmd_Sno, out var equCmd, db).ResultCode == DBResult.Success)
                                {
                                    if (equCmd[0].ReNeqFlag != "F" && equCmd[0].CmdSts == "9")
                                    {
                                        string cmdsts="";
                                        string cmdabnormal="";
                                        string remark="";
                                        bool bflag=false;

                                        if (equCmd[0].CompleteCode == "92")//正常完成
                                        {
                                            cmdsts = CmdSts.CompleteWaitUpdate;
                                            cmdabnormal = "NA";
                                            remark = "存取車搬送命令完成";
                                            bflag = true;
                                        }
                                        else if (equCmd[0].CompleteCode.StartsWith("W"))
                                        {
                                            if (EQU_CMD.UpdateEquCmdRetry(equCmd[0].CmdSno, db).ResultCode != DBResult.Success)
                                            {
                                                return false;
                                            }
                                            bflag = false;
                                        }
                                        else if(equCmd[0].CompleteCode == clsEnum.Cmd_Abnormal.EF.ToString()) //地上盤強制取消 EF
                                        {
                                            cmdsts = CmdSts.CmdCancel;
                                            cmdabnormal = clsEnum.Cmd_Abnormal.EF.ToString();
                                            remark = "存取車地上盤強制取消命令";
                                            bflag = true;
                                        }
                                        else if (equCmd[0].CompleteCode == clsEnum.Cmd_Abnormal.FF.ToString()) //地上盤強制完成 FF
                                        {
                                            cmdsts = CmdSts.CompleteWaitUpdate;
                                            cmdabnormal = clsEnum.Cmd_Abnormal.FF.ToString();
                                            remark = "存取車地上盤強制完成命令";
                                            bflag=true;

                                        }
                                        if (bflag == true)
                                        {
                                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                            {
                                                return false;
                                            }
                                            if ((cmdMst.IOType != "3") || equCmd[0].CompleteCode == clsEnum.Cmd_Abnormal.EF.ToString())
                                            {
                                                if (CMD_MST.UpdateCmdMst(equCmd[0].CmdSno, cmdsts, Trace.StoreOutCraneCmdFinish, db) != ExecuteSQLResult.Success)
                                                {
                                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                                    return false;
                                                }
                                            }
                                            if (CMD_MST.UpdateCmdMstRemarkandAbnormal(equCmd[0].CmdSno, remark, cmdabnormal, db).ResultCode != DBResult.Success)
                                            {
                                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            if (EQU_CMD.DeleteEquCmd(equCmd[0].CmdSno, db).ResultCode != DBResult.Success)
                                            {
                                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                            return true;
                        }
                        else return false;
                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }
        
        #endregion StoreOut

        #region Other

        public bool FunLocToLoc()
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        if (CMD_MST.GetLocToLoc(out var dataObject, db).ResultCode == DBResult.Success)
                        {

                            string source = $"{dataObject[0].Loc}";
                            string dest = $"{dataObject[0].NewLoc}";
                            string cmdSno = $"{dataObject[0].Cmd_Sno}";
                            string IOtype = $"{dataObject[0].IO_Type}";
                            int CraneNo = Convert.ToInt32($"{dataObject[0].Equ_No}");


                            clsWriLog.L2LLogTrace(5, "LocToLoc", $"LocToLoc Command Received => {cmdSno}");

                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                            {
                                clsWriLog.L2LLogTrace(5, "LocToLoc", $"Create Crane LocToLoc Command, Begin Fail => {cmdSno}");
                                return false;
                            }
                            if (CMD_MST.UpdateCmdMstTransferring(cmdSno, Trace.LoctoLocReady, db).ResultCode != DBResult.Success)
                            {
                                clsWriLog.L2LLogTrace(5, "LocToLoc", $"Create Crane LocToLoc Command, Update CmdMst Fail => {cmdSno}");
                                
                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            if (EQU_CMD.InsertLocToLocEquCmd(5, "LocToLoc", CraneNo, cmdSno, source, dest, 1, db) == false)
                            {
                                clsWriLog.L2LLogTrace(5, "LocToLoc", $"Create Crane LocToLoc Command, Insert EquCmd Fail => {cmdSno}");
                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                            {
                                clsWriLog.L2LLogTrace(5, "LocToLoc", $"Create Crane LocToLoc Command Commit Fail => {cmdSno}");

                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            return true;
                            
                        }
                        else return false;
                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunLocToLocCmdFinish()
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        if (CMD_MST.GetLoctoLocFinish(out var dataObject, db).ResultCode == DBResult.Success)
                        {
                            foreach (var cmdMst in dataObject.Data)
                            {
                                if (EQU_CMD.GetEquCmd(cmdMst.Cmd_Sno, out var equCmd, db).ResultCode == DBResult.Success)
                                {
                                    if (equCmd[0].ReNeqFlag != "F" && equCmd[0].CmdSts == "9")
                                    {
                                        string cmdsts = "";
                                        string cmdabnormal = "";
                                        string remark = "";
                                        bool bflag = false;

                                        if (equCmd[0].CompleteCode == "92")//正常完成
                                        {
                                            cmdsts = CmdSts.CompleteWaitUpdate;
                                            cmdabnormal = "NA";
                                            remark = "存取車搬送命令完成";
                                            bflag = true;
                                        }
                                        else if (equCmd[0].CompleteCode.StartsWith("W"))
                                        {
                                            if (EQU_CMD.UpdateEquCmdRetry(equCmd[0].CmdSno, db).ResultCode != DBResult.Success)
                                            {
                                                return false;
                                            }
                                            bflag = false;
                                        }
                                        else if (equCmd[0].CompleteCode == clsEnum.Cmd_Abnormal.EF.ToString()) //地上盤強制取消 EF
                                        {
                                            cmdsts = CmdSts.CmdCancel;
                                            cmdabnormal = clsEnum.Cmd_Abnormal.EF.ToString();
                                            remark = "存取車地上盤強制取消命令";
                                            bflag = true;

                                        }
                                        else if (equCmd[0].CompleteCode == clsEnum.Cmd_Abnormal.FF.ToString()) //地上盤強制完成 FF
                                        {
                                            cmdsts = CmdSts.CompleteWaitUpdate;
                                            cmdabnormal = clsEnum.Cmd_Abnormal.FF.ToString();
                                            remark = "存取車地上盤強制完成命令";
                                            bflag = true;
                                        }
                                        if (bflag == true)
                                        {
                                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                            {
                                                return false;
                                            }
                                            if (CMD_MST.UpdateCmdMst(equCmd[0].CmdSno, cmdsts, Trace.LoctoLocReadyFinish, db).ResultCode != DBResult.Success)
                                            {
                                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            if (CMD_MST.UpdateCmdMstRemarkandAbnormal(equCmd[0].CmdSno, remark, cmdabnormal, db).ResultCode != DBResult.Success)
                                            {
                                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            if (EQU_CMD.DeleteEquCmd(equCmd[0].CmdSno, db).ResultCode != DBResult.Success)
                                            {
                                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                            return true;
                        }
                        else return false;
                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }
        #endregion


      


    }
}
