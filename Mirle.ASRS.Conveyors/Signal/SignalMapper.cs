using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirle.MPLC;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.DataBlocks.DeviceRange;
using Mirle.MPLC.DataType;

namespace Mirle.ASRS.Conveyors.Signal
{
    public class SignalMapper
    {
        private readonly IMPLCProvider _mplc;
        private readonly Dictionary<int, BufferSignal> _bufferSignals = new Dictionary<int, BufferSignal>();
        private readonly SystemSignal _systemSignal = new SystemSignal();
        private readonly int _signalGroup = 0;

        public IEnumerable<BufferSignal> BufferSignals => _bufferSignals.Values;

        public SignalMapper(IMPLCProvider mplc,int PLCNo)
        {
            _mplc = mplc;
            _signalGroup = PLCNo;

            MappingSystem();
            MappingBuffer();
        }
        private List<ConveyorDefine> GetDefaultMaps()
        {
            List<ConveyorDefine> Conveyors = new List<ConveyorDefine>();
            ConveyorDefine conveyor = new ConveyorDefine();
            conveyor.SignalGroup = 1;
            conveyor.Buffers = new List<BufferDefine>()
            {
                new BufferDefine() { BufferIndex = 1, BufferName = "A01-1" },
                new BufferDefine() { BufferIndex = 2, BufferName = "A01-2" },
                new BufferDefine() { BufferIndex = 3, BufferName = "A01-3" },
                new BufferDefine() { BufferIndex = 4, BufferName = "A01-4" },
                new BufferDefine() { BufferIndex = 5, BufferName = "A01-5" },
                new BufferDefine() { BufferIndex = 6, BufferName = "A01-6" },
                new BufferDefine() { BufferIndex = 7, BufferName = "A01-7" },
                new BufferDefine() { BufferIndex = 8, BufferName = "A01-8" },
                new BufferDefine() { BufferIndex = 9, BufferName = "A01-9" },
                new BufferDefine() { BufferIndex = 10, BufferName = "A01-10" },
                new BufferDefine() { BufferIndex = 11, BufferName = "A01-11" },
                new BufferDefine() { BufferIndex = 12, BufferName = "A01-12" },
                new BufferDefine() { BufferIndex = 13, BufferName = "A02-1" },
                new BufferDefine() { BufferIndex = 14, BufferName = "A02-2" },
                new BufferDefine() { BufferIndex = 15, BufferName = "A02-3" },
                new BufferDefine() { BufferIndex = 16, BufferName = "A02-4" },
                new BufferDefine() { BufferIndex = 17, BufferName = "A02-5" },
                new BufferDefine() { BufferIndex = 18, BufferName = "A02-6" },
                new BufferDefine() { BufferIndex = 19, BufferName = "A02-7" },
                new BufferDefine() { BufferIndex = 20, BufferName = "A02-8" },
                new BufferDefine() { BufferIndex = 21, BufferName = "A02-9" },
                new BufferDefine() { BufferIndex = 22, BufferName = "A02-10" },
                new BufferDefine() { BufferIndex = 23, BufferName = "A02-11" },
                new BufferDefine() { BufferIndex = 24, BufferName = "A02-12" },
                new BufferDefine() { BufferIndex = 25, BufferName = "A04" },
                new BufferDefine() { BufferIndex = 26, BufferName = "A05-1" },
                new BufferDefine() { BufferIndex = 27, BufferName = "A05-2" },
                new BufferDefine() { BufferIndex = 28, BufferName = "A06-1" },
                new BufferDefine() { BufferIndex = 29, BufferName = "A06-2" },
                new BufferDefine() { BufferIndex = 30, BufferName = "A11-01" },
                new BufferDefine() { BufferIndex = 31, BufferName = "A11-02" },
                new BufferDefine() { BufferIndex = 32, BufferName = "A05-3" },
                new BufferDefine() { BufferIndex = 33, BufferName = "A05-4" },
                new BufferDefine() { BufferIndex = 34, BufferName = "A05-5" },
                new BufferDefine() { BufferIndex = 35, BufferName = "A05-6" },
                new BufferDefine() { BufferIndex = 36, BufferName = "A05-7" },
                new BufferDefine() { BufferIndex = 37, BufferName = "A05-8" },
                new BufferDefine() { BufferIndex = 38, BufferName = "A06-3" },
                new BufferDefine() { BufferIndex = 39, BufferName = "A06-4" },
                new BufferDefine() { BufferIndex = 40, BufferName = "A11-03" },
                new BufferDefine() { BufferIndex = 41, BufferName = "A11-04" },
                new BufferDefine() { BufferIndex = 42, BufferName = "A07" },
                new BufferDefine() { BufferIndex = 43, BufferName = "A08" },
                new BufferDefine() { BufferIndex = 44, BufferName = "A09" },
                new BufferDefine() { BufferIndex = 45, BufferName = "A010" },
                new BufferDefine() { BufferIndex = 46, BufferName = "A07-01" },
                new BufferDefine() { BufferIndex = 47, BufferName = "A11-05" },
                new BufferDefine() { BufferIndex = 48, BufferName = "A11-06" },
            };
            Conveyors.Add(conveyor);

            conveyor = new ConveyorDefine();
            conveyor.SignalGroup = 2;
            conveyor.Buffers = new List<BufferDefine>()
            {
                new BufferDefine() { BufferIndex = 1, BufferName = "B01-1" },
                new BufferDefine() { BufferIndex = 2, BufferName = "B01-2" },
                new BufferDefine() { BufferIndex = 3, BufferName = "B01-3" },
                new BufferDefine() { BufferIndex = 4, BufferName = "B01-4" },
                new BufferDefine() { BufferIndex = 5, BufferName = "B01-5" },
                new BufferDefine() { BufferIndex = 6, BufferName = "B01-6" },
                new BufferDefine() { BufferIndex = 7, BufferName = "B01-7" },
                new BufferDefine() { BufferIndex = 8, BufferName = "B01-8" },
                new BufferDefine() { BufferIndex = 9, BufferName = "B01-9" },
                new BufferDefine() { BufferIndex = 10, BufferName = "B01-10" },
                new BufferDefine() { BufferIndex = 11, BufferName = "B01-11" },
                new BufferDefine() { BufferIndex = 12, BufferName = "B01-12" },
                new BufferDefine() { BufferIndex = 13, BufferName = "B02-01" },
                new BufferDefine() { BufferIndex = 14, BufferName = "B02-02" },
                new BufferDefine() { BufferIndex = 15, BufferName = "B02-03" },
                new BufferDefine() { BufferIndex = 16, BufferName = "B02-04" },
                new BufferDefine() { BufferIndex = 17, BufferName = "B02-05" },
                new BufferDefine() { BufferIndex = 18, BufferName = "B02-06" },
                new BufferDefine() { BufferIndex = 19, BufferName = "B02-07" },
                new BufferDefine() { BufferIndex = 20, BufferName = "B02-08" },
                new BufferDefine() { BufferIndex = 21, BufferName = "B02-09" },
                new BufferDefine() { BufferIndex = 22, BufferName = "B02-10" },
                new BufferDefine() { BufferIndex = 23, BufferName = "B02-11" },
                new BufferDefine() { BufferIndex = 24, BufferName = "B02-12" },
                new BufferDefine() { BufferIndex = 25, BufferName = "B03" },
            };
            Conveyors.Add(conveyor);

            return Conveyors;
        }

        private void MappingSystem()
        {
            int plcIndex = 101;
            int pcIndex = 3101;
            _systemSignal.Heartbeat = new Word(_mplc, $"D{plcIndex}");
            _systemSignal.Alarm = new Word(_mplc, $"D{plcIndex + 6}");

            _systemSignal.ControllerSignal.Heartbeat = new Word(_mplc, $"D{pcIndex}");
            _systemSignal.ControllerSignal.SystemTimeCalibration = new WordBlock(_mplc, $"D{pcIndex + 1}", 6);

        }

        private void MappingBuffer()
        {
            List<ConveyorDefine> define;

            if (_signalGroup == 1)
            {
                var fileName = @".\Config\PLC1SignalDefine.xml";
                 define = XmlFile.Deserialize<List<ConveyorDefine>>(fileName);
                if (define is null)
                {
                    define = GetDefaultMaps();
                    define.Serialize(fileName);
                }
            }
            else
            {
                var fileName = @".\Config\PLC2SignalDefine.xml";
                define = XmlFile.Deserialize<List<ConveyorDefine>>(fileName);
                if (define is null)
                {
                    define = GetDefaultMaps();
                    define.Serialize(fileName);
                }
            }

                    int plcIndex = 111;
                    int pcIndex = 3111;

            var conveyor = define.Find(r => r.SignalGroup == _signalGroup);

            if (conveyor.SignalGroup == 1)
            {

                var readyBufferIndex = new Dictionary<int, int>();
                readyBufferIndex.Add(1, 0);//A01-1
                readyBufferIndex.Add(3, 2);//A01-03
                readyBufferIndex.Add(5, 4);//A01-05
                readyBufferIndex.Add(7, 6);//A01-07
                readyBufferIndex.Add(9, 8);//A01-09
                readyBufferIndex.Add(11, 10);//A01-11

                var BCRnoticeBufferIndex = new Dictionary<int, int>();
                BCRnoticeBufferIndex.Add(43, 42);//A08
                BCRnoticeBufferIndex.Add(48, 47);//A11-06

                var LoadHightBufferIndex = new Dictionary<int, int>();
                LoadHightBufferIndex.Add(43, 42);//A08
                LoadHightBufferIndex.Add(48, 47);//A11-06

                var PathchangeBufferIndex = new Dictionary<int, int>();
                PathchangeBufferIndex.Add(1, 0);//A01-1
                PathchangeBufferIndex.Add(3, 2);//A01-03
                PathchangeBufferIndex.Add(5, 4);//A01-05
                PathchangeBufferIndex.Add(7, 6);//A01-07
                PathchangeBufferIndex.Add(9, 8);//A01-09
                PathchangeBufferIndex.Add(11, 10);//A01-11

                var BCRcompleteBufferIndex = new Dictionary<int, int>();
                BCRcompleteBufferIndex.Add(43, 42);//A08
                BCRcompleteBufferIndex.Add(48, 47);//A11-06

                for (int bufferIndex = 0; bufferIndex < conveyor.Buffers.Count; bufferIndex++)
                {
                    string bufferName = string.Empty;
                    if (conveyor.Buffers.Exists(r => r.BufferIndex == bufferIndex + 1))
                    {
                        var bufferDefine = conveyor.Buffers.Find(r => r.BufferIndex == bufferIndex + 1);
                        bufferName = bufferDefine.BufferName;
                    }

                    var buffer = new BufferSignal(bufferIndex + 1, bufferName);
                    buffer.CommandId = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10)}");
                    buffer.CmdMode = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 1}");

                    buffer.StatusSignal.InMode = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.1");
                    buffer.StatusSignal.OutMode = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.2");
                    buffer.StatusSignal.Error = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.4");
                    buffer.StatusSignal.Auto = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.5");
                    buffer.StatusSignal.Manual = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.6");
                    buffer.StatusSignal.Presence = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.7");
                    buffer.StatusSignal.Position = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.8");
                    buffer.StatusSignal.Finish = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.9");
                    buffer.StatusSignal.EmergencyStop = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.A");

                    if (readyBufferIndex.ContainsKey(bufferIndex + 1))
                    {
                        buffer.Ready = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 3}");
                    }
                    else
                    {
                        buffer.Ready = new Word();
                    }

                    if (BCRnoticeBufferIndex.ContainsKey(bufferIndex + 1))
                    {
                        buffer.BCRnotice = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 3}");
                    }
                    else
                    {
                        buffer.BCRnotice = new Word();
                    }

                    if (LoadHightBufferIndex.ContainsKey(bufferIndex + 1))
                    {
                        buffer.LoadHeight = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 5}");
                    }
                    else
                    {
                        buffer.LoadHeight = new Word();
                    }

                    buffer.PathChangeNotice = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 4}");

                    buffer.Alarm = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 6}");

                    buffer.InitialNotice = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 9}");

                    #region PLC->PC增加單一特殊點位位置

                    if(bufferIndex+1==43)
                    {
                        buffer.BCRsignal.Item_No1 = new Word(_mplc, $"D591");//料號
                        buffer.BCRsignal.Item_No2 = new Word(_mplc, $"D592");
                        buffer.BCRsignal.Item_No3 = new Word(_mplc, $"D593");
                        buffer.BCRsignal.Item_No4 = new Word(_mplc, $"D594");
                        buffer.BCRsignal.Item_No5 = new Word(_mplc, $"D595");
                        buffer.BCRsignal.Item_No6 = new Word(_mplc, $"D596");
                        buffer.BCRsignal.Item_No7 = new Word(_mplc, $"D597");
                        buffer.BCRsignal.Item_No8 = new Word(_mplc, $"D598");
                        buffer.BCRsignal.Item_No9 = new Word(_mplc, $"D599");
                        buffer.BCRsignal.Item_No10 = new Word(_mplc, $"D600");

                        buffer.BCRsignal.Lot_ID1 = new Word(_mplc, $"D601");//效期
                        buffer.BCRsignal.Lot_ID2 = new Word(_mplc, $"D602");
                        buffer.BCRsignal.Lot_ID3 = new Word(_mplc, $"D603");
                        buffer.BCRsignal.Lot_ID4 = new Word(_mplc, $"D604");
                        buffer.BCRsignal.Lot_ID5 = new Word(_mplc, $"D605");

                        buffer.BCRsignal.Plt_Id1 = new Word(_mplc, $"D606");//效期
                        buffer.BCRsignal.Plt_Id2 = new Word(_mplc, $"D607");
                        buffer.BCRsignal.Plt_Id3 = new Word(_mplc, $"D608");
                        buffer.BCRsignal.Plt_Id4 = new Word(_mplc, $"D609");
                        buffer.BCRsignal.Plt_Id5 = new Word(_mplc, $"D610");
                    }
                    else
                    {
                        buffer.BCRsignal.Item_No1 = new Word();//料號
                        buffer.BCRsignal.Item_No2 = new Word();
                        buffer.BCRsignal.Item_No3 = new Word();
                        buffer.BCRsignal.Item_No4 = new Word();
                        buffer.BCRsignal.Item_No5 = new Word();
                        buffer.BCRsignal.Item_No6 = new Word();
                        buffer.BCRsignal.Item_No7 = new Word();
                        buffer.BCRsignal.Item_No8 = new Word();
                        buffer.BCRsignal.Item_No9 = new Word();
                        buffer.BCRsignal.Item_No10 = new Word();
                        buffer.BCRsignal.Lot_ID1 = new Word();//效期
                        buffer.BCRsignal.Lot_ID2 = new Word();
                        buffer.BCRsignal.Lot_ID3 = new Word();
                        buffer.BCRsignal.Lot_ID4 = new Word();
                        buffer.BCRsignal.Lot_ID5 = new Word();
                        buffer.BCRsignal.Plt_Id1 = new Word();//版號
                        buffer.BCRsignal.Plt_Id2 = new Word();
                        buffer.BCRsignal.Plt_Id3 = new Word();
                        buffer.BCRsignal.Plt_Id4 = new Word();
                        buffer.BCRsignal.Plt_Id5 = new Word();
                    }

                    //if (bufferIndex + 1 == 48)
                    //{
                    //    buffer.BCRsignal.Item_No1 = new Word(_mplc, $"D611");//料號
                    //    buffer.BCRsignal.Item_No2 = new Word(_mplc, $"D612");
                    //    buffer.BCRsignal.Item_No3 = new Word(_mplc, $"D613");
                    //    buffer.BCRsignal.Item_No4 = new Word(_mplc, $"D614");
                    //    buffer.BCRsignal.Item_No5 = new Word(_mplc, $"D615");
                    //    buffer.BCRsignal.Item_No6 = new Word(_mplc, $"D616");
                    //    buffer.BCRsignal.Item_No7 = new Word(_mplc, $"D617");
                    //    buffer.BCRsignal.Item_No8 = new Word(_mplc, $"D618");
                    //    buffer.BCRsignal.Item_No9 = new Word(_mplc, $"D619");
                    //    buffer.BCRsignal.Item_No10 = new Word(_mplc, $"D620");

                    //    buffer.BCRsignal.Lot_ID1 = new Word(_mplc, $"D621");//效期
                    //    buffer.BCRsignal.Lot_ID2 = new Word(_mplc, $"D622");
                    //    buffer.BCRsignal.Lot_ID3 = new Word(_mplc, $"D623");
                    //    buffer.BCRsignal.Lot_ID4 = new Word(_mplc, $"D624");
                    //    buffer.BCRsignal.Lot_ID5 = new Word(_mplc, $"D625");

                    //    buffer.BCRsignal.Plt_Id1 = new Word(_mplc, $"D626");//效期
                    //    buffer.BCRsignal.Plt_Id2 = new Word(_mplc, $"D627");
                    //    buffer.BCRsignal.Plt_Id3 = new Word(_mplc, $"D628");
                    //    buffer.BCRsignal.Plt_Id4 = new Word(_mplc, $"D629");
                    //    buffer.BCRsignal.Plt_Id5 = new Word(_mplc, $"D630");
                    //}
                    //else
                    //{
                    //    buffer.BCRsignal.Item_No1 = new Word();//料號
                    //    buffer.BCRsignal.Item_No2 = new Word();
                    //    buffer.BCRsignal.Item_No3 = new Word();
                    //    buffer.BCRsignal.Item_No4 = new Word();
                    //    buffer.BCRsignal.Item_No5 = new Word();
                    //    buffer.BCRsignal.Item_No6 = new Word();
                    //    buffer.BCRsignal.Item_No7 = new Word();
                    //    buffer.BCRsignal.Item_No8 = new Word();
                    //    buffer.BCRsignal.Item_No9 = new Word();
                    //    buffer.BCRsignal.Item_No10 = new Word();
                    //    buffer.BCRsignal.Lot_ID1 = new Word();//效期
                    //    buffer.BCRsignal.Lot_ID2 = new Word();
                    //    buffer.BCRsignal.Lot_ID3 = new Word();
                    //    buffer.BCRsignal.Lot_ID4 = new Word();
                    //    buffer.BCRsignal.Lot_ID5 = new Word();
                    //    buffer.BCRsignal.Plt_Id1 = new Word();//版號
                    //    buffer.BCRsignal.Plt_Id2 = new Word();
                    //    buffer.BCRsignal.Plt_Id3 = new Word();
                    //    buffer.BCRsignal.Plt_Id4 = new Word();
                    //    buffer.BCRsignal.Plt_Id5 = new Word();
                    //}
                    #endregion

                    buffer.ControllerSignal.CommandId = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10)}");
                    buffer.ControllerSignal.CmdMode = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10) + 1}");

                    #region PC->PLC增加單一特殊點位位置


                    #endregion

                    if (PathchangeBufferIndex.ContainsKey(bufferIndex + 1))
                    {
                        buffer.ControllerSignal.PathChangeNotice = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10) + 4}");
                    }
                    else
                    {
                        buffer.ControllerSignal.PathChangeNotice = new Word();
                    }

                    if (BCRcompleteBufferIndex.ContainsKey(bufferIndex + 1))
                    {
                        buffer.ControllerSignal.BcrComplete = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10) + 3}");
                    }
                    else
                    {
                        buffer.ControllerSignal.BcrComplete = new Word();
                    }

                    buffer.ControllerSignal.InitialNotice = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10) + 9}");

                    _bufferSignals.Add(bufferIndex + 1, buffer);
                }
            }
            else if (conveyor.SignalGroup==2)
            {
                var readyBufferIndex = new Dictionary<int, int>();
                readyBufferIndex.Add(1, 0);//B01-1
                readyBufferIndex.Add(3, 2);//B01-03
                readyBufferIndex.Add(5, 4);//B01-05
                readyBufferIndex.Add(7, 6);//B01-07
                readyBufferIndex.Add(9, 8);//B01-09
                readyBufferIndex.Add(11, 10);//B01-11

                var BCRcompleteBufferIndex = new Dictionary<int, int>();
                BCRcompleteBufferIndex.Add(25, 26);//B03

                for (int bufferIndex = 0; bufferIndex < conveyor.Buffers.Count; bufferIndex++)
                {
                    string bufferName = string.Empty;
                    if (conveyor.Buffers.Exists(r => r.BufferIndex == bufferIndex + 1))
                    {
                        var bufferDefine = conveyor.Buffers.Find(r => r.BufferIndex == bufferIndex + 1);
                        bufferName = bufferDefine.BufferName;
                    }

                    var buffer = new BufferSignal(bufferIndex + 1, bufferName);
                    buffer.CommandId = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10)}");
                    buffer.CmdMode = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 1}");

                    buffer.StatusSignal.InMode = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.1");
                    buffer.StatusSignal.OutMode = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.2");
                    buffer.StatusSignal.Error = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.4");
                    buffer.StatusSignal.Auto = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.5");
                    buffer.StatusSignal.Manual = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.6");
                    buffer.StatusSignal.Presence = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.7");
                    buffer.StatusSignal.Position = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.8");
                    buffer.StatusSignal.Finish = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.9");
                    buffer.StatusSignal.EmergencyStop = new Bit(_mplc, $"D{plcIndex + (bufferIndex * 10) + 2}.A");

                    if (readyBufferIndex.ContainsKey(bufferIndex + 1))
                    {
                        buffer.Ready = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 3}");
                    }
                    else
                    {
                        buffer.Ready = new Word();
                    }


                    buffer.PathChangeNotice = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 4}");

                    buffer.Alarm = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 6}");

                    buffer.InitialNotice = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 9}");

                    #region PLC->PC增加單一特殊點位位置
                    if (bufferIndex + 1 == 25)
                    {
                        buffer.BCRnotice = new Word(_mplc, $"D{plcIndex + (bufferIndex * 10) + 3}");
                        buffer.BCRsignal.Item_No1 = new Word(_mplc, $"D361");//料號
                        buffer.BCRsignal.Item_No2 = new Word(_mplc, $"D362");
                        buffer.BCRsignal.Item_No3 = new Word(_mplc, $"D363");
                        buffer.BCRsignal.Item_No4 = new Word(_mplc, $"D364");
                        buffer.BCRsignal.Item_No5 = new Word(_mplc, $"D365");
                        buffer.BCRsignal.Item_No6 = new Word(_mplc, $"D366");
                        buffer.BCRsignal.Item_No7 = new Word(_mplc, $"D367");
                        buffer.BCRsignal.Item_No8 = new Word(_mplc, $"D368");
                        buffer.BCRsignal.Item_No9 = new Word(_mplc, $"D369");
                        buffer.BCRsignal.Item_No10 = new Word(_mplc, $"D370");

                        buffer.BCRsignal.Lot_ID1 = new Word(_mplc, $"D371");//效期
                        buffer.BCRsignal.Lot_ID2 = new Word(_mplc, $"D372");
                        buffer.BCRsignal.Lot_ID3 = new Word(_mplc, $"D373");
                        buffer.BCRsignal.Lot_ID4 = new Word(_mplc, $"D374");
                        buffer.BCRsignal.Lot_ID5 = new Word(_mplc, $"D375");

                        buffer.BCRsignal.Plt_Id1 = new Word(_mplc, $"D376");//效期
                        buffer.BCRsignal.Plt_Id2 = new Word(_mplc, $"D377");
                        buffer.BCRsignal.Plt_Id3 = new Word(_mplc, $"D378");
                        buffer.BCRsignal.Plt_Id4 = new Word(_mplc, $"D379");
                        buffer.BCRsignal.Plt_Id5 = new Word(_mplc, $"D380");
                    }
                    else
                    {
                        buffer.BCRnotice = new Word();
                        buffer.BCRsignal.Item_No1 = new Word();//料號
                        buffer.BCRsignal.Item_No2 = new Word();
                        buffer.BCRsignal.Item_No3 = new Word();
                        buffer.BCRsignal.Item_No4 = new Word();
                        buffer.BCRsignal.Item_No5 = new Word();
                        buffer.BCRsignal.Item_No6 = new Word();
                        buffer.BCRsignal.Item_No7 = new Word();
                        buffer.BCRsignal.Item_No8 = new Word();
                        buffer.BCRsignal.Item_No9 = new Word();
                        buffer.BCRsignal.Item_No10 = new Word();
                        buffer.BCRsignal.Lot_ID1 = new Word();//效期
                        buffer.BCRsignal.Lot_ID2 = new Word();
                        buffer.BCRsignal.Lot_ID3 = new Word();
                        buffer.BCRsignal.Lot_ID4 = new Word();
                        buffer.BCRsignal.Lot_ID5 = new Word();
                        buffer.BCRsignal.Plt_Id1 = new Word();//版號
                        buffer.BCRsignal.Plt_Id2 = new Word();
                        buffer.BCRsignal.Plt_Id3 = new Word();
                        buffer.BCRsignal.Plt_Id4 = new Word();
                        buffer.BCRsignal.Plt_Id5 = new Word();
                    }
                    #endregion

                    #region PC->PLC增加單一特殊點位位置

                    #endregion

                    buffer.ControllerSignal.CommandId = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10)}");
                    buffer.ControllerSignal.CmdMode = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10) + 1}");
                    buffer.ControllerSignal.PathChangeNotice = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10) + 4}");

                    if (BCRcompleteBufferIndex.ContainsKey(bufferIndex + 1))
                    {
                        buffer.ControllerSignal.BcrComplete = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10) + 3}");
                    }
                    else
                    {
                        buffer.ControllerSignal.BcrComplete = new Word();
                    }

                    buffer.ControllerSignal.InitialNotice = new Word(_mplc, $"D{pcIndex + (bufferIndex * 10) + 9}");

                    _bufferSignals.Add(bufferIndex + 1, buffer);
                }

            }
        }

        public SystemSignal GetSystemSignal()
        {
            return _systemSignal;
        }
    }
}
