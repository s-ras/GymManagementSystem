using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace GymManagementSystem
{
    public partial class frmCustomerMembershipRecord : Form
    {
        public Subscription foundSubscription;
        public CustomerMembership foundCustomerMembership;
        public Customer foundCustomer;

        public frmCustomerMembershipRecord()
        {
            InitializeComponent();
        }

        public void GetData()
        {
            try
            {

                using (var db = new GMS_DBEntities())
                {
                    var query = from m in db.Subscriptions
                                join cm in db.CustomerMemberships on m.M_ID equals cm.MembershipID
                                join c in db.Customers on cm.CustomerID equals c.C_ID
                                orderby cm.BillDate
                                select new
                                {
                                    cm.CM_ID,
                                    m.M_ID,
                                    c.C_ID,
                                    c.Name,
                                    m.Type,
                                    cm.DateFrom,
                                    cm.DateTo,
                                    cm.TotalCharges,
                                    cm.DiscountAmount,
                                    cm.TotalPaid,
                                    cm.Balance,
                                    cm.BillDate
                                };
                    dgw.DataSource = query.ToList();
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
                    var query = from m in db.Subscriptions
                                join cm in db.CustomerMemberships on m.M_ID equals cm.MembershipID
                                join c in db.Customers on cm.CustomerID equals c.C_ID
                                where c.Name.Contains(txtSearchNames.Text)
                                orderby cm.BillDate
                                select new
                                {
                                    cm.CM_ID,
                                    m.M_ID,
                                    c.C_ID,
                                    c.Name,
                                    m.Type,
                                    cm.DateFrom,
                                    cm.DateTo,
                                    cm.TotalCharges,
                                    cm.DiscountAmount,
                                    cm.TotalPaid,
                                    cm.Balance,
                                    cm.BillDate
                                };
                    dgw.DataSource = query.ToList();

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

                int cm_id = int.Parse(foundRow.Cells[0].Value.ToString());
                int s_id = int.Parse(foundRow.Cells[1].Value.ToString());
                int c_id = int.Parse(foundRow.Cells[2].Value.ToString());

                using (var db = new GMS_DBEntities())
                {
                    var cm_query = from cm in db.CustomerMemberships where cm.CM_ID == cm_id select cm;
                    foundCustomerMembership = cm_query.FirstOrDefault<CustomerMembership>();

                    var s_query = from s in db.Subscriptions where s.M_ID == s_id select s;
                    foundSubscription = s_query.FirstOrDefault<Subscription>();

                    var c_query = from c in db.Customers where c.C_ID == c_id select c;
                    foundCustomer = c_query.FirstOrDefault<Customer>();

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
