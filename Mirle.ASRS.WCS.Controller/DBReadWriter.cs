using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirle.DataBase;
using Mirle.DB.WMS.Proc;
using Mirle.Def;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.FileData;

namespace Mirle.MPLC.DataBase
{
    //20210927 Louis
    public class DBReadWriter : IMPLCProvider, IDisposable
    {
        private class EquPlcData : ValueObject
        {
            public string SerialNo { get; private set; }
            public string PlcData { get; private set; }

            protected override ValueObject ConvaertDataRow(DataRow row)
            {
                if (row.Table.Columns.Contains("SerialNo"))
                {
                    SerialNo = Convert.ToString(row["SerialNo"]);
                }
                if (row.Table.Columns.Contains("EquPlcData"))
                {
                    PlcData = Convert.ToString(row["EquPlcData"]);
                }
                return this;
            }
        }

        private readonly List<FileDataBlock> _dataBlocks = new List<FileDataBlock>();
        private readonly ThreadWorker _cacheWorker;
        private clsDbConfig _config = new clsDbConfig();

        private DBOptions _options = new DBOptions();

        public bool IsConnected => true;

        public int Interval
        {
            get => _cacheWorker.Interval;
            set => _cacheWorker.Interval = value;
        }

        public DBReadWriter(clsDbConfig dbConfig)
        {
            _config = dbConfig;
            _cacheWorker = new ThreadWorker(CacheProc, 200);
        }

        private void CacheProc()
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    string sql = "SELECT * FROM EQUPLCDATA ";
                    sql += "WHERE EQUNO='CV' ";
                    sql += "ORDER BY SERIALNO";
                    if (db.GetData<EquPlcData>(sql, out DataObject<EquPlcData> dataObject) == GetDataResult.Success)
                    {
                        string strTmp = string.Empty;
                        for (int i = 0; i < dataObject.Count; i++)
                        {
                            strTmp += dataObject[i].PlcData;
                        }
                        var rawA = new RawRecord(strTmp);
                        foreach (var block in _dataBlocks)
                        {
                            try
                            {
                                byte[] newByteArray = rawA.GetBlockByIndex(block.ColumnIndex);
                                block.SetRawData(newByteArray);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
        }

        public void Start()
        {
            _cacheWorker.Start();
        }
        public void Stop()
        {
            _cacheWorker.Pause();
        }

        public virtual void AddDataBlock(FileDataBlock newDataBlock)
        {
            _dataBlocks.Add(newDataBlock);
        }

        public virtual bool GetBit(string address)
        {
            foreach (var block in _dataBlocks)
            {
                if (block.TryGetBit(address, out bool value))
                {
                    return value;
                }
            }
            return false;
        }

        public virtual void SetBitOn(string address)
        {
            foreach (var block in _dataBlocks)
            {
                if (block.TrySetBitOn(address))
                {
                    return;
                }
            }
        }

        public virtual void SetBitOff(string address)
        {
            foreach (var block in _dataBlocks)
            {
                if (block.TrySetBitOff(address))
                {
                    return;
                }
            }
        }

        public virtual int ReadWord(string address)
        {
            foreach (var block in _dataBlocks)
            {
                if (block.TryGetWord(address, out int value))
                {
                    return value;
                }
            }
            return 0;
        }

        public virtual void WriteWord(string address, int data)
        {
            foreach (var block in _dataBlocks)
            {
                if (block.TrySetWord(address, data))
                {
                    return;
                }
            }
        }

        public virtual int[] ReadWords(string startAddress, int length)
        {
            foreach (var block in _dataBlocks)
            {
                if (block.TryGetWords(startAddress, out int[] data, length))
                {
                    return data;
                }
            }
            return new int[length];
        }

        public virtual void WriteWords(string startAddress, int[] data)
        {
            foreach (var block in _dataBlocks)
            {
                if (block.TrySetWords(startAddress, data))
                {
                    return;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cacheWorker?.Pause();
                    _cacheWorker?.Dispose();
                }

                disposedValue = true;
            }
        }

        ~DBReadWriter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
