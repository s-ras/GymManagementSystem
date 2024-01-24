using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GymManagementSystem
{
    public partial class frmSubscriptions : Form
    {
        Logger cf = new Logger();

        private int id = 0;

        public frmSubscriptions()
        {
            InitializeComponent();
        }

        private void frmCategory_Load(object sender, EventArgs e)
        {
            Autocomplete();
            GetData();
        }

        public void GetID()
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    if (db.Subscriptions.Any())
                    {
                        id = db.Subscriptions.DefaultIfEmpty().Max(s => s.M_ID); ;
                    }
                    id++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void delete_records()
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    var query = from s in db.Subscriptions where s.M_ID == id select s;
                    Subscription foundSub = query.FirstOrDefault<Subscription>();

                    if (foundSub != null)
                    {
                        db.Subscriptions.Remove(foundSub);
                        db.SaveChanges();
                        cf.AddLog("", System.DateTime.Now, "deleted the membership type '" + txtMembershipType.Text + "'");

                        MessageBox.Show("Successfully deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No Record found", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    Reset();
                    Autocomplete();
                    GetData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Autocomplete()
        {
            try
            {

                using (var db = new GMS_DBEntities())
                {
                    var query = from s in db.Subscriptions select s;
                    var lst = query.ToList();
                    AutoCompleteStringCollection col = new AutoCompleteStringCollection();
                    for (int i = 0; i < lst.Count; i++)
                    {
                        col.Add(lst[i].Type);
                    }
                    txtMembershipType.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    txtMembershipType.AutoCompleteCustomSource = col;
                    txtMembershipType.AutoCompleteMode = AutoCompleteMode.Suggest;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Reset()
        {
            txtMembershipType.Text = String.Empty;
            id = 0;
            txtChargesPerMonth.Text = String.Empty;
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            txtMembershipType.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtMembershipType.Text == "")
            {
                MessageBox.Show("Please enter sub category", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMembershipType.Focus();
                return;
            }
            if (txtChargesPerMonth.Text == "")
            {
                MessageBox.Show("Please enter charges/month", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChargesPerMonth.Focus();
                return;
            }
            try
            {

                GetID();
                using (var db = new GMS_DBEntities())
                {
                    var query = from s in db.Subscriptions where s.Type == txtMembershipType.Text select s;
                    Subscription foundSub = query.FirstOrDefault<Subscription>();

                    if (foundSub != null)
                    {
                        MessageBox.Show("Membership Type Already Exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtMembershipType.Text = String.Empty;
                        txtMembershipType.Focus();
                        return;
                    }
                    else
                    {
                        Subscription newSub = new Subscription();
                        newSub.Type = txtMembershipType.Text;
                        newSub.ChargesPerMonth = decimal.Parse(txtChargesPerMonth.Text);
                        newSub.M_ID = id;
                        db.Subscriptions.Add(newSub);
                        db.SaveChanges();
                        cf.AddLog("", System.DateTime.Now, "added the new membership type '" + txtMembershipType.Text + "'");
                        Autocomplete();
                        GetData();
                        btnSave.Enabled = false;
                        MessageBox.Show("Successfully saved", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                delete_records();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMembershipType.Text == "")
                {
                    MessageBox.Show("Please enter sub category", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMembershipType.Focus();
                    return;
                }
                if (txtChargesPerMonth.Text == "")
                {
                    MessageBox.Show("Please enter charges/month", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtChargesPerMonth.Focus();
                    return;
                }

                using (var db = new GMS_DBEntities())
                {
                    var query = from s in db.Subscriptions where s.M_ID == id select s;
                    Subscription foundSub = query.FirstOrDefault<Subscription>();

                    if (foundSub != null)
                    {
                        foundSub.Type = txtMembershipType.Text;
                        foundSub.ChargesPerMonth = decimal.Parse(txtChargesPerMonth.Text);
                        db.SaveChanges();
                        cf.AddLog("", System.DateTime.Now, "updated the membership type '" + txtMembershipType.Text + "' details");
                        Autocomplete();
                        GetData();
                        btnUpdate.Enabled = false;
                        MessageBox.Show("Successfully updated", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            DataGridViewRow dr = dgw.SelectedRows[0];
            id = int.Parse(dr.Cells[0].Value.ToString());
            txtMembershipType.Text = dr.Cells[1].Value.ToString().Trim();
            txtChargesPerMonth.Text = dr.Cells[2].Value.ToString();
            btnDelete.Enabled = true;
            btnUpdate.Enabled = true;
            txtMembershipType.Focus();
            btnSave.Enabled = false;
        }

        public void GetData()
        {
            try
            {

                using (var db = new GMS_DBEntities())
                {
                    var query = (from s in db.Subscriptions select new { s.M_ID, s.Type, s.ChargesPerMonth });
                    var lst = query.ToList();
                    dgw.DataSource = lst;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtChargesPerMonth_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allows 0-9, backspace, and decimal
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }
        }
    }
}
