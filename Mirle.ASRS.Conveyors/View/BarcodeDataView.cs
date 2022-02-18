using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mirle.ASRS.Converyor.View
{
    public partial class BarcodeDataView : UserControl
    {
        public int BufferIndex { get; set; }
        public BarcodeDataView()
        {
            InitializeComponent();
        }
        public void Refresh(string bcr1, string bcr2, string bcr3) 
        {
            Refresh(tbBcr1, bcr1);
            Refresh(tbBcr2, bcr2);
            Refresh(tbBcr3, bcr3);
        }
        private void Refresh(TextBox textBox, string value) 
        {
            if (InvokeRequired)
            {
                var action = new Action<TextBox, string>(Refresh);
                Invoke(action, textBox, value);
            }
            else 
            {
                textBox.Text = value;
            }
        }
    }
}
