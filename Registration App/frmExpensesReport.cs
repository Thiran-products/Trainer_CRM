using DGVPrinterHelper;
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
    public partial class frmExpensesReport : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmExpensesReport()
        {
            InitializeComponent();
        }

        #region Form load
        private void frmExpensesReport_Load(object sender, EventArgs e)
        {
            getExpensesDetails(0);
            getCategory();
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

        #region Get expenses details
        private void getExpensesDetails(int categoryId)
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by te.mstCategoryId) as Sno,* from trnExpenses te " +
                "inner join mstCategory mc on te.mstCategoryId = mc.mstCategoryId " +
                "where te.isActive = 1 and te.mstCategoryId = case when @categoryId = 0 then te.mstCategoryId else @categoryId end " +
                "and (date(te.createdDate) >= @fromDate and date(te.createdDate) <= @toDate); ";
            cmd.Parameters.AddWithValue("@categoryId", categoryId);
            cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(dtpFromDate.Text).ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(dtpToDate.Text).ToString("yyyy-MM-dd"));
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();
            dgvExpenses.DataSource = null;

            if (dtRegistration.Rows.Count > 0)
            {
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
            else
            {
                dgvExpenses.DataSource = null;
            }
            
        }
        #endregion

        #region Get report
        private void button1_Click(object sender, EventArgs e)
        {
            getExpensesDetails(Convert.ToInt32(cmbCategory.SelectedValue));
        }
        #endregion


        private void funPrint()
        {
            DGVPrinter print = new DGVPrinter();
            print.Title = "Expenses Report";
            print.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            print.PageNumbers = true;
            print.PageNumberInHeader = false;
            print.PorportionalColumns = true;
            print.HeaderCellAlignment = StringAlignment.Near;
            //print.PrintPreviewDataGridView(dgvExpenses);
            print.PrintDataGridView(dgvExpenses);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            funPrint();
        }
    }
}
