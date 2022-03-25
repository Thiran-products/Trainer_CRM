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
    public partial class frmLocation : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        public frmLocation()
        {
            InitializeComponent();
        }

        private void frmLocation_Load(object sender, EventArgs e)
        {
            reload();
        }

        #region Reload form
        private void reload()
        {
            lblLocationd.Text = "0";
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;

            getLocatioDetails(0);
        }
        #endregion

        #region Get location
        private void getLocatioDetails(int locationId)
        {
            try
            {
                DataTable dtRegistration = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select row_number() over(order by locationId) as Sno,* from mstLocation where locationId = case when @locationId = 0 then locationId else @locationId end and isActive = 1;";
                cmd.Parameters.AddWithValue("@locationId", locationId);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtRegistration);
                dgvLocationDetails.DataSource = null;
                dgvLocationDetails.AutoGenerateColumns = false;
                dgvLocationDetails.ColumnCount = 3;
                dgvLocationDetails.Columns[0].HeaderText = "locationId";
                dgvLocationDetails.Columns[0].DataPropertyName = "locationId";
                dgvLocationDetails.Columns[0].Visible = false;
                dgvLocationDetails.Columns[1].Width = 60;
                dgvLocationDetails.Columns[1].HeaderText = "S No";
                dgvLocationDetails.Columns[1].DataPropertyName = "Sno";
                dgvLocationDetails.Columns[2].HeaderText = "Location";
                dgvLocationDetails.Columns[2].DataPropertyName = "locationName";
                dgvLocationDetails.Columns[2].Width = 248;
                dgvLocationDetails.DataSource = dtRegistration;
                cnn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Save location
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "INSERT INTO mstLocation(locationName)VALUES(@locationName);";
                cmd.Parameters.AddWithValue("@locationName", txtLocation.Text.Trim());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Location saved successfully!!!");
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Update location
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update mstLocation set locationName = @locationName where locationId = @locationId;";
                cmd.Parameters.AddWithValue("@locationName", txtLocation.Text.Trim());
                cmd.Parameters.AddWithValue("@locationId", lblLocationd.Text);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Location updated successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Delete location
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "update mstLocation set isActive = 0 where locationId = @locationId;";
                cmd.Parameters.AddWithValue("@locationId", lblLocationd.Text);
                int iResult = cmd.ExecuteNonQuery();
                if (iResult > 0)
                {
                    MessageBox.Show("Location deleted successfully!!!");
                }
                cnn.Close();
                this.Controls.Clear();
                this.InitializeComponent();
                reload();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Edit location
        private void dgvLocationDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.RowIndex != -1)
                {
                    btnSave.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    int iCategoryId = Convert.ToInt32(dgvLocationDetails.Rows[e.RowIndex].Cells[0].Value.ToString());
                    DataTable dtRegistration = new DataTable();
                    cnn.Open();
                    cmd = cnn.CreateCommand();
                    cmd.CommandText = "select row_number() over(order by locationId) as Sno,* from mstLocation where locationId = case when @locationId = 0 then locationId else @locationId end and isActive = 1;";
                    cmd.Parameters.AddWithValue("@locationId", iCategoryId);
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    sda.Fill(dtRegistration);
                    lblLocationd.Text = dtRegistration.Rows[0]["locationId"].ToString();
                    txtLocation.Text = dtRegistration.Rows[0]["locationName"].ToString();
                    cnn.Close();
                }
            }
            catch (Exception ex)
            {
                cnn.Close();
                throw ex;
            }
        }
        #endregion
    }
}
