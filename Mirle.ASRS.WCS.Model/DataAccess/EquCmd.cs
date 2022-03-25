﻿using System;
using System.Data;

using Mirle.DataBase;

namespace Mirle.ASRS.WCS.Model.DataAccess
{
    public sealed class EquCmd : ValueObject
    {
        public string CmdSno { get; private set; }
        public string CmdMode { get; private set; }
        public string EquNo { get; private set; }
        public string CmdSts { get; private set; }
        public string EquSts { get; private set; }
        public string AlarmDesc { get; private set; }
        public string CompleteCode { get; private set; }
        public string ReNeqFlag { get; private set; }
        protected override ValueObject ConvaertDataRow(DataRow row)
        {
            if (row.Table.Columns.Contains("CMDSNO"))
            {
                CmdSno = Convert.ToString(row["CMDSNO"]);
            }
            if (row.Table.Columns.Contains("CMDMODE"))
            {
                CmdMode = Convert.ToString(row["CMDMODE"]);
            }
            if (row.Table.Columns.Contains("CMDSTS"))
            {
                CmdSts = Convert.ToString(row["CMDSTS"]);
            }
            if (row.Table.Columns.Contains("COMPLETECODE"))
            {
                CompleteCode = Convert.ToString(row["COMPLETECODE"]);
            }
            if (row.Table.Columns.Contains("RENEWFLAG"))
            {
                ReNeqFlag = Convert.ToString(row["RENEWFLAG"]);
            }
            if (row.Table.Columns.Contains("AlarmDesc"))
            {
                AlarmDesc = Convert.ToString(row["AlarmDesc"]);
            }
            if (row.Table.Columns.Contains("EquSts"))
            {
                EquSts = Convert.ToString(row["EquSts"]);
            }
            return this;
        }
    }
}
