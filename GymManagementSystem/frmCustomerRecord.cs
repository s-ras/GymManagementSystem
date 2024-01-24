using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
namespace GymManagementSystem
{
    public partial class frmCustomerRecord : Form
    {
        public Customer found;

        public frmCustomerRecord()
        {
            InitializeComponent();
        }
        public void GetData()
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    var query = from c in db.Customers orderby c.Name select c;
                    var lst = query.ToList();
                    dgw.DataSource = lst;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearchNames_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    var query = from c in db.Customers
                                where c.Name == txtSearchNames.Text
                                orderby c.Name
                                select c;
                    var lst = query.ToList();
                    dgw.DataSource = lst;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Reset()
        {
            txtSearchNames.Text = String.Empty;
            GetData();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void dgw_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (dgw.RowHeadersWidth < Convert.ToInt32((size.Width + 20)))
            {
                dgw.RowHeadersWidth = Convert.ToInt32((size.Width + 20));
            }
            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));

        }

        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {

                var foundRow = dgw.SelectedRows[0];
                int id = int.Parse(foundRow.Cells[0].Value.ToString());
                using (var db = new GMS_DBEntities())
                {
                    var query = from c in db.Customers where c.C_ID == id select c;
                    found = query.FirstOrDefault<Customer>();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmCustomerRecord_Load(object sender, EventArgs e)
        {
            GetData();
        }

    }
}
