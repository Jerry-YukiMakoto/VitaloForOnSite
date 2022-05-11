namespace Mirle.MPLCViewer.View
{
    partial class frmFindMultipleView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonFindAllMotor = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonBit = new System.Windows.Forms.RadioButton();
            this.radioButtonWord = new System.Windows.Forms.RadioButton();
            this.radioButtonDWord = new System.Windows.Forms.RadioButton();
            this.radioButtonWordBlock = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxLength = new System.Windows.Forms.TextBox();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.textBoxEqualsValue = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBoxOption = new System.Windows.Forms.GroupBox();
            this.radioButtonHexadecimal = new System.Windows.Forms.RadioButton();
            this.radioButtonDecimal = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonDifferent = new System.Windows.Forms.RadioButton();
            this.radioButtonNotEquals = new System.Windows.Forms.RadioButton();
            this.radioButtonEquals = new System.Windows.Forms.RadioButton();
            this.richTextBoxAddress = new System.Windows.Forms.RichTextBox();
            this.buttonExportCsv = new System.Windows.Forms.Button();
            this.radioButtonAll = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBoxOption.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonFindAllMotor
            // 
            this.buttonFindAllMotor.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.buttonFindAllMotor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFindAllMotor.Location = new System.Drawing.Point(321, 42);
            this.buttonFindAllMotor.Name = "buttonFindAllMotor";
            this.buttonFindAllMotor.Size = new System.Drawing.Size(92, 40);
            this.buttonFindAllMotor.TabIndex = 0;
            this.buttonFindAllMotor.Text = "Find";
            this.buttonFindAllMotor.UseVisualStyleBackColor = true;
            this.buttonFindAllMotor.Click += new System.EventHandler(this.ButtonFind_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 179);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(399, 401);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Address:";
            // 
            // radioButtonBit
            // 
            this.radioButtonBit.AutoSize = true;
            this.radioButtonBit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonBit.Location = new System.Drawing.Point(12, 18);
            this.radioButtonBit.Name = "radioButtonBit";
            this.radioButtonBit.Size = new System.Drawing.Size(41, 20);
            this.radioButtonBit.TabIndex = 4;
            this.radioButtonBit.Text = "Bit";
            this.radioButtonBit.UseVisualStyleBackColor = true;
            // 
            // radioButtonWord
            // 
            this.radioButtonWord.AutoSize = true;
            this.radioButtonWord.Checked = true;
            this.radioButtonWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonWord.Location = new System.Drawing.Point(59, 18);
            this.radioButtonWord.Name = "radioButtonWord";
            this.radioButtonWord.Size = new System.Drawing.Size(59, 20);
            this.radioButtonWord.TabIndex = 4;
            this.radioButtonWord.TabStop = true;
            this.radioButtonWord.Text = "Word";
            this.radioButtonWord.UseVisualStyleBackColor = true;
            // 
            // radioButtonDWord
            // 
            this.radioButtonDWord.AutoSize = true;
            this.radioButtonDWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonDWord.Location = new System.Drawing.Point(124, 18);
            this.radioButtonDWord.Name = "radioButtonDWord";
            this.radioButtonDWord.Size = new System.Drawing.Size(69, 20);
            this.radioButtonDWord.TabIndex = 4;
            this.radioButtonDWord.Text = "DWord";
            this.radioButtonDWord.UseVisualStyleBackColor = true;
            // 
            // radioButtonWordBlock
            // 
            this.radioButtonWordBlock.AutoSize = true;
            this.radioButtonWordBlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonWordBlock.Location = new System.Drawing.Point(199, 18);
            this.radioButtonWordBlock.Name = "radioButtonWordBlock";
            this.radioButtonWordBlock.Size = new System.Drawing.Size(93, 20);
            this.radioButtonWordBlock.TabIndex = 4;
            this.radioButtonWordBlock.Text = "WordBlock";
            this.radioButtonWordBlock.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(211, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Length:";
            // 
            // textBoxLength
            // 
            this.textBoxLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLength.Location = new System.Drawing.Point(280, 12);
            this.textBoxLength.Name = "textBoxLength";
            this.textBoxLength.Size = new System.Drawing.Size(133, 26);
            this.textBoxLength.TabIndex = 2;
            this.textBoxLength.Text = "7";
            // 
            // timerUI
            // 
            this.timerUI.Enabled = true;
            this.timerUI.Tick += new System.EventHandler(this.TimerUI_Tick);
            // 
            // textBoxEqualsValue
            // 
            this.textBoxEqualsValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxEqualsValue.Location = new System.Drawing.Point(193, 146);
            this.textBoxEqualsValue.Name = "textBoxEqualsValue";
            this.textBoxEqualsValue.Size = new System.Drawing.Size(218, 26);
            this.textBoxEqualsValue.TabIndex = 2;
            this.textBoxEqualsValue.Text = "0";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonWordBlock);
            this.groupBox1.Controls.Add(this.radioButtonWord);
            this.groupBox1.Controls.Add(this.radioButtonBit);
            this.groupBox1.Controls.Add(this.radioButtonDWord);
            this.groupBox1.Location = new System.Drawing.Point(16, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 42);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Type";
            // 
            // groupBoxOption
            // 
            this.groupBoxOption.Controls.Add(this.radioButtonHexadecimal);
            this.groupBoxOption.Controls.Add(this.radioButtonDecimal);
            this.groupBoxOption.Location = new System.Drawing.Point(16, 87);
            this.groupBoxOption.Name = "groupBoxOption";
            this.groupBoxOption.Size = new System.Drawing.Size(299, 42);
            this.groupBoxOption.TabIndex = 6;
            this.groupBoxOption.TabStop = false;
            this.groupBoxOption.Text = "Option";
            // 
            // radioButtonHexadecimal
            // 
            this.radioButtonHexadecimal.AutoSize = true;
            this.radioButtonHexadecimal.Checked = true;
            this.radioButtonHexadecimal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonHexadecimal.Location = new System.Drawing.Point(94, 18);
            this.radioButtonHexadecimal.Name = "radioButtonHexadecimal";
            this.radioButtonHexadecimal.Size = new System.Drawing.Size(106, 20);
            this.radioButtonHexadecimal.TabIndex = 4;
            this.radioButtonHexadecimal.TabStop = true;
            this.radioButtonHexadecimal.Text = "Hexadecimal";
            this.radioButtonHexadecimal.UseVisualStyleBackColor = true;
            // 
            // radioButtonDecimal
            // 
            this.radioButtonDecimal.AutoSize = true;
            this.radioButtonDecimal.Checked = true;
            this.radioButtonDecimal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonDecimal.Location = new System.Drawing.Point(12, 18);
            this.radioButtonDecimal.Name = "radioButtonDecimal";
            this.radioButtonDecimal.Size = new System.Drawing.Size(76, 20);
            this.radioButtonDecimal.TabIndex = 4;
            this.radioButtonDecimal.TabStop = true;
            this.radioButtonDecimal.Text = "Decimal";
            this.radioButtonDecimal.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonAll);
            this.groupBox2.Controls.Add(this.radioButtonDifferent);
            this.groupBox2.Controls.Add(this.radioButtonNotEquals);
            this.groupBox2.Controls.Add(this.radioButtonEquals);
            this.groupBox2.Location = new System.Drawing.Point(16, 134);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(170, 40);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Equals";
            // 
            // radioButtonDifferent
            // 
            this.radioButtonDifferent.AutoSize = true;
            this.radioButtonDifferent.Checked = true;
            this.radioButtonDifferent.Location = new System.Drawing.Point(46, 18);
            this.radioButtonDifferent.Name = "radioButtonDifferent";
            this.radioButtonDifferent.Size = new System.Drawing.Size(40, 16);
            this.radioButtonDifferent.TabIndex = 1;
            this.radioButtonDifferent.TabStop = true;
            this.radioButtonDifferent.Text = "diff";
            this.radioButtonDifferent.UseVisualStyleBackColor = true;
            // 
            // radioButtonNotEquals
            // 
            this.radioButtonNotEquals.AutoSize = true;
            this.radioButtonNotEquals.Location = new System.Drawing.Point(133, 18);
            this.radioButtonNotEquals.Name = "radioButtonNotEquals";
            this.radioButtonNotEquals.Size = new System.Drawing.Size(33, 16);
            this.radioButtonNotEquals.TabIndex = 0;
            this.radioButtonNotEquals.Text = "!=";
            this.radioButtonNotEquals.UseVisualStyleBackColor = true;
            // 
            // radioButtonEquals
            // 
            this.radioButtonEquals.AutoSize = true;
            this.radioButtonEquals.Location = new System.Drawing.Point(91, 18);
            this.radioButtonEquals.Name = "radioButtonEquals";
            this.radioButtonEquals.Size = new System.Drawing.Size(35, 16);
            this.radioButtonEquals.TabIndex = 0;
            this.radioButtonEquals.Text = "==";
            this.radioButtonEquals.UseVisualStyleBackColor = true;
            // 
            // richTextBoxAddress
            // 
            this.richTextBoxAddress.Location = new System.Drawing.Point(86, 8);
            this.richTextBoxAddress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.richTextBoxAddress.Name = "richTextBoxAddress";
            this.richTextBoxAddress.Size = new System.Drawing.Size(100, 25);
            this.richTextBoxAddress.TabIndex = 8;
            this.richTextBoxAddress.Text = "";
            // 
            // buttonExportCsv
            // 
            this.buttonExportCsv.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.buttonExportCsv.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExportCsv.Location = new System.Drawing.Point(321, 89);
            this.buttonExportCsv.Name = "buttonExportCsv";
            this.buttonExportCsv.Size = new System.Drawing.Size(92, 40);
            this.buttonExportCsv.TabIndex = 9;
            this.buttonExportCsv.Text = "ExportCsv";
            this.buttonExportCsv.UseVisualStyleBackColor = true;
            this.buttonExportCsv.Click += new System.EventHandler(this.buttonExportCsv_Click);
            // 
            // radioButtonAll
            // 
            this.radioButtonAll.AutoSize = true;
            this.radioButtonAll.Checked = true;
            this.radioButtonAll.Location = new System.Drawing.Point(6, 18);
            this.radioButtonAll.Name = "radioButtonAll";
            this.radioButtonAll.Size = new System.Drawing.Size(34, 16);
            this.radioButtonAll.TabIndex = 10;
            this.radioButtonAll.TabStop = true;
            this.radioButtonAll.Text = "all";
            this.radioButtonAll.UseVisualStyleBackColor = true;
            // 
            // frmFindMultipleView
            // 
            this.AcceptButton = this.buttonFindAllMotor;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 591);
            this.Controls.Add(this.buttonExportCsv);
            this.Controls.Add(this.richTextBoxAddress);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBoxOption);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxLength);
            this.Controls.Add(this.textBoxEqualsValue);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonFindAllMotor);
            this.Name = "frmFindMultipleView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FindMultipleView";
            this.Load += new System.EventHandler(this.FrmFindView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxOption.ResumeLayout(false);
            this.groupBoxOption.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFindAllMotor;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonBit;
        private System.Windows.Forms.RadioButton radioButtonWord;
        private System.Windows.Forms.RadioButton radioButtonDWord;
        private System.Windows.Forms.RadioButton radioButtonWordBlock;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLength;
        private System.Windows.Forms.Timer timerUI;
        private System.Windows.Forms.TextBox textBoxEqualsValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBoxOption;
        private System.Windows.Forms.RadioButton radioButtonHexadecimal;
        private System.Windows.Forms.RadioButton radioButtonDecimal;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonNotEquals;
        private System.Windows.Forms.RadioButton radioButtonEquals;
        private System.Windows.Forms.RadioButton radioButtonDifferent;
        private System.Windows.Forms.RichTextBox richTextBoxAddress;
        private System.Windows.Forms.Button buttonExportCsv;
        private System.Windows.Forms.RadioButton radioButtonAll;
    }
}
