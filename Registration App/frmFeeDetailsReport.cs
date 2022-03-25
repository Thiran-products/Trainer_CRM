using DGVPrinterHelper;
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
    public partial class frmFeeDetailsReport : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmFeeDetailsReport()
        {
            InitializeComponent();
        }

        #region reload form
        private void reloadForm()
        {
            cmbCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            getCourseDetails();
            getRegistrationDetails();
            getFeesDetails();
        }
        #endregion

        #region Load course drop down
        private void getCourseDetails()
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select 0 as mstCourseId,'Select Course' as courseName" +
                "\n union all \n" +
                "select mstCourseId,courseName from mstCourse where mstCourseId = case when @mstCourseId = 0 then mstCourseId else @mstCourseId end and isActive = 1; ";
            cmd.Parameters.AddWithValue("@mstCourseId", 0);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();
            if (dtRegistration.Rows.Count > 0)
            {
                cmbCourse.DataSource = dtRegistration;
                cmbCourse.DisplayMember = "courseName";
                cmbCourse.ValueMember = "mstCourseId";
                cmbCourse.SelectedIndex = 0;
            }
        }
        #endregion

        #region Load user drop down
        private void getRegistrationDetails()
        {
            DataTable dtStudentDetails = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select 0 as mstTrainingRegistrationId, 'Select Name' as name" +
                "\nunion all\n " +
                "select mstTrainingRegistrationId, name from mstTrainingRegistration where mstTrainingRegistrationId = case when @mstTrainingRegistrationId = 0 then mstTrainingRegistrationId else @mstTrainingRegistrationId end and isActive = 1; ";
            cmd.Parameters.AddWithValue("@mstTrainingRegistrationId", 0);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtStudentDetails);
            cnn.Close();
            if (dtStudentDetails.Rows.Count > 0)
            {
                cmbUserName.DataSource = dtStudentDetails;
                cmbUserName.DisplayMember = "name";
                cmbUserName.ValueMember = "mstTrainingRegistrationId";
                cmbUserName.SelectedIndex = 0;
            }
        }
        #endregion

        #region Form load event
        private void frmFeeDetailsReport_Load(object sender, EventArgs e)
        {
            reloadForm();
        }
        #endregion

        #region Get fees details
        private void getFeesDetails()
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by trnFeesDetailsId) as Sno,* from trnFeesDetails tfd " +
                "inner join mstCourse mc on tfd.courseId = mc.mstCourseId " +
                "inner join mstTrainingRegistration mtr on tfd.userId = mtr.mstTrainingRegistrationId " +
                "where " +
                "tfd.isActive = 1 and (date(tfd.createdDate) >= @fromDate and date(tfd.createdDate) <= @toDate) " +
                "and tfd.userId = case when @userId = 0 then tfd.userId else @userId end and tfd.courseId = case when @courseId = 0 then tfd.courseId else @courseId end;";
            cmd.Parameters.AddWithValue("@userId", cmbUserName.SelectedValue);
            cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
            cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(dtpFromDate.Text).ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(dtpToDate.Text).ToString("yyyy-MM-dd"));
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();

            dgvFeeDetails.DataSource = null;
            dgvFeeDetails.AutoGenerateColumns = false;
            dgvFeeDetails.ColumnCount = 8;
            dgvFeeDetails.Columns[0].HeaderText = "trnFeesDetailsId";
            dgvFeeDetails.Columns[0].DataPropertyName = "trnFeesDetailsId";
            dgvFeeDetails.Columns[0].Visible = false;
            dgvFeeDetails.Columns[1].Width = 60;
            dgvFeeDetails.Columns[1].HeaderText = "S No";
            dgvFeeDetails.Columns[1].DataPropertyName = "Sno";
            dgvFeeDetails.Columns[2].HeaderText = "Name";
            dgvFeeDetails.Columns[2].DataPropertyName = "name";
            dgvFeeDetails.Columns[3].Width = 50;
            dgvFeeDetails.Columns[3].HeaderText = "Course";
            dgvFeeDetails.Columns[3].DataPropertyName = "courseName";
            dgvFeeDetails.Columns[4].HeaderText = "Fees";
            dgvFeeDetails.Columns[4].DataPropertyName = "courseFees";
            dgvFeeDetails.Columns[5].HeaderText = "DOB";
            dgvFeeDetails.Columns[5].DataPropertyName = "DOB";
            dgvFeeDetails.Columns[6].HeaderText = "Payment Type";
            dgvFeeDetails.Columns[6].DataPropertyName = "paymentType";
            dgvFeeDetails.Columns[6].Width = 240;
            dgvFeeDetails.Columns[7].HeaderText = "Mobile#";
            dgvFeeDetails.Columns[7].DataPropertyName = "mobileNo";

            dgvFeeDetails.DataSource = dtRegistration;
        }
        #endregion

        #region Get fees details report
        private void button1_Click(object sender, EventArgs e)
        {
            getFeesDetails();
        }
        #endregion

        private void funPrint()
        {
            DGVPrinter print = new DGVPrinter();
            print.Title = "Fees Details Report";
            print.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            print.PageNumbers = true;
            print.PageNumberInHeader = false;
            print.PorportionalColumns = true;
            print.HeaderCellAlignment = StringAlignment.Near;
            //print.PrintPreviewDataGridView(dgvAttendanceDetails);
            print.PrintDataGridView(dgvFeeDetails);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            funPrint();
        }
    }
}
