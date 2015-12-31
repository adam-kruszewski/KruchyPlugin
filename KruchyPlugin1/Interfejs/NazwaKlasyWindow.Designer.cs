namespace KruchyCompany.KruchyPlugin1.Interfejs
{
    partial class NazwaKlasyWindow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonAnuluj = new System.Windows.Forms.Button();
            this.buttonZatwierdz = new System.Windows.Forms.Button();
            this.tbNazwaKlasy = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.buttonAnuluj);
            this.panel1.Controls.Add(this.buttonZatwierdz);
            this.panel1.Controls.Add(this.tbNazwaKlasy);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 91);
            this.panel1.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(167, 29);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // buttonAnuluj
            // 
            this.buttonAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnuluj.Location = new System.Drawing.Point(377, 56);
            this.buttonAnuluj.Name = "buttonAnuluj";
            this.buttonAnuluj.Size = new System.Drawing.Size(75, 23);
            this.buttonAnuluj.TabIndex = 4;
            this.buttonAnuluj.Text = "Anuluj";
            this.buttonAnuluj.UseVisualStyleBackColor = true;
            this.buttonAnuluj.Click += new System.EventHandler(this.buttonAnuluj_Click);
            // 
            // buttonZatwierdz
            // 
            this.buttonZatwierdz.Location = new System.Drawing.Point(287, 56);
            this.buttonZatwierdz.Name = "buttonZatwierdz";
            this.buttonZatwierdz.Size = new System.Drawing.Size(75, 23);
            this.buttonZatwierdz.TabIndex = 3;
            this.buttonZatwierdz.Text = "Ok";
            this.buttonZatwierdz.UseVisualStyleBackColor = true;
            this.buttonZatwierdz.Click += new System.EventHandler(this.buttonZatwierdz_Click);
            // 
            // tbNazwaKlasy
            // 
            this.tbNazwaKlasy.Location = new System.Drawing.Point(167, 3);
            this.tbNazwaKlasy.Name = "tbNazwaKlasy";
            this.tbNazwaKlasy.Size = new System.Drawing.Size(285, 20);
            this.tbNazwaKlasy.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nazwa klasy";
            // 
            // NazwaKlasyWindow
            // 
            this.AcceptButton = this.buttonZatwierdz;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonAnuluj;
            this.ClientSize = new System.Drawing.Size(455, 91);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Name = "NazwaKlasyWindow";
            this.Text = "NazwaKlasyWindow";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbNazwaKlasy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAnuluj;
        private System.Windows.Forms.Button buttonZatwierdz;
        private System.Windows.Forms.CheckBox checkBox1;

    }
}