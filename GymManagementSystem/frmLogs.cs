using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace GymManagementSystem
{
    public partial class frmLogs : Form
    {

        public frmLogs()
        {
            InitializeComponent();
        }

        public void FillCombo()
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    var query = from r in db.Registrations orderby r.UserID select r;
                    var lst = query.ToList();
                    foreach (var item in lst)
                    {
                        cmbUserID.Items.Add(item.UserID);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Reset()
        {
            cmbUserID.SelectedIndex = -1;
            dtpDateFrom.Value = System.DateTime.Today;
            dtpDateTo.Value = System.DateTime.Now;
            GetData();
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    var query = from l in db.Logs
                                where l.Date >= dtpDateFrom.Value
                                where l.Date < dtpDateTo.Value
                                orderby l.Date
                                select
                                    new
                                    {
                                        l.UserID,
                                        l.Date,
                                        l.Operation
                                    }
;
                    var lst = query.ToList();
                    dgw.DataSource = lst;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetData()
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    var query = from l in db.Logs
                                orderby l.Date
                                select
                                    new
                                    {
                                        l.UserID,
                                        l.Date,
                                        l.Operation
                                    }
                                    ;
                    var lst = query.ToList();
                    dgw.DataSource = lst;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DeleteRecord()
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Logs]");
                    db.SaveChanges();

                    Logger.Log("deleted the all logs");
                    MessageBox.Show("Successfully deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                    GetData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbUserID_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (var db = new GMS_DBEntities())
                {
                    var query = from l in db.Logs
                                where l.UserID == cmbUserID.Text
                                orderby l.Date
                                select
                                    new
                                    {
                                        l.UserID,
                                        l.Date,
                                        l.Operation
                                    };

                    var lst = query.ToList();
                    dgw.DataSource = lst;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmLogs_Load(object sender, EventArgs e)
        {
            FillCombo();
            GetData();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
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

        private void btnDeleteAllLogs_Click(object sender, EventArgs e)
        {

            try
            {
                if (MessageBox.Show("Do you really want to delete all logs?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    DeleteRecord();
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

    }
}