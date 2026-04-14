using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Overhead
{
    public partial class Overhead : Form
    {
        public Overhead()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            dgvOrder.Rows.Add();
            dgvOrder.Rows[dgvOrder.Rows.Count - 1].Cells[0].Value = dgvOrder.Rows.Count;
        }

        private void dgvOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Summ_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            double sum = 0;
            try
            {
                foreach (DataGridViewRow i in dgvOrder.Rows)
                {
                    sum += Convert.ToDouble(i.Cells[2].Value)
                        * Convert.ToDouble(i.Cells[3].Value);
                }
                Summ.Text = sum.ToString();
            }
            catch { Summ.Text = "Некорректно"; }
        }
    }
}
