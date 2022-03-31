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
    public partial class frmAttendance : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        Boolean isFormLoad;

        #region Constructor
        public frmAttendance()
        {
            InitializeComponent();
            connetionString = @"Data Source=172.16.19.200\MSSQL2017;Initial Catalog=Registration;User ID=sa;Password=12345678";
        }
        #endregion

        #region Form load
        private void frmAttendance_Load(object sender, EventArgs e)
        {
            reloadForm();
        }
        #endregion

        #region Load user drop down
        private void getRegistrationDetails()
        {
            DataTable dtStudentDetails = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select 0 as mstTrainingRegistrationId, 'Select Roll No' as name" +
                "\nunion all\n " +
                "select mstTrainingRegistrationId, studentId as name from mstTrainingRegistration where mstTrainingRegistrationId = case when @mstTrainingRegistrationId = 0 then mstTrainingRegistrationId else @mstTrainingRegistrationId end and isActive = 1; ";
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

        #region reload form
        private void reloadForm()
        {
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            lblAttendanceId.Text = "0";
            cmbCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            isFormLoad = true;
            getRegistrationDetails();
            getAttendanceDetails(0);
            isFormLoad = false;
            getCourseDetails();
        }
        #endregion

        #region Save checkin
        private void btnSave_Click(object sender, EventArgs e)
        {
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "insert into trnAttendance(userId,courseId,attendanceDate,checkInTime,checkOutTime,createdDate)values(@userId,@courseId,@attendanceDate,datetime('now', 'localtime'));";
            cmd.Parameters.AddWithValue("@userId", cmbUserName.SelectedValue);
            cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
            cmd.Parameters.AddWithValue("@attendanceDate", DateTime.Now.Date);
            int iResult = cmd.ExecuteNonQuery();
            if (iResult > 0)
            {
                MessageBox.Show("Attendance saved successfully!!!");
            }
            cnn.Close();
            this.Controls.Clear();
            this.InitializeComponent();
            reloadForm();
        }
        #endregion

        #region Get attendance details
        private void getAttendanceDetails(int trnAttendanceDetailsId)
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by ta.trnAttendanceId) as Sno,* from trnAttendance ta " +
                "inner join mstTrainingRegistration mr on ta.userId = mr.mstTrainingRegistrationId " +
                "inner join mstCourse mc on ta.courseId = mc.mstCourseId " +
                "where ta.trnAttendanceId = case when @trnAttendanceId = 0 then ta.trnAttendanceId else @trnAttendanceId end and ta.isActive = 1 " +
                "and (strftime('%m', date(ta.createdDate))='" + DateTime.Now.ToString("MM") + "' and strftime('%Y', date(ta.createdDate))='" + DateTime.Now.ToString("yyyy") + "');";
            cmd.Parameters.AddWithValue("@trnAttendanceId", trnAttendanceDetailsId);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();

            dgvAttendanceDetails.AutoGenerateColumns = false;
            dgvAttendanceDetails.ColumnCount = 5;
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

            dgvAttendanceDetails.DataSource = dtRegistration;
        }
        #endregion

        #region Edit attendance details
        private void dgvAttendanceDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                btnSave.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                getCourseDetails();
                getRegistrationDetails();
                int iAttendanceDetailsId = Convert.ToInt32(dgvAttendanceDetails.Rows[e.RowIndex].Cells[0].Value.ToString());
                DataTable dtRegistration = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select row_number() over(order by ta.trnAttendanceId) as Sno,* from trnAttendance ta " +
                    "inner join mstTrainingRegistration mr on ta.userId = mr.mstTrainingRegistrationId " +
                    "inner join mstCourse mc on ta.courseId = mc.mstCourseId " +
                    "where ta.trnAttendanceId = case when @trnAttendanceId = 0 then ta.trnAttendanceId else @trnAttendanceId end and ta.isActive = 1";
                cmd.Parameters.AddWithValue("@trnAttendanceId", iAttendanceDetailsId);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtRegistration);
                cnn.Close();
                lblAttendanceId.Text = dtRegistration.Rows[0]["trnAttendanceId"].ToString();
                cmbUserName.SelectedValue = Convert.ToInt32(dtRegistration.Rows[0]["userId"]);
                cmbCourse.SelectedValue = Convert.ToInt32(dtRegistration.Rows[0]["courseId"]);
            }
        }
        #endregion

        #region Update attendance details
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "update trnAttendance set userId=@userId,courseId=@courseId,attendanceDate=@attendanceDate " +
                "where trnAttendanceId = @trnAttendanceId";
            cmd.Parameters.AddWithValue("@userId", cmbUserName.SelectedValue);
            cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
            cmd.Parameters.AddWithValue("@attendanceDate", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@trnAttendanceId", lblAttendanceId.Text);
            int iResult = cmd.ExecuteNonQuery();
            if (iResult > 0)
            {
                MessageBox.Show("Attendance updated successfully!!!");
            }
            cnn.Close();
            this.Controls.Clear();
            this.InitializeComponent();
            reloadForm();
        }
        #endregion

        #region Delete attendance details
        private void btnDelete_Click(object sender, EventArgs e)
        {
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "update trnAttendance set isActive = 0 where trnAttendanceId = @trnAttendanceId";
            cmd.Parameters.AddWithValue("@trnAttendanceId", lblAttendanceId.Text);
            int iResult = cmd.ExecuteNonQuery();
            if (iResult > 0)
            {
                MessageBox.Show("Attendance deleted successfully!!!");
            }
            cnn.Close();
            this.Controls.Clear();
            this.InitializeComponent();
            reloadForm();
        }
        #endregion

        private void cmbUserName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbUserName_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isFormLoad == false)
                {
                    DataTable dtRegistration = new DataTable();
                    cnn.Open();
                    cmd = cnn.CreateCommand();
                    cmd.CommandText = "select courseTypeId,name from mstTrainingRegistration where mstTrainingRegistrationId = " + cmbUserName.SelectedValue + "";
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    sda.Fill(dtRegistration);
                    cnn.Close();
                    if (dtRegistration.Rows.Count > 0)
                    {
                        cmbCourse.SelectedValue = Convert.ToInt32(dtRegistration.Rows[0]["courseTypeId"]);
                        label8.Text = dtRegistration.Rows[0]["name"].ToString();
                    }
                    else
                    {
                        cmbCourse.SelectedValue = 0;
                        label8.Text = "";
                    }
                }
                else
                {
                    cmbCourse.SelectedValue = 0;
                    label8.Text = "";
                }
                cmbCourse.Enabled = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblAttendanceId.Text = "";
            cmbCourse.SelectedValue = 0;
            cmbUserName.SelectedValue = 0;
            label8.Text = "";
        }
    }
}
