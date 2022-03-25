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
    public partial class frmExpenses : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;

        #region Constructor
        public frmExpenses()
        {
            InitializeComponent();
        }
        #endregion

        #region Form load
        private void frmExpenses_Load(object sender, EventArgs e)
        {
            reload();
        }
        #endregion

        #region Reload form
        private void reload()
        {
            lblExpensesId.Text = "0";
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            getCategory();
            getExpensesDetails(0);
        }
        #endregion

        #region Load Category
        private void getCategory()
        {
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            DataTable dtStudentDetails = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select 0 as mstCategoryId, 'Select Category' as categoryName" +
                "\n   union all \n" +
                "select mstCategoryId, categoryName from mstCategory where isActive = 1; ";
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtStudentDetails);
            cnn.Close();

            if (dtStudentDetails.Rows.Count > 0)
            {
                cmbCategory.DataSource = dtStudentDetails;
                cmbCategory.DisplayMember = "categoryName";
                cmbCategory.ValueMember = "mstCategoryId";
                cmbCategory.SelectedIndex = 0;
            }
        }
        #endregion

        #region Save expenses
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "insert into trnExpenses(mstCategoryId,description,amount,createdDate)values(@mstCategoryId,@description,@amount,datetime('now', 'localtime'));";
                cmd.Parameters.AddWithValue("@mstCategoryId", cmbCategory.SelectedValue);
                cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@amount", txtAmount.Text.Trim());
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Expenses saved successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
            catch (Exception ex)
            {
                cnn.Close();
                throw ex;
            }
        }
        #endregion

        #region Update expenses
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update trnExpenses set mstCategoryId = @mstCategoryId,description = @description, amount = @amount where trnExpensesId = @trnExpensesId;";
                cmd.Parameters.AddWithValue("@trnExpensesId", lblExpensesId.Text);
                cmd.Parameters.AddWithValue("@mstCategoryId", cmbCategory.SelectedValue);
                cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@amount", txtAmount.Text.Trim());
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Expenses updated successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
            catch (Exception ex)
            {
                cnn.Close();
                throw ex;
            }
        }
        #endregion

        #region Delete expenses
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update trnExpenses set isActive = 0 where trnExpensesId = @trnExpensesId;";
                cmd.Parameters.AddWithValue("@trnExpensesId", lblExpensesId.Text);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Expenses deleted successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
            catch (Exception ex)
            {
                cnn.Close();
                throw ex;
            }
        }
        #endregion

        #region Get expenses details
        private void getExpensesDetails(int trnExpensesId)
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by te.mstCategoryId) as Sno,* from trnExpenses te " +
                "inner join mstCategory mc on te.mstCategoryId = mc.mstCategoryId " +
                "where te.trnExpensesId = case when @trnExpensesId = 0 then te.trnExpensesId else @trnExpensesId end and te.isActive = 1; ";
            cmd.Parameters.AddWithValue("@trnExpensesId", trnExpensesId);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();

            dgvExpenses.AutoGenerateColumns = false;
            dgvExpenses.ColumnCount = 5;
            dgvExpenses.Columns[0].HeaderText = "trnExpensesId";
            dgvExpenses.Columns[0].DataPropertyName = "trnExpensesId";
            dgvExpenses.Columns[0].Visible = false;
            dgvExpenses.Columns[1].Width = 60;
            dgvExpenses.Columns[1].HeaderText = "S No";
            dgvExpenses.Columns[1].DataPropertyName = "Sno";
            dgvExpenses.Columns[2].Width = 250;
            dgvExpenses.Columns[2].HeaderText = "Category";
            dgvExpenses.Columns[2].DataPropertyName = "categoryName";
            dgvExpenses.Columns[3].Width = 250;
            dgvExpenses.Columns[3].HeaderText = "Description";
            dgvExpenses.Columns[3].DataPropertyName = "description";
            dgvExpenses.Columns[4].Width = 150;
            dgvExpenses.Columns[4].HeaderText = "Amount";
            dgvExpenses.Columns[4].DataPropertyName = "amount";
            dgvExpenses.DataSource = dtRegistration;
        }
        #endregion

        #region Edit expenses
        private void dgvExpenses_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.RowIndex != -1)
                {
                    getCategory();
                    btnSave.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    int iExpensesId = Convert.ToInt32(dgvExpenses.Rows[e.RowIndex].Cells[0].Value.ToString());
                    DataTable dtRegistration = new DataTable();
                    cnn.Open();
                    cmd = cnn.CreateCommand();
                    cmd.CommandText = "select row_number() over(order by te.mstCategoryId) as Sno,* from trnExpenses te " +
                        "inner join mstCategory mc on te.mstCategoryId = mc.mstCategoryId " +
                        "where te.trnExpensesId = case when @trnExpensesId = 0 then te.trnExpensesId else @trnExpensesId end and te.isActive = 1; ";
                    cmd.Parameters.AddWithValue("@trnExpensesId", iExpensesId);
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    sda.Fill(dtRegistration);
                    cmbCategory.SelectedValue = dtRegistration.Rows[0]["mstCategoryId"];
                    txtDescription.Text = dtRegistration.Rows[0]["description"].ToString();
                    txtAmount.Text = dtRegistration.Rows[0]["amount"].ToString();
                    lblExpensesId.Text = dtRegistration.Rows[0]["trnExpensesId"].ToString();
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

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
