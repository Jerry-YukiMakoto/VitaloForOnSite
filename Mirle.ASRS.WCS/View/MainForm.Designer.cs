namespace Mirle.ASRS.WCS.View
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tlpMainSts = new System.Windows.Forms.TableLayoutPanel();
            this.lblTimer = new System.Windows.Forms.Label();
            this.picMirle = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelConn = new System.Windows.Forms.TableLayoutPanel();
            this.chkOnline = new System.Windows.Forms.CheckBox();
            this.lblPLCConn = new System.Windows.Forms.Label();
            this.lblDBConn = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.spcView = new System.Windows.Forms.SplitContainer();
            this.spcMainView = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanelSideBtn = new System.Windows.Forms.TableLayoutPanel();
            this.testbutton = new System.Windows.Forms.Button();
            this.btnEquMaintain = new System.Windows.Forms.Button();
            this.btnCmdMaintain = new System.Windows.Forms.Button();
            this.GridCmd = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tlpMainSts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMirle)).BeginInit();
            this.tableLayoutPanelConn.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcView)).BeginInit();
            this.spcView.Panel1.SuspendLayout();
            this.spcView.Panel2.SuspendLayout();
            this.spcView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcMainView)).BeginInit();
            this.spcMainView.Panel2.SuspendLayout();
            this.spcMainView.SuspendLayout();
            this.tableLayoutPanelSideBtn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridCmd)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tlpMainSts);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.spcView);
            this.splitContainer1.Panel2.Margin = new System.Windows.Forms.Padding(50, 0, 0, 0);
            this.splitContainer1.Size = new System.Drawing.Size(1924, 1050);
            this.splitContainer1.SplitterDistance = 112;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            // 
            // tlpMainSts
            // 
            this.tlpMainSts.ColumnCount = 4;
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.1643F));
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.1643F));
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 57.50709F));
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.1643F));
            this.tlpMainSts.Controls.Add(this.lblTimer, 0, 0);
            this.tlpMainSts.Controls.Add(this.picMirle, 0, 0);
            this.tlpMainSts.Controls.Add(this.tableLayoutPanelConn, 3, 0);
            this.tlpMainSts.Controls.Add(this.panel1, 2, 0);
            this.tlpMainSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMainSts.Location = new System.Drawing.Point(0, 0);
            this.tlpMainSts.Margin = new System.Windows.Forms.Padding(4);
            this.tlpMainSts.Name = "tlpMainSts";
            this.tlpMainSts.RowCount = 1;
            this.tlpMainSts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainSts.Size = new System.Drawing.Size(1924, 112);
            this.tlpMainSts.TabIndex = 0;
            // 
            // lblTimer
            // 
            this.lblTimer.BackColor = System.Drawing.SystemColors.Control;
            this.lblTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTimer.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimer.ForeColor = System.Drawing.Color.Black;
            this.lblTimer.Location = new System.Drawing.Point(276, 0);
            this.lblTimer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(264, 112);
            this.lblTimer.TabIndex = 268;
            this.lblTimer.Text = "yyyy/MM/dd hh:mm:ss";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picMirle
            // 
            this.picMirle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picMirle.Image = ((System.Drawing.Image)(resources.GetObject("picMirle.Image")));
            this.picMirle.Location = new System.Drawing.Point(4, 4);
            this.picMirle.Margin = new System.Windows.Forms.Padding(4);
            this.picMirle.Name = "picMirle";
            this.picMirle.Size = new System.Drawing.Size(264, 104);
            this.picMirle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMirle.TabIndex = 267;
            this.picMirle.TabStop = false;
            // 
            // tableLayoutPanelConn
            // 
            this.tableLayoutPanelConn.ColumnCount = 1;
            this.tableLayoutPanelConn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelConn.Controls.Add(this.chkOnline, 0, 2);
            this.tableLayoutPanelConn.Controls.Add(this.lblPLCConn, 0, 0);
            this.tableLayoutPanelConn.Controls.Add(this.lblDBConn, 0, 1);
            this.tableLayoutPanelConn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelConn.Location = new System.Drawing.Point(1654, 4);
            this.tableLayoutPanelConn.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanelConn.Name = "tableLayoutPanelConn";
            this.tableLayoutPanelConn.RowCount = 3;
            this.tableLayoutPanelConn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelConn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelConn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelConn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelConn.Size = new System.Drawing.Size(266, 104);
            this.tableLayoutPanelConn.TabIndex = 0;
            // 
            // chkOnline
            // 
            this.chkOnline.AutoSize = true;
            this.chkOnline.Checked = true;
            this.chkOnline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkOnline.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkOnline.Location = new System.Drawing.Point(4, 72);
            this.chkOnline.Margin = new System.Windows.Forms.Padding(4);
            this.chkOnline.Name = "chkOnline";
            this.chkOnline.Size = new System.Drawing.Size(258, 28);
            this.chkOnline.TabIndex = 4;
            this.chkOnline.Text = "OnLine";
            this.chkOnline.UseVisualStyleBackColor = true;
            this.chkOnline.Visible = false;
            // 
            // lblPLCConn
            // 
            this.lblPLCConn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPLCConn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPLCConn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPLCConn.Location = new System.Drawing.Point(4, 0);
            this.lblPLCConn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPLCConn.Name = "lblPLCConn";
            this.lblPLCConn.Size = new System.Drawing.Size(258, 34);
            this.lblPLCConn.TabIndex = 3;
            this.lblPLCConn.Text = "PLC Status";
            this.lblPLCConn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDBConn
            // 
            this.lblDBConn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDBConn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDBConn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDBConn.Location = new System.Drawing.Point(4, 34);
            this.lblDBConn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDBConn.Name = "lblDBConn";
            this.lblDBConn.Size = new System.Drawing.Size(258, 34);
            this.lblDBConn.TabIndex = 1;
            this.lblDBConn.Text = "DB Status";
            this.lblDBConn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(547, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(657, 106);
            this.panel1.TabIndex = 269;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 18;
            this.listBox1.Location = new System.Drawing.Point(161, 9);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(476, 94);
            this.listBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(0, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 40);
            this.label1.TabIndex = 1;
            this.label1.Text = "Crane異常";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spcView
            // 
            this.spcView.BackColor = System.Drawing.SystemColors.Control;
            this.spcView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcView.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.spcView.Location = new System.Drawing.Point(0, 0);
            this.spcView.Margin = new System.Windows.Forms.Padding(4);
            this.spcView.Name = "spcView";
            this.spcView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcView.Panel1
            // 
            this.spcView.Panel1.Controls.Add(this.spcMainView);
            // 
            // spcView.Panel2
            // 
            this.spcView.Panel2.Controls.Add(this.GridCmd);
            this.spcView.Size = new System.Drawing.Size(1924, 932);
            this.spcView.SplitterDistance = 757;
            this.spcView.SplitterWidth = 6;
            this.spcView.TabIndex = 0;
            // 
            // spcMainView
            // 
            this.spcMainView.BackColor = System.Drawing.SystemColors.Control;
            this.spcMainView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spcMainView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMainView.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.spcMainView.IsSplitterFixed = true;
            this.spcMainView.Location = new System.Drawing.Point(0, 0);
            this.spcMainView.Margin = new System.Windows.Forms.Padding(4);
            this.spcMainView.Name = "spcMainView";
            // 
            // spcMainView.Panel1
            // 
            this.spcMainView.Panel1.BackColor = System.Drawing.SystemColors.Control;
            // 
            // spcMainView.Panel2
            // 
            this.spcMainView.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.spcMainView.Panel2.Controls.Add(this.tableLayoutPanelSideBtn);
            this.spcMainView.Size = new System.Drawing.Size(1924, 757);
            this.spcMainView.SplitterDistance = 1738;
            this.spcMainView.SplitterWidth = 6;
            this.spcMainView.TabIndex = 0;
            // 
            // tableLayoutPanelSideBtn
            // 
            this.tableLayoutPanelSideBtn.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanelSideBtn.ColumnCount = 1;
            this.tableLayoutPanelSideBtn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSideBtn.Controls.Add(this.testbutton, 0, 4);
            this.tableLayoutPanelSideBtn.Controls.Add(this.btnEquMaintain, 0, 2);
            this.tableLayoutPanelSideBtn.Controls.Add(this.btnCmdMaintain, 0, 1);
            this.tableLayoutPanelSideBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSideBtn.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelSideBtn.Name = "tableLayoutPanelSideBtn";
            this.tableLayoutPanelSideBtn.RowCount = 7;
            this.tableLayoutPanelSideBtn.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelSideBtn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.tableLayoutPanelSideBtn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.tableLayoutPanelSideBtn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.tableLayoutPanelSideBtn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.tableLayoutPanelSideBtn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.tableLayoutPanelSideBtn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.tableLayoutPanelSideBtn.Size = new System.Drawing.Size(178, 755);
            this.tableLayoutPanelSideBtn.TabIndex = 0;
            // 
            // testbutton
            // 
            this.testbutton.AutoSize = true;
            this.testbutton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testbutton.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.testbutton.Location = new System.Drawing.Point(4, 381);
            this.testbutton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.testbutton.Name = "testbutton";
            this.testbutton.Size = new System.Drawing.Size(170, 113);
            this.testbutton.TabIndex = 7;
            this.testbutton.Text = "test";
            this.testbutton.UseVisualStyleBackColor = true;
            this.testbutton.Click += new System.EventHandler(this.test_Click);
            // 
            // btnEquMaintain
            // 
            this.btnEquMaintain.AutoSize = true;
            this.btnEquMaintain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEquMaintain.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEquMaintain.Location = new System.Drawing.Point(4, 131);
            this.btnEquMaintain.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnEquMaintain.Name = "btnEquMaintain";
            this.btnEquMaintain.Size = new System.Drawing.Size(170, 113);
            this.btnEquMaintain.TabIndex = 6;
            this.btnEquMaintain.Text = "設備命令查詢";
            this.btnEquMaintain.UseVisualStyleBackColor = true;
            this.btnEquMaintain.Click += new System.EventHandler(this.btnEquMaintain_Click);
            // 
            // btnCmdMaintain
            // 
            this.btnCmdMaintain.AutoSize = true;
            this.btnCmdMaintain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCmdMaintain.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCmdMaintain.Location = new System.Drawing.Point(4, 6);
            this.btnCmdMaintain.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnCmdMaintain.Name = "btnCmdMaintain";
            this.btnCmdMaintain.Size = new System.Drawing.Size(170, 113);
            this.btnCmdMaintain.TabIndex = 4;
            this.btnCmdMaintain.Text = "系統命令查詢";
            this.btnCmdMaintain.UseVisualStyleBackColor = true;
            this.btnCmdMaintain.Click += new System.EventHandler(this.btnCmdMaintain_Click);
            // 
            // GridCmd
            // 
            this.GridCmd.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridCmd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCmd.Location = new System.Drawing.Point(0, 0);
            this.GridCmd.Margin = new System.Windows.Forms.Padding(4);
            this.GridCmd.Name = "GridCmd";
            this.GridCmd.RowHeadersWidth = 62;
            this.GridCmd.RowTemplate.Height = 24;
            this.GridCmd.Size = new System.Drawing.Size(1924, 169);
            this.GridCmd.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(200, 100);
            this.tabControl1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1924, 1050);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "維他露自動倉儲系統";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tlpMainSts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMirle)).EndInit();
            this.tableLayoutPanelConn.ResumeLayout(false);
            this.tableLayoutPanelConn.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.spcView.Panel1.ResumeLayout(false);
            this.spcView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcView)).EndInit();
            this.spcView.ResumeLayout(false);
            this.spcMainView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMainView)).EndInit();
            this.spcMainView.ResumeLayout(false);
            this.tableLayoutPanelSideBtn.ResumeLayout(false);
            this.tableLayoutPanelSideBtn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridCmd)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblDBConn;
        private System.Windows.Forms.SplitContainer spcView;
        private System.Windows.Forms.SplitContainer spcMainView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSideBtn;
        private System.Windows.Forms.TableLayoutPanel tlpMainSts;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelConn;
        private System.Windows.Forms.PictureBox picMirle;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnCmdMaintain;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.DataGridView GridCmd;
        private System.Windows.Forms.Button testbutton;
        private System.Windows.Forms.Label lblPLCConn;
        private System.Windows.Forms.CheckBox chkOnline;
        private System.Windows.Forms.Button btnEquMaintain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBox1;
    }
}

