using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Data.Entity.Infrastructure.Design.Executor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
namespace GymManagementSystem
{
    public partial class frmRegistration : Form
    {
        private Registration selectedRegistration = null;

        public frmRegistration()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            selectedRegistration = null;
            LoadRegistrationInfo();
        }

        private void LoadRegistrationInfo()
        {
            if (selectedRegistration != null)
            {
                txtContactNo.Text = selectedRegistration.ContactNo;
                txtEmailID.Text = selectedRegistration.EmailID;
                txtName.Text = selectedRegistration.Name;
                txtPassword.Text = selectedRegistration.Password;
                txtUserID.Text = selectedRegistration.UserID;
                cmbUserType.Text = selectedRegistration.UserType;
                txtUserID.Focus();
                btnSave.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                txtUserID.Enabled = false;
            }
            else
            {
                txtContactNo.Text = String.Empty;
                txtEmailID.Text = String.Empty;
                txtName.Text = String.Empty;
                txtPassword.Text = String.Empty;
                txtUserID.Text = String.Empty;
                cmbUserType.SelectedIndex = -1;
                txtUserID.Focus();
                btnSave.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                txtUserID.Enabled = true;
            }
        }

        private void delete_records()
        {

            try
            {
                if (selectedRegistration.UserID == "admin")
                {
                    MessageBox.Show("Admin Account can not be deleted", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var db = new GMS_DBEntities())
                {
                    var query = from r in db.Registrations where r.UserID == selectedRegistration.UserID select r;
                    Registration foundReg = query.FirstOrDefault<Registration>();
                    if (foundReg != null)
                    {
                        db.Registrations.Remove(foundReg);
                        db.SaveChanges();

                        Logger.Log("deleted the user having user id'" + txtUserID.Text + "'");

                        MessageBox.Show("Successfully deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Reset();
                        GetData();
                    }
                    else
                    {
                        MessageBox.Show("No Record found", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                if (txtUserID.Text == "")
                {
                    MessageBox.Show("Please enter user id", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUserID.Focus();
                    return;
                }
                if (cmbUserType.Text == "")
                {
                    MessageBox.Show("Please select user type", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbUserType.Focus();
                    return;
                }
                if (txtPassword.Text == "")
                {
                    MessageBox.Show("Please enter password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }
                if (txtName.Text == "")
                {
                    MessageBox.Show("Please enter name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }
                if (txtContactNo.Text == "")
                {
                    MessageBox.Show("Please enter contact no.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtContactNo.Focus();
                    return;
                }

                using (var db = new GMS_DBEntities())
                {

                    var query = from r in db.Registrations where r.UserID == txtUserID.Text select r;
                    Registration foundReg = query.FirstOrDefault<Registration>();
                    if (foundReg != null)
                    {
                        MessageBox.Show("User ID Already Exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Reset();
                        return;
                    }

                    Registration newReg = new Registration();
                    newReg.Name = txtName.Text;
                    newReg.ContactNo = txtContactNo.Text;
                    newReg.EmailID = txtEmailID.Text;
                    newReg.Password = txtPassword.Text;
                    newReg.UserType = cmbUserType.Text;
                    newReg.JoiningDate = DateTime.Now;
                    newReg.UserID = txtUserID.Text;

                    db.Registrations.Add(newReg);
                    db.SaveChanges();

                    Logger.Log("added the new user having user id'" + txtUserID.Text + "'");

                    GetData();
                    btnSave.Enabled = false;
                    MessageBox.Show("Successfully Registered", "User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GetData()
        {
            try
            {

                using (var db = new GMS_DBEntities())
                {
                    var query = from r in db.Registrations
                                orderby r.UserID
                                select new { r.UserID, r.UserType, r.Name, r.EmailID, r.ContactNo, r.JoiningDate };
                    var lst = query.ToList();
                    dgw.DataSource = lst;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtEmailID_Validating(object sender, CancelEventArgs e)
        {
            System.Text.RegularExpressions.Regex rEMail = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            if (txtEmailID.Text.Length > 0)
            {
                if (!rEMail.IsMatch(txtEmailID.Text))
                {
                    MessageBox.Show("invalid email address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtEmailID.SelectAll();
                    e.Cancel = true;
                }
            }
        }

        private void txtUserID_Validating(object sender, CancelEventArgs e)
        {
            System.Text.RegularExpressions.Regex rEMail = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_]");
            if (txtUserID.Text.Length > 0)
            {
                if (!rEMail.IsMatch(txtUserID.Text))
                {
                    MessageBox.Show("only letters,numbers and underscore is allowed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUserID.SelectAll();
                    e.Cancel = true;
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUserID.Text == "")
                {
                    MessageBox.Show("Please enter user id", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUserID.Focus();
                    return;
                }
                if (cmbUserType.Text == "")
                {
                    MessageBox.Show("Please select user type", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbUserType.Focus();
                    return;
                }
                if (txtPassword.Text == "")
                {
                    MessageBox.Show("Please enter password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }
                if (txtName.Text == "")
                {
                    MessageBox.Show("Please enter name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }
                if (txtContactNo.Text == "")
                {
                    MessageBox.Show("Please enter contact no.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtContactNo.Focus();
                    return;
                }

                using (var db = new GMS_DBEntities())
                {
                    var query = from r in db.Registrations where r.UserID == selectedRegistration.UserID select r;
                    Registration foundReg = query.FirstOrDefault<Registration>();
                    if (foundReg != null)
                    {
                        foundReg.Name = txtName.Text;
                        foundReg.ContactNo = txtContactNo.Text;
                        foundReg.EmailID = txtEmailID.Text;
                        foundReg.Password = txtPassword.Text;
                        foundReg.UserType = cmbUserType.Text;
                        db.SaveChanges();

                        Logger.Log("updated the user details having user id'" + txtUserID.Text + "'");
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

        private void frmRegistration_Load(object sender, EventArgs e)
        {
            GetData();
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
            string id = dr.Cells[0].Value.ToString();

            using (var db = new GMS_DBEntities())
            {
                var query = from r in db.Registrations where r.UserID == id select r;
                Registration foundReg = query.FirstOrDefault<Registration>();
                if (foundReg != null)
                {
                    selectedRegistration = foundReg;
                    LoadRegistrationInfo();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                delete_records();
            }
        }


    }
}