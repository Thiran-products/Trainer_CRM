using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registration_App
{
    public partial class frmCategory : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source="+dbPath+ "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;

        #region Constructor
        public frmCategory()
        {
            InitializeComponent();
        }
        #endregion

        #region Form load
        private void frmCategory_Load(object sender, EventArgs e)
        {
            reload();
        }
        #endregion

        #region Reload form
        private void reload()
        {
            lblCategoryId.Text = "0";
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;

            getCategoryDetails(0);
        }
        #endregion

        #region Save category
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "INSERT INTO mstCategory(categoryName,createdDate,isActive)VALUES(@categoryName,'" + DateTime.Now + "',1);";
                cmd.Parameters.AddWithValue("@categoryName", txtCategory.Text.Trim());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Category saved successfully!!!");
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

        #region Update category
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update mstCategory set categoryName = @categoryName where mstCategoryId = @mstCategoryId;";
                cmd.Parameters.AddWithValue("@categoryName", txtCategory.Text.Trim());
                cmd.Parameters.AddWithValue("@mstCategoryId", lblCategoryId.Text);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Category updated successfully!!!");
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

        #region Delete category
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update mstCategory set isActive = 0 where mstCategoryId = @mstCategoryId;";
                cmd.Parameters.AddWithValue("@mstCategoryId", lblCategoryId.Text);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Category deleted successfully!!!");
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

        #region Get category
        private void getCategoryDetails(int mstCategoryId)
        {
            try
            {
                DataTable dtRegistration = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select row_number() over(order by mstCategoryId) as Sno,* from mstCategory where mstCategoryId = case when @mstCategoryId = 0 then mstCategoryId else @mstCategoryId end and isActive = 1;";
                cmd.Parameters.AddWithValue("@mstCategoryId", mstCategoryId);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtRegistration);
                dgvCategoryDetails.DataSource = null;
                dgvCategoryDetails.AutoGenerateColumns = false;
                dgvCategoryDetails.ColumnCount = 3;
                dgvCategoryDetails.Columns[0].HeaderText = "mstCategoryId";
                dgvCategoryDetails.Columns[0].DataPropertyName = "mstCategoryId";
                dgvCategoryDetails.Columns[0].Visible = false;
                dgvCategoryDetails.Columns[1].Width = 60;
                dgvCategoryDetails.Columns[1].HeaderText = "S No";
                dgvCategoryDetails.Columns[1].DataPropertyName = "Sno";
                dgvCategoryDetails.Columns[2].HeaderText = "Category";
                dgvCategoryDetails.Columns[2].DataPropertyName = "categoryName";
                dgvCategoryDetails.Columns[2].Width = 248;
                dgvCategoryDetails.DataSource = dtRegistration;
                cnn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Edit category
        private void dgvCategoryDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.RowIndex != -1)
                {
                    btnSave.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    int iCategoryId = Convert.ToInt32(dgvCategoryDetails.Rows[e.RowIndex].Cells[0].Value.ToString());
                    DataTable dtRegistration = new DataTable();
                    cnn.Open();
                    cmd = cnn.CreateCommand();
                    cmd.CommandText = "select row_number() over(order by mstCategoryId) as Sno,* from mstCategory where mstCategoryId = case when @mstCategoryId = 0 then mstCategoryId else @mstCategoryId end and isActive = 1;";
                    cmd.Parameters.AddWithValue("@mstCategoryId", iCategoryId);
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    sda.Fill(dtRegistration);
                    lblCategoryId.Text = dtRegistration.Rows[0]["mstCategoryId"].ToString();
                    txtCategory.Text = dtRegistration.Rows[0]["categoryName"].ToString();
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

        private void button1_Click(object sender, EventArgs e)
        {
            lblCategoryId.Text = "";
            txtCategory.Text = "";
        }
    }
}
