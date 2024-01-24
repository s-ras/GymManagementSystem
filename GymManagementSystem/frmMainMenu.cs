using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Entity;
namespace GymManagementSystem
{
    public partial class frmMainMenu : Form
    {
        private string searchedPhrase = String.Empty;

        public frmMainMenu()
        {
            InitializeComponent();
        }

        private void registrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRegistration frm = new frmRegistration();
            frm.Reset();
            frm.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frm = new frmAbout();
            frm.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = System.DateTime.Now.ToString();
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmLogs frm = new frmLogs();
            frm.Reset();
            frm.ShowDialog();
        }

        public void Refresh_Main_Menu_Grid()
        {
            try
            {

                using (var db = new GMS_DBEntities())
                {

                    if (searchedPhrase.Length > 0)
                    {
                        var query = from m in db.Subscriptions
                                    join cm in db.CustomerMemberships on m.M_ID equals cm.MembershipID
                                    join c in db.Customers on cm.CustomerID equals c.C_ID
                                    where c.Name.Contains(searchedPhrase)
                                    orderby cm.BillDate
                                    select new
                                    {
                                        c.CustomerID,
                                        c.Name,
                                        c.City,
                                        m.Type,
                                        cm.DateFrom,
                                        cm.DateTo
                                    };
                        var lst = query.ToList();
                        dgw.DataSource = lst;
                    }
                    else
                    {
                        var query = from m in db.Subscriptions
                                    join cm in db.CustomerMemberships on m.M_ID equals cm.MembershipID
                                    join c in db.Customers on cm.CustomerID equals c.C_ID
                                    orderby cm.BillDate
                                    select new
                                    {
                                        c.CustomerID,
                                        c.Name,
                                        c.City,
                                        m.Type,
                                        cm.DateFrom,
                                        cm.DateTo
                                    };
                        var lst = query.ToList();
                        dgw.DataSource = lst;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh_Main_Menu_Grid();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            btnRefresh.PerformClick();
        }

        private void backupToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if ((!System.IO.Directory.Exists("C:\\DBBackup")))
                {
                    System.IO.Directory.CreateDirectory("C:\\DBBackup");
                }
                string filePath = "C:\\DBBackup\\GMS_DB " + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".bak";

                using (var db = new GMS_DBEntities())
                {

                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "backup database [" + System.Windows.Forms.Application.StartupPath +
                                                  "\\GMS_DB.mdf] to disk='" + filePath + "'with init,stats=10");
                    db.SaveChanges();
                    Logger.Log("Successfully backup the database");
                    MessageBox.Show("Successfully performed", "Database Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor = Cursors.Default;
        }

        private void restoreToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                var _with1 = openFileDialog1;
                _with1.Filter = ("DB Backup File|*.bak;");
                _with1.FilterIndex = 4;
                openFileDialog1.FileName = "";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    using (var db = new GMS_DBEntities())
                    {
                        Cursor = Cursors.WaitCursor;
                        db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "USE Master ALTER DATABASE [" +
                                                      System.Windows.Forms.Application.StartupPath +
                                                      "\\gms_db.mdf] SET Single_User WITH Rollback Immediate Restore database [" +
                                                      System.Windows.Forms.Application.StartupPath +
                                                      "\\gms_db.mdf] FROM disk='" + openFileDialog1.FileName +
                                                      "' WITH REPLACE ALTER DATABASE [" +
                                                      System.Windows.Forms.Application.StartupPath +
                                                      "\\gms_db.mdf] SET Multi_User");
                        db.SaveChanges();
                        Logger.Log("Successfully restore the database");
                        MessageBox.Show("Successfully performed", "Database Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void customerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmCustomer frm = new frmCustomer();
            frm.Reset();
            frm.Show();
        }

        private void membershipToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmCustomerMembership frm = new frmCustomerMembership();
            frm.Reset();
            frm.Show();
        }

        private void logoutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Logger.Log("Successfully logged out");
            Program.acc = null;
            Program.keepOpen = true;
            Program.isLoggedIn = false;
            this.Close();
        }

        private void frmMainMenu_Load(object sender, EventArgs e)
        {
            lblUser.Text = Program.acc.UserID.Trim();
            lblUserType.Text = Program.acc.UserType.Trim();
            ApplyUserPrivilage();

            Refresh_Main_Menu_Grid();
        }

        private void ApplyUserPrivilage()
        {
            if (Program.acc.UserType == "Admin")
            {
                subscriptionsToolStripMenuItem.Enabled = true;
                usersToolStripMenuItem.Enabled = true;
                databaseToolStripMenuItem.Enabled = true;
            }
            else if (Program.acc.UserType == "Operator")
            {
                subscriptionsToolStripMenuItem.Enabled = false;
                usersToolStripMenuItem.Enabled = false;
                databaseToolStripMenuItem.Enabled = false;
            }
            customerToolStripMenuItem.Enabled = true;
            membershipToolStripMenuItem1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (searchedPhrase.Length > 0 && txtSearch.Text.Length == 0)
            {
                searchedPhrase = txtSearch.Text;
                Refresh_Main_Menu_Grid();
            }
            else
            {
                searchedPhrase = txtSearch.Text;
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Refresh_Main_Menu_Grid();
        }

        private void subscriptionsEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSubscriptions frm = new frmSubscriptions();
            frm.Reset();
            frm.ShowDialog();
        }
    }
}
