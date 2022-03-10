﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirle.DataBase;

namespace Mirle.ASRS.WCS.Model.DataAccess
{
    public sealed class CmdMst : ValueObject
    {
        public string Cmd_Sno { get; private set; }
        public string Cmd_Mode { get; private set; }
        public string StnNo { get; private set; }
        public string Loc { get; private set; }
        public string NewLoc { get; private set; }
        public string LoadType { get; private set; }
        public string TrayId { get; private set; }
        public string Trace { get; private set; }
        public string IOType { get; private set; }
        public string IO_Type { get; private set; }
        public string COUNT { get; internal set; }
        public string Stn_No { get; private set; }
        public string Equ_No { get; private set; }

        protected override ValueObject ConvaertDataRow(DataRow row)
        {

            if (row.Table.Columns.Contains("STNNO"))
            {
                StnNo = Convert.ToString(row["STNNO"]);
            }
            if (row.Table.Columns.Contains("LOC"))
            {
                Loc = Convert.ToString(row["LOC"]);
            }
            if (row.Table.Columns.Contains("NEWLOC"))
            {
                NewLoc = Convert.ToString(row["NEWLOC"]);
            }
            if (row.Table.Columns.Contains("LOADTYPE"))
            {
                LoadType = Convert.ToString(row["LOADTYPE"]);
            }
            if (row.Table.Columns.Contains("TRAYID"))
            {
                TrayId = Convert.ToString(row["TRAYID"]);
            }
            if (row.Table.Columns.Contains("TRACE"))
            {
                Trace = Convert.ToString(row["TRACE"]);
            }
            if (row.Table.Columns.Contains("IOType"))
            {
                IOType = Convert.ToString(row["IOType"]);
            }
            if (row.Table.Columns.Contains("COUNT"))
            {
                COUNT = Convert.ToString(row["COUNT"]);
            }
            return this;
        }
    }
    public struct struCmdMst
    {
        public string CmdSno { get; set; }
        public string CmdSts { get; set; }
        public string CmdAbnormal { get; set; }
        public string Prty { get; set; }
        public string StnNo { get; set; }
        public string CmdMode { get; set; }
        public string IoType { get; set; }
        public string WhId { get; set; }
        public string Loc { get; set; }
        public string NewLoc { get; set; }
        public int MixQty { get; set; }
        public double Avail { get; set; }
        public string ZoneId { get; set; }
        public string LocId { get; set; }
        public string CrtDate { get; set; }
        public string ExpDate { get; set; }
        public string EndDate { get; set; }
        public string TrnUser { get; set; }
        public string HostName { get; set; }
        public string FinLoc { get; set; }
        public string PltCount { get; set; }
        public string EquNo { get; set; }
        public string Trace { get; set; }
        public string Tkt_IO { get; set; }
        public string Remark { get; set; }
        public string Plt_Id { get; set; }


        /// <summary>
        /// 清除結構變數
        /// </summary>
        public void Clear()
        {
            CmdSno = "";
            CmdSts = "";
            CmdAbnormal = "";
            Prty = "";
            StnNo = "";
            CmdMode = "";
            IoType = "";
            WhId = "";
            Loc = "";
            NewLoc = "";
            MixQty = 0;
            Avail = 0;
            ZoneId = "";
            LocId = "";
            CrtDate = "";
            ExpDate = "";
            EndDate = "";
            TrnUser = "";
            HostName = "";
            FinLoc = "";
            PltCount = "";
            EquNo = "";
            Trace = "";
            Tkt_IO = "";
        }
    }
}
