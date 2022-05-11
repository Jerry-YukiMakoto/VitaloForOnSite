using CsvHelper;
using Mirle.MPLC;
using Mirle.MPLC.DataType;
using Mirle.MPLC.FileData;
using Mirle.MPLCViewer.Model;
using Mirle.MPLCViewer.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;
using Mirle.ASRS.Converyor;

namespace Mirle.MPLCViewer.View
{
    public partial class frmFindMultipleView : Form
    {
        private readonly MPLCViewerINI _config;
        private readonly frmMain _frmMainView;
        private readonly FileReader _fReader;
        private readonly IMPLCViewController _mplcViewController;
        private readonly string[] _filePaths;
        DataTable resultDatatable;

        private static string _fileTime = "";

        public frmFindMultipleView(MPLCViewerINI config, frmMain frmMainView, FileReader fReader, IMPLCViewController mplcViewController, string[] filePaths)
        {
            _config = config;
            _frmMainView = frmMainView;
            _fReader = fReader;
            _mplcViewController = mplcViewController;
            _filePaths = filePaths;
            InitializeComponent();
        }

        private async void ButtonFind_Click(object sender, EventArgs e)
        {
            buttonFindAllMotor.Enabled = false;
            TopMost = false;
            bool DontCheckNegative = false;

            try
            {
                var dataView = _fReader.GetDataView();
                IDataType signal = null;
                var results = new List<FindMultipleResult>();
                

                foreach (var address in richTextBoxAddress.Text.ToUpper().Split(','))
                {
                    if (radioButtonBit.Checked)
                    {
                        signal = new Bit(dataView, address);
                        results.AddRange(await SearchResults(dataView, signal, DontCheckNegative));
                    }
                    else if (radioButtonWord.Checked)
                    {
                        signal = new Word(dataView, address);
                        results.AddRange(await SearchResults(dataView, signal, DontCheckNegative));
                    }
                    else if (radioButtonDWord.Checked)
                    {
                        signal = new DWord(dataView, address);
                        results.AddRange(await SearchResults(dataView, signal, DontCheckNegative));
                    }
                    else if (radioButtonWordBlock.Checked)
                    {
                        var wordBlock = new WordBlock(dataView, address, Convert.ToInt32(textBoxLength.Text));
                        results.AddRange(await SearchResultsForWordBlock(dataView, wordBlock));
                    }
                }

                resultDatatable = new DataTable();
                InitDataTableCols(resultDatatable);
                AddResultToDataTable(results,resultDatatable);
                if (resultDatatable.Rows.Count == 0) MessageBox.Show($"No Data Found!! Address: {richTextBoxAddress.Text}");

                dataGridView1.DataSource = resultDatatable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "Find Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonFindAllMotor.Enabled = true;
                TopMost = true;
            }
        }

        private void InitDataTableCols(DataTable dataTable)
        {
            dataTable.Columns.Add("Time");
            foreach (var address in richTextBoxAddress.Text.ToUpper().Split(','))
            {
                dataTable.Columns.Add(address, typeof(string));
            }
        }

        private void AddResultToDataTable(List<FindMultipleResult> results, DataTable dataTable)
        {
            var ResultGroup = from Result in results group Result by Result.Time into newResultgroup orderby newResultgroup.Key select newResultgroup;
            foreach (var result in ResultGroup)
            {
                DataRow dr1 = dataTable.NewRow();
                dr1["Time"] = result.Key;
                foreach (var address in richTextBoxAddress.Text.ToUpper().Split(','))
                {
                    Dictionary<string, string> names = new Dictionary<string, string>();
                    if (result.Where(p => p.Address == address).Count() > 0)
                    {
                        dr1[address] = result.Where(p => p.Address == address).Select(p => p.Value).ToList()[0];
                    }
                    else
                    {
                        dr1[address] = string.Empty;
                    }
                }
                dataTable.Rows.Add(dr1);
            }
        }

        private Task<List<FindMultipleResult>> SearchResults(FileDataViewer dataView, IDataType signal, bool needCheckNegative)
        {
            return Task.Run(() =>
             {
                 var result = new List<FindMultipleResult>();
                 var points = dataView.Query(DateTime.MinValue, DateTime.MaxValue).ToList();
                 var lastValue = 0;
                 foreach (var point in points)
                 {
                     dataView.RefreshRawData(point);
                     var actual = Convert.ToInt32(signal);
                     if (needCheckNegative)
                         actual = checkNegative(actual);

                     var excepted = radioButtonHexadecimal.Checked
                         ? Convert.ToInt32(textBoxEqualsValue.Text)
                         : Convert.ToInt32(textBoxEqualsValue.Text);
                     var actualString = radioButtonHexadecimal.Checked
                         ? actual.ToString("X") : actual.ToString();

                     if (radioButtonDifferent.Checked && actual != lastValue)
                     {
                         result.Add(new FindMultipleResult() {Address = signal.Address, Time = point.ToString("HH:mm:ss.fffff"), Value = actualString });
                         lastValue = actual;
                     }
                     else if (radioButtonEquals.Checked && actual == excepted)
                     {
                         result.Add(new FindMultipleResult() { Address = signal.Address, Time = point.ToString("HH:mm:ss.fffff"), Value = actualString });
                     }
                     else if (radioButtonNotEquals.Checked && actual != excepted)
                     {
                         result.Add(new FindMultipleResult() { Address = signal.Address, Time = point.ToString("HH:mm:ss.fffff"), Value = actualString });
                     }
                     else
                     {
                         result.Add(new FindMultipleResult() { Address = signal.Address, Time = point.ToString("HH:mm:ss.fffff"), Value = actualString });
                     }
                 }

                 return result;
             });
        }

        private static void ExportResult(DataTable result, string path)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    using (var csv = new CsvWriter(writer))
                    {
                        foreach (DataColumn column in result.Columns)
                        {
                            csv.WriteField(column.ColumnName);
                        }
                        csv.NextRecord();
                        // Write row values
                        foreach (DataRow row in result.Rows)
                        {
                            foreach (DataColumn column in result.Columns)
                            {
                                csv.WriteField(row[column]);
                            }
                            csv.NextRecord();
                        }

                    }
                }
                MessageBox.Show("Export Result Success!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export Result Fail!! : {path}.csv"
                    , "Export Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Task<List<FindMultipleResult>> SearchResultsForWordBlock(FileDataViewer dataView, WordBlock signal)
        {
            return Task.Run(() =>
            {
                var result = new List<FindMultipleResult>();
                var points = dataView.Query(DateTime.MinValue, DateTime.MaxValue).ToList();
                var lastValue = string.Empty;
                foreach (var point in points)
                {
                    dataView.RefreshRawData(point);
                    var actual = signal.GetValue().ToASCII().ToUpper();

                    if (radioButtonDifferent.Checked && actual != lastValue)
                    {
                        result.Add(new FindMultipleResult() { Address = signal.Address, Time = point.ToString("HH:mm:ss.fffff"), Value = actual });
                        lastValue = actual;
                    }
                    else if (radioButtonEquals.Checked && actual.Contains(textBoxEqualsValue.Text.ToUpper()))
                    {
                        result.Add(new FindMultipleResult() { Address = signal.Address, Time = point.ToString("HH:mm:ss.fffff"), Value = actual });
                    }
                    else if (radioButtonNotEquals.Checked && !actual.Contains(textBoxEqualsValue.Text.ToUpper()))
                    {
                        result.Add(new FindMultipleResult() { Address = signal.Address, Time = point.ToString("HH:mm:ss.fffff"), Value = actual });
                    }
                    else
                    {
                        result.Add(new FindMultipleResult() { Address = signal.Address, Time = point.ToString("HH:mm:ss.fffff"), Value = actual });
                    }
                }

                return result;
            });
        }

        private void FrmFindView_Load(object sender, EventArgs e)
        {
            if (_filePaths != null)
            {
                _fileTime = _filePaths.FirstOrDefault();
                _fileTime = _fileTime.Substring(_fileTime.Length - 14, 10);   //ex:  .....2019091321.log
            }
        }

        IDataType lastSignal;
        private void TimerUI_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBoxAddress.Text)) lastSignal = null;
            textBoxLength.Enabled = radioButtonWordBlock.Checked;
            groupBoxOption.Enabled = radioButtonWord.Checked || radioButtonDWord.Checked;
            textBoxEqualsValue.Enabled = !(radioButtonAll.Checked || radioButtonDifferent.Checked);
            var signal = _mplcViewController.CurrentFocusedSignal;
            var currentAddress = string.Empty;

            if (signal != null)
            {
                if (lastSignal != null)
                {
                    if (lastSignal.GetType().Name != signal.GetType().Name)
                    {
                        TopMost = false;
                        MessageBox.Show("所選的Address Type與目前不一致，請重新選擇!!");
                        TopMost = true;
                        return;
                    }
                }
                switch (signal)
                {
                    case Bit bit:
                        currentAddress = bit.Address;
                        radioButtonBit.Checked = true;
                        lastSignal = bit;
                        break;

                    case Word word:
                        currentAddress = word.Address;
                        radioButtonWord.Checked = true;
                        lastSignal = word;
                        break;

                    case DWord dWord:
                        currentAddress = dWord.Address;
                        radioButtonDWord.Checked = true;
                        lastSignal = dWord;
                        break;

                    case WordBlock wordBlock:
                        currentAddress = wordBlock.Address;
                        textBoxLength.Text = wordBlock.Length.ToString();
                        radioButtonWordBlock.Checked = true;
                        lastSignal = wordBlock;
                        break;
                }

                if (!richTextBoxAddress.Text.Contains(currentAddress))
                {
                    richTextBoxAddress.Text += richTextBoxAddress.Text == string.Empty ? currentAddress : "," + currentAddress;
                }
            }

        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var rowIndex = e.RowIndex;
                var row = dataGridView1.Rows[rowIndex];
                var s = row.Cells[nameof(FindMultipleResult.Time)].Value.ToString();
                var dateTime = DateTime.ParseExact(s, "HH:mm:ss.fffff", null);

                _frmMainView.FindPointByDateTime(dateTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
            }
        }
        
        private int checkNegative(int value)
        {
            if (value > 32767)
                value = value - 65536;
            return value;
        }

        private void buttonExportCsv_Click(object sender, EventArgs e)
        {
            if (resultDatatable == null || resultDatatable.Rows.Count == 0)
            {
                MessageBox.Show("Export Result Fail!! No data can be exported!!");
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "請選擇要儲存的檔案路徑";
            saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            saveFile.Filter = "CSV file (*.csv)|*.csv";

            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                ExportResult(resultDatatable, saveFile.FileName);
            }
        }
    }
}
