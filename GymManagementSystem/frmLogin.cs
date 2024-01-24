using System;
using System.Linq;
using System.Windows.Forms;
namespace GymManagementSystem
{
    public partial class frmLogin : Form
    {

        public frmLogin()
        {
            InitializeComponent();
        }



        private void btnOK_Click(object sender, EventArgs e)
        {

            if (UserID.Text == "")
            {
                MessageBox.Show("Please enter user id", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UserID.Focus();
                return;
            }
            if (Password.Text == "")
            {
                MessageBox.Show("Please enter password", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Password.Focus();
                return;
            }
            try
            {

                using (var db = new GMS_DBEntities())
                {
                    var query = from r in db.Registrations
                                where r.UserID == UserID.Text
                                where r.Password == Password.Text
                                select r;

                    Registration foundReg = query.FirstOrDefault<Registration>();

                    if (foundReg != null)
                    {
                        Program.isLoggedIn = true;
                        Program.acc = foundReg;
                        Logger.Log("successfully logged in");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Login Failed...Try again !", "Login Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UserID.Text = String.Empty;
                        Password.Text = String.Empty;
                        UserID.Focus();
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
