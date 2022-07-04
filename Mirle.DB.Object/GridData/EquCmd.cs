using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Mirle.DataBase;
using Mirle.Grid;
using Mirle.Grid.T1JUMAK0;

namespace Mirle.DB.Object.GridData
{
    public class EquCmd: IGrid
    {
        public void SubShowCmdtoGrid(ref DataGridView oGrid)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                oGrid.SuspendLayout();
                oGrid.Rows.Clear();
                int iRet = clsDB_Proc.GetDB_Object().GetEqu_Cmd().FunGetEquMst_Grid(ref dtTmp);
                if (iRet == DBResult.Success)
                {
                    for (int i = 0; i < dtTmp.Rows.Count; i++)
                    {
                        oGrid.Rows.Add();
                        oGrid.Rows[oGrid.RowCount - 1].HeaderCell.Value = Convert.ToString(oGrid.RowCount);
                        oGrid[ColumnDef.EquCmd.CmdSno.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CMDSNO"]);
                        oGrid[ColumnDef.EquCmd.EquNo.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["EquNo"]);
                        oGrid[ColumnDef.EquCmd.CmdMode.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CmdMode"]);
                        oGrid[ColumnDef.EquCmd.Cmdsts.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CmdSts"]);
                        oGrid[ColumnDef.EquCmd.Loc.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["Source"]);
                        oGrid[ColumnDef.EquCmd.NewLoc.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["Destination"]);
                        oGrid[ColumnDef.EquCmd.RCVDT.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["RCVDT"]);
                        oGrid[ColumnDef.EquCmd.CompleteCode.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CompleteCode"]);
                        oGrid[ColumnDef.EquCmd.RedNewFlag.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["ReNewFlag"]);
                        
                    }
                }
                oGrid.ResumeLayout();
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                dtTmp = null;
            }
        }
    }
}
