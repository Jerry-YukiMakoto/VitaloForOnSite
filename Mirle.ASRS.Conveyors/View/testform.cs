using Mirle.MPLC;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.DataBlocks.DeviceRange;
using Mirle.MPLC.SharedMemory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mirle.ASRS.WCS.Controller
{
    public partial class Form1 : Form
    {
        private IMPLCProvider _test;

        public Form1()
        {
            InitializeComponent();
        }
        private void SimMainView_Load(object sender, EventArgs e)
        {
            var smWriter = new SMReadWriter();
            var blockInfos = GetBlockInfos();
            foreach (var block in blockInfos)
            {
                smWriter.AddDataBlock(new SMDataBlockInt32(block.DeviceRange, $@"Global\{block.SharedMemoryName}"));
            }
            testsimulator(smWriter);
        }

        private IEnumerable<BlockInfo> GetBlockInfos()
        {
            yield return new BlockInfo(new DDeviceRange("D101", "D630"), "Read", 0);
            yield return new BlockInfo(new DDeviceRange("D3101", "D3590"), "Write", 1);
        }

        private void testsimulator(IMPLCProvider test)
        {
            _test = test;
        }

        private void buttonwrite(object sender, EventArgs e)
        {
            int text1 = 0;
            if (textBox1.Text != "")
            {
                text1 = int.Parse(textBox1.Text);
            }

            int text2 = 0;
            if (textBox2.Text != "")
            {
                text2 = int.Parse(textBox2.Text);
            }

            int text3 = 0;
            if (textBox3.Text != "")
            {
                text3 = int.Parse(textBox3.Text);
            }

            int text4 = 0;
            if (textBox4.Text != "")
            {
                text4 = int.Parse(textBox4.Text);
            }

            int text5 = 0;
            if (textBox5.Text != "")
            {
                text5 = int.Parse(textBox5.Text);
            }
            int text6 = 0;
            if (textBox6.Text != "")
            {
                text6 = int.Parse(textBox6.Text);
            }
            int text7 = 0;
            if (textBox7.Text != "")
            {
                text7 = int.Parse(textBox7.Text);
            }

            if (CMD.Checked)
            {
                _test.WriteWord("D" + (101 + text1 * 10), text2);
            }
            if (BCRItem_Name.Checked)
            {
                _test.WriteWord("D591", text3);
                _test.WriteWord("D592", text4);
                _test.WriteWord("D593", text5);
                _test.WriteWord("D594", text6);
                _test.WriteWord("D595", text7);
                _test.WriteWord("D596", text3);
                _test.WriteWord("D597", text4);
                _test.WriteWord("D598", text5);
                _test.WriteWord("D599", text6);
                _test.WriteWord("D600", text7);
            }
            if (BCRPlt_Id.Checked)
            {
                _test.WriteWord("D606", text3);
                _test.WriteWord("D607", text4);
                _test.WriteWord("D608", text5);
                _test.WriteWord("D609", text6);
                _test.WriteWord("D610", text7);
            }
            if (BCRLotID.Checked)
            {
                _test.WriteWord("D601", text3);
                _test.WriteWord("D602", text4);
                _test.WriteWord("D603", text5);
                _test.WriteWord("D604", text6);
                _test.WriteWord("D605", text7);
            }
            if (BCRItem_Name.Checked)
            {
                _test.WriteWord("D611", text3);
                _test.WriteWord("D612", text4);
                _test.WriteWord("D613", text5);
                _test.WriteWord("D614", text6);
                _test.WriteWord("D615", text7);
                _test.WriteWord("D616", text3);
                _test.WriteWord("D617", text4);
                _test.WriteWord("D618", text5);
                _test.WriteWord("D619", text6);
                _test.WriteWord("D620", text7);
            }
            if (BCRPlt_Id.Checked)
            {
                _test.WriteWord("D626", text3);
                _test.WriteWord("D627", text4);
                _test.WriteWord("D628", text5);
                _test.WriteWord("D629", text6);
                _test.WriteWord("D630", text7);
            }
            if (BCRLotID.Checked)
            {
                _test.WriteWord("D621", text3);
                _test.WriteWord("D622", text4);
                _test.WriteWord("D623", text5);
                _test.WriteWord("D624", text6);
                _test.WriteWord("D625", text7);
            }
            if (mode.Checked)
            {
                _test.WriteWord("D" + (101 + text1 * 10 + 1), text2);
            }
            if (Auto.Checked)
            {
                _test.SetBitOn("D" + (101 + text1 * 10 + 2.5));
            }
            if (presence.Checked)
            {
                _test.SetBitOn("D" + (101 + text1 * 10 + 2.7));
            }
            if (initialnotice.Checked)
            {
                _test.WriteWord("D" + (101 + text1 * 10 + 9), text2);
            }
            if (ready.Checked)
            {
                _test.WriteWord("D" + (101 + text1 * 10 + 3), text2);
            }
            if (path.Checked)
            {
                _test.WriteWord("D" + (101 + text1 * 10 + 4), text2);
            }
            if (switchmode.Checked)
            {
                _test.WriteWord("D" + (101 + text1 * 10 + 8), text2);
            }
            if (Inmode.Checked)
            {
                _test.SetBitOn("D" + (101 + text1 * 10 + 2.1));
            }
            if (Outmode.Checked)
            {
                _test.SetBitOn("D" + (101 + text1 * 10 + 2.2));
            }
            if (error.Checked)
            {
                _test.SetBitOn("D" + (101 + text1 * 10 + 2.4));
            }
            if (emptynumber.Checked)
            {
                _test.WriteWord("D" + (101 + text1 * 10 + 7), text2);
            }
            if (errorOff.Checked)
            {
                _test.SetBitOff("D" + (101 + text1 * 10 + 2.4));
            }
            if (InmodeOFF.Checked)
            {
                _test.SetBitOff("D" + (101 + text1 * 10 + 2.1));
            }
            if (outmodeoff.Checked)
            {
                _test.SetBitOff("D" + (101 + text1 * 10 + 2.2));
            }
            if (Autooff.Checked)
            {
                _test.SetBitOff("D" + (101 + text1 * 10 + 2.5));
            }
            if (presenceOFF.Checked)
            {
                _test.SetBitOff("D" + (101 + text1 * 10 + 2.7));
            }
           

            showwritevalue.Text = _test.ReadWord("D601").ToString();
            showwritevalue.Text += $",{_test.ReadWord("D" + (3101 + text1 * 10+1))}";
            showwritevalue.Text += $",{_test.ReadWord("D" + (3101 + text1 * 10+4))}";
            showwritevalue.Text += $",{_test.ReadWord("D" + (3101 + text1 * 10 + 9))}";
            showwritevalue.Text += $",{_test.ReadWord("D" + (3101 + text1 * 10 + 8))}";

        }
    }
}
