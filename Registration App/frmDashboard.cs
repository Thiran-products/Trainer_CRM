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
    public partial class frmDashboard : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmDashboard()
        {
            InitializeComponent();
        }

        private void frmDashboard_Load(object sender, EventArgs e)
        {
            lblSales.Text = "0";
            lblStudents.Text = "0";
            lblCourse.Text = "0";
            lblExpenses.Text = "0";
            lblLocation.Text = "0";
            getDashboardDetails();
        }

        private void getDashboardDetails()
        {
            try
            {
                DataSet dtDachboardDetails = new DataSet();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select sum(amountPaid) as sales from trnFeesDetails where isActive = 1; " +
                    "select count(mstTrainingRegistrationId) as students from mstTrainingRegistration where isActive = 1; " +
                    "select sum(amount) as expenses from trnExpenses where isActive = 1; " +
                    "select count(locationId) as locations from mstLocation where isActive = 1 and locationName != ''; " +
                    "select count(mstCourseId) as courses from mstCourse where isActive = 1;" +
                    "select sum(amountBalance) as amountBalance from (select max(trnFeesDetailsId),amountBalance from trnFeesDetails where isactive = 1 " +
                    "group by userId,courseId order by trnFeesDetailsId desc) as a;";
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtDachboardDetails);
                cnn.Close();
                lblSales.Text = dtDachboardDetails.Tables[0].Rows.Count > 0 ? dtDachboardDetails.Tables[0].Rows[0]["sales"].ToString() : "0";
                lblStudents.Text = dtDachboardDetails.Tables[1].Rows.Count > 0 ? dtDachboardDetails.Tables[1].Rows[0]["students"].ToString() : "0";
                lblExpenses.Text = dtDachboardDetails.Tables[2].Rows.Count > 0 ? dtDachboardDetails.Tables[2].Rows[0]["expenses"].ToString() : "0";
                lblLocation.Text = dtDachboardDetails.Tables[3].Rows.Count > 0 ? dtDachboardDetails.Tables[3].Rows[0]["locations"].ToString() : "0";
                lblCourse.Text = dtDachboardDetails.Tables[4].Rows.Count > 0 ? dtDachboardDetails.Tables[4].Rows[0]["courses"].ToString() : "0";
                label6.Text = dtDachboardDetails.Tables[5].Rows.Count > 0 ? dtDachboardDetails.Tables[5].Rows[0]["amountBalance"].ToString() : "0";
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
