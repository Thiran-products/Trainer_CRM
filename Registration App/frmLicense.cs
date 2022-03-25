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
    public partial class frmLicense : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmLicense()
        {
            InitializeComponent();
        }

        #region Check key
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtKey.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter key");
                    txtKey.Focus();
                }
                else
                {
                    DataTable dtMAC = new DataTable();
                    cnn.Open();
                    cmd = cnn.CreateCommand();
                    cmd.CommandText = "select * from mstLoginUsers where roleId = 1 and password ='" + txtKey.Text.Trim() + "' and isActive = 1;";
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    sda.Fill(dtMAC);
                    cnn.Close();

                    if (dtMAC.Rows.Count > 0)
                    {
                        cnn.Open();
                        cmd = cnn.CreateCommand();
                        cmd.CommandText = "insert into mstMACAddress (macAddress,startDate,endDate) values ('" + GetMacAddress().ToString() + "', datetime('now', 'localtime'), DATE('now','localtime','start of day','+7 day'));";
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                        this.Close();

                        frmRegistrationApp registrationApp = new frmRegistrationApp();
                        registrationApp.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("You are not authorized to access");
                        this.Show();
                        txtKey.Focus();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //remove the entire system menu:
        private const int WS_SYSMENU = 0x80000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
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
    }
}
