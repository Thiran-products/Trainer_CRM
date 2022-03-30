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
    public partial class frmCourseMaster : Form
    {

        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmCourseMaster()
        {
            InitializeComponent();
        }

        private void frmCourseMaster_Load(object sender, EventArgs e)
        {
            reload();
        }

        private void reload()
        {
            lblCourseId.Visible = false;
            lblCourseId.Text = "";
            btnSaveCourse.Enabled = true;
            button1.Enabled = false;
            button2.Enabled = false;
            getCourseDetails(0);
        }

        private void btnSaveCourse_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(txtFees.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                txtFees.Focus();
            }
            else
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "insert into mstCourse(courseName,courseFees,startDate,endDate) values (@courseName,@courseFees,@duration);";
                cmd.Parameters.AddWithValue("@courseName", txtCourse.Text.Trim());
                cmd.Parameters.AddWithValue("@courseFees", txtFees.Text.Trim());
                cmd.Parameters.AddWithValue("@startDate", txtDuration.Text.Trim());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Course added successfully!!!");
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
                //int iResult = cmd.ExecuteNonQuery();
                //if (iResult > 0)
                //{
                //    MessageBox.Show("Course added successfully!!!");
                //    cnn.Close();
                //    this.Controls.Clear();
                //    this.InitializeComponent();
                //    reload();
                //}
                //else
                //{
                //    MessageBox.Show("Course already exists");
                //}
            }
        }

        private void getCourseDetails(int mstRegistrationId)
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by mstCourseId) as Sno,* from mstCourse where mstCourseId = case when @mstCourseId = 0 then mstCourseId else @mstCourseId end and isActive = 1;";
            cmd.Parameters.AddWithValue("@mstCourseId", mstRegistrationId);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);

            dgvCourseList.DataSource = null;
            dgvCourseList.AutoGenerateColumns = false;
            dgvCourseList.ColumnCount = 4;
            dgvCourseList.Columns[0].HeaderText = "mstCourseId";
            dgvCourseList.Columns[0].DataPropertyName = "mstCourseId";
            dgvCourseList.Columns[0].Visible = false;
            dgvCourseList.Columns[1].HeaderText = "S No";
            dgvCourseList.Columns[1].DataPropertyName = "Sno";
            dgvCourseList.Columns[1].Width = 50;
            dgvCourseList.Columns[2].HeaderText = "Course Name";
            dgvCourseList.Columns[2].DataPropertyName = "courseName";
            dgvCourseList.Columns[2].Width = 200;
            dgvCourseList.Columns[3].HeaderText = "Course Fees";
            dgvCourseList.Columns[3].DataPropertyName = "courseFees";
            dgvCourseList.Columns[3].Width = 200;

            dgvCourseList.DataSource = dtRegistration;

            dgvCourseList.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            cnn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(lblCourseId.Text == "")
            {
                MessageBox.Show("Please select aleast one course");
            }
            else
            {
                if (Regex.IsMatch(txtFees.Text, "[^0-9]"))
                {
                    MessageBox.Show("Please enter only numbers");
                    txtFees.Focus();
                }
                else
                {
                    cnn.Open();
                    cmd = cnn.CreateCommand();
                    cmd.CommandText = "update mstCourse set courseName = @courseName, courseFees = @courseFees, courseDuration=@duration where mstCourseId = @mstCourseId;";
                    cmd.Parameters.AddWithValue("@courseName", txtCourse.Text.Trim());
                    cmd.Parameters.AddWithValue("@courseFees", txtFees.Text.Trim());
                    cmd.Parameters.AddWithValue("@duration", txtDuration.Text.Trim());
                    cmd.Parameters.AddWithValue("@mstCourseId", lblCourseId.Text);
                    int iResult = cmd.ExecuteNonQuery();
                    if (iResult > 0)
                    {
                        MessageBox.Show("Course updated successfully!!!");
                    }
                    cnn.Close();
                    this.Controls.Clear();
                    this.InitializeComponent();
                    reload();
                }
            }
        }

        private void dgvCourseList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex != -1)
            {
                btnSaveCourse.Enabled = false;
                button1.Enabled = true;
                button2.Enabled = true;
                int iRegistrationId = Convert.ToInt32(dgvCourseList.Rows[e.RowIndex].Cells[0].Value.ToString());
                DataTable dtRegistration = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select row_number() over(order by mstCourseId) as Sno,* from mstCourse where mstCourseId = case when @mstCourseId = 0 then mstCourseId else @mstCourseId end and isActive = 1;";
                cmd.Parameters.AddWithValue("@mstCourseId", iRegistrationId);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtRegistration);
                cnn.Close();

                if (dtRegistration != null)
                {
                    lblCourseId.Text = dtRegistration.Rows[0]["mstCourseId"].ToString();
                    txtCourse.Text = dtRegistration.Rows[0]["courseName"].ToString();
                    txtFees.Text = Convert.ToInt32(dtRegistration.Rows[0]["courseFees"]).ToString();
                    txtDuration.Text = Convert.ToInt32(dtRegistration.Rows[0]["courseDuration"]).ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lblCourseId.Text == "")
            {
                MessageBox.Show("Please select course");
            }
            else
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update mstCourse set isActive = 0 where mstCourseId = @mstCourseId;";
                cmd.Parameters.AddWithValue("@mstCourseId", lblCourseId.Text);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Course deleted successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtCourse.Text = "";
            txtDuration.Text = "";
            txtFees.Text = "";
            lblCourseId.Text = "";
        }
    }
}
