using System;
using Mirle.Def;
using System.Data;
using Mirle.Structure;
using Mirle.DataBase;
using Mirle.ASRS.WCS.Model.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace Mirle.DB.Fun
{
    public class clsItmMst
    {
        public GetDataResult GetItmMstDtl(string Item_No , out DataObject<ItmMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Itm_Mst  ";
            sql += $"WHERE Item_No IN ('{clsConstValue.CmdMode.StockIn}') ";
            return db.GetData(sql, out dataObject);
        }

    }
}
