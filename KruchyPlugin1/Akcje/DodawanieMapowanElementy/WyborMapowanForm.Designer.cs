namespace KruchyCompany.KruchyPlugin1.Akcje.DodawanieMapowanElementy
{
    partial class WyborMapowanForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAnuluj = new System.Windows.Forms.Button();
            this.buttonGenerujMapowania = new System.Windows.Forms.Button();
            this.buttonOdznaczWszystkie = new System.Windows.Forms.Button();
            this.buttonZaznaczWszystkie = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.treeView1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(459, 348);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(453, 302);
            this.treeView1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonAnuluj);
            this.flowLayoutPanel1.Controls.Add(this.buttonGenerujMapowania);
            this.flowLayoutPanel1.Controls.Add(this.buttonOdznaczWszystkie);
            this.flowLayoutPanel1.Controls.Add(this.buttonZaznaczWszystkie);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 311);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(453, 34);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // buttonAnuluj
            // 
            this.buttonAnuluj.Location = new System.Drawing.Point(375, 3);
            this.buttonAnuluj.Name = "buttonAnuluj";
            this.buttonAnuluj.Size = new System.Drawing.Size(75, 23);
            this.buttonAnuluj.TabIndex = 4;
            this.buttonAnuluj.Text = "&Anuluj";
            this.buttonAnuluj.UseVisualStyleBackColor = true;
            this.buttonAnuluj.Click += new System.EventHandler(this.buttonAnuluj_Click);
            // 
            // buttonGenerujMapowania
            // 
            this.buttonGenerujMapowania.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonGenerujMapowania.Location = new System.Drawing.Point(294, 3);
            this.buttonGenerujMapowania.Name = "buttonGenerujMapowania";
            this.buttonGenerujMapowania.Size = new System.Drawing.Size(75, 23);
            this.buttonGenerujMapowania.TabIndex = 3;
            this.buttonGenerujMapowania.Text = "&Generuj mapowania";
            this.buttonGenerujMapowania.UseVisualStyleBackColor = true;
            this.buttonGenerujMapowania.Click += new System.EventHandler(this.buttonGenerujMapowania_Click);
            // 
            // buttonOdznaczWszystkie
            // 
            this.buttonOdznaczWszystkie.Location = new System.Drawing.Point(175, 3);
            this.buttonOdznaczWszystkie.Name = "buttonOdznaczWszystkie";
            this.buttonOdznaczWszystkie.Size = new System.Drawing.Size(113, 23);
            this.buttonOdznaczWszystkie.TabIndex = 2;
            this.buttonOdznaczWszystkie.Text = "Odznacz wszystkie";
            this.buttonOdznaczWszystkie.UseVisualStyleBackColor = true;
            this.buttonOdznaczWszystkie.Click += new System.EventHandler(this.buttonOdznaczWszystkie_Click);
            // 
            // buttonZaznaczWszystkie
            // 
            this.buttonZaznaczWszystkie.Location = new System.Drawing.Point(47, 3);
            this.buttonZaznaczWszystkie.Name = "buttonZaznaczWszystkie";
            this.buttonZaznaczWszystkie.Size = new System.Drawing.Size(122, 23);
            this.buttonZaznaczWszystkie.TabIndex = 1;
            this.buttonZaznaczWszystkie.Text = "Zaznacz wszystkie";
            this.buttonZaznaczWszystkie.UseVisualStyleBackColor = true;
            this.buttonZaznaczWszystkie.Click += new System.EventHandler(this.buttonZaznaczWszystkie_Click);
            // 
            // WyborMapowanForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 348);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WyborMapowanForm";
            this.Text = "Wybieranie pól do mapowań";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonAnuluj;
        private System.Windows.Forms.Button buttonGenerujMapowania;
        private System.Windows.Forms.Button buttonOdznaczWszystkie;
        private System.Windows.Forms.Button buttonZaznaczWszystkie;
    }
}