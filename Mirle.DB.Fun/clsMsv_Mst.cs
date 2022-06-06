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
    public class clsMsv_Mst
    {

        public ExecuteSQLResult ShowMVS(string Cmd_Sno,string Stn_No /*, out DataObject<Mvs_Mst> dataObject*/, SqlServer db)
        {

            string sql = $"Update Mvs_Mst ";
            sql += $"set Cmd_Sno = '{Cmd_Sno}' ";
            sql += $"Where Stn_No = '{Stn_No}'";
            return db.ExecuteSQL2(sql);

        }

    }
}

