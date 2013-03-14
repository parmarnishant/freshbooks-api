namespace Freshbooks.Test.Util
{
    partial class SettingsForm
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
            System.Windows.Forms.Button Cancel;
            System.Windows.Forms.Button Save;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.ToolTip toolTip1;
            System.Windows.Forms.TextBox textBox4;
            settings1 = new Freshbooks.Test.Properties.Settings();
            System.Windows.Forms.TextBox textBox3;
            System.Windows.Forms.TextBox textBox2;
            System.Windows.Forms.TextBox textBox1;
            System.Windows.Forms.CheckBox checkBox1 = new System.Windows.Forms.CheckBox();
            Cancel = new System.Windows.Forms.Button();
            Save = new System.Windows.Forms.Button();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            textBox4 = new System.Windows.Forms.TextBox();
            textBox3 = new System.Windows.Forms.TextBox();
            textBox2 = new System.Windows.Forms.TextBox();
            textBox1 = new System.Windows.Forms.TextBox();
            tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Cancel.Location = new System.Drawing.Point(357, 181);
            Cancel.Name = "Cancel";
            Cancel.Size = new System.Drawing.Size(75, 23);
            Cancel.TabIndex = 2;
            Cancel.Text = "&Cancel";
            Cancel.UseVisualStyleBackColor = true;
            // 
            // Save
            // 
            Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            Save.Location = new System.Drawing.Point(438, 181);
            Save.Name = "Save";
            Save.Size = new System.Drawing.Size(75, 23);
            Save.TabIndex = 1;
            Save.Text = "&Save";
            Save.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(textBox4, 1, 3);
            tableLayoutPanel1.Controls.Add(textBox3, 1, 2);
            tableLayoutPanel1.Controls.Add(textBox2, 1, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(textBox1, 1, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(501, 163);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 11);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(102, 13);
            label1.TabIndex = 0;
            label1.Text = "Test Account Name";
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 47);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(87, 13);
            label2.TabIndex = 2;
            label2.Text = "Test User Token";
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(3, 83);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(99, 13);
            label3.TabIndex = 4;
            label3.Text = "Developer Account";
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(3, 119);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(123, 13);
            label4.TabIndex = 6;
            label4.Text = "Developer OAuth Secret";
            // 
            // textBox4
            // 
            textBox4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings1, "OAuthSecret", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            textBox4.Location = new System.Drawing.Point(158, 116);
            textBox4.Name = "textBox4";
            textBox4.Size = new System.Drawing.Size(340, 20);
            textBox4.TabIndex = 7;
            textBox4.Text = settings1.OAuthSecret;
            toolTip1.SetToolTip(textBox4, "For OAuth testing, specify the \"OAuth Secret\" that was assigned to the developer " +
                    "account.");
            // 
            // textBox3
            // 
            textBox3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings1, "ConsumerKey", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            textBox3.Location = new System.Drawing.Point(158, 80);
            textBox3.Name = "textBox3";
            textBox3.Size = new System.Drawing.Size(340, 20);
            textBox3.TabIndex = 5;
            textBox3.Text = settings1.ConsumerKey;
            toolTip1.SetToolTip(textBox3, "For OAuth testing, specify the developer account name that has been granted acces" +
                    "s to use OAuth.");
            // 
            // textBox2
            // 
            textBox2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings1, "UserToken", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            textBox2.Location = new System.Drawing.Point(158, 44);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(340, 20);
            textBox2.TabIndex = 3;
            textBox2.Text = settings1.UserToken;
            toolTip1.SetToolTip(textBox2, "Specify the \'Authentication Token\' from the \"My Account\" => \"Freshbooks API\" page" +
                    " for the administrative user on the test account.");
            // 
            // textBox1
            // 
            textBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings1, "FreshbooksAccountName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            textBox1.Location = new System.Drawing.Point(158, 8);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(340, 20);
            textBox1.TabIndex = 1;
            textBox1.Text = settings1.FreshbooksAccountName;
            toolTip1.SetToolTip(textBox1, "Specify the freshbooks account name you would to use for testing.");
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = global::Freshbooks.Test.Properties.Settings.Default.ShowSettings;
            checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Freshbooks.Test.Properties.Settings.Default, "ShowSettings", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox1.Location = new System.Drawing.Point(18, 185);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(139, 17);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "Show settings at startup";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = Cancel;
            this.ClientSize = new System.Drawing.Size(525, 216);
            this.Controls.Add(checkBox1);
            this.Controls.Add(tableLayoutPanel1);
            this.Controls.Add(Save);
            this.Controls.Add(Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Unit Test Settings";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Freshbooks.Test.Properties.Settings settings1;
    }
}