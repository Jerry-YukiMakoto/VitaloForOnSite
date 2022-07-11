using System;
using System.Collections.Generic;
using Mirle.ASRS.WCS.Controller;
using Mirle.DB.Fun;
using Mirle.Def;
using Mirle.DataBase;
using Mirle.Def.T1JUMAK0;
using Mirle.Grid.T1JUMAK0;
using Mirle.Structure;
using Mirle.Structure.Info;
using System.Linq;
using System.Data;
using Mirle.ASRS.WCS.Model.PLCDefinitions;
using Mirle.ASRS.WCS.Model.DataAccess;
using System.Text.RegularExpressions;

namespace Mirle.DB.Proc
{
    public class clsProc
    {
        private clsPortDef PortDef = new clsPortDef();
        private Fun.clsCmd_Mst CMD_MST = new Fun.clsCmd_Mst();
        private Fun.clsEqu_Cmd EQU_CMD = new Fun.clsEqu_Cmd();
        private Fun.clsLocMst Loc_Mst = new Fun.clsLocMst();
        private Fun.clsItmMst Itm_Mst = new Fun.clsItmMst();
        private Fun.clsMsv_Mst MVS_Mst = new Fun.clsMsv_Mst();
        private Fun.clsSno SNO = new Fun.clsSno();
        private Fun.clsLocMst LocMst = new Fun.clsLocMst();
        private Fun.clsProc proc;
        private Fun.clsAlarmData alarmData = new Fun.clsAlarmData();
        private Fun.clsCmd_Mst_His CMD_MST_HIS = new Fun.clsCmd_Mst_His();
        private Fun.clsUnitStsLog unitStsLog = new Fun.clsUnitStsLog();
        public static Dictionary<string, int> dicCountByCrane = new Dictionary<string, int>();
        

        public List<Element_Port>[] GetLstPort()
        {
            return PortDef.GetLstPort();
        }

        private clsDbConfig _config = new clsDbConfig();
        private clsDbConfig _config_WMS = new clsDbConfig();
        private clsDbConfig _config_Sqlite = new clsDbConfig();
        public clsProc(clsDbConfig config, clsDbConfig config_Sqlite)
        {
            _config = config;
            _config_Sqlite = config_Sqlite;
            dicCountByCrane.Add("1", 1);
            dicCountByCrane.Add("2", 1);
            dicCountByCrane.Add("3", 1);
            dicCountByCrane.Add("4", 1);
            dicCountByCrane.Add("5", 1);
            dicCountByCrane.Add("6", 1);
        }

        public Fun.clsProc GetFunProcess()
        {
            return proc;
        }

        #region StoreIn
        public bool FunOutsourceStoreInWriPlc(string sStnNo, int bufferIndex)//委外入庫與盤點入庫共用流程
        {
            string Item_No="";
            string Plt_Id="";
            string Lot_No="";
            string Loc = "";
            string Equ_No = "";
            int path = 0;
            int IsHigh = 0;
            string cmdSno="";
            int CmdMode=0;
            int IOType;
            string cmdPlt_Id;
            bool cmdcheck = false;
            string Plt_Qty="";
            string Item_Type = "";
            bool IsCycle=false;
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
                            Item_No = Regex.Replace(_conveyor.GetBuffer(bufferIndex).Item_No.Trim(), @"[^A-Z,a-z,0-9]", string.Empty);
                            Plt_Id = Regex.Replace(_conveyor.GetBuffer(bufferIndex).Plt_Id.Trim(), @"[^A-Z,a-z,0-9]", string.Empty);
                            Lot_No = Regex.Replace(_conveyor.GetBuffer(bufferIndex).Lot_ID.Trim(), @"[^A-Z,a-z,0-9]", string.Empty);
                            bool Pltfish= Plt_Id.Contains("-");//如果是餘板會有槓槓來與滿板分別，可以根據此條件尋找餘板的命令

                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"BCR Notice Start:Item_No=>{Item_No} Plt_Id=>{Plt_Id} Lot_No=>{Lot_No}");//當讀取通知開始，紀錄讀到的的值

                            if (CMD_MST.GetCmdMstByStoreInStart(sStnNo, Item_No, Lot_No, Pltfish, Plt_Id, out var dataObject1, db).ResultCode == DBResult.Success)
                            {
                                cmdSno = dataObject1[0].Cmd_Sno;
                                CmdMode = Convert.ToInt32(dataObject1[0].Cmd_Mode);
                                IOType = Convert.ToInt32(dataObject1[0].IO_Type);
                                cmdPlt_Id = dataObject1[0].Plt_Id;
                                Plt_Qty = dataObject1[0].Plt_Qty;
                                cmdcheck = true;
                            }
                            else if (CMD_MST.GetCmdMstByCycleStart(sStnNo, Item_No, Lot_No, Pltfish, Plt_Id, out var dataObject, db).ResultCode == DBResult.Success)
                            {
                                cmdSno = dataObject[0].Cmd_Sno;
                                CmdMode = Convert.ToInt32(dataObject[0].Cmd_Mode);
                                IOType = Convert.ToInt32(dataObject[0].IO_Type);
                                Equ_No = dataObject[0].Equ_No;
                                cmdPlt_Id = dataObject[0].Plt_Id;
                                Plt_Qty = dataObject[0].Plt_Qty;
                                Loc = dataObject[0].Loc;
                                IsCycle = true;//確認是盤點命令參數
                                cmdcheck = true;
                            }
                            else if (CMD_MST.GetCmdMstByStoreInStart(sStnNo, Item_No, Lot_No, out var dataObject2, db).ResultCode == DBResult.Success)
                            {
                                cmdSno = dataObject1[0].Cmd_Sno;
                                CmdMode = Convert.ToInt32(dataObject1[0].Cmd_Mode);
                                IOType = Convert.ToInt32(dataObject1[0].IO_Type);
                                cmdPlt_Id = dataObject1[0].Plt_Id;
                                Plt_Qty = dataObject1[0].Plt_Qty;
                                cmdcheck = true;
                            }
                            else//都蒐集不到資料執行退板
                            {
                                cmdcheck = false;
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Can't Find Cmd Please Check ");
                            }

                            if (string.IsNullOrWhiteSpace(Plt_Qty)) //數量並未填入，發出異常(像餘板一定要額外輸)
                            {
                                cmdcheck = false;
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Can't Find Plt_Qty Please Check ");
                            }

                            if(Itm_Mst.GetItmMstDtl(Item_No, out var dataObject3,db).ResultCode==DBResult.Success)
                            {
                                Item_Type = dataObject3[0].Item_Type;//根據料號的群組決定儲位的放法
                            }else
                            {
                                cmdcheck=false;
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Can't find Itm_Grp Pease Check ");
                            }


                            if (cmdcheck) //都蒐集不到資料執行退板
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Get StoreIn Command => {cmdSno}, " +
                                        $"{CmdMode}");

                                #region//根據buffer狀態更新命令
                                if (_conveyor.GetBuffer(bufferIndex).Auto != true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotAutoMode, db);
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
                                if (_conveyor.GetBuffer(bufferIndex).Presence != true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.PresenceNotExist, db);
                                    return false;
                                }
                                #endregion

                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready Receive StoreIn Command=> {cmdSno}");

                                if (!IsCycle)//盤點不需尋找新儲位，盤點會回到原本的儲位
                                {

                                    Equ_No = GetEquNo();//照順序選擇線別
                                    int Sequ_No = Convert.ToInt32(Equ_No);
                                    for (int i = 0; i < 6; i++)//從起始的線別開始尋找是否有線別異常
                                    {
                                        if (EQU_CMD.GetEquStatus(Sequ_No, out var dataObject4, db).ResultCode == DBResult.NoDataSelect)
                                        {
                                            if (Sequ_No != 6)
                                            {
                                                Sequ_No %= 6;
                                            }
                                            Equ_No = Sequ_No.ToString();
                                            break;
                                        }
                                        Sequ_No++;
                                    }

                                    IsHigh = _conveyor.GetBuffer(bufferIndex).LoadHeight;//根據荷高去選儲位位置

                                    if (IsHigh == 1)
                                    {
                                        if (Loc_Mst.GetLocMst_EmptyLochigh(Equ_No, Item_Type, out var dataObject2, db).ResultCode == DBResult.Success)
                                        {
                                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find High Loc succeess => {cmdSno}");
                                            Loc = dataObject2[0].Loc;
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
                                        if (Loc_Mst.GetLocMst_EmptyLoc(Equ_No, Item_Type, out var dataObject2, db).ResultCode == DBResult.Success)
                                        {
                                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find Loc succeess => {cmdSno}");
                                            Loc = dataObject2[0].Loc;
                                        }
                                        else if(Loc_Mst.GetLocMst_EmptyLochigh(Equ_No, Item_Type, out var dataObject4, db).ResultCode == DBResult.Success)
                                        {
                                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find Loc succeess => {cmdSno}");
                                            Loc = dataObject4[0].Loc;
                                        }
                                        else
                                        {
                                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                                , $"Find Loc fail");
                                            return false;
                                        }
                                    }

                                }

                                path = StoreInfindpathbyEquNo(Convert.ToInt32(Equ_No));//根據線別選入庫站

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "begin fail");
                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMstTransferring(cmdSno, Trace.StoreInWriteCmdToCV, Loc, Plt_Id, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd_mst succeess => {cmdSno}");
                                }
                                else
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte cmd_mst fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (!IsCycle)//盤點不須更新明細檔
                                {
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
                                }
                                if (!IsCycle)//盤點入庫不須更新Loc資料表
                                {
                                    if (Loc_Mst.UpdateStoreInLocMst(Loc, db).ResultCode == DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Update Loc succeess => {cmdSno}");
                                    }
                                }
                                var WritePlccheck = _conveyor.GetBuffer(bufferIndex).WriteCommandIdAsync(cmdSno, CmdMode).Result;//確認寫入PLC的方法是否正常運作，傳回結果和有異常的時候的訊息
                                bool Result = WritePlccheck;
                                if (Result != true)//寫入命令和模式
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).WritePathChabgeNotice(path).Result;
                                Result = WritePlccheck;
                                if (Result != true)//路徑編號寫入
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Path-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).BCRNoticeComplete(1).Result;
                                Result = WritePlccheck;
                                if (Result != true)//通知讀取完成
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC BCR-Notice Fail");

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
                            else
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "OutSource WithDraw Start");
                                path = 31;//退板
                                var WritePlccheck = _conveyor.GetBuffer(bufferIndex).WritePathChabgeNotice(path).Result;
                                bool Result = WritePlccheck;
                                if (Result != true)//路徑編號寫入
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC WithDraw-Path-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).BCRNoticeComplete(1).Result;
                                Result = WritePlccheck;
                                if (Result != true)//通知讀取完成
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC With Draw BCR-Notice Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                else return true;
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

        public bool FunProduceStoreInWriPlc(string sStnNo, int bufferIndex)//生產入庫
        {
            string Item_No = "";
            string Plt_Id = "";
            string Lot_No = ""; //效期
            string Loc = "";
            string Equ_No = "";
            int IsHigh = 0;
            string cmdSno="";
            string Item_Desc = "";
            string Item_Unit = "";
            string Item_Type = "";
            string Item_Grp = "";
            string Qty_Plt ="";
            int path = 0;
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
                            Item_No = Regex.Replace(_conveyor.GetBuffer(bufferIndex).Item_No.Trim(), @"[^A-Z,a-z,0-9]", string.Empty);
                            Plt_Id = Regex.Replace(_conveyor.GetBuffer(bufferIndex).Plt_Id.Trim(), @"[^A-Z,a-z,0-9]", string.Empty);
                            Lot_No = Regex.Replace(_conveyor.GetBuffer(bufferIndex).Lot_ID.Trim(), @"[^A-Z,a-z,0-9]", string.Empty);

                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"BCR Notice Start:Item_No=>{Item_No} Plt_Id=>{Plt_Id} Lot_No=>{Lot_No}");//當讀取通知開始，紀錄讀到的的值

                            #region//根據buffer狀態更新命令
                            if (_conveyor.GetBuffer(bufferIndex).Auto != true)
                                {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                        , $"Auto Not On");
                                return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).Error == true)
                                {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                    , $"Error ON");
                                return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).CommandId > 0 || _conveyor.GetBuffer(bufferIndex).PCCommandId > 0)
                                {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                     , $"CommanId exist On Buffer");
                                return false;
                                }
                                if (_conveyor.GetBuffer(bufferIndex).Presence == false)
                                {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                    , $" No Presence PLease Check");
                                return false;
                                }
                                #endregion

                            clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready Receive StoreIn Command=> {cmdSno}");

                            if (Itm_Mst.GetItmMstDtl(Item_No, out var dataObject3, db).ResultCode == DBResult.Success && Plt_Id!="" && Lot_No!="")//當搜尋不到料號資料(和效期或版號是空值時)，執行退版動作
                            {
                                Item_Desc = dataObject3[0].Item_Desc;
                                Item_Unit = dataObject3[0].Item_Unit;
                                Item_Type = dataObject3[0].Item_Type;//根據料號的群組決定儲位的放法 U高價位/D低價位
                                Qty_Plt = dataObject3[0].Qty_Plt;
                                Item_Grp = dataObject3[0].Item_Grp;

                                Equ_No = GetEquNo();//貨物照線別順序輪流放
                                int Sequ_No = Convert.ToInt32(Equ_No);
                                for (int i = 0; i < 6; i++)//從輪流放的的線別排除有異常的Crane
                                {
                                    if (EQU_CMD.GetEquStatus(Sequ_No, out var dataObject1, db).ResultCode == DBResult.NoDataSelect)
                                    {
                                        if (Sequ_No != 6)
                                        {
                                            Sequ_No %= 6;
                                        }
                                        Equ_No = Sequ_No.ToString();
                                        break;
                                    }
                                    Sequ_No++;
                                }

                                IsHigh = _conveyor.GetBuffer(bufferIndex).LoadHeight;//根據荷高去選儲位位置，高荷高只找高儲位，低荷高先找低再找高

                                if (IsHigh == 1)
                                {
                                    if (Loc_Mst.GetLocMst_EmptyLochigh(Equ_No,Item_Type ,out var dataObject2, db).ResultCode == DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find High Loc success");
                                        Loc = dataObject2[0].Loc;
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
                                    if (Loc_Mst.GetLocMst_EmptyLoc(Equ_No,Item_Type, out var dataObject2, db).ResultCode == DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find Loc success");
                                        Loc = dataObject2[0].Loc;
                                    }
                                    else if (Loc_Mst.GetLocMst_EmptyLochigh(Equ_No, Item_Type, out var dataObject4, db).ResultCode == DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Find Loc succeess");
                                        Loc = dataObject4[0].Loc;
                                    }
                                    else
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find Loc fail");
                                        return false;
                                    }
                                }

                                cmdSno = SNO.FunGetSeqNo(clsEnum.enuSnoType.CMDSNO, db); //尋找最新不重複的命令號
                                if (cmdSno == "" || cmdSno == "00000")
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName
                                            , $"Find cmdSno fail");
                                    return false;
                                }

                                struCmdMst stuCmdMst = new struCmdMst(); //生產由WCS建立命令
                                stuCmdMst.CmdSno = cmdSno;
                                stuCmdMst.CmdSts = "1";
                                stuCmdMst.CmdAbnormal = "NA";
                                stuCmdMst.Prty = "5";
                                stuCmdMst.StnNo = sStnNo;
                                stuCmdMst.CmdMode = "1";
                                stuCmdMst.IoType = "13";
                                stuCmdMst.WhId = "ASRS";
                                stuCmdMst.EquNo = Equ_No;
                                stuCmdMst.Loc = Loc;
                                stuCmdMst.NewLoc = "";
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
                                stuCmdDtl.Plt_Qty = Convert.ToDouble(Qty_Plt);
                                stuCmdDtl.Trn_Qty = Convert.ToDouble(Qty_Plt);
                                stuCmdDtl.In_Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                stuCmdDtl.Item_No = Item_No;
                                stuCmdDtl.Lot_No = Lot_No;
                                stuCmdDtl.Plt_Id = Plt_Id;
                                stuCmdDtl.Company_ID = "ASRS";
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
                                if (Loc_Mst.UpdateStoreInLocMst(Loc, db).ResultCode == DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Update StoreIn Loc succeess => {cmdSno}");
                                }
                                else
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Update StoreIn Loc fail => {cmdSno}");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }

                                var WritePlccheck = _conveyor.GetBuffer(bufferIndex).WriteCommandIdAsync(cmdSno, 1).Result;//確認寫入PLC的方法是否正常運作，傳回結果和有異常的時候的訊息
                                bool Result = WritePlccheck;
                                if (Result != true)//寫入命令和模式
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }

                                path = StoreInfindpathbyEquNo(Convert.ToInt32(Equ_No));//根據線別選入庫站

                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).WritePathChabgeNotice(path).Result;
                                Result = WritePlccheck;
                                if (Result != true)//寫入路徑
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Path-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).BCRNoticeComplete(1).Result;
                                Result = WritePlccheck;
                                if (Result != true)//通知讀取完成
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC BCR-Notice Fail");

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
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "Produce WithDraw Start");

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
                                stuCmdMst.Loc = "";
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
                                stuCmdDtl.Lot_No = Lot_No;
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
                                var WritePlccheck = _conveyor.GetBuffer(bufferIndex).WriteCommandIdAsync(cmdSno, 9).Result;//確認寫入PLC的方法是否正常運作，傳回結果和有異常的時候的訊息
                                bool Result = WritePlccheck;
                                if (Result != true)//寫入命令和模式
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Command-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).WritePathChabgeNotice(31).Result; //退版路徑編號
                                Result = WritePlccheck;
                                if (Result != true)//寫入路徑
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Path-mode Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).BCRNoticeComplete(1).Result;
                                Result = WritePlccheck;
                                if (Result != true)//通知讀取完成
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC BCR-Notice Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "Commit Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }


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

        public bool StoreIn_RejectFinish(int bufferIndex)//生產入庫口退版到退版出口時，更新命令為完成
        {

            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        if (_conveyor.GetBuffer(bufferIndex).CommandId > 0 && _conveyor.GetBuffer(bufferIndex).Presence)//當有貨物到退版站口時，更新命令使命令結束
                        {
                            string cmdSno = (_conveyor.GetBuffer(bufferIndex).CommandId).ToString().PadLeft(5, '0');

                            if (CMD_MST.GetCmdMstByStoreInReject(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)//搜尋退版命令
                            {

                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To Reject=> {cmdSno}");

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "(StoreIn)Buffer Get cmdsno To Reject, Begin Fail");

                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMst(cmdSno, CmdSts.CompleteWaitUpdate, Trace.StoreInReject, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To Reject, Update CmdMst Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.StoreInRejectFinish, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To Reject, Update CmdMst Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To Reject, Commit Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                else return true;

                            }
                            else return false;
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

        public bool StoreIn_ShowOnKanBan(string sStnNo,int bufferIndex)//為了看板程式，更新命令Trace，使其可以顯示此命令
        {

            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        if (_conveyor.GetBuffer(bufferIndex).CommandId > 0 && _conveyor.GetBuffer(bufferIndex).Presence)//當有貨物在入庫站口時，更新命令使看板開始顯示
                        {
                            string cmdSno = (_conveyor.GetBuffer(bufferIndex).CommandId).ToString().PadLeft(5, '0');

                            if (CMD_MST.GetCmdMstByStoreInForKanBan(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)//搜尋尚未顯示於看板上的貨物命令
                            {

                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To show KanBan=> {cmdSno}");

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "(StoreIn)Buffer Get cmdsno To show KanBan, Begin Fail");

                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMst(cmdSno, Trace.StoreInKanBanStart, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To show KanBan, Update CmdMst Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.StoreInKanBanStart , db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To show KanBan, Update CmdMst Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (MVS_Mst.ShowMVS(cmdSno, StnNo.A11_06, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To show KanBan, Update MVS_MST Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To show KanBan, Commit Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                else return true;

                            }
                            else return false;
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

        public bool StoreIn_ShowOnKanBanFinish(string sStnNo, int bufferIndex)//為了看板程式，更新命令Trace，使其可以停止顯示此命令
        {

            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        if (_conveyor.GetBuffer(bufferIndex).CommandId > 0 && !_conveyor.GetBuffer(bufferIndex).Presence)//當貨物離開入庫站口時，更新命令使看板停止顯示
                        {
                            string cmdSno = (_conveyor.GetBuffer(bufferIndex).CommandId).ToString().PadLeft(5, '0');

                            if (CMD_MST.GetCmdMstByStoreInForKanBanFinish(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)//搜尋已經顯示於看板上的貨物命令
                            {

                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To stop showing KanBan=> {cmdSno}");

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "(StoreIn)Buffer Get cmdsno To stop showing KanBan, Begin Fail");

                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMst(cmdSno, Trace.StoreInKanBanFinish, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To stop showing KanBan, Update CmdMst Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.StoreInKanBanFinsh, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To stop showing KanBan, Update CmdMst Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (MVS_Mst.ShowMVS(" ", StnNo.A11_06, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To stop showing KanBan, Update MVS_MST Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreIn)Buffer Get cmdsno To stop showing KanBan, Commit Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                else return true;

                            }
                            else return false;
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
        public bool FunStoreIn_BcrCheck(int bufferIndex)
        {
            string Item_No = "";
            string Plt_Id = "";
            string Lot_No = "";

            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor2();
                        if (_conveyor.GetBuffer(bufferIndex).BcrNotice == 1)
                        {
                            string cmdSno = (_conveyor.GetBuffer(bufferIndex).CommandId).ToString().PadLeft(5, '0');
                            Item_No = _conveyor.GetBuffer(bufferIndex).Item_No;
                            Plt_Id = _conveyor.GetBuffer(bufferIndex).Plt_Id;
                            Lot_No = _conveyor.GetBuffer(bufferIndex).Lot_ID;
                            if (CMD_MST.GetCmdMstAndDtlCheck(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)
                            {
                                #region//根據buffer狀態更新命令
                                if (_conveyor.GetBuffer(bufferIndex).Auto != true)
                                {
                                    CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.NotAutoMode, db);
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

                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready BCRCheck=> {cmdSno}");

                                string IOType = dataObject[0].IO_Type;
                                string dest = "";
                                string Cmd_mode = dataObject[0].Cmd_Mode;

                                if(Item_No!=dataObject[0].Item_No || Plt_Id!=dataObject[0].Plt_Id || Lot_No!=dataObject[0].Lot_No)
                                {
                                    //掃到與資料庫不一樣做異常處理

                                }

                                if (Cmd_mode == "3")//如果是撿料，入庫儲位欄位是LOC，一般入庫是NewLoc
                                {
                                    dest = $"{dataObject[0].Loc}";
                                }
                                else
                                {
                                    dest = $"{dataObject[0].NewLoc}";
                                }

                                var WritePlccheck = _conveyor.GetBuffer(bufferIndex).BCRNoticeComplete(1).Result;
                                bool Result = WritePlccheck;
                                if (Result != true)//通知讀取完成
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC BCR-Notice Fail");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                else return true;

                            }
                            else return false;
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


        public bool FunStoreIn_CreateEquCmd(int bufferIndex)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor2();
                        
                        string cmdSno = (_conveyor.GetBuffer(bufferIndex).CommandId).ToString().PadLeft(5, '0');
                        string dest = "";
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

                            string IOType = dataObject[0].IO_Type;
                            
                            string Cmd_mode = dataObject[0].Cmd_Mode;
                            int EquNo = 0;
                            bool EquCheck = true;//檢查到站口的貨物是否符合命令號的目的地，不符合退板處理

                            if (Cmd_mode == "3")
                            {
                                dest = $"{dataObject[0].Loc}";
                            }
                            else
                            {
                                dest = $"{dataObject[0].Loc}";
                            }

                            if(bufferIndex == 1)
                            {
                                EquNo = 1;
                            }
                            else if(bufferIndex == 3)
                            {
                                EquNo= 2;
                            }
                            else if(bufferIndex==5)
                            {
                                EquNo = 3;
                            }
                            else if (bufferIndex == 7)
                            {
                                EquNo = 4;
                            }
                            else if (bufferIndex == 9)
                            {
                                EquNo = 5;
                            }
                            else if (bufferIndex == 11)
                            {
                                EquNo = 6;
                            }

                            if ((dest.Substring(0,2)=="01"|| dest.Substring(0, 2) == "02") && EquNo!=1)
                            {
                                EquCheck= false;
                            }
                            else if ((dest.Substring(0, 2) == "03" || dest.Substring(0, 2) == "04") && EquNo != 2)
                            {
                                EquCheck = false;
                            }
                            else if ((dest.Substring(0, 2) == "05" || dest.Substring(0, 2) == "06") && EquNo != 3)
                            {
                                EquCheck = false;
                            }
                            else if ((dest.Substring(0, 2) == "07" || dest.Substring(0, 2) == "08") && EquNo != 4)
                            {
                                EquCheck = false;
                            }
                            else if ((dest.Substring(0, 2) == "09" || dest.Substring(0, 2) == "10") && EquNo != 5)
                            {
                                EquCheck = false;
                            }
                            else if ((dest.Substring(0, 2) == "11" || dest.Substring(0, 2) == "12") && EquNo != 6)
                            {
                                EquCheck = false;
                            }

                            if (EquCheck == true)
                            {

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
                                if (EQU_CMD.InsertStoreInEquCmd(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, EquNo, cmdSno, CranePortNo.Floor2, dest, 5, db) == false)
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
                            else
                            {
                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"LOC and EQU Abnormal, Start Update Command to 退版  => {cmdSno}");

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "Create  STS Abnormal Command, Begin Fail");

                                    return false;
                                }
                                if (Loc_Mst.UpdateStoreInLocMstAbnormal(dest, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create STS Abnormal Command, Update LocMst Fail => {cmdSno}");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                string abnormal_IOtype="";//退版分成生產入庫退版和單據入庫退版
                                if(IOType==IOtype.ProduceStoreIn.ToString())
                                {
                                    abnormal_IOtype = IOtype.abnormalOUT.ToString();
                                }
                                else if(IOType==IOtype.ErpStoreIn.ToString())
                                {
                                    abnormal_IOtype = IOtype.ERPtktabnormalOUT.ToString();
                                }
                                else
                                {
                                    abnormal_IOtype = IOtype.abnormalOUT.ToString();
                                }

                                if (CMD_MST.UpdateCmdMstabnormal(cmdSno, "2", Trace.StoreInAbnormaStartd,abnormal_IOtype, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create STS Abnormal Command, Update CmdMst Fail => {cmdSno}");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (CMD_MST.UpdateCmdDtlabnormal(cmdSno, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create STS Abnormal Command, Update CmdDtl Fail => {cmdSno}");
                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create STS Abnormal Command, Commit Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                else return true;
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

        public bool FunSTS_Abonormal_WritePLC(int bufferIndex)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor2 = ControllerReader.GetCVControllerr().GetConveryor2();
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        string cmd_Sno = _conveyor2.GetBuffer(bufferIndex).CommandId.ToString().PadLeft(5, '0');

                        if (CMD_MST.GetCmdMstBySTSCrane(cmd_Sno, out var dataObject, db).ResultCode == DBResult.Success)
                        {
                            cmd_Sno = dataObject[0].Cmd_Sno;
                            string source = CranePortNo.Floor2;
                            int Cmd_Mode = Convert.ToInt32(dataObject[0].Cmd_Mode);

                            #region//站口狀態確認
                            if (_conveyor.GetBuffer(bufferIndex).Auto != true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmd_Sno, Remark.NotAutoMode, db);
                                return false;
                            }
                            if (_conveyor.GetBuffer(bufferIndex).CommandId >0 )
                            {
                                CMD_MST.UpdateCmdMstRemark(cmd_Sno, Remark.CmdLeftOver, db);
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
                            if (_conveyor.GetBuffer(bufferIndex).Presence == true)
                            {
                                CMD_MST.UpdateCmdMstRemark(cmd_Sno, Remark.PresenceExist, db);
                                return false;
                            }
                            #endregion


                            clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready Receive STS Command => {cmd_Sno}, " +
                                   $"{Cmd_Mode}");

                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Begin Fail => {cmd_Sno}");
                                return false;
                            }
                            if (CMD_MST.UpdateCmdMstTransferring(cmd_Sno, Trace.StoreInAbnormalWritePLC, db).ResultCode == DBResult.Success)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Upadte STS cmd Success => {cmd_Sno}, " +
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
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"STS WritePLC Command-mode Fail");
                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            int path = PathNotice.OutPath1_toA11_04;

                            WritePlccheck = _conveyor.GetBuffer(bufferIndex).WritePathChabgeNotice(path).Result;//錯誤時回傳exmessage
                            Result = WritePlccheck;
                            if (Result != true)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"STS WritePLC Path-Mode Fail");
                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "STS Commit Fail");
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

        public bool FunSTSabnormal_CreateEquCmd(int bufferIndex)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        string cmdSno = _conveyor.GetBuffer(bufferIndex).CommandId.ToString().PadLeft(5, '0');

                        if (CMD_MST.GetCmdMstBySTSabnormal(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)
                        {
                            cmdSno = dataObject[0].Cmd_Sno;
                            string source = CranePortNo.Floor2;
                            int EquNo = 0;
                            if (bufferIndex == 1)
                            {
                                EquNo = 1;
                            }
                            else if (bufferIndex == 3)
                            {
                                EquNo = 2;
                            }
                            else if (bufferIndex == 5)
                            {
                                EquNo = 3;
                            }
                            else if (bufferIndex == 7)
                            {
                                EquNo = 4;
                            }
                            else if (bufferIndex == 9)
                            {
                                EquNo = 5;
                            }
                            else if (bufferIndex == 11)
                            {
                                EquNo = 6;
                            }

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


                            clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Buffer Ready STS=> {cmdSno}");

                            if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane STS Command, Begin Fail => {cmdSno}");
                                return false;
                            }
                            if (CMD_MST.UpdateCmdMst(cmdSno, Trace.StoreInAbnormalCreateSTSCmd, db).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane STS Command, Update CmdMst Fail => {cmdSno}");
                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            if (EQU_CMD.InsertSTSEquCmd(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, EquNo, cmdSno, CranePortNo.Floor2, CranePortNo.Floor1, 5, db) == false)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane STS Command, Insert EquCmd Fail => {cmdSno}");
                                db.TransactionCtrl2(TransactionTypes.Rollback);
                                return false;
                            }
                            if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                            {
                                clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"Create Crane STS Command, Commit Fail => {cmdSno}");
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

        public bool FunStoreStsEquCmdFinish()
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        if (CMD_MST.GetCmdMstBySTSFinish(out var dataObject, db).ResultCode == DBResult.Success)
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
                                            cmdsts = CmdSts.Transferring;
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
                                            cmdsts = CmdSts.Transferring;
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
                                            if (CMD_MST.UpdateCmdMst(equCmd[0].CmdSno, cmdsts, Trace.StoreOutCraneCmdFinish, db).ResultCode != DBResult.Success)
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
                                            if (CMD_MST.UpdateCmdMstEnd(equCmd[0].CmdSno, cmdsts, Trace.StoreInCraneCmdFinish, db).ResultCode != DBResult.Success)
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
                                if (Stn_No == StnNo.A11_04)
                                {
                                    path = PathNotice.OutPath1_toA11_04;
                                }
                                else if(Stn_No == StnNo.A11_02)
                                {
                                    path = PathNotice.OutPath2_toA11_02;
                                }
                                #endregion

                                WritePlccheck = _conveyor.GetBuffer(bufferIndex).WritePathChabgeNotice(path).Result;//錯誤時回傳exmessage
                                Result = WritePlccheck;
                                if (Result != true)
                                {
                                    clsWriLog.StoreOutLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"WritePLC Path-Mode Fail");
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
                        string cmdSno = _conveyor.GetBuffer(bufferIndex).CommandId.ToString().PadLeft(5, '0');

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
                            if (EQU_CMD.InsertStoreOutEquCmd(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, Equ_No, cmdSno, source, CranePortNo.Floor1 , 5, db) == false)
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
                                            cmdsts = CmdSts.Transferring;
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
                                            cmdsts = CmdSts.Transferring;
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
                                            if (CMD_MST.UpdateCmdMst(equCmd[0].CmdSno, cmdsts, Trace.StoreOutCraneCmdFinish, db) != ExecuteSQLResult.Success)
                                            {
                                               db.TransactionCtrl2(TransactionTypes.Rollback);
                                               return false;
                                            }
                                            if (CMD_MST.UpdateCmdMstRemarkandAbnormalforStoreOut(equCmd[0].CmdSno, remark, cmdabnormal, db).ResultCode != DBResult.Success)
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

        public bool StoreOut_ShowOnKanBanStart(int bufferIndex,string Stn_no)//為了看板程式，更新命令Trace，使其可以開始顯示此命令
        {

            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        if (_conveyor.GetBuffer(bufferIndex).CommandId > 0 && _conveyor.GetBuffer(bufferIndex).Presence)//當貨物到出庫站口，更新命令使看板顯示開始
                        {
                            string cmdSno = (_conveyor.GetBuffer(bufferIndex).CommandId).ToString().PadLeft(5, '0');

                            if (CMD_MST.GetCmdMstByStoreOutForKanBan(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)//搜尋尚未顯示於看板上的貨物命令
                            {

                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To start showing KanBan=> {cmdSno}");

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "(StoreOut)Buffer Get cmdsno To start showing KanBan, Begin Fail");

                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMst(cmdSno, Trace.StoreOutKanBanStart, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To start showing KanBan, Update CmdMst Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.StoreOutKanBanStart, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To start showing KanBan, Update CmdMst Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if(MVS_Mst.ShowMVS(cmdSno,Stn_no,db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To start showing KanBan, Update MVS_MST Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To start showing KanBan, Commit Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                else return true;

                            }
                            else return false;
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

        public bool StoreOut_ShowOnKanBanFinish(int bufferIndex,string Stn_no)//為了看板程式，更新命令Trace，使其可以停止顯示此命令
        {

            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        if (_conveyor.GetBuffer(bufferIndex).CommandId > 0 && !_conveyor.GetBuffer(bufferIndex).Presence)//當貨物被搬離的時候，更新命令為回報結束的狀態
                        {
                            string cmdSno = (_conveyor.GetBuffer(bufferIndex).CommandId).ToString().PadLeft(5, '0');

                            if (CMD_MST.GetCmdMstByStoreOutForKanBanFinish(cmdSno, out var dataObject, db).ResultCode == DBResult.Success)//搜尋已經顯示於看板上的貨物命令
                            {
                                string cmdmode= dataObject[0].Cmd_Mode;

                                clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To stop showing KanBan=> {cmdSno}");

                                if (db.TransactionCtrl2(TransactionTypes.Begin).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, "(StoreOut)Buffer Get cmdsno To stop showing KanBan, Begin Fail");

                                    return false;
                                }
                                if (cmdmode != "3")
                                {
                                    if (CMD_MST.UpdateCmdMstEnd(cmdSno, CmdSts.CompleteWaitUpdate, Trace.StoreOutKanBanFinish, db).ResultCode != DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To stop showing KanBan, Update CmdMst Fail => {cmdSno}");

                                        db.TransactionCtrl2(TransactionTypes.Rollback);
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (CMD_MST.UpdateCmdMst(cmdSno, CmdSts.Transferring, Trace.StoreOutKanBanFinish, db).ResultCode != DBResult.Success)
                                    {
                                        clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To stop showing KanBan, Update CmdMst Fail => {cmdSno}");

                                        db.TransactionCtrl2(TransactionTypes.Rollback);
                                        return false;
                                    }
                                }
                                if (CMD_MST.UpdateCmdMstRemark(cmdSno, Remark.StoreOutKanBanFinish, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To stop showing KanBan, Update CmdMst Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (MVS_Mst.ShowMVS(" ", Stn_no, db).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To stop showing KanBan, Update MVS_MST Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                if (db.TransactionCtrl2(TransactionTypes.Commit).ResultCode != DBResult.Success)
                                {
                                    clsWriLog.StoreInLogTrace(_conveyor.GetBuffer(bufferIndex).BufferIndex, _conveyor.GetBuffer(bufferIndex).BufferName, $"(StoreOut)Buffer Get cmdsno To stop showing KanBan, Commit Fail => {cmdSno}");

                                    db.TransactionCtrl2(TransactionTypes.Rollback);
                                    return false;
                                }
                                else return true;

                            }
                            else return false;
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


        #region//根據線別判斷路徑編號
        private int StoreInfindpathbyEquNo(int Equ_No)
        {
            if (Equ_No==1)
            {
                return 11; //B01_1
            }
            else if(Equ_No==2)
            {
                return 12; //B02_1
            }
            else if(Equ_No==3)
            {
                return 13; //B03_1
            }
            else if(Equ_No==4)
            {
                return 14; //B04_1
            }
            else if(Equ_No==5)
            {
                return 15; //B05_1
            }
            else if (Equ_No==6)
            {
                return 16; //B06_1
            }
            return 0;
        }
        #endregion


        public static string GetEquNo()
        {
            string sLine = "";  //最終線別
            int[] iAry = new int[7];
            int temp = 0;
            string stemp = "";
            //List<int> intermediate_list = new List<int>();
            try
            {
                iAry[1] = dicCountByCrane["1"];
                iAry[2] = dicCountByCrane["2"];
                iAry[3] = dicCountByCrane["3"];
                iAry[4] = dicCountByCrane["4"];
                iAry[5] = dicCountByCrane["5"];
                iAry[6] = dicCountByCrane["6"];
                for (int i = 1; i < iAry.Length; i++)
                {
                    for (int k = i + 1; k < iAry.Length; k++)
                    {
                        if (iAry[i] > iAry[k])
                        {
                            temp = iAry[k];
                            iAry[k] = iAry[i];
                            iAry[i] = temp;
                        }
                    }
                    //Console.WriteLine($"{Ary[i]}");
                }
                for (int i = 1; i < iAry.Length; i++)
                {
                    stemp = i.ToString();
                    if (dicCountByCrane[stemp] == iAry[1])
                    {
                        sLine = stemp;
                        dicCountByCrane[stemp]++;
                        break;
                    }
                }
                return sLine;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return "";
            }

        }

        public static string GetEquNoForProduction()
        {
            string sLine = "";  //最終線別
            int[] iAry = new int[7];
            int temp = 0;
            string stemp = "";
            //List<int> intermediate_list = new List<int>();
            try
            {
                iAry[1] = dicCountByCrane["1"];
                iAry[2] = dicCountByCrane["2"];
                iAry[3] = dicCountByCrane["3"];
                iAry[4] = dicCountByCrane["4"];
                iAry[5] = dicCountByCrane["5"];
                iAry[6] = dicCountByCrane["6"];
                for (int i = 1; i < iAry.Length; i++)
                {
                    for (int k = i + 1; k < iAry.Length; k++)
                    {
                        if (iAry[i] > iAry[k])
                        {
                            temp = iAry[k];
                            iAry[k] = iAry[i];
                            iAry[i] = temp;
                        }
                    }
                    //Console.WriteLine($"{Ary[i]}");
                }
                for (int i = 1; i < iAry.Length; i++)
                {
                    stemp = i.ToString();
                    if (dicCountByCrane[stemp] == iAry[1])
                    {
                        sLine = stemp;
                        dicCountByCrane[stemp]++;
                        break;
                    }
                }
                return sLine;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return "";
            }

        }




    }
}
