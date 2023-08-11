
namespace RestorantOto
{
    partial class FrmRezerve
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
            this.btnlist = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnEkle = new System.Windows.Forms.Button();
            this.BtnSil = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.icsil = new System.Windows.Forms.Button();
            this.dsil = new System.Windows.Forms.Button();
            this.disil = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.icalan1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dışMekanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.digerAlanlarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnlist
            // 
            this.btnlist.Location = new System.Drawing.Point(1656, 661);
            this.btnlist.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnlist.Name = "btnlist";
            this.btnlist.Size = new System.Drawing.Size(202, 89);
            this.btnlist.TabIndex = 10;
            this.btnlist.Text = "Listele";
            this.btnlist.UseVisualStyleBackColor = true;
            this.btnlist.Click += new System.EventHandler(this.btnlist_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(925, 13);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(870, 622);
            this.dataGridView1.TabIndex = 7;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // btnEkle
            // 
            this.btnEkle.Location = new System.Drawing.Point(925, 661);
            this.btnEkle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEkle.Name = "btnEkle";
            this.btnEkle.Size = new System.Drawing.Size(202, 89);
            this.btnEkle.TabIndex = 12;
            this.btnEkle.Text = "Masa Ekle";
            this.btnEkle.UseVisualStyleBackColor = true;
            this.btnEkle.Click += new System.EventHandler(this.btnEkle_Click);
            // 
            // BtnSil
            // 
            this.BtnSil.Location = new System.Drawing.Point(925, 793);
            this.BtnSil.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnSil.Name = "BtnSil";
            this.BtnSil.Size = new System.Drawing.Size(202, 89);
            this.BtnSil.TabIndex = 13;
            this.BtnSil.Text = "Masa Sil";
            this.BtnSil.UseVisualStyleBackColor = true;
            this.BtnSil.Click += new System.EventHandler(this.BtnSil_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1198, 661);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 89);
            this.button1.TabIndex = 14;
            this.button1.Text = "İç Alan Ekle";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1328, 661);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(120, 89);
            this.button3.TabIndex = 16;
            this.button3.Text = "Dış Alan Ekle";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1463, 661);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(120, 89);
            this.button4.TabIndex = 17;
            this.button4.Text = "Diğer Alan Ekle";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // icsil
            // 
            this.icsil.Location = new System.Drawing.Point(1198, 793);
            this.icsil.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.icsil.Name = "icsil";
            this.icsil.Size = new System.Drawing.Size(120, 89);
            this.icsil.TabIndex = 18;
            this.icsil.Text = "İç Alan Sil";
            this.icsil.UseVisualStyleBackColor = true;
            this.icsil.Click += new System.EventHandler(this.icsil_Click);
            // 
            // dsil
            // 
            this.dsil.Location = new System.Drawing.Point(1328, 793);
            this.dsil.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dsil.Name = "dsil";
            this.dsil.Size = new System.Drawing.Size(120, 89);
            this.dsil.TabIndex = 19;
            this.dsil.Text = "Dış Alan Sil";
            this.dsil.UseVisualStyleBackColor = true;
            this.dsil.Click += new System.EventHandler(this.dsil_Click);
            // 
            // disil
            // 
            this.disil.Location = new System.Drawing.Point(1463, 793);
            this.disil.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.disil.Name = "disil";
            this.disil.Size = new System.Drawing.Size(120, 89);
            this.disil.TabIndex = 20;
            this.disil.Text = "Diğer Alan Sil";
            this.disil.UseVisualStyleBackColor = true;
            this.disil.Click += new System.EventHandler(this.disil_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(882, 992);
            this.panel1.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(725, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.icalan1ToolStripMenuItem,
            this.dışMekanToolStripMenuItem,
            this.digerAlanlarToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(882, 36);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // icalan1ToolStripMenuItem
            // 
            this.icalan1ToolStripMenuItem.Name = "icalan1ToolStripMenuItem";
            this.icalan1ToolStripMenuItem.Size = new System.Drawing.Size(106, 32);
            this.icalan1ToolStripMenuItem.Text = "İç Alanlar";
            // 
            // dışMekanToolStripMenuItem
            // 
            this.dışMekanToolStripMenuItem.Name = "dışMekanToolStripMenuItem";
            this.dışMekanToolStripMenuItem.Size = new System.Drawing.Size(119, 32);
            this.dışMekanToolStripMenuItem.Text = "Dış Alanlar";
            // 
            // digerAlanlarToolStripMenuItem
            // 
            this.digerAlanlarToolStripMenuItem.Name = "digerAlanlarToolStripMenuItem";
            this.digerAlanlarToolStripMenuItem.Size = new System.Drawing.Size(140, 32);
            this.digerAlanlarToolStripMenuItem.Text = "Diğer Alanlar";
            // 
            // FrmRezerve
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1924, 992);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.disil);
            this.Controls.Add(this.dsil);
            this.Controls.Add(this.icsil);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BtnSil);
            this.Controls.Add(this.btnEkle);
            this.Controls.Add(this.btnlist);
            this.Controls.Add(this.dataGridView1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FrmRezerve";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rezerve";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmRezerve_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnlist;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnEkle;
        private System.Windows.Forms.Button BtnSil;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button icsil;
        private System.Windows.Forms.Button dsil;
        private System.Windows.Forms.Button disil;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem icalan1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dışMekanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem digerAlanlarToolStripMenuItem;
        private System.Windows.Forms.Label label1;
    }
}