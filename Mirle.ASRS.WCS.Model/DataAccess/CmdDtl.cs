using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirle.DataBase;

namespace Mirle.ASRS.WCS.Model.DataAccess
{
    public sealed class CmdDtl : ValueObject
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
    public struct struCmdDtl
    {
        public string Cmd_Txno { get; set; }         
        public string Cmd_Sno { get; set; }
        public double Plt_Qty { get; set; }
        public double Trn_Qty { get; set; }
        public string Loc { get; set; }
        public string In_Date { get; set; }
        public string Item_No { get; set; }
        public string Lot_No { get; set; }
        public string Plt_Id { get; set; }
        public string Tkt_Io { get; set; }
        public string Tkt_Type { get; set; }
        public string Company_ID { get; set; }
        public string Item_Desc { get; set; }
        public string Uom { get; set; }
        public string Created_by { get; set; }
        public string Created_Date { get; set; }


        /// <summary>
        /// 清除結構變數
        /// </summary>
        public void Clear()
        {
            Cmd_Txno = "";
            Cmd_Sno = "";
            Plt_Qty = 0;
            Trn_Qty = 0;
            Loc = "";
            Plt_Id = "";
            Lot_No = "";
            In_Date = "";
            Item_No = "";
            Company_ID = "";
            Item_Desc = "";
            Uom = "";
            Created_by = "";
            Created_Date = "";
            Tkt_Io = "";
            Tkt_Type = "";
        }
    }
}
