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
    public partial class frmRegistrationReport : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmRegistrationReport()
        {
            InitializeComponent();
        }

        #region reload form
        private void reloadForm()
        {
            cmbCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocation.DropDownStyle = ComboBoxStyle.DropDownList;
            getCourseDetails();
            getRegistrationDetails();
            getLocationDetails();
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

        #region Load location drop down
        private void getLocationDetails()
        {
            DataTable dtRegistration = new DataTable();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select 0 as locationId,'Select Location' as locationName" +
                "\n union all \n" +
                "select locationId,locationName from mstLocation where locationId = case when @locationId = 0 then locationId else @locationId end and isActive = 1; ";
            cmd.Parameters.AddWithValue("@locationId", 0);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();
            if (dtRegistration.Rows.Count > 0)
            {
                cmbLocation.DataSource = dtRegistration;
                cmbLocation.DisplayMember = "locationName";
                cmbLocation.ValueMember = "locationId";
                cmbLocation.SelectedIndex = 0;
            }
        }
        #endregion

        #region Get registration details
        private void getRegistrationDetails()
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by a.mstTrainingRegistrationId) as Sno,*,date(a.createdDate) as joinDate,case when b.isCourseCompleted = 1 then 'Completed' else 'On Going' end as courseCompleted,max(b.trnfeesdetailsId)" +
                " from mstTrainingRegistration as a " +
                "inner join trnFeesDetails as b on a.mstTrainingRegistrationId = b.userId " +
                "where a.courseTypeId = case when @courseId = 0 then a.courseTypeId else @courseId end and a.isActive = 1 and b.isActive = 1 " +
                "and (date(a.createdDate) >= @fromDate and date(a.createdDate) <= @toDate) " +
                "and a.locationId = case when @locationId = 0 then a.locationId else @locationId end group by studentId;";
            cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
            cmd.Parameters.AddWithValue("@locationId", cmbLocation.SelectedValue);
            cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(dtpFromDate.Text).ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(dtpToDate.Text).ToString("yyyy-MM-dd"));
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();
            
            dgvRegistrationReport.DataSource = null;
            dgvRegistrationReport.AutoGenerateColumns = false;
            dgvRegistrationReport.ColumnCount = 12;
            dgvRegistrationReport.Columns[0].HeaderText = "mstTrainingRegistrationId";
            dgvRegistrationReport.Columns[0].DataPropertyName = "mstTrainingRegistrationId";
            dgvRegistrationReport.Columns[0].Visible = false;
            dgvRegistrationReport.Columns[1].Width = 60;
            dgvRegistrationReport.Columns[1].HeaderText = "S No";
            dgvRegistrationReport.Columns[1].DataPropertyName = "Sno";
            dgvRegistrationReport.Columns[2].HeaderText = "Name";
            dgvRegistrationReport.Columns[2].DataPropertyName = "name";
            dgvRegistrationReport.Columns[3].Width = 100;
            dgvRegistrationReport.Columns[3].HeaderText = "Age";
            dgvRegistrationReport.Columns[3].DataPropertyName = "age";
            dgvRegistrationReport.Columns[4].HeaderText = "E-Mail";
            dgvRegistrationReport.Columns[4].DataPropertyName = "emailId";
            dgvRegistrationReport.Columns[5].Width = 120;
            dgvRegistrationReport.Columns[5].HeaderText = "Course Name";
            dgvRegistrationReport.Columns[5].DataPropertyName = "courseName";
            dgvRegistrationReport.Columns[6].HeaderText = "Join Date";
            dgvRegistrationReport.Columns[6].DataPropertyName = "joinDate";
            dgvRegistrationReport.Columns[7].HeaderText = "Course Completed";
            dgvRegistrationReport.Columns[7].DataPropertyName = "courseCompleted";
            dgvRegistrationReport.Columns[8].HeaderText = "Date Of Birth";
            dgvRegistrationReport.Columns[8].DataPropertyName = "DOB";
            dgvRegistrationReport.Columns[9].HeaderText = "Address";
            dgvRegistrationReport.Columns[9].DataPropertyName = "postalAddress";
            dgvRegistrationReport.Columns[10].HeaderText = "Mobile#";
            dgvRegistrationReport.Columns[10].DataPropertyName = "mobileNo";
            dgvRegistrationReport.Columns[11].HeaderText = "Location";
            dgvRegistrationReport.Columns[11].DataPropertyName = "locationName";

            dgvRegistrationReport.DataSource = dtRegistration;
        }
        #endregion

        #region Form load
        private void frmRegistrationReport_Load(object sender, EventArgs e)
        {
            reloadForm();
        }
        #endregion

        #region Get report
        private void button1_Click(object sender, EventArgs e)
        {
            getRegistrationDetails();
        }

        #endregion

        private void funPrint()
        {
            DGVPrinter print = new DGVPrinter();
            print.Title = "Students Report";
            print.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            print.PageNumbers = true;
            print.PageNumberInHeader = false;
            print.PorportionalColumns = true;
            print.HeaderCellAlignment = StringAlignment.Near;
            print.PrintPreviewDataGridView(dgvRegistrationReport);
            //print.PrintDataGridView(dgvRegistrationReport);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            funPrint();
        }
    }
}
