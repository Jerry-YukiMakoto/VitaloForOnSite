using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirle.DataBase;

namespace Mirle.ASRS.WCS.Model.DataAccess
{
    public sealed class ItmMst : ValueObject
    {
        public string Item_No { get; private set; }
        public string Item_Desc { get; private set; }
        public string Item_Spec { get; private set; }
        public string Item_Unit { get; private set; }
        public string Item_Type { get; private set; }
        public string Item_Grp { get; private set; }
        public string Box_Plt { get; private set; }
        public string Qty_Plt { get; private set; }

        protected override ValueObject ConvaertDataRow(DataRow row)
        {
            if (row.Table.Columns.Contains("Item_No"))
            {
                Item_No = Convert.ToString(row["Item_No"]);
            }
            if (row.Table.Columns.Contains("Item_Desc"))
            {
                Item_Desc = Convert.ToString(row["Item_Desc"]);
            }
            if (row.Table.Columns.Contains("Item_Spec"))
            {
                Item_Spec = Convert.ToString(row["Item_Spec"]);
            }
            if (row.Table.Columns.Contains("Item_Unit"))
            {
                Item_Unit = Convert.ToString(row["Item_Unit"]);
            }
            if (row.Table.Columns.Contains("Item_Type"))
            {
                Item_Type = Convert.ToString(row["Item_Type"]);
            }
            if (row.Table.Columns.Contains("Item_Grp"))
            {
                Item_Grp = Convert.ToString(row["Item_Grp"]);
            }
            if (row.Table.Columns.Contains("Qty_Plt"))
            {
                Qty_Plt = Convert.ToString(row["Qty_Plt"]);
            }
            return this;
        }
    }
}
