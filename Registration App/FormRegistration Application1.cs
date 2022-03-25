using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registration_App
{
    public partial class frmRegistrationApp : Form
    {
        public frmRegistrationApp()
        {
            InitializeComponent();
        }

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void frmRegistration_Load(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmDashboard dashboard = new frmDashboard() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(dashboard);
            dashboard.Show();
        }

        private void courseToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void trainingRegistrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
        }

        private void registraionReportToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
         
        }

        private void expensesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void expensesReportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
          
        }

        private void addAttendanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void reportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
          
        }

        private void addFeesToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void reportToolStripMenuItem2_Click(object sender, EventArgs e)
        {
           
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void mainpanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {

        }

        private void courseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmCourseMaster course = new frmCourseMaster() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(course);
            course.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmRegistrationReport registrationReport = new frmRegistrationReport() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(registrationReport);
            registrationReport.Show();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmAttendanceReport attendanceReport = new frmAttendanceReport() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(attendanceReport);
            attendanceReport.Show();
        }

        private void feeDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmFeeDetailsReport feeDetailsReport = new frmFeeDetailsReport() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(feeDetailsReport);
            feeDetailsReport.Show();
        }

        private void expensesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmExpensesReport expensesReport = new frmExpensesReport() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(expensesReport);
            expensesReport.Show();
        }

        private void expensesCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmCategory category = new frmCategory() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(category);
            category.Show();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmExpenses expenses = new frmExpenses() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(expenses);
            expenses.Show();
        }

        private void feesDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmFeeDetails feeDetails = new frmFeeDetails() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(feeDetails);
            feeDetails.Show();
        }

        private void attendanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmAttendance attendance = new frmAttendance() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(attendance);
            attendance.Show();
        }

        private void registrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmRegistration registration = new frmRegistration() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(registration);
            registration.Show();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmDashboard dashboard = new frmDashboard() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(dashboard);
            dashboard.Show();
        }

        private void serviceLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mainpanel.Controls.Clear();
            frmLocation location = new frmLocation() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            this.mainpanel.Controls.Add(location);
            location.Show();
        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Data backup button
            string dbFilePath = Application.StartupPath + @"\RegistrationDB.db";
            string dbNewFilePath = Application.StartupPath + @"\data\backup\RegistrationDB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".db";
            string dirPath = Application.StartupPath + @"\data\backup";
            try
            {
                if (!Directory.Exists(dirPath))
                {//Create a backup folder
                    Directory.CreateDirectory(dirPath);
                }
                File.Copy(dbFilePath, dbNewFilePath);//Copy files
                if (File.Exists(dbNewFilePath))
                {
                    MessageBox.Show("The database file is backed up successfully, the file path: " + dbNewFilePath);
                }
            }
            catch
            {
                MessageBox.Show("Data backup failed. Do not perform other operations while backing up!");
            }
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Data recovery button
            string backUpDir = Application.StartupPath + @"\data\backup";
            string dbFilePath = Application.StartupPath + @"\RegistrationDB.db";
            //Select the file
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "db file|*.db";//Filter file types
            openFileDialog1.InitialDirectory = backUpDir;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {//Restore [overwrite] files
                File.Copy(openFileDialog1.FileName, dbFilePath, true);//Copy the file and overwrite if it exists (you can also play the messagebox and let the user choose whether to overwrite)
                if (File.GetLastWriteTime(openFileDialog1.FileName).ToString("yyyyMMddHHmmss") == File.GetLastWriteTime(dbFilePath).ToString("yyyyMMddHHmmss"))
                {//Judge by comparing the modification date of 2 files
                    MessageBox.Show("The data is restored successfully.");
                }
                else
                {
                    MessageBox.Show("Data recovery failed. Please manually copy files to recover.");
                }
            }
            openFileDialog1.Dispose();
        }

        private void frmRegistrationApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://thiransolution.com/");
        }
    }
}
