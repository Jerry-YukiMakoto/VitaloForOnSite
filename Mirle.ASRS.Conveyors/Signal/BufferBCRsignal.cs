﻿
using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyors.Signal
{
    public class BufferBCRsignal
    {
        public WordBlock Item_No { get; protected internal set; }
        public WordBlock Lot_ID { get; protected internal set; }
        public WordBlock Plt_Id { get; protected internal set; }
    }
}