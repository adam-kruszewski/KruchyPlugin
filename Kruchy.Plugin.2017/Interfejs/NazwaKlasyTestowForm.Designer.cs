namespace KruchyCompany.KruchyPlugin1.Interfejs
{
    partial class NazwaKlasyTestowForm
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
            this.buttonAnuluj = new System.Windows.Forms.Button();
            this.buttonGeneruj = new System.Windows.Forms.Button();
            this.tbInterfejsTestowany = new System.Windows.Forms.TextBox();
            this.comboRodzajMigracji = new System.Windows.Forms.ComboBox();
            this.tbNazwaKlasyTestowej = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelNazwaKlasy = new System.Windows.Forms.Label();
            this.checkBoxIntegracyjny = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxIntegracyjny);
            this.panel1.Controls.Add(this.buttonAnuluj);
            this.panel1.Controls.Add(this.buttonGeneruj);
            this.panel1.Controls.Add(this.tbInterfejsTestowany);
            this.panel1.Controls.Add(this.comboRodzajMigracji);
            this.panel1.Controls.Add(this.tbNazwaKlasyTestowej);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.labelNazwaKlasy);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(418, 141);
            this.panel1.TabIndex = 0;
            // 
            // buttonAnuluj
            // 
            this.buttonAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAnuluj.Location = new System.Drawing.Point(334, 109);
            this.buttonAnuluj.Name = "buttonAnuluj";
            this.buttonAnuluj.Size = new System.Drawing.Size(75, 23);
            this.buttonAnuluj.TabIndex = 7;
            this.buttonAnuluj.Text = "Anuluj";
            this.buttonAnuluj.UseVisualStyleBackColor = true;
            this.buttonAnuluj.Click += new System.EventHandler(this.buttonAnuluj_Click);
            // 
            // buttonGeneruj
            // 
            this.buttonGeneruj.Location = new System.Drawing.Point(253, 109);
            this.buttonGeneruj.Name = "buttonGeneruj";
            this.buttonGeneruj.Size = new System.Drawing.Size(75, 23);
            this.buttonGeneruj.TabIndex = 6;
            this.buttonGeneruj.Text = "Generuj";
            this.buttonGeneruj.UseVisualStyleBackColor = true;
            this.buttonGeneruj.Click += new System.EventHandler(this.buttonGeneruj_Click);
            // 
            // tbInterfejsTestowany
            // 
            this.tbInterfejsTestowany.Location = new System.Drawing.Point(119, 83);
            this.tbInterfejsTestowany.Name = "tbInterfejsTestowany";
            this.tbInterfejsTestowany.Size = new System.Drawing.Size(290, 20);
            this.tbInterfejsTestowany.TabIndex = 5;
            // 
            // comboRodzajMigracji
            // 
            this.comboRodzajMigracji.FormattingEnabled = true;
            this.comboRodzajMigracji.Location = new System.Drawing.Point(119, 32);
            this.comboRodzajMigracji.Name = "comboRodzajMigracji";
            this.comboRodzajMigracji.Size = new System.Drawing.Size(290, 21);
            this.comboRodzajMigracji.TabIndex = 4;
            // 
            // tbNazwaKlasyTestowej
            // 
            this.tbNazwaKlasyTestowej.Location = new System.Drawing.Point(119, 9);
            this.tbNazwaKlasyTestowej.Name = "tbNazwaKlasyTestowej";
            this.tbNazwaKlasyTestowej.Size = new System.Drawing.Size(290, 20);
            this.tbNazwaKlasyTestowej.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Interfejs testowany";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Rodzaj testów";
            // 
            // labelNazwaKlasy
            // 
            this.labelNazwaKlasy.AutoSize = true;
            this.labelNazwaKlasy.Location = new System.Drawing.Point(3, 9);
            this.labelNazwaKlasy.Name = "labelNazwaKlasy";
            this.labelNazwaKlasy.Size = new System.Drawing.Size(109, 13);
            this.labelNazwaKlasy.TabIndex = 0;
            this.labelNazwaKlasy.Text = "Nazwa klasy testowej";
            // 
            // checkBoxIntegracyjny
            // 
            this.checkBoxIntegracyjny.AutoSize = true;
            this.checkBoxIntegracyjny.Location = new System.Drawing.Point(119, 60);
            this.checkBoxIntegracyjny.Name = "checkBoxIntegracyjny";
            this.checkBoxIntegracyjny.Size = new System.Drawing.Size(83, 17);
            this.checkBoxIntegracyjny.TabIndex = 8;
            this.checkBoxIntegracyjny.Text = "Integracyjny";
            this.checkBoxIntegracyjny.UseVisualStyleBackColor = true;
            // 
            // NazwaKlasyTestowForm
            // 
            this.AcceptButton = this.buttonGeneruj;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonAnuluj;
            this.ClientSize = new System.Drawing.Size(418, 141);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Name = "NazwaKlasyTestowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dodawanie klasy testowej";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NazwaKlasyTestowForm_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NazwaKlasyTestowForm_KeyPress);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelNazwaKlasy;
        private System.Windows.Forms.Button buttonAnuluj;
        private System.Windows.Forms.Button buttonGeneruj;
        private System.Windows.Forms.TextBox tbInterfejsTestowany;
        private System.Windows.Forms.ComboBox comboRodzajMigracji;
        private System.Windows.Forms.TextBox tbNazwaKlasyTestowej;
        private System.Windows.Forms.CheckBox checkBoxIntegracyjny;
    }
}