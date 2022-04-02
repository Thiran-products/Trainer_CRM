using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registration_App
{
    public partial class frmBranch : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmBranch()
        {
            InitializeComponent();
        }

        #region Reload form
        private void reload()
        {
            lblBranchId.Text = "0";
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            getBranchDetails(0);
        }
        #endregion

        private void frmBranch_Load(object sender, EventArgs e)
        {
            reload();
        }

        #region Save branch
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "INSERT INTO mstBranch(branchName,isActive)VALUES(@branchName,1);";
                cmd.Parameters.AddWithValue("@branchName", txtBranch.Text.Trim());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Branch Added successfully!!!");
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Update branch
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update mstBranch set branchName = @branchName where mstBranchId = @mstBranchId;";
                cmd.Parameters.AddWithValue("@branchName", txtBranch.Text.Trim());
                cmd.Parameters.AddWithValue("@mstBranchId", lblBranchId.Text);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Branch updated successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Delete branch
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update mstBranch set isActive = 0 where mstBranchId = @mstBranchId;";
                cmd.Parameters.AddWithValue("@mstBranchId", lblBranchId.Text);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Branch deleted successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Edit Branch
        private void dgvBranchDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex != -1)
                {
                    btnSave.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    int iCategoryId = Convert.ToInt32(dgvBranchDetails.Rows[e.RowIndex].Cells[0].Value.ToString());
                    DataTable dtRegistration = new DataTable();
                    cnn.Open();
                    cmd = cnn.CreateCommand();
                    cmd.CommandText = "select * from mstBranch where mstBranchId = @mstBranchId and isActive = 1;";
                    cmd.Parameters.AddWithValue("@mstBranchId", iCategoryId);
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    sda.Fill(dtRegistration);
                    lblBranchId.Text = dtRegistration.Rows[0]["mstBranchId"].ToString();
                    txtBranch.Text = dtRegistration.Rows[0]["branchName"].ToString();
                    cnn.Close();
                }
            }
            catch (Exception ex)
            {
                cnn.Close();
                throw ex;
            }
        }
        #endregion

        #region Get branch
        private void getBranchDetails(int mstBranchId)
        {
            try
            {
                DataTable dtRegistration = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select row_number() over(order by mstBranchId) as Sno,* from mstBranch where mstBranchId = case when @mstBranchId = 0 then mstBranchId else @mstBranchId end and isActive = 1;";
                cmd.Parameters.AddWithValue("@mstBranchId", mstBranchId);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtRegistration);
                dgvBranchDetails.DataSource = null;
                dgvBranchDetails.AutoGenerateColumns = false;
                dgvBranchDetails.ColumnCount = 3;
                dgvBranchDetails.Columns[0].HeaderText = "mstBranchId";
                dgvBranchDetails.Columns[0].DataPropertyName = "mstBranchId";
                dgvBranchDetails.Columns[0].Visible = false;
                dgvBranchDetails.Columns[1].Width = 60;
                dgvBranchDetails.Columns[1].HeaderText = "S No";
                dgvBranchDetails.Columns[1].DataPropertyName = "Sno";
                dgvBranchDetails.Columns[2].HeaderText = "Branch";
                dgvBranchDetails.Columns[2].DataPropertyName = "branchName";
                dgvBranchDetails.Columns[2].Width = 248;
                dgvBranchDetails.DataSource = dtRegistration;
                cnn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            lblBranchId.Text = "";
            txtBranch.Text = "";
        }
    }
}
