using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirle.DataBase;
using Mirle.Def;
using Mirle.MPLC.DataBlocks;

namespace Mirle.MPLC.DataBase
{

    public class DBReadOnlyReader : DBReadWriter, IDisposable
    {

        public DBReadOnlyReader(clsDbConfig dbConfig) : base(dbConfig)
        {
        }

        public override void SetBitOn(string address)
        {
            return;
        }
        public override void SetBitOff(string address)
        {
            return;
        }

        public override void WriteWord(string address, int data)
        {
            return;
        }

        public override void WriteWords(string startAddress, int[] data)
        {
            return;
        }
    }
}
