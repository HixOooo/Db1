namespace Db1
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSalesHistory;
        private System.Windows.Forms.Button btnMaterialCalc;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnAddMaterial;
        private System.Windows.Forms.Panel panelToolbar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.btnMaterialCalc = new System.Windows.Forms.Button();
            this.btnSalesHistory = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAddMaterial = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panelToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 50);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(800, 450);
            this.flowLayoutPanel.TabIndex = 0;
            // 
            // panelToolbar
            // 
            this.panelToolbar.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.panelToolbar.Controls.Add(this.btnRefresh);
            this.panelToolbar.Controls.Add(this.btnMaterialCalc);
            this.panelToolbar.Controls.Add(this.btnSalesHistory);
            this.panelToolbar.Controls.Add(this.btnEdit);
            this.panelToolbar.Controls.Add(this.btnAdd);
            this.panelToolbar.Controls.Add(this.btnAddMaterial);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(800, 50);
            this.panelToolbar.TabIndex = 1;
            // 
            // btnMaterialCalc
            // 
            this.btnMaterialCalc.BackColor = System.Drawing.Color.Beige;
            this.btnMaterialCalc.Location = new System.Drawing.Point(360, 10);
            this.btnMaterialCalc.Name = "btnMaterialCalc";
            this.btnMaterialCalc.Size = new System.Drawing.Size(120, 30);
            this.btnMaterialCalc.TabIndex = 0;
            this.btnMaterialCalc.Text = "Расчет материала";
            this.btnMaterialCalc.UseVisualStyleBackColor = false;
            this.btnMaterialCalc.Click += new System.EventHandler(this.BtnMaterialCalc_Click);
            // 
            // btnSalesHistory
            // 
            this.btnSalesHistory.BackColor = System.Drawing.Color.Beige;
            this.btnSalesHistory.Location = new System.Drawing.Point(230, 10);
            this.btnSalesHistory.Name = "btnSalesHistory";
            this.btnSalesHistory.Size = new System.Drawing.Size(120, 30);
            this.btnSalesHistory.TabIndex = 1;
            this.btnSalesHistory.Text = "История продаж";
            this.btnSalesHistory.UseVisualStyleBackColor = false;
            this.btnSalesHistory.Click += new System.EventHandler(this.BtnSalesHistory_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.Beige;
            this.btnEdit.Location = new System.Drawing.Point(120, 10);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(100, 30);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Редактировать";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.Beige;
            this.btnAdd.Location = new System.Drawing.Point(10, 10);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 30);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnAddMaterial
            // 
            this.btnAddMaterial.BackColor = System.Drawing.Color.Beige;
            this.btnAddMaterial.Location = new System.Drawing.Point(486, 10);
            this.btnAddMaterial.Name = "btnAddMaterial";
            this.btnAddMaterial.Size = new System.Drawing.Size(120, 30);
            this.btnAddMaterial.TabIndex = 5;
            this.btnAddMaterial.Text = "Добавить материал";
            this.btnAddMaterial.UseVisualStyleBackColor = false;
            this.btnAddMaterial.Click += new System.EventHandler(this.BtnAddMaterial_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Beige;
            this.btnRefresh.BackgroundImage = global::Db1.Properties.Resources.clock;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(753, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(35, 32);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.flowLayoutPanel);
            this.Controls.Add(this.panelToolbar);
            this.Name = "MainForm";
            this.Text = "Управление партнерами";
            this.panelToolbar.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
