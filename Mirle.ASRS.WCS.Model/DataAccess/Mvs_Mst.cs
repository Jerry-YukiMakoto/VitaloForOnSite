using Mirle.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.WCS.Model.DataAccess
{
    public sealed class Mvs_Mst : ValueObject
    {
        public string Cmd_Sno { get; private set; }
        public string Mvs_ID { get; private set; }
        public string Dsp_Flag { get; private set; }
        public bool OpenFlag { get; private set; }
        public string StnNo { get; private set; }

        protected override ValueObject ConvaertDataRow(DataRow row)
        {
            if (row.Table.Columns.Contains("Cmd_Sno"))
            {
                Cmd_Sno = Convert.ToString(row["Cmd_Sno"]);
            }
            if (row.Table.Columns.Contains("Mvs_ID"))
            {
                Mvs_ID = Convert.ToString(row["Mvs_ID"]);
            }
            if (row.Table.Columns.Contains("Dsp_Flag"))
            {
                Dsp_Flag = Convert.ToString(row["Dsp_Flag"]);
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
}
