using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registration_App
{
    public partial class frmRegistration : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        string sStudentId = string.Empty;

        #region constructor
        public frmRegistration()
        {
            InitializeComponent();
            sStudentId = "TE" + DateTime.Now.Year.ToString() + "MT";
        }
        #endregion

        #region Form load
        private void frmRegistration_Load(object sender, EventArgs e)
        {
            reload();
        }
        #endregion

        #region Reload form
        private void reload()
        {
            lblRegistrationId.Text = "0";
            cbCourseType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocation.DropDownStyle = ComboBoxStyle.DropDownList;
            btnRegister.Enabled = true;
            button1.Enabled = false;
            button2.Enabled = false;

            getRegistrationDetails(0);
            getCourseDetails();
            getLocationDetails();
            sStudentId = "TE" + DateTime.Now.Year.ToString() + "MT";
        }
        #endregion

        #region Validate email id
        public bool ValidateEmailId(string emailId)
        {
            /*Regular Expressions for email id*/
            Regex rEMail = new Regex(@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            if (!rEMail.IsMatch(emailId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Add registration
        private void btnRegister_Click(object sender, EventArgs e)
        {
            //Regex re = new Regex("^[0-9]{9}");
            //if (re.IsMatch(txtMobileNo.Text.Trim()) == false || txtMobileNo.Text.Trim().Length < 10)
            //{
            //    MessageBox.Show("Please enter valid mobile number");
            //    txtMobileNo.Focus();
            //}
            //else if (re.IsMatch(txtWPNo.Text.Trim()) == false || txtMobileNo.Text.Trim().Length < 10)
            //{
            //    MessageBox.Show("Please enter valid what's app number");
            //    txtWPNo.Focus();
            //}
            //else if (ValidateEmailId(txtEmailId.Text.Trim()) == false)
            //{
            //    MessageBox.Show("Please enter valid email address");
            //    txtEmailId.Focus();
            //}
            //else
            //{
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by mstTrainingRegistrationId) as Sno,* from mstTrainingRegistration order by mstTrainingRegistrationId desc;";
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();

            if (dtRegistration.Rows.Count > 0)
            {
                if ((Convert.ToInt32(dtRegistration.Rows[0]["mstTrainingRegistrationId"]) + 1).ToString().Length == 1)
                {
                    sStudentId += "00" + (Convert.ToInt32(dtRegistration.Rows[0]["mstTrainingRegistrationId"]) + 1).ToString();
                }
                else if ((Convert.ToInt32(dtRegistration.Rows[0]["mstTrainingRegistrationId"]) + 1).ToString().Length == 2)
                {
                    sStudentId += "0" + (Convert.ToInt32(dtRegistration.Rows[0]["mstTrainingRegistrationId"]) + 1).ToString();
                }
                else
                {
                    sStudentId += (Convert.ToInt32(dtRegistration.Rows[0]["mstTrainingRegistrationId"]) + 1).ToString();
                }
            }
            else
            {
                sStudentId += "001";
            }

            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "insert into mstTrainingRegistration(name,courseTypeId,courseName,DOB,age,postalAddress,mobileNo,whatsppN0,emailId,createdDate,locationId,locationName,studentId)"
                + "values(@name, @courseTypeId, @courseName, @DOB, @age, @postalAddress, @mobileNo, @whatsppN0, @emailId, datetime('now', 'localtime'),@locationId,@locationName,@studentId); ";
            cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@courseTypeId", cbCourseType.SelectedValue);
            cmd.Parameters.AddWithValue("@courseName", cbCourseType.Text);
            cmd.Parameters.AddWithValue("@DOB", dtpDOB.Value.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@age", txtAge.Text.Trim());
            cmd.Parameters.AddWithValue("@postalAddress", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@mobileNo", txtMobileNo.Text.Trim());
            cmd.Parameters.AddWithValue("@whatsppN0", txtWPNo.Text.Trim());
            cmd.Parameters.AddWithValue("@emailId", txtEmailId.Text.Trim());
            cmd.Parameters.AddWithValue("@locationId", cmbLocation.SelectedValue);
            cmd.Parameters.AddWithValue("@locationName", cmbLocation.Text);
            cmd.Parameters.AddWithValue("@studentId", sStudentId);
            int iResult = cmd.ExecuteNonQuery();
            if (iResult > 0)
            {
                MessageBox.Show("Registration successfully!!!");
            }
            cnn.Close();
            this.Controls.Clear();
            this.InitializeComponent();
            reload();
            //}
        }
        #endregion

        #region Get registration details
        private void getRegistrationDetails(int mstRegistrationId)
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by a.mstTrainingRegistrationId) as Sno,*,date(a.createdDate) as joinDate" +
                " from mstTrainingRegistration as a " +
                "where a.mstTrainingRegistrationId = case when @mstTrainingRegistrationId = 0 then a.mstTrainingRegistrationId else @mstTrainingRegistrationId end and a.isActive = 1" +
                " and(strftime('%m', date(a.createdDate)) = '" + DateTime.Now.ToString("MM") + "' and strftime('%Y', date(a.createdDate)) = '" + DateTime.Now.ToString("yyyy") + "'); ";
            //cmd.CommandText = "select row_number() over(order by a.mstTrainingRegistrationId) as Sno,*,date(a.createdDate) as joinDate,case when b.isCourseCompleted = 1 then 'Completed' else 'On Going' end as courseCompleted,max(b.trnfeesdetailsId)" +
            //    " from mstTrainingRegistration as a " +
            //    "inner join trnFeesDetails as b on a.mstTrainingRegistrationId = b.userId " +
            //    "where a.mstTrainingRegistrationId = case when @mstTrainingRegistrationId = 0 then a.mstTrainingRegistrationId else @mstTrainingRegistrationId end and a.isActive = 1 and b.isActive = 1" +
            //    " and(strftime('%m', date(a.createdDate)) = '" + DateTime.Now.ToString("MM") + "' and strftime('%Y', date(a.createdDate)) = '" + DateTime.Now.ToString("yyyy") + "'); ";
            cmd.Parameters.AddWithValue("@mstTrainingRegistrationId", mstRegistrationId);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);

            cnn.Close();
            dgvRegistrationReport.DataSource = null;
            dgvRegistrationReport.AutoGenerateColumns = false;
            dgvRegistrationReport.ColumnCount = 8;
            dgvRegistrationReport.Columns[0].HeaderText = "mstTrainingRegistrationId";
            dgvRegistrationReport.Columns[0].DataPropertyName = "mstTrainingRegistrationId";
            dgvRegistrationReport.Columns[0].Visible = false;
            dgvRegistrationReport.Columns[1].Width = 50;
            dgvRegistrationReport.Columns[1].HeaderText = "Sno";
            dgvRegistrationReport.Columns[1].DataPropertyName = "Sno";
            dgvRegistrationReport.Columns[2].HeaderText = "Roll No";
            dgvRegistrationReport.Columns[2].DataPropertyName = "studentId";
            dgvRegistrationReport.Columns[3].Width = 50;
            dgvRegistrationReport.Columns[3].HeaderText = "Name";
            dgvRegistrationReport.Columns[3].DataPropertyName = "name";
            dgvRegistrationReport.Columns[4].HeaderText = "Course";
            dgvRegistrationReport.Columns[4].DataPropertyName = "courseName";
            dgvRegistrationReport.Columns[5].HeaderText = "Mobile No";
            dgvRegistrationReport.Columns[5].DataPropertyName = "mobileNo";
            dgvRegistrationReport.Columns[6].HeaderText = "Location";
            dgvRegistrationReport.Columns[6].DataPropertyName = "locationName";
            dgvRegistrationReport.Columns[7].HeaderText = "Date Of Join";
            dgvRegistrationReport.Columns[7].DataPropertyName = "joinDate";
            //dgvRegistrationReport.Columns[8].HeaderText = "Status";
            //dgvRegistrationReport.Columns[8].DataPropertyName = "courseCompleted";

            dgvRegistrationReport.DataSource = dtRegistration;
        }
        #endregion

        #region Edit registration details
        private void dgvRegistrationReport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                btnRegister.Enabled = false;
                button1.Enabled = true;
                button2.Enabled = true;
                getCourseDetails();
                getLocationDetails();
                int iRegistrationId = Convert.ToInt32(dgvRegistrationReport.Rows[e.RowIndex].Cells[0].Value.ToString());
                DataTable dtRegistration = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select row_number() over(order by mstTrainingRegistrationId) as Sno,* from mstTrainingRegistration where mstTrainingRegistrationId = case when @mstTrainingRegistrationId = 0 then mstTrainingRegistrationId else @mstTrainingRegistrationId end and isActive = 1;";
                cmd.Parameters.AddWithValue("@mstTrainingRegistrationId", iRegistrationId);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtRegistration);
                cnn.Close();
                lblRegistrationId.Text = iRegistrationId.ToString();
                cbCourseType.SelectedValue = Convert.ToInt32(dtRegistration.Rows[0]["courseTypeId"]);
                txtName.Text = dtRegistration.Rows[0]["name"].ToString();
                dtpDOB.Text = dtRegistration.Rows[0]["DOB"].ToString();
                txtAge.Text = dtRegistration.Rows[0]["age"].ToString();
                txtAddress.Text = dtRegistration.Rows[0]["postalAddress"].ToString();
                txtMobileNo.Text = dtRegistration.Rows[0]["mobileNo"].ToString();
                txtWPNo.Text = dtRegistration.Rows[0]["whatsppN0"].ToString();
                txtEmailId.Text = dtRegistration.Rows[0]["emailId"].ToString();
                cmbLocation.SelectedValue = dtRegistration.Rows[0]["locationId"] == null ? 0 : Convert.ToInt32(dtRegistration.Rows[0]["locationId"]);
            }
        }
        #endregion

        #region Update registration details
        private void button1_Click(object sender, EventArgs e)
        {
            if (lblRegistrationId.Text == "0")
            {
                MessageBox.Show("Please select row to update");
            }
            else
            {
                //Regex re = new Regex("^[0-9]{9}");
                //if (re.IsMatch(txtMobileNo.Text.Trim()) == false)
                //{
                //    MessageBox.Show("Please enter valid mobile number");
                //    txtMobileNo.Focus();
                //}
                //else if (re.IsMatch(txtWPNo.Text.Trim()) == false)
                //{
                //    MessageBox.Show("Please enter valid what's app number");
                //    txtWPNo.Focus();
                //}
                //else if (ValidateEmailId(txtEmailId.Text.Trim()) == false)
                //{
                //    MessageBox.Show("Please enter valid email address");
                //    txtEmailId.Focus();
                //}
                //else
                //{
                cnn.Open();
                cmd = cnn.CreateCommand();
                if (lblRegistrationId.Text.Trim().ToString().Length == 1)
                {
                    sStudentId += "00" + lblRegistrationId.Text.Trim().ToString();
                }
                else if (lblRegistrationId.Text.Trim().ToString().Length == 2)
                {
                    sStudentId += "0" + lblRegistrationId.Text.Trim().ToString();
                }
                else
                {
                    sStudentId += lblRegistrationId.Text.Trim().ToString();
                }
                cmd.CommandText = "update mstTrainingRegistration set name=@name,courseTypeId=@courseTypeId,courseName=@courseName,DOB=@DOB,age=@age,postalAddress=@postalAddress,mobileNo=@mobileNo,whatsppN0=@whatsppN0," +
                    "emailId = @emailId,locationId = @locationId, locationName = @locationName, studentId = @studentId where mstTrainingRegistrationId = @mstTrainingRegistrationId; ";
                cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@courseTypeId", cbCourseType.SelectedValue);
                cmd.Parameters.AddWithValue("@courseName", cbCourseType.Text);
                cmd.Parameters.AddWithValue("@DOB", dtpDOB.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@age", txtAge.Text.Trim());
                cmd.Parameters.AddWithValue("@postalAddress", txtAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@mobileNo", txtMobileNo.Text.Trim());
                cmd.Parameters.AddWithValue("@whatsppN0", txtWPNo.Text.Trim());
                cmd.Parameters.AddWithValue("@emailId", txtEmailId.Text.Trim());
                cmd.Parameters.AddWithValue("@mstTrainingRegistrationId", lblRegistrationId.Text);
                cmd.Parameters.AddWithValue("@locationId", cmbLocation.SelectedValue);
                cmd.Parameters.AddWithValue("@locationName", cmbLocation.Text);
                cmd.Parameters.AddWithValue("@studentId", sStudentId);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Registration updated successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
                //}
            }
        }
        #endregion

        #region Delete registration
        private void button2_Click(object sender, EventArgs e)
        {
            if (lblRegistrationId.Text == "0")
            {
                MessageBox.Show("Please select row to delete");
            }
            else
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update mstTrainingRegistration set isActive=0 where mstTrainingRegistrationId = @mstTrainingRegistrationId;";
                cmd.Parameters.AddWithValue("@mstTrainingRegistrationId", lblRegistrationId.Text);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Registration deleted successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }

        }
        #endregion

        #region Load course drop down
        private void getCourseDetails()
        {
            DataTable dtRegistration = new DataTable();
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
                cbCourseType.DataSource = dtRegistration;
                cbCourseType.DisplayMember = "courseName";
                cbCourseType.ValueMember = "mstCourseId";
                cbCourseType.SelectedIndex = 0;
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

        private void clearForm()
        {
            txtName.Text = "";
            lblRegistrationId.Text = "";
            cmbLocation.SelectedValue = 0;
            cbCourseType.SelectedValue = 0;
            dtpDOB.Text = DateTime.Now.Date.ToString();
            txtAge.Text = "";
            txtAddress.Text = "";
            txtMobileNo.Text = "";
            txtWPNo.Text = "";
            txtEmailId.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearForm();
        }
    }
}
