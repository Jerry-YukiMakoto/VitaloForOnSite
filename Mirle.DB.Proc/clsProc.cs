﻿using System;
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

        public bool FunMoveTaskForceClear(string taskNo, ref string strEM)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        CmdMstInfo cmd = new CmdMstInfo();
                        if (CMD_MST.FunGetCommand_byTaskNo(taskNo, ref cmd, db))
                        {
                            if (cmd.CmdSts == clsConstValue.CmdSts.strCmd_Running)
                            {
                                strEM = "Error: 命令已開始執行，無法取消！";
                                return false;
                            }

                            int iRet_Task = EQU_CMD.CheckHasEquCmd(cmd.CmdSno, db);
                            if (iRet_Task == DBResult.Exception)
                            {
                                strEM = "取得設備命令失敗！";
                                return false;
                            }



                            if (iRet_Task == DBResult.Success) EQU_CMD.FunInsertHisEquCmd(cmd.CmdSno, db);

                            if (db.TransactionCtrl(TransactionTypes.Begin) != DBResult.Success)
                            {
                                strEM = "Error: Begin失敗！";
                                if (strEM != cmd.Remark)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmd.CmdSno, strEM, db);
                                }

                                return false;
                            }

                            if (CMD_MST.UpdateCmdMstRemark(cmd.CmdSno, clsConstValue.CmdSts.strCmd_Cancel, "WMS命令取消", db).ResultCode != DBResult.Success)
                            {
                                db.TransactionCtrl(TransactionTypes.Rollback);
                                return false;
                            }

                            if (iRet_Task == DBResult.Success)
                            {
                                if (EQU_CMD.DeleteEquCmd(cmd.CmdSno, db).ResultCode != DBResult.Success)
                                {
                                    db.TransactionCtrl(TransactionTypes.Rollback);
                                    return false;
                                }
                            }

                            db.TransactionCtrl(TransactionTypes.Commit);
                            return true;
                        }
                        else
                        {
                            strEM = $"<taskNo> {taskNo} => 取得命令資料失敗！";
                            return false;
                        }
                    }
                    else
                    {
                        strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                strEM = ex.Message;
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        #region StoreIn
        public bool FunOutsourceStoreInWriPlc(string sStnNo, int bufferIndex)
        {
            string Item_No="";
            string Plt_Id="";
            string Lot_No="";
            string Loc = "";
            string Equ_No = "";
            string IsHigh = "";
            string[] Cranests = new string[6];
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        if (CMD_MST.GetCmdMstByStoreInStart(sStnNo,Item_No,Lot_No ,out var dataObject, db).ResultCode == DBResult.Success) //讀取CMD_MST
                        {

                            string cmdSno = dataObject[0].Cmd_Sno;
                            int CmdMode = Convert.ToInt32(dataObject[0].Cmd_Mode);
                            int IOType = Convert.ToInt32(dataObject[0].IO_Type);
                            var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                            

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
                            if (_conveyor.GetBuffer(bufferIndex).Ready != Ready.StoreInReady)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotStoreInReady, db);
                                return false;
                            }
                            #endregion

                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready Receive StoreIn Command=> {cmdSno}");

                            if (EQU_CMD.GetEquStatus( out var dataObject1, db).ResultCode == DBResult.Success)
                            {
                                for(int i=0; i< dataObject1.Count;i++)
                                {  
                                   Cranests[i]=dataObject1[i].EquNo;
                                }    
                            }

                            if(IsHigh=="H")
                            {
                                if (Loc_Mst.GetLocMst_EmptyLochigh(Equ_No, out var dataObject2, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find Loc succeess => {cmdSno}");
                                    Loc = dataObject2[0].Loc;
                                }
                            }
                            else
                            {
                                if (Loc_Mst.GetLocMst_EmptyLoc(Equ_No, out var dataObject2, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find Loc succeess => {cmdSno}");
                                    Loc = dataObject2[0].Loc;
                                }
                            }

                            
                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "begin fail");
                                return false;
                            }
                            if (CMD_MST.UpdateCmdMstTransferring(cmdSno, Trace.StoreInWriteCmdToCV,Plt_Id, db).ResultCode == DBResult.Success)
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
                            if (Loc_Mst.UpdateStoreInLocMst(Loc, db).ResultCode == DBResult.Success)
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
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
        }

        public bool FunStoreInA2ToA4WriPlc(string sStnNo, int bufferIndex)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        if (CMD_MST.GetCmdMstByStoreInStart(sStnNo, out var dataObject, db).ResultCode == DBResult.Success)
                        {
                            string cmdSno = dataObject[0].Cmd_Sno;
                            string IOType = dataObject[0].IOType;
                            int CmdMode = Convert.ToInt32(dataObject[0].CmdMode);
                            var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();

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
                            if (_conveyor.GetBuffer(bufferIndex).CmdMode == 6 || _conveyor.GetBuffer(bufferIndex - 1).CmdMode == 6)//為了不跟撿料命令衝突的條件
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.CycleOperating, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).Presence == true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.PresenceExist, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex - 1).Ready != Ready.StoreInReady)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotStoreInReady, db);
                                return false;
                            }
                            #endregion

                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready Receive StoreIn Command=> {cmdSno}");

                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "begin fail");
                                return false;
                            }
                            if (CMD_MST.UpdateCmdMstTransferring(cmdSno, Trace.StoreInWriteCmdToCV, db).ResultCode == DBResult.Success)
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd succeess => {cmdSno}");
                            }
                            else
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd fail => {cmdSno}");

                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            var WritePlccheck = _conveyor.GetBuffer(bufferIndex).WriteCommandIdAsync(cmdSno, CmdMode).Result;//確認寫入PLC的方法是否正常運作，傳回結果和有異常的時候的訊息
                            bool Result = WritePlccheck;
                            if (Result != true)//寫入命令和模式
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
                            else
                            {
                                DisplayTaskStatusInfo info = new DisplayTaskStatusInfo
                                {
                                    //填入回報訊息
                                    lineId = "1",
                                    locationID = ((bufferIndex-2)/2).ToString(),
                                    taskNo = cmdSno.ToString(),
                                    state = "1", //任務開始
                                };
                                if (!clsWmsApi.GetApiProcess().GetDisplayTaskStatus().FunReport(info))
                                {
                                    return false;
                                }
                                //填入訊息
                                TaskStateUpdateInfo info1 = new TaskStateUpdateInfo
                                {
                                    lineId = "1",
                                    taskNo = cmdSno,
                                    palletNo = cmdSno,
                                    businessType = IOType.ToString(),
                                    state = "12",
                                    errMsg = ""
                                };
                                if (!clsWmsApi.GetApiProcess().GetTaskStateUpdate().FunReport(info1))
                                {
                                    db.TransactionCtrl(TransactionTypes.Rollback);
                                    return false;
                                }
                                return true;
                            }
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

        public bool FunStoreInCreateEquCmd(int bufferIndex)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        string cmdSno = (_conveyor.GetBuffer(bufferIndex).CommandId).ToString();

                        if (CMD_MST.GetCmdMstByStoreInCrane(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)
                        {
                            //clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "Buffer StoreIn Get Command");

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

                            string source = $"{CranePortNo.A1}";
                            string IOType = dataObject[0].IOType;
                            string dest = "";
                            if (IOType == IOtype.NormalstoreOut.ToString() )//如果是撿料，入庫儲位欄位是LOC，一般入庫是NewLoc
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
                            if (EQU_CMD.InsertStoreInEquCmd(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, 1, cmdSno, source, dest, 5, db) == false)
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

        public bool FunStoreInA2toA4CreateEquCmd(int bufferIndex)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        
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
                            if (bufferIndex == 5)
                            {
                                source = $"{CranePortNo.A5}";
                            }
                            else if (bufferIndex == 7)
                            {
                                source = $"{CranePortNo.A7}";
                            }
                            else if (bufferIndex == 9)
                            {
                                source = $"{CranePortNo.A9}";
                            }
                            string IOType = dataObject[0].IOType;
                            string dest = "";

                            if (IOType == IOtype.NormalstoreOut.ToString())//如果是撿料，入庫儲位欄位是LOC，一般入庫是NewLoc
                            {
                                dest = $"{dataObject[0].Loc}";
                            }
                            else
                            {
                                dest = $"{dataObject[0].NewLoc}";
                            }
                            //clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "Buffer StoreIn Get Command");

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
                            if (EQU_CMD.InsertStoreInEquCmd(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, 1, cmdSno, source, dest, 5, db) == false)
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
                    StnNo.A6,
                    StnNo.A8,
                    StnNo.A10,
                };
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();

                        if (CMD_MST.GetCmdMstByStoreInFinish(stn1, out var dataObject,db).ResultCode == DBResult.Success)
                        {
                            foreach (var cmdMst in dataObject.Data)
                            {
                                string locationId = cmdMst.StnNo;
                                if(locationId==StnNo.A6)
                                {
                                    locationId = "1";
                                }
                                else if(locationId==StnNo.A6)
                                {
                                    locationId = "2";
                                }
                                else if(locationId == StnNo.A8)
                                {
                                    locationId = "3";
                                }
                                else if(locationId == StnNo.A10)
                                {
                                    locationId = "4";
                                }

                                if (EQU_CMD.GetEquCmd(cmdMst.CmdSno, out var equCmd,db).ResultCode == DBResult.Success)
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

                                            //填入訊息
                                            TaskStateUpdateInfo info1 = new TaskStateUpdateInfo
                                            {
                                                lineId = "1",
                                                taskNo = cmdMst.CmdSno,
                                                palletNo = cmdMst.CmdSno,
                                                businessType = cmdMst.IOType,
                                                state = "13",
                                                errMsg =""
                                            };
                                            if(!clsWmsApi.GetApiProcess().GetTaskStateUpdate().FunReport(info1))
                                            {
                                                db.TransactionCtrl(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            DisplayTaskStatusInfo info = new DisplayTaskStatusInfo
                                            {
                                                //填入回報訊息
                                                lineId = "1",
                                                locationID = locationId,
                                                taskNo = cmdMst.CmdSno,
                                                state = "2", //任務結束
                                            };
                                            if (!clsWmsApi.GetApiProcess().GetDisplayTaskStatus().FunReport(info))
                                            {
                                                return false;
                                            }
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

                                            //填入訊息
                                            TaskStateUpdateInfo info = new TaskStateUpdateInfo
                                            {
                                                lineId = "1",
                                                taskNo = cmdMst.CmdSno,
                                                palletNo = cmdMst.CmdSno,
                                                businessType = cmdMst.IOType,
                                                state = "15",
                                                errMsg = ""
                                            };
                                            if (!clsWmsApi.GetApiProcess().GetTaskStateUpdate().FunReport(info))
                                            {
                                                db.TransactionCtrl(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            DisplayTaskStatusInfo info1 = new DisplayTaskStatusInfo
                                            {
                                                //填入回報訊息
                                                lineId = "1",
                                                locationID = locationId,
                                                taskNo = cmdMst.CmdSno,
                                                state = "2", //任務結束
                                            };
                                            if (!clsWmsApi.GetApiProcess().GetDisplayTaskStatus().FunReport(info1))
                                            {
                                                return false;
                                            }
                                        }
                                        else if (equCmd[0].CompleteCode == clsEnum.Cmd_Abnormal.FF.ToString()) //地上盤強制完成 FF
                                        {
                                            cmdsts = CmdSts.CompleteWaitUpdate;
                                            cmdabnormal = clsEnum.Cmd_Abnormal.FF.ToString();
                                            remark = "存取車地上盤強制完成命令";
                                            bflag = true;

                                            //填入訊息
                                            TaskStateUpdateInfo info = new TaskStateUpdateInfo
                                            {
                                                lineId = "1",
                                                taskNo = cmdMst.CmdSno,
                                                palletNo = cmdMst.CmdSno,
                                                businessType = cmdMst.IOType,
                                                state = "14",
                                                errMsg = ""
                                            };
                                            if (!clsWmsApi.GetApiProcess().GetTaskStateUpdate().FunReport(info))
                                            {
                                                db.TransactionCtrl(TransactionTypes.Rollback);
                                                return false;
                                            }
                                            DisplayTaskStatusInfo info1 = new DisplayTaskStatusInfo
                                            {
                                                //填入回報訊息
                                                lineId = "1",
                                                locationID = locationId,
                                                taskNo = cmdMst.CmdSno,
                                                state = "2", //任務結束
                                            };
                                            if (!clsWmsApi.GetApiProcess().GetDisplayTaskStatus().FunReport(info1))
                                            {
                                                return false;
                                            }
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
