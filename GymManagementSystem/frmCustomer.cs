using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
namespace GymManagementSystem
{
    public partial class frmCustomer : Form
    {
        public frmCustomer()
        {
            InitializeComponent();
        }

        private Customer selectedCustomer = null;
        private int customer_id = 0;

        private void LoadCustomerInfo()
        {
            if (selectedCustomer != null)
            {
                txtCustomerID.Text = selectedCustomer.CustomerID;
                txtCustomerName.Text = selectedCustomer.Name;
                txtAddress.Text = selectedCustomer.Address;
                txtCity.Text = selectedCustomer.City;
                txtContactNo.Text = selectedCustomer.ContactNo;
                txtEmailID.Text = selectedCustomer.Email;
                if (selectedCustomer.Photo.Length > 0)
                {
                    using (MemoryStream stream = new MemoryStream(selectedCustomer.Photo))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                        Picture.Image = image;
                    }
                }
                else
                {
                    Picture.Image = Properties.Resources.profile_picture_default;
                }

            }
            else
            {
                txtCustomerID.Text = String.Empty;
                txtCustomerName.Text = String.Empty;
                txtAddress.Text = String.Empty;
                txtCity.Text = String.Empty;
                txtContactNo.Text = String.Empty;
                txtEmailID.Text = String.Empty;
                Picture.Image = Properties.Resources.profile_picture_default;
            }
        }

        public void Reset()
        {
            selectedCustomer = null;
            LoadCustomerInfo();

            GetID();
        }

        private void delete_records()
        {

            try
            {

                using (var db = new GMS_DBEntities())
                {
                    var query = from c in db.Customers where c.C_ID == selectedCustomer.C_ID select c;
                    Customer foundCustomer = query.FirstOrDefault<Customer>();
                    if (foundCustomer != null)
                    {
                        db.Customers.Remove(foundCustomer);
                        db.SaveChanges();
                        MessageBox.Show("Successfully deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No Record found", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    Reset();

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

        public void GetID()
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    if (db.Customers.Any())
                    {
                        customer_id = db.Customers.Max(c => c.C_ID);
                    }
                    customer_id++;
                    txtCustomerID.Text = "M" + customer_id.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                if (txtCustomerName.Text == "")
                {
                    MessageBox.Show("Please enter customer name", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomerName.Focus();
                    return;
                }
                if (txtAddress.Text == "")
                {
                    MessageBox.Show("Please enter address", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAddress.Focus();
                    return;
                }
                if (txtCity.Text == "")
                {
                    MessageBox.Show("Please enter city", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCity.Focus();
                    return;
                }
                if (txtContactNo.Text == "")
                {
                    MessageBox.Show("Please enter contact no.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtContactNo.Focus();
                    return;
                }

                using (var db = new GMS_DBEntities())
                {
                    MemoryStream ms = new MemoryStream();
                    Bitmap bmpImage = new Bitmap(Picture.Image);
                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] data = ms.GetBuffer();

                    Customer newCustomer = new Customer();
                    newCustomer.C_ID = customer_id;
                    newCustomer.Email = txtEmailID.Text;
                    newCustomer.Photo = data;
                    newCustomer.City = txtCity.Text;
                    newCustomer.Name = txtCustomerName.Text;
                    newCustomer.ContactNo = txtContactNo.Text;
                    newCustomer.Address = txtAddress.Text;

                    db.Customers.Add(newCustomer);
                    db.SaveChanges();

                    Logger.Log("added the new customer'" + txtCustomerName.Text + "' having Customer id '" + txtCustomerID.Text + "'");

                    btnSave.Enabled = false;
                    MessageBox.Show("Successfully saved", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCustomerID.Text == "")
                {
                    MessageBox.Show("Please enter user id", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomerID.Focus();
                    return;
                }

                if (txtAddress.Text == "")
                {
                    MessageBox.Show("Please enter password", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAddress.Focus();
                    return;
                }
                if (txtCity.Text == "")
                {
                    MessageBox.Show("Please enter name", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCity.Focus();
                    return;
                }
                if (txtEmailID.Text == "")
                {
                    MessageBox.Show("Please enter contact no.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmailID.Focus();
                    return;
                }

                using (var db = new GMS_DBEntities())
                {
                    var query = from c in db.Customers where c.C_ID == selectedCustomer.C_ID select c;
                    Customer foundCustomer = query.FirstOrDefault<Customer>();
                    if (foundCustomer != null)
                    {
                        MemoryStream ms = new MemoryStream();
                        Bitmap bmpImage = new Bitmap(Picture.Image);
                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] data = ms.GetBuffer();

                        foundCustomer.Email = txtEmailID.Text;
                        foundCustomer.Photo = data;
                        foundCustomer.City = txtCity.Text;
                        foundCustomer.Name = txtCustomerName.Text;
                        foundCustomer.ContactNo = txtContactNo.Text;
                        foundCustomer.Address = txtAddress.Text;

                        db.SaveChanges();

                        Logger.Log("updated the customer'" + txtCustomerName.Text + "' having Customer id '" + txtCustomerID.Text + "'");

                        MessageBox.Show("Successfully updated", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Reset();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            try
            {
                var _with1 = openFileDialog1;

                _with1.Filter = ("Image Files |*.png; *.bmp; *.jpg;*.jpeg; *.gif;");
                _with1.FilterIndex = 4;
                openFileDialog1.FileName = "";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Picture.Image = Image.FromFile(openFileDialog1.FileName);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                delete_records();
            }
        }

        private void BRemove_Click(object sender, EventArgs e)
        {
            Picture.Image = Properties.Resources.profile_picture_default;
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            Reset();

            this.Hide();

            using (var frm = new frmCustomerRecord())
            {
                this.Hide();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    selectedCustomer = frm.found;
                    LoadCustomerInfo();
                }
            }

            this.Show();

        }

    }
}