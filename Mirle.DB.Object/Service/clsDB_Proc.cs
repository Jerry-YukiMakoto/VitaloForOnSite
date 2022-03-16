using System;
using Mirle.Def;
using System.Windows.Forms;
using Mirle.Structure;
using Mirle.Structure.Info;
using System.Collections.Generic;
using System.Linq;

namespace Mirle.DB.Object
{
    public class clsDB_Proc
    {
        private static Proc.clsHost wcs;
        public static void Initial(clsDbConfig dbConfig)
        {
            wcs = new Proc.clsHost(dbConfig, Application.StartupPath + "\\Sqlite\\LCSCODE.DB");
        }

        public static Proc.clsHost GetDB_Object()
        {
            return wcs;
        }


    }
}
