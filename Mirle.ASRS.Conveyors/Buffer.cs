using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mirle.ASRS.Conveyors.Signal;
using Mirle.MPLC.DataType;
using Mirle;

namespace Mirle.ASRS.Conveyors
{
    public class Buffer
    {
        private readonly bool[] _alarmBit = new bool[16];

        private bool _onIniatlNotice = false;

        public delegate void AlarmEventHandler(object sender, AlarmEventArgs e);
        public delegate void BufferEventHandler(object sender, BufferEventArgs e);
        public delegate void InitialNoticeEventHandler(object sender, BufferEventArgs e);

        public event AlarmEventHandler OnAlarmTrigger;
        public event AlarmEventHandler OnAlarmClear;
        public event BufferEventHandler OnBufferCommandReceive;
        public event BufferEventHandler OnBufferPathNoticeChange;
        public event InitialNoticeEventHandler OnIniatlNotice;
        public event InitialNoticeEventHandler OnIniatlNoticeComplete;

        public BufferSignal Signal { get; }

        public int BufferIndex => Signal.BufferIndex;
        public string BufferName => Signal.BufferName;

        public int CommandId => Signal.CommandId.GetValue();
        public int PathNotice => Signal.PathChangeNotice.GetValue();
        public int CmdMode => Signal.CmdMode.GetValue();
        public int Ready => Signal.Ready.GetValue();
        public int BcrNotice => Signal.BCRnotice.GetValue();
        public int LoadHeight => Signal.LoadHeight.GetValue();
        public int InitialNotice => Signal.InitialNotice.GetValue();
        public bool InMode => Signal.StatusSignal.InMode.IsOn() && Signal.StatusSignal.OutMode.IsOff();
        public bool OutMode => Signal.StatusSignal.InMode.IsOff() && Signal.StatusSignal.OutMode.IsOn();
        public bool Error => Signal.StatusSignal.Error.IsOn();
        public bool Auto => Signal.StatusSignal.Auto.IsOn() && Signal.StatusSignal.Manual.IsOff();
        public bool Manual => Signal.StatusSignal.Auto.IsOff() && Signal.StatusSignal.Manual.IsOn();
        public bool Presence => Signal.StatusSignal.Presence.IsOn();
        public bool Position => Signal.StatusSignal.Position.IsOn();
        public string Item_No => Signal.BCRsignal.Item_No1.GetValue().ToString()
            + Signal.BCRsignal.Item_No2.GetValue().ToString()
            + Signal.BCRsignal.Item_No3.GetValue().ToString()
            + Signal.BCRsignal.Item_No4.GetValue().ToString()
            + Signal.BCRsignal.Item_No5.GetValue().ToString()
            + Signal.BCRsignal.Item_No6.GetValue().ToString()
            + Signal.BCRsignal.Item_No7.GetValue().ToString()
            + Signal.BCRsignal.Item_No8.GetValue().ToString()
            + Signal.BCRsignal.Item_No9.GetValue().ToString()
            + Signal.BCRsignal.Item_No10.GetValue().ToString();
        public string Lot_ID => Signal.BCRsignal.Lot_ID1.GetValue().ToString()
            + Signal.BCRsignal.Lot_ID2.GetValue().ToString()
            + Signal.BCRsignal.Lot_ID3.GetValue().ToString()
            + Signal.BCRsignal.Lot_ID4.GetValue().ToString()
            + Signal.BCRsignal.Lot_ID5.GetValue().ToString();
        public string Plt_Id => Signal.BCRsignal.Plt_Id1.GetValue().ToString()
            + Signal.BCRsignal.Plt_Id2.GetValue().ToString()
            + Signal.BCRsignal.Plt_Id3.GetValue().ToString()
            + Signal.BCRsignal.Plt_Id4.GetValue().ToString()
            + Signal.BCRsignal.Plt_Id5.GetValue().ToString();



        public Buffer(BufferSignal signal)
        {
            Signal = signal;
        }

        protected internal virtual void Refresh()//自己寫入的值，自己清，當寫入的值和確認PLC寫的值條件=>確認寫入成功，才清理自己寫的值
        {
            if (Signal.CmdMode.GetValue() > 0 && Signal.CmdMode.GetValue() == Signal.ControllerSignal.CmdMode.GetValue()
                )
            {
                Signal.ControllerSignal.CmdMode.Clear();
                OnBufferCommandReceive?.Invoke(this, new BufferEventArgs(Signal.BufferIndex, Signal.BufferName));
            }
            else if (Signal.CommandId.GetValue() > 0 && Signal.CommandId.GetValue() == Signal.ControllerSignal.CommandId.GetValue())
            {
                Signal.ControllerSignal.CommandId.Clear();
                OnBufferCommandReceive?.Invoke(this, new BufferEventArgs(Signal.BufferIndex, Signal.BufferName));
            }

            if (Signal.PathChangeNotice.GetValue() > 0 &&  Signal.PathChangeNotice.GetValue()==Signal.ControllerSignal.PathChangeNotice.GetValue())
            {
                Signal.ControllerSignal.PathChangeNotice.SetValue(0);
                OnBufferPathNoticeChange?.Invoke(this, new BufferEventArgs(Signal.BufferIndex, Signal.BufferName));
            }
            if (Signal.BCRnotice.GetValue() == 0 && Signal.ControllerSignal.BcrComplete.GetValue()==1)
            {
                Signal.ControllerSignal.BcrComplete.SetValue(0);
                OnBufferPathNoticeChange?.Invoke(this, new BufferEventArgs(Signal.BufferIndex, Signal.BufferName));
            }
            CheckIniatlNotice();
            CheckAlarmBit();
        }

        private void CheckAlarmBit()
        {
            for (int i = 0; i < 15; i++)
            {
                CheckAlarmBit(i, ref _alarmBit[i]);
            }
        }
        private void CheckAlarmBit(int bitIndex, ref bool reportedFlag)
        {
            if (Signal.Alarm.GetBit(bitIndex).IsOn() && reportedFlag == false)
            {
                reportedFlag = true;
                OnAlarmTrigger?.Invoke(this, new AlarmEventArgs(Signal.BufferIndex, Signal.BufferName, bitIndex));
            }
            else if (Signal.Alarm.GetBit(bitIndex).IsOff() && reportedFlag)
            {
                reportedFlag = false;
                OnAlarmClear?.Invoke(this, new AlarmEventArgs(Signal.BufferIndex, Signal.BufferName, bitIndex));
            }
        }
        private void CheckIniatlNotice()
        {
            if (_onIniatlNotice == false && Signal.ControllerSignal.InitialNotice.GetValue() == 1)
            {
                _onIniatlNotice = true;
                OnIniatlNotice?.Invoke(this, new BufferEventArgs(Signal.BufferIndex, Signal.BufferName));
            }
            else if (_onIniatlNotice == true && Signal.ControllerSignal.InitialNotice.GetValue() == 1 && Signal.InitialNotice.GetValue() == 1)
            {
                _onIniatlNotice = false;
                Signal.ControllerSignal.InitialNotice.SetValue(0);
                OnIniatlNoticeComplete?.Invoke(this, new BufferEventArgs(Signal.BufferIndex, Signal.BufferName));
            }
        }

        public Task<bool> WriteCommandIdAsync(string Command, int commandMode)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (CommandId != 0) return false;

                    Signal.ControllerSignal.CmdMode.SetValue(commandMode);
                    Signal.ControllerSignal.CommandId.SetValue(Convert.ToInt32(Command));

                    Task.Delay(500).Wait();
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public Task<bool> BCRNoticeComplete(int BCRNoticeComplete)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (CommandId != 0) return false;

                    Signal.ControllerSignal.BcrComplete.SetValue(BCRNoticeComplete);
                    Task.Delay(500).Wait();
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public Task<bool> A4EmptysupplyOn()
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.ControllerSignal.A4Emptysupply.SetValue(1);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }
        public Task<bool> InitialNoticeTrigger()
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.ControllerSignal.InitialNotice.SetValue(1);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public Task<bool> Switch_Mode(int mode)
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.ControllerSignal.Switch_Mode.SetValue(mode);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public Task<bool> WritePathChabgeNotice(int path)
        {
            return Task.Run(() =>
            {
                try
                {
                    Signal.ControllerSignal.PathChangeNotice.SetValue(path);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}
