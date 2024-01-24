using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace GymManagementSystem
{
    public partial class frmCustomerMembership : Form
    {
        private int membership_id = 0;

        private Customer selectedCustomer = null;
        private Subscription selectedSubscription = null;
        private CustomerMembership selectedCustomerMembership = null;

        public frmCustomerMembership()
        {
            InitializeComponent();
        }

        private void LoadCustomerInfo()
        {
            if (selectedCustomer != null)
            {
                txtMemberName.Text = selectedCustomer.Name;
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
                txtMemberName.Text = String.Empty;
                txtAddress.Text = String.Empty;
                txtCity.Text = String.Empty;
                txtContactNo.Text = String.Empty;
                txtEmailID.Text = String.Empty;
                Picture.Image = Properties.Resources.profile_picture_default;
            }
        }

        private void LoadSubscriptionInfo()
        {
            if (selectedSubscription != null)
            {
                txtChargesPerMonth.Text = selectedSubscription.ChargesPerMonth.ToString();
                cmbSubscription.Text = selectedSubscription.Type;
            }
            else
            {
                cmbSubscription.SelectedIndex = -1;
                txtChargesPerMonth.Text = String.Empty;
            }
        }

        private void LoadCustomerMembershipInfo()
        {
            if (selectedCustomerMembership != null)
            {
                txtBalance.Text = selectedCustomerMembership.Balance.ToString();
                txtTotalPaid.Text = selectedCustomerMembership.TotalPaid.ToString();
                txtTotalCharges.Text = selectedCustomerMembership.TotalCharges.ToString();
                txtSubTotal.Text = selectedCustomerMembership.SubTotal.ToString();
                txtMonths.Text = selectedCustomerMembership.Months.ToString();
                txtMembershipID.Text = "M" + selectedCustomerMembership.MembershipID.ToString();
                txtDiscountAmount.Text = selectedCustomerMembership.DiscountAmount.ToString();
                txtDiscountPer.Text = selectedCustomerMembership.DiscountPer.ToString();
                dtpBillDate.Value = selectedCustomerMembership.BillDate;
                dtpDateFrom.Value = selectedCustomerMembership.DateFrom;
                dtpDateTo.Value = selectedCustomerMembership.DateTo;
            }
            else
            {
                txtBalance.Text = String.Empty;
                txtTotalPaid.Text = String.Empty;
                txtTotalCharges.Text = String.Empty;
                txtSubTotal.Text = String.Empty;
                txtMonths.Text = String.Empty;
                txtMembershipID.Text = String.Empty;
                txtDiscountAmount.Text = String.Empty;
                txtDiscountPer.Text = String.Empty;
                dtpBillDate.Value = System.DateTime.Now;
                dtpDateFrom.Value = System.DateTime.Today;
                dtpDateTo.Value = System.DateTime.Today;
            }
        }

        public void Calculate()
        {
            if (selectedSubscription != null)
            {
                try
                {
                    double totalCharge = 0;
                    double discountAmount = 0;
                    double subTotal = 0;
                    double balance = 0;
                    double parsedDiscount = 0;
                    double parsedTotalPaid = 0;
                    int parsedMonths = 0;

                    int.TryParse(txtMonths.Text, out parsedMonths);
                    double.TryParse(txtDiscountPer.Text, out parsedDiscount);
                    double.TryParse(txtTotalPaid.Text, out parsedTotalPaid);

                    totalCharge = Convert.ToDouble(parsedMonths * selectedSubscription.ChargesPerMonth);
                    totalCharge = Math.Round(totalCharge, 2);
                    txtTotalCharges.Text = totalCharge.ToString();

                    discountAmount = Convert.ToDouble((totalCharge * parsedDiscount) / 100);
                    discountAmount = Math.Round(discountAmount, 2);
                    txtDiscountAmount.Text = discountAmount.ToString();

                    subTotal = Convert.ToDouble(totalCharge - discountAmount);
                    subTotal = Math.Round(subTotal, 2);
                    txtSubTotal.Text = subTotal.ToString();

                    balance = Convert.ToDouble(subTotal - parsedTotalPaid);
                    balance = Math.Round(balance, 2);
                    txtBalance.Text = balance.ToString();

                    dtpDateTo.Text = dtpDateFrom.Value.Date.AddMonths(parsedMonths).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void GetID()
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    if (db.CustomerMemberships.Any())
                    {
                        membership_id = db.CustomerMemberships.Max(cs => cs.CM_ID);
                    }
                    membership_id++;
                    txtMembershipID.Text = "M" + membership_id.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void Reset()
        {
            selectedCustomer = null;
            LoadCustomerInfo();

            membership_id = 0;
            GetID();

            selectedSubscription = null;
            LoadSubscriptionInfo();

            selectedCustomerMembership = null;
            LoadCustomerMembershipInfo();

            dtpBillDate.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
        }

        private void txtDiscountPer_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void txtTotalPaid_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void txtMonths_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        public void FillCombo()
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    var query = from s in db.Subscriptions orderby s.Type select s;
                    var lst = query.ToList();
                    foreach (var item in lst)
                    {
                        cmbSubscription.Items.Add(item.Type);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbSubscriptionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                using (var db = new GMS_DBEntities())
                {
                    var query = from s in db.Subscriptions
                                where s.Type == cmbSubscription.Text
                                select s;
                    Subscription foundSub = query.FirstOrDefault<Subscription>();
                    if (foundSub != null)
                    {
                        selectedSubscription = foundSub;
                        LoadSubscriptionInfo();
                        Calculate();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmCustomerMembership_Load(object sender, EventArgs e)
        {
            FillCombo();
        }

        private void txtMonths_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtDiscountPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allows 0-9, backspace, and decimal
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }
        }

        private void txtTotalPaid_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allows 0-9, backspace, and decimal
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnSelectCustomer_Click(object sender, EventArgs e)
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMemberName.Text == "")
                {
                    MessageBox.Show("Please retrieve member info", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMemberName.Focus();
                    return;
                }
                if (txtMonths.Text == "")
                {
                    MessageBox.Show("Please enter months", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMonths.Focus();
                    return;
                }
                if (cmbSubscription.Text == "")
                {
                    MessageBox.Show("Please select membership type", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSubscription.Focus();
                    return;
                }
                if (txtDiscountPer.Text == "")
                {
                    MessageBox.Show("Please enter discount %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscountPer.Focus();
                    return;
                }
                if (txtTotalPaid.Text == "")
                {
                    MessageBox.Show("Please enter total paid", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTotalPaid.Focus();
                    return;
                }
                if (int.Parse(txtDiscountPer.Text) > 100)
                {
                    MessageBox.Show("Discount cannot be more than 100%", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtDiscountPer.Focus();
                    return;
                }

                Calculate();

                if (validatePayments())
                {
                    using (var db = new GMS_DBEntities())
                    {
                        var query = from cs in db.CustomerMemberships
                                    where cs.DateFrom <= dtpDateTo.Value.Date
                                    where cs.DateTo >= dtpDateFrom.Value.Date
                                    where cs.CustomerID == selectedCustomer.C_ID
                                    select cs
                                ;
                        CustomerMembership foundCM = query.FirstOrDefault<CustomerMembership>();
                        if (foundCM != null)
                        {
                            MessageBox.Show("Membership has not expired yet.." + "\n" + "Renewal is not allowed now", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        CustomerMembership newCM = new CustomerMembership();
                        newCM.CM_ID = membership_id;
                        newCM.CustMembershipID = txtMembershipID.Text;
                        newCM.BillDate = dtpBillDate.Value;
                        newCM.CustomerID = selectedCustomer.C_ID;
                        newCM.MembershipID = selectedSubscription.M_ID;
                        newCM.DateFrom = dtpDateFrom.Value;
                        newCM.Months = int.Parse(txtMonths.Text);
                        newCM.DateTo = dtpDateTo.Value;
                        newCM.ChargesPerMonth = decimal.Parse(txtChargesPerMonth.Text);
                        newCM.TotalCharges = decimal.Parse(txtTotalCharges.Text);
                        newCM.DiscountPer = decimal.Parse(txtDiscountPer.Text);
                        newCM.DiscountAmount = decimal.Parse(txtDiscountAmount.Text);
                        newCM.SubTotal = decimal.Parse(txtSubTotal.Text);
                        newCM.TotalPaid = decimal.Parse(txtTotalPaid.Text);
                        newCM.Balance = decimal.Parse(txtBalance.Text);

                        db.CustomerMemberships.Add(newCM);
                        db.SaveChanges();
                    }
                    Logger.Log("added the new membership having membership id '" + txtMembershipID.Text + "' of member '" + txtMemberName.Text + "'");
                    btnSave.Enabled = false;
                    MessageBox.Show("Successfully saved", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
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

        private void delete_records()
        {

            try
            {
                using (var db = new GMS_DBEntities())
                {
                    var query = from cs in db.CustomerMemberships where cs.CM_ID == selectedCustomerMembership.CM_ID select cs;
                    CustomerMembership cmFound = query.FirstOrDefault<CustomerMembership>();

                    if (cmFound != null)
                    {
                        db.CustomerMemberships.Remove(cmFound);
                        db.SaveChanges();

                        Logger.Log("deleted the membership record having membership id '" + txtMembershipID.Text + "' of member '" + txtMemberName.Text + "'");
                        MessageBox.Show("Successfully deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMemberName.Text == "")
                {
                    MessageBox.Show("Please retrieve member info", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMemberName.Focus();
                    return;
                }
                if (txtMonths.Text == "")
                {
                    MessageBox.Show("Please enter months", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMonths.Focus();
                    return;
                }
                if (cmbSubscription.Text == "")
                {
                    MessageBox.Show("Please select membership type", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSubscription.Focus();
                    return;
                }
                if (txtDiscountPer.Text == "")
                {
                    MessageBox.Show("Please enter discount %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscountPer.Focus();
                    return;
                }
                if (txtTotalPaid.Text == "")
                {
                    MessageBox.Show("Please enter total paid", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTotalPaid.Focus();
                    return;
                }
                if (int.Parse(txtDiscountPer.Text) > 100)
                {
                    MessageBox.Show("Discount cannot be more than 100%", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtDiscountPer.Focus();
                    return;
                }

                Calculate();

                if (validatePayments())
                {

                    using (var db = new GMS_DBEntities())
                    {
                        var query = from cs in db.CustomerMemberships where cs.CM_ID == selectedCustomerMembership.CM_ID select cs;
                        CustomerMembership cmFound = query.FirstOrDefault<CustomerMembership>();

                        if (cmFound != null)
                        {

                            cmFound.CustMembershipID = txtMembershipID.Text;
                            cmFound.BillDate = dtpBillDate.Value;
                            cmFound.CustomerID = selectedCustomer.C_ID;
                            cmFound.MembershipID = selectedSubscription.M_ID;
                            cmFound.DateFrom = dtpDateFrom.Value;
                            cmFound.Months = int.Parse(txtMonths.Text);
                            cmFound.DateTo = dtpDateTo.Value;
                            cmFound.ChargesPerMonth = decimal.Parse(txtChargesPerMonth.Text);
                            cmFound.TotalCharges = decimal.Parse(txtTotalCharges.Text);
                            cmFound.DiscountPer = decimal.Parse(txtDiscountPer.Text);
                            cmFound.DiscountAmount = decimal.Parse(txtDiscountAmount.Text);
                            cmFound.SubTotal = decimal.Parse(txtSubTotal.Text);
                            cmFound.TotalPaid = decimal.Parse(txtSubTotal.Text);
                            cmFound.Balance = decimal.Parse(txtBalance.Text);

                            db.SaveChanges();

                            Logger.Log("updated the membership record having membership id '" + txtMembershipID.Text + "' of member '" + txtMemberName.Text + "'");

                            btnUpdate.Enabled = false;
                            MessageBox.Show("Successfully updated", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            Reset();
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            Reset();
            this.Hide();
            using (var frm = new frmCustomerMembershipRecord())
            {
                this.Hide();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    selectedCustomer = frm.foundCustomer;
                    selectedSubscription = frm.foundSubscription;
                    selectedCustomerMembership = frm.foundCustomerMembership;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    LoadCustomerInfo();
                    LoadSubscriptionInfo();
                    LoadCustomerMembershipInfo();
                }
            }
            this.Show();
        }

        private bool validatePayments()
        {
            double parsedSubTotal = 0;
            double parsedTotalPaid = 0;
            double.TryParse(txtSubTotal.Text, out parsedSubTotal);
            double.TryParse(txtTotalPaid.Text, out parsedTotalPaid);
            if (parsedTotalPaid > parsedSubTotal)
            {
                MessageBox.Show("Total paid can not more than sub total", "Input Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtTotalPaid.Text = "";
                txtTotalPaid.Focus();
                return false;

            }

            return true;

        }


    }
}
