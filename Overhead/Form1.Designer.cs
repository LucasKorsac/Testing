namespace Overhead
{
    partial class Overhead
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.Add = new System.Windows.Forms.Button();
            this.Number = new System.Windows.Forms.TextBox();
            this.Summ = new System.Windows.Forms.TextBox();
            this.Number_overhead = new System.Windows.Forms.Label();
            this.Summ_overhead = new System.Windows.Forms.Label();
            this.dgvOrder = new System.Windows.Forms.DataGridView();
            this.Num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProdName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PriceUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NDS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PricewNDS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Summ_overhead);
            this.panel1.Controls.Add(this.Number_overhead);
            this.panel1.Controls.Add(this.Summ);
            this.panel1.Controls.Add(this.Number);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(799, 100);
            this.panel1.TabIndex = 0;
            // 
            // Add
            // 
            this.Add.Location = new System.Drawing.Point(12, 324);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(104, 40);
            this.Add.TabIndex = 1;
            this.Add.Text = "Добавить";
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // Number
            // 
            this.Number.Location = new System.Drawing.Point(77, 31);
            this.Number.Name = "Number";
            this.Number.Size = new System.Drawing.Size(100, 22);
            this.Number.TabIndex = 0;
            // 
            // Summ
            // 
            this.Summ.Location = new System.Drawing.Point(366, 31);
            this.Summ.Name = "Summ";
            this.Summ.Size = new System.Drawing.Size(100, 22);
            this.Summ.TabIndex = 1;
            this.Summ.TextChanged += new System.EventHandler(this.Summ_TextChanged);
            // 
            // Number_overhead
            // 
            this.Number_overhead.AutoSize = true;
            this.Number_overhead.Location = new System.Drawing.Point(21, 34);
            this.Number_overhead.Name = "Number_overhead";
            this.Number_overhead.Size = new System.Drawing.Size(50, 16);
            this.Number_overhead.TabIndex = 2;
            this.Number_overhead.Text = "Номер";
            // 
            // Summ_overhead
            // 
            this.Summ_overhead.AutoSize = true;
            this.Summ_overhead.Location = new System.Drawing.Point(310, 31);
            this.Summ_overhead.Name = "Summ_overhead";
            this.Summ_overhead.Size = new System.Drawing.Size(50, 16);
            this.Summ_overhead.TabIndex = 3;
            this.Summ_overhead.Text = "Сумма";
            // 
            // dgvOrder
            // 
            this.dgvOrder.AllowUserToAddRows = false;
            this.dgvOrder.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgvOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Num,
            this.ProdName,
            this.Quan,
            this.PriceUnit,
            this.Price,
            this.NDS,
            this.PricewNDS});
            this.dgvOrder.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvOrder.Location = new System.Drawing.Point(0, 100);
            this.dgvOrder.Name = "dgvOrder";
            this.dgvOrder.RowHeadersWidth = 51;
            this.dgvOrder.RowTemplate.Height = 24;
            this.dgvOrder.Size = new System.Drawing.Size(799, 145);
            this.dgvOrder.TabIndex = 2;
            this.dgvOrder.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOrder_CellContentClick);
            // 
            // Num
            // 
            this.Num.HeaderText = "Номер";
            this.Num.MinimumWidth = 6;
            this.Num.Name = "Num";
            this.Num.ReadOnly = true;
            this.Num.Width = 79;
            // 
            // ProdName
            // 
            this.ProdName.HeaderText = "Наименование товара";
            this.ProdName.MinimumWidth = 6;
            this.ProdName.Name = "ProdName";
            this.ProdName.Width = 169;
            // 
            // Quan
            // 
            this.Quan.HeaderText = "Количество";
            this.Quan.MinimumWidth = 6;
            this.Quan.Name = "Quan";
            this.Quan.Width = 114;
            // 
            // PriceUnit
            // 
            this.PriceUnit.HeaderText = "Цена за единицу";
            this.PriceUnit.MinimumWidth = 6;
            this.PriceUnit.Name = "PriceUnit";
            this.PriceUnit.Width = 135;
            // 
            // Price
            // 
            this.Price.HeaderText = "Цена";
            this.Price.MinimumWidth = 6;
            this.Price.Name = "Price";
            this.Price.Width = 69;
            // 
            // NDS
            // 
            this.NDS.HeaderText = "Ставка НДС";
            this.NDS.MinimumWidth = 6;
            this.NDS.Name = "NDS";
            this.NDS.Width = 105;
            // 
            // PricewNDS
            // 
            this.PricewNDS.HeaderText = "Цена с НДС";
            this.PricewNDS.MinimumWidth = 6;
            this.PricewNDS.Name = "PricewNDS";
            this.PricewNDS.Width = 76;
            // 
            // Overhead
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 382);
            this.Controls.Add(this.dgvOrder);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.panel1);
            this.Name = "Overhead";
            this.Text = "Накладная";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Summ_overhead;
        private System.Windows.Forms.Label Number_overhead;
        private System.Windows.Forms.TextBox Summ;
        private System.Windows.Forms.TextBox Number;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.DataGridView dgvOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn Num;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProdName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quan;
        private System.Windows.Forms.DataGridViewTextBoxColumn PriceUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn NDS;
        private System.Windows.Forms.DataGridViewTextBoxColumn PricewNDS;
    }
}

