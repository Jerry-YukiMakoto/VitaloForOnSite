using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mirle.ASRS.AWCS.View
{
    public partial class AlarmView : Form
    {
        public AlarmView()
        {
            InitializeComponent();
        }
        public void UpdateListData(Dictionary<string, string> data)
        {
            List<string> dataText = new List<string>();
            foreach (var _data in data)
            {
                string context = $"[{_data.Value}]- {_data.Key}";
                dataText.Add(context);
            }
            listBox1.Items.Clear();
            listBox1.Items.AddRange(dataText.ToArray());
        }
        public void ClearListData()
        {
            listBox1.Items.Clear();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            listBox1.Visible = !listBox1.Visible;
            if (listBox1.Visible)
            {
                Size = new Size(1000, 220);
            }
            else
            {
                Size = new Size(1000, 69);
            }
        }
    }
}
