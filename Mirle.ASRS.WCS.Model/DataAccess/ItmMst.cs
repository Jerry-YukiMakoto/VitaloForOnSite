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
        public double Qty_Plt { get; private set; }

        protected override ValueObject ConvaertDataRow(DataRow row)
        {
            return this;
        }
    }
}
