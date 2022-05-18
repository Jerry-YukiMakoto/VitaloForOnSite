using Mirle.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.WCS.Model.DataAccess
{
    public sealed class CVCAlarm : ValueObject
    {
        public string EmpSno { get; private set; }
        public string EmpNo { get; private set; }
        public string EmpName { get; private set; }
        public bool OpenFlag { get; private set; }
        public string StnNo { get; private set; }

        protected override ValueObject ConvaertDataRow(DataRow row)
        {
            if (row.Table.Columns.Contains("EMP_SNO"))
            {
                EmpSno = Convert.ToString(row["EMP_SNO"]);
            }
            if (row.Table.Columns.Contains("EMP_NO"))
            {
                EmpNo = Convert.ToString(row["EMP_NO"]);
            }
            if (row.Table.Columns.Contains("EMP_NAME"))
            {
                EmpName = Convert.ToString(row["EMP_NAME"]);
            }
            if (row.Table.Columns.Contains("OPEN_FLAG"))
            {
                OpenFlag = Convert.ToString(row["OPEN_FLAG"]) == "1";
            }
            if (row.Table.Columns.Contains("STNNO"))
            {
                StnNo = Convert.ToString(row["STNNO"]);
            }
            return this;
        }
    }
    public sealed class AlarmCVCInfo : ValueObject
    {
        public string BufferIndex { get; private set; }
        public string AlarmBit { get; private set; }
        public string AlarmDesc { get; private set; }
        public string AlarmCode { get; private set; }

        protected override ValueObject ConvaertDataRow(DataRow row)
        {
            if (row.Table.Columns.Contains("BUFFERINDEX"))
            {
                BufferIndex = Convert.ToString(row["BUFFERINDEX"]);
            }
            if (row.Table.Columns.Contains("ALARMCVCBIT"))
            {
                AlarmBit = Convert.ToString(row["ALARMCVCBIT"]);
            }
            if (row.Table.Columns.Contains("ALARMCVCDESC"))
            {
                AlarmDesc = Convert.ToString(row["ALARMCVCDESC"]);
            }
            if (row.Table.Columns.Contains("ALARMCVCCODE"))
            {
                AlarmCode = Convert.ToString(row["ALARMCVCCODE"]);
            }

            return this;
        }
    }
    public sealed class AlarmCVCLog : ValueObject
    {
        public string StrDt { get; private set; }
        public string AlarmCvcCode { get; private set; }
        public string ClrDt { get; private set; }
        public string TotalSecs { get; private set; }

        protected override ValueObject ConvaertDataRow(DataRow row)
        {
            if (row.Table.Columns.Contains("STRDT"))
            {
                StrDt = Convert.ToString(row["STRDT"]);
            }
            if (row.Table.Columns.Contains("ALARMCVCCODE"))
            {
                AlarmCvcCode = Convert.ToString(row["ALARMCVCCODE"]);
            }
            if (row.Table.Columns.Contains("CLRDT"))
            {
                ClrDt = Convert.ToString(row["CLRDT"]);
            }
            if (row.Table.Columns.Contains("TOTALSECS"))
            {
                TotalSecs = Convert.ToString(row["TOTALSECS"]);
            }

            return this;
        }
    }
}
