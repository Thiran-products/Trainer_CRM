using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registration_App
{
    public partial class frmLogin : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Please enter user name");
                textBox1.Focus();
            }
            else if (textBox2.Text.Trim() == "")
            {
                MessageBox.Show("Please enter password");
                textBox2.Focus();
            }
            else
            {
                string vMacDetails = GetMacAddress().ToString();
                checkMacAddressDetails(vMacDetails);
            }
        }

        #region Get Mac address from PC
        public static PhysicalAddress GetMacAddress()
        {
            var myInterfaceAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .OrderByDescending(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Select(n => n.GetPhysicalAddress())
                .FirstOrDefault();

            return myInterfaceAddress;
        }
        #endregion

        #region Check mac details from DB
        private void checkMacAddressDetails(string sMacAddress)
        {
            try
            {
                DateTime dtCurrentDate = DateTime.Now;
                DataTable dtMAC = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select * from mstMACAddress where macAddress ='"+ sMacAddress + "' and (date(startDate) <= '"+ dtCurrentDate.ToString("yyyy-MM-dd") + "' and date(endDate) >= '" + dtCurrentDate.ToString("yyyy-MM-dd") + "') and isActive = 1;";
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtMAC);
                cnn.Close();
                if(dtMAC.Rows.Count > 0)
                {
                    DataTable dtUser= new DataTable();
                    cnn.Open();
                    cmd = cnn.CreateCommand();
                    cmd.CommandText = "select * from mstLoginUsers where userName ='" + textBox1.Text.Trim() + "' and password = '"+textBox2.Text.Trim()+"' and isActive = 1;";
                    SQLiteDataAdapter sdar = new SQLiteDataAdapter(cmd);
                    sdar.Fill(dtUser);
                    cnn.Close();
                    if(dtUser.Rows.Count > 0)
                    {
                        frmRegistrationApp registrationApp = new frmRegistrationApp();
                        registrationApp.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid credential");
                    }
                }
                else
                {
                    frmLicense license = new frmLicense();
                    license.Show();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
