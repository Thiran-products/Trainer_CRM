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
    public partial class frmAttendanceReport : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmAttendanceReport()
        {
            InitializeComponent();
        }

        #region reload form
        private void reloadForm()
        {
            cmbCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            getCourseDetails();
            getRegistrationDetails();
            getAttendanceDetails();
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

        #region Form load
        private void frmAttendanceReport_Load(object sender, EventArgs e)
        {
            reloadForm();
        }
        #endregion

        #region Get attendance details
        private void getAttendanceDetails()
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by ta.trnAttendanceId) as Sno,* from trnAttendance ta " +
                "inner join mstTrainingRegistration mr on ta.userId = mr.mstTrainingRegistrationId " +
                "inner join mstCourse mc on ta.courseId = mc.mstCourseId " +
                "where ta.isActive = 1 and (date(ta.createdDate) >= @fromDate and date(ta.createdDate) <= @toDate) " +
                "and ta.userId = case when @userId = 0 then ta.userId else @userId end and ta.courseId = case when @courseId = 0 then ta.courseId else @courseId end; ";
            cmd.Parameters.AddWithValue("@userId", cmbUserName.SelectedValue);
            cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
            cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(dtpFromDate.Text).ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(dtpToDate.Text).ToString("yyyy-MM-dd"));
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();
            dgvAttendanceDetails.DataSource = null;
            dgvAttendanceDetails.AutoGenerateColumns = false;
            dgvAttendanceDetails.ColumnCount = 7;
            dgvAttendanceDetails.Columns[0].HeaderText = "trnAttendanceId";
            dgvAttendanceDetails.Columns[0].DataPropertyName = "trnAttendanceId";
            dgvAttendanceDetails.Columns[0].Visible = false;
            dgvAttendanceDetails.Columns[1].Width = 60;
            dgvAttendanceDetails.Columns[1].HeaderText = "S No";
            dgvAttendanceDetails.Columns[1].DataPropertyName = "Sno";
            dgvAttendanceDetails.Columns[2].Width = 160;
            dgvAttendanceDetails.Columns[2].HeaderText = "Name";
            dgvAttendanceDetails.Columns[2].DataPropertyName = "name";
            dgvAttendanceDetails.Columns[3].Width = 168;
            dgvAttendanceDetails.Columns[3].HeaderText = "Course";
            dgvAttendanceDetails.Columns[3].DataPropertyName = "courseName";
            dgvAttendanceDetails.Columns[4].Width = 150;
            dgvAttendanceDetails.Columns[4].HeaderText = "Attendance Date";
            dgvAttendanceDetails.Columns[4].DataPropertyName = "attendanceDate";
            dgvAttendanceDetails.Columns[5].HeaderText = "Check In";
            dgvAttendanceDetails.Columns[5].DataPropertyName = "checkInTime";
            dgvAttendanceDetails.Columns[6].HeaderText = "check Out";
            dgvAttendanceDetails.Columns[6].DataPropertyName = "checkOutTime";
            dgvAttendanceDetails.DataSource = dtRegistration;
        }
        #endregion

        #region Get report
        private void button1_Click(object sender, EventArgs e)
        {
            getAttendanceDetails();
        }
        #endregion

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {

        }
        private void funPrint()
        {
            DGVPrinter print = new DGVPrinter();
            print.Title = "Attendance Report";
            print.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            print.PageNumbers = true;
            print.PageNumberInHeader = false;
            print.PorportionalColumns = true;
            print.HeaderCellAlignment = StringAlignment.Near;
            //print.PrintPreviewDataGridView(dgvAttendanceDetails);
            print.PrintDataGridView(dgvAttendanceDetails);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            funPrint();
        }
    }
}
