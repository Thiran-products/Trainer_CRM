using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registration_App
{
    
    public partial class frmFeeDetails : Form
    {
        private static string dbPath = Application.StartupPath + "\\" + "RegistrationDB.db;";
        private static string connetionString = "Data Source=" + dbPath + "version=3;New=false;Compress=True;";

        private static SQLiteConnection cnn = new SQLiteConnection(connetionString);
        private static SQLiteCommand cmd;
        Boolean isFormLoad;
        string sPaymenType;
        List<string> vString = new List<string>();
        int balance = 0;
        #region Constructor
        public frmFeeDetails()
        {
            InitializeComponent();
            connetionString = @"Data Source=172.16.19.200\MSSQL2017;Initial Catalog=Registration;User ID=sa;Password=12345678";
        }
        #endregion

        #region Form load
        private void frmFeeDetails_Load(object sender, EventArgs e)
        {
            lblFeeDetailId.Text = "0";
            lblFeeDetailId.Visible = false;
            cmbCourse.DropDownStyle = ComboBoxStyle.DropDownList;

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            isFormLoad = true;
            getCourseDetails();
            getRegistrationDetails();
            isFormLoad = false;
            getFeesDetails(0);
        }
        #endregion

        #region reload form
        private void reloadForm()
        {
            lblFeeDetailId.Text = "0";
            lblFeeDetailId.Visible = false;
            cmbCourse.DropDownStyle = ComboBoxStyle.DropDownList;

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            isFormLoad = true;
            getCourseDetails();
            getRegistrationDetails();
            isFormLoad = false;
            getFeesDetails(0);
            richTextBox1.Text = "";
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

        #region Save fees details
        private void btnSave_Click(object sender, EventArgs e)
        {
            DataTable dtRegistrationsAlreadyPaid = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select * from trnFeesDetails where userId = @mstTrainingRegistrationId and courseId = @courseId and isActive = 1;";
            cmd.Parameters.AddWithValue("@mstTrainingRegistrationId", cmbUserName.SelectedValue);
            cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
            SQLiteDataAdapter sdapt = new SQLiteDataAdapter(cmd);
            sdapt.Fill(dtRegistrationsAlreadyPaid);
            cnn.Close();

            if(dtRegistrationsAlreadyPaid.Rows.Count > 0)
            {
                DataTable dtRegistrations = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select max(trnFeesDetailsId),* from trnFeesDetails where userId = @mstTrainingRegistrationId and courseId = @courseId and isActive = 1;";
                cmd.Parameters.AddWithValue("@mstTrainingRegistrationId", cmbUserName.SelectedValue);
                cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
                SQLiteDataAdapter sdap = new SQLiteDataAdapter(cmd);
                sdap.Fill(dtRegistrations);
                cnn.Close();

                if (dtRegistrations.Rows.Count > 0)
                {
                    balance = Convert.ToInt32(dtRegistrations.Rows[0]["amountBalance"] == DBNull.Value ? 0 : dtRegistrations.Rows[0]["amountBalance"]) - Convert.ToInt32(txtAmount.Text);
                }
            }
            else
            {
                DataTable dtRegistration = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select * from mstCourse where mstCourseId = @mstCourseId; ";
                cmd.Parameters.AddWithValue("@mstCourseId", cmbCourse.SelectedValue);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtRegistration);
                cnn.Close();
                if (dtRegistration.Rows.Count > 0)
                {
                    balance = Convert.ToInt32(dtRegistration.Rows[0]["courseFees"] == DBNull.Value ? 0 : dtRegistration.Rows[0]["courseFees"]) - Convert.ToInt32(txtAmount.Text);
                }
            }

            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "insert into trnFeesDetails(userId,courseId,paymentType,amountPaid,amountBalance,isCourseCompleted,createdDate) " +
                "values(@userId, @courseId, @paymentType, @amountPaid, @amountBalance, @isCourseCompleted,@receiptDate)";
            cmd.Parameters.AddWithValue("@userId", cmbUserName.SelectedValue);
            cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
            sPaymenType = chkBank.Checked == true ? chkBank.Text.Trim() : "-";
            sPaymenType += "," + (chkCase.Checked == true ? chkCase.Text.Trim() : "-");
            sPaymenType += "," + (chkCheque.Checked == true ? chkCheque.Text.Trim() : "-");
            sPaymenType += "," + (chkOnline.Checked == true ? chkOnline.Text.Trim() : "-");
            vString = sPaymenType.Split(',').ToList();
            cmd.Parameters.AddWithValue("@paymentType", string.Join(",", vString.Where(a => !a.Contains("-")).ToList()));
            cmd.Parameters.AddWithValue("@amountPaid", txtAmount.Text);
            cmd.Parameters.AddWithValue("@receiptDate", Convert.ToDateTime(dtp_date.Text).Date);
            cmd.Parameters.AddWithValue("@amountBalance", balance);
            cmd.Parameters.AddWithValue("@isCourseCompleted", chkCompleted.Checked == true ? 1 : 0);
            int iResult = cmd.ExecuteNonQuery();
            if (iResult > 0)
            {
                MessageBox.Show("Fees detail saved successfully!!!");
            }
            cnn.Close();
            lblBalanceAmt.Text = balance.ToString();

            print_bill();


            this.Controls.Clear();
            this.InitializeComponent();
            reloadForm();
        }
        #endregion

        #region Course drop down change event
        private void cmbCourse_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isFormLoad == false)
                {
                    DataTable dtRegistration = new DataTable();
                    cnn.Open();
                    cmd = cnn.CreateCommand();
                    cmd.CommandText = "select row_number() over(order by mstCourseId) as Sno,* from mstCourse where mstCourseId =@mstCourseId and isActive = 1;";
                    cmd.Parameters.AddWithValue("@mstCourseId", cmbCourse.SelectedValue);
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    sda.Fill(dtRegistration);
                    cnn.Close();
                    if (Convert.ToInt32(cmbCourse.SelectedValue) != 0)
                    {
                        txtfee.Text = dtRegistration.Rows[0]["courseFees"].ToString();
                        txtDuration.Text = dtRegistration.Rows[0]["courseDuration"].ToString();
                    }
                    else
                    {
                        txtfee.Text = "";
                        txtDuration.Text = "";
                    }
                }
                else
                {
                    txtfee.Text = "";
                    txtDuration.Text = "";
                }
                txtfee.Enabled = false;
                txtDuration.Enabled = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Get fees details
        private void getFeesDetails(int trnFeesDetailsId)
        {
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select row_number() over(order by trnFeesDetailsId) as Sno,*,date(tfd.createdDate) as billDate from trnFeesDetails tfd " +
                "inner join mstCourse mc on tfd.courseId = mc.mstCourseId " +
                "inner join mstTrainingRegistration mtr on tfd.userId = mtr.mstTrainingRegistrationId " +
                "where " +
                "tfd.trnFeesDetailsId = case when @trnFeesDetailsId = 0 then tfd.trnFeesDetailsId else @trnFeesDetailsId end and " +
                "tfd.isActive = 1 " +
                "and (strftime('%m', date(tfd.createdDate))='" + DateTime.Now.ToString("MM") + "' and strftime('%Y', date(tfd.createdDate))='" + DateTime.Now.ToString("yyyy") + "');";
            cmd.Parameters.AddWithValue("@trnFeesDetailsId", trnFeesDetailsId);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();

            dgvFeeDetails.AutoGenerateColumns = false;
            dgvFeeDetails.ColumnCount = 8;
            dgvFeeDetails.Columns[0].HeaderText = "trnFeesDetailsId";
            dgvFeeDetails.Columns[0].DataPropertyName = "trnFeesDetailsId";
            dgvFeeDetails.Columns[0].Visible = false;
            dgvFeeDetails.Columns[1].Width = 60;
            dgvFeeDetails.Columns[1].HeaderText = "Sno";
            dgvFeeDetails.Columns[1].DataPropertyName = "Sno";
            dgvFeeDetails.Columns[2].HeaderText = "Name";
            dgvFeeDetails.Columns[2].DataPropertyName = "name";
            dgvFeeDetails.Columns[3].Width = 50;
            dgvFeeDetails.Columns[3].HeaderText = "Course";
            dgvFeeDetails.Columns[3].DataPropertyName = "courseName";
            dgvFeeDetails.Columns[4].HeaderText = "Payment Type";
            dgvFeeDetails.Columns[4].DataPropertyName = "paymentType";
            dgvFeeDetails.Columns[5].HeaderText = "Fee Paid";
            dgvFeeDetails.Columns[5].DataPropertyName = "amountPaid";
            dgvFeeDetails.Columns[6].HeaderText = "Bill Date";
            dgvFeeDetails.Columns[6].DataPropertyName = "billDate";
            dgvFeeDetails.Columns[7].HeaderText = "Outstanding Fees";
            dgvFeeDetails.Columns[7].DataPropertyName = "amountBalance";

            dgvFeeDetails.DataSource = dtRegistration;
        }
        #endregion

        #region Edit fees details
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex != -1)
            {
                btnSave.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                getCourseDetails();
                getRegistrationDetails();
                int iFeesDetailsId = Convert.ToInt32(dgvFeeDetails.Rows[e.RowIndex].Cells[0].Value.ToString());
                DataTable dtRegistration = new DataTable();
                cnn.Open();
                cmd = cnn.CreateCommand();
                cmd.CommandText = "select row_number() over(order by trnFeesDetailsId) as Sno,* from trnFeesDetails tfd " +
                    "inner join mstCourse mc on tfd.courseId = mc.mstCourseId " +
                    "inner join mstTrainingRegistration mtr on tfd.userId = mtr.mstTrainingRegistrationId " +
                    "where " +
                    "tfd.trnFeesDetailsId = case when @trnFeesDetailsId = 0 then tfd.trnFeesDetailsId else @trnFeesDetailsId end and " +
                    "tfd.isActive = 1 and mc.isActive = 1 and mtr.isActive = 1; ";
                cmd.Parameters.AddWithValue("@trnFeesDetailsId", iFeesDetailsId);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                sda.Fill(dtRegistration);
                cnn.Close();
                lblFeeDetailId.Text = iFeesDetailsId.ToString();
                cmbUserName.SelectedValue = Convert.ToInt32(dtRegistration.Rows[0]["userId"]);
                cmbCourse.SelectedValue = Convert.ToInt32(dtRegistration.Rows[0]["courseId"]);
                txtfee.Text = dtRegistration.Rows[0]["courseFees"].ToString();
                txtDuration.Text = dtRegistration.Rows[0]["courseDuration"].ToString();
                if (dtRegistration.Rows[0]["paymentType"].ToString() != "" && dtRegistration.Rows[0]["paymentType"].ToString() != null)
                {
                    var vCheckedValues = dtRegistration.Rows[0]["paymentType"].ToString().Split(',');
                    foreach (var v in vCheckedValues)
                    {
                        if (v == "Cash")
                        {
                            chkCase.Checked = true;
                        }
                        else if (v == "Cheque")
                        {
                            chkCheque.Checked = true;
                        }
                        else if (v == "Online Banking")
                        {
                            chkOnline.Checked = true;
                        }
                        else if (v == "Bank")
                        {
                            chkBank.Checked = true;
                        }
                    }
                }
                else
                {
                    chkCase.Checked = false;
                    chkCheque.Checked = false;
                    chkOnline.Checked = false;
                    chkBank.Checked = false;
                }
                txtAmount.Text = dtRegistration.Rows[0]["amountPaid"].ToString();
                if (Convert.ToInt32(dtRegistration.Rows[0]["isCourseCompleted"]) == 1)
                {
                    chkCompleted.Checked = true;
                }
                else
                {
                    chkCompleted.Checked = false;
                }
            }
        }
        #endregion

        #region Update fees details
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int balance = 0;
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select * from mstCourse where mstCourseId = @mstCourseId; ";
            cmd.Parameters.AddWithValue("@mstCourseId", cmbCourse.SelectedValue);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();
            if (dtRegistration.Rows.Count > 0)
            {
                balance = Convert.ToInt32(dtRegistration.Rows[0]["courseFees"] == DBNull.Value ? 0 : dtRegistration.Rows[0]["courseFees"]) - Convert.ToInt32(txtAmount.Text);
            }

            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "update trnFeesDetails set userId=@userId,courseId=@courseId,paymentType=@paymentType,amountPaid=@amountPaid,amountBalance=@amountBalance,isCourseCompleted=@isCourseCompleted " +
                "where trnFeesDetailsId = @trnFeesDetailsId; ";
            cmd.Parameters.AddWithValue("@userId", cmbUserName.SelectedValue);
            cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
            string sPaymenType = chkBank.Checked == true ? chkBank.Text.Trim() : "-";
            sPaymenType += "," + (chkCase.Checked == true ? chkCase.Text.Trim() : "-");
            sPaymenType += "," + (chkCheque.Checked == true ? chkCheque.Text.Trim() : "-");
            sPaymenType += "," + (chkOnline.Checked == true ? chkOnline.Text.Trim() : "-");
            var vString = sPaymenType.Split(',').ToList();
            cmd.Parameters.AddWithValue("@paymentType", string.Join(",", vString.Where(a => !a.Contains("-")).ToList()));
            cmd.Parameters.AddWithValue("@amountPaid", txtAmount.Text);
            cmd.Parameters.AddWithValue("@amountBalance", balance);
            cmd.Parameters.AddWithValue("@isCourseCompleted", chkCompleted.Checked == true ? 1 : 0);
            cmd.Parameters.AddWithValue("@trnFeesDetailsId", lblFeeDetailId.Text);
            int iResult = cmd.ExecuteNonQuery();
            if (iResult > 0)
            {
                MessageBox.Show("Fees detail updated successfully!!!");
            }
            cnn.Close();
            this.Controls.Clear();
            this.InitializeComponent();
            reloadForm();
        }
        #endregion

        #region Delete fees details
        private void btnDelete_Click(object sender, EventArgs e)
        {
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "update trnFeesDetails set isActive = 0 where trnFeesDetailsId = @trnFeesDetailsId;";
            cmd.Parameters.AddWithValue("@trnFeesDetailsId", lblFeeDetailId.Text);
            int iResult = cmd.ExecuteNonQuery();
            if (iResult > 0)
            {
                MessageBox.Show("Fees detail deleted successfully!!!");
            }
            cnn.Close();
            this.Controls.Clear();
            this.InitializeComponent();
            reloadForm();
        }
        #endregion

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
        private void print_bill()
        {
            Console.WriteLine("Printing Started ");


            PrintDialog printDlg = new PrintDialog();
            PrintDocument printDoc = new PrintDocument();
           /* printDoc.DocumentName = cmbUserName.Text.Trim() + "'s Fees Receipt";
            //printDoc.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("thiran_biller", 290, 550);
            printDoc.DefaultPageSettings.Margins = new Margins(30, 30, 20, 20);
            printDlg.Document = printDoc;
            // printDlg.AllowSelection = true;
            // printDlg.AllowSomePages = true;
            //Call ShowDialog
            richTextBox1.Text += "\n";
            richTextBox1.Text += "----------------------------------------------------\n";
            richTextBox1.Text += "                          THIRD EYE \n";
            richTextBox1.Text += "             Your Business Address \n";
            richTextBox1.Text += "----------------------------------------------------\n";
            richTextBox1.Text += "                        FEES RECEIPT \n";
            richTextBox1.Text += "Date              :    " + DateTime.Now.ToString("dd/MM/yyyy") + "\n";
            richTextBox1.Text += "Name              :    " + label8.Text.Trim() + "\n";
            richTextBox1.Text += "Course            :    " + cmbCourse.Text.Trim() + "\n";
            richTextBox1.Text += "Course Fees       :    " + txtfee.Text.Trim() + "\n";
            richTextBox1.Text += "Fees paid         :    " + txtAmount.Text.Trim() + "\n";
            richTextBox1.Text += "Fees Balance      :    " + txtAmount.Text.Trim() + "\n";
            richTextBox1.Text += "\n";
            richTextBox1.Text += "----------------------------------------------------\n";
            richTextBox1.Text += "                         Thank You \n";
            richTextBox1.Text += "     Powered by thiransolution.com \n";
            richTextBox1.Text += "\n";*/

            /*if (printDlg.ShowDialog() == DialogResult.OK)
            {
                printDoc.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
                printDoc.Print();
            }*/

            printDlg.Document = printDoc;

          
                if (printDlg.ShowDialog() == DialogResult.OK)
                {
               // printDoc.DefaultPageSettings.PaperSize = new PaperSize("thiran_biller", 290, 420);
                PaperSize ps = new PaperSize();
                ps.RawKind = (int)PaperKind.A4;
                printDoc.DefaultPageSettings.PaperSize = ps;
                printDoc.DefaultPageSettings.Margins = new Margins(40, 40, 20, 20);
                printDoc.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
                printDoc.Print();
                }
           

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string sBillNo = string.Empty;
            DataTable dtRegistration = new DataTable();
            cnn.Open();
            cmd = cnn.CreateCommand();
            cmd.CommandText = "select * from trnFeesDetails order by trnFeesDetailsId desc;";
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            sda.Fill(dtRegistration);
            cnn.Close();

            if (dtRegistration.Rows.Count > 0)
            {
                if ((Convert.ToInt32(dtRegistration.Rows[0]["trnFeesDetailsId"]) + 1).ToString().Length == 1)
                {
                    sBillNo += "00" + (Convert.ToInt32(dtRegistration.Rows[0]["trnFeesDetailsId"]) + 1).ToString();
                }
                else if ((Convert.ToInt32(dtRegistration.Rows[0]["trnFeesDetailsId"]) + 1).ToString().Length == 2)
                {
                    sBillNo += "0" + (Convert.ToInt32(dtRegistration.Rows[0]["trnFeesDetailsId"]) + 1).ToString();
                }
                else
                {
                    sBillNo += (Convert.ToInt32(dtRegistration.Rows[0]["trnFeesDetailsId"]) + 1).ToString();
                }
            }
            else
            {
                sBillNo += "000";
            }



            Dictionary<string, string> print_items = new Dictionary<string, string>();

            Graphics graphics = e.Graphics;

            Font regular = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Regular);
            Font small = new Font(FontFamily.GenericSansSerif, 8.0f, FontStyle.Regular);
            Font xsmall = new Font(FontFamily.GenericSansSerif, 6.0f, FontStyle.Regular);
            Font title_font = new Font("Arial", 20.0f, FontStyle.Bold);
            Font subtitle_font = new Font("Arial", 10.0f, FontStyle.Bold);
            Font date_font = new Font("Arial", 8.0f, FontStyle.Bold);
            Font total_font = new Font("Arial", 15.0f, FontStyle.Bold);
            Font bold = new Font(FontFamily.GenericSansSerif, 11.0f, FontStyle.Bold);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Far;

            //String billdate = DateTime.Now.ToString("dd/MM/yyyy");
            String billdate = dtp_date.Text.Trim();

            String Business_name = "ThirdEye";
            String Business_tag = "CREATIVE TRAINERS & MENTORS";
            String addressLine1 = "3rd Floor, Super Bazar Complex, Fort Road, Kannur Kerala - 670001";
            String contact = "Contact  : 9447438753 | 0497 2763677";

            //print header
            graphics.DrawString(Business_name, title_font, Brushes.Black, 350, 40);  // y 20 +
            graphics.DrawString(Business_tag, subtitle_font, Brushes.Black, 300, 80);

            //graphics.DrawLine(Pens.Black, 10, 90, 800, 90);                       // y 100+
            graphics.DrawString(addressLine1, small, Brushes.Black, 245, 100);
            graphics.DrawString(contact, small, Brushes.Black, 330, 115);
            graphics.DrawLine(Pens.Black, 20, 130, 800, 130);

            graphics.DrawString("Bill No : " + sBillNo, subtitle_font, Brushes.Black, 40, 140);
            graphics.DrawString("Date : " + billdate, date_font, Brushes.Black, 780, 140, stringFormat);
            graphics.DrawString("Name : " + label8.Text.Trim(), subtitle_font, Brushes.Black, 40, 160);
            graphics.DrawString("Payment Type: " + string.Join(",", vString.Where(a => !a.Contains("-")).ToList()), subtitle_font, Brushes.Black, 40, 180);
            graphics.DrawLine(Pens.Black, 20, 200, 800, 200);

            graphics.DrawString("Courses", subtitle_font, Brushes.Black, 40, 210);
            graphics.DrawString("Fees", subtitle_font, Brushes.Black, 780, 210, stringFormat);
            graphics.DrawLine(Pens.Black, 20, 230, 800, 230);

            if (cmbCourse.Text != String.Empty)
                print_items.Add(cmbCourse.Text, txtfee.Text.Trim());

            print_items.Add("--", "--");


            List<string> K_val = print_items.Keys.ToList();
            List<string> V_val = print_items.Values.ToList();
            int y_ax = 240;
            Console.WriteLine("Print Length - " + K_val.Count);
            for (int i = 0; i < K_val.Count; i++)
            {
                graphics.DrawString(K_val[i], regular, Brushes.Black, 45, y_ax);
                graphics.DrawString(V_val[i], regular, Brushes.Black, 780, y_ax, stringFormat);
                y_ax = y_ax + 20;
            }

            //print footer
            graphics.DrawLine(Pens.Black, 20, 300, 800, 300);
            graphics.DrawString("Amount Paid", subtitle_font, Brushes.Black, 550, 310);
            graphics.DrawString(txtAmount.Text.Trim(), subtitle_font, Brushes.Black, 780, 305, stringFormat);

            graphics.DrawString("OutStanding Amount", subtitle_font, Brushes.Black, 550, 330);
            graphics.DrawString(lblBalanceAmt.Text.Trim(), subtitle_font, Brushes.Black, 780, 330, stringFormat);


            graphics.DrawLine(Pens.Black, 20, 350, 800, 350);
            graphics.DrawLine(Pens.Black, 20, 355, 800, 355);
            graphics.DrawString("------------       Thank You        ------------", small, Brushes.Black, 320, 365);
            graphics.DrawString("Powerd by Thiransolution.com", xsmall, Brushes.Black, 360, 385);


            /// Pos Bill Size 
           /* graphics.DrawString(Business_name, title_font, Brushes.Black, 70, 20);  // y 20 +
            graphics.DrawString(Business_tag, subtitle_font, Brushes.Black, 20, 60);

            graphics.DrawLine(Pens.Black, 10, 90, 280, 90);                       // y 100+
            graphics.DrawString(addressLine1, small, Brushes.Black, 30, 100);
            graphics.DrawString(addressLine2, small, Brushes.Black, 35, 115);
            graphics.DrawLine(Pens.Black, 10, 130, 280, 130);

            graphics.DrawString("Bill No : " + "001", subtitle_font, Brushes.Black, 20, 140);
            graphics.DrawString("Date : " + billdate, date_font, Brushes.Black, 270, 140, stringFormat);
            graphics.DrawString("C.Name : " + label8.Text.Trim(), subtitle_font, Brushes.Black, 20, 160);
            graphics.DrawString("Payment Type: " + string.Join(",", vString.Where(a => !a.Contains("-")).ToList()), subtitle_font, Brushes.Black, 20, 180);
            graphics.DrawLine(Pens.Black, 10, 200, 280, 200);

            graphics.DrawString("Courses", subtitle_font, Brushes.Black, 20, 210);
            graphics.DrawString("Fees", subtitle_font, Brushes.Black, 270, 210, stringFormat);
            graphics.DrawLine(Pens.Black, 10, 230, 280, 230);

            if (cmbCourse.Text != String.Empty )
                print_items.Add(cmbCourse.Text, txtfee.Text.Trim());

                print_items.Add("--", "--");


            List<string> K_val = print_items.Keys.ToList();
            List<string> V_val = print_items.Values.ToList();
            int y_ax = 240;
            Console.WriteLine("Print Length - " + K_val.Count);
            for (int i = 0; i < K_val.Count; i++)
            {
                graphics.DrawString(K_val[i], regular, Brushes.Black, 20, y_ax);
                graphics.DrawString(V_val[i], regular, Brushes.Black, 270, y_ax, stringFormat);
                y_ax = y_ax + 20;
            }

            //print footer
            graphics.DrawLine(Pens.Black, 10, 300, 280, 300);
            graphics.DrawString("Amount Paid", subtitle_font, Brushes.Black, 20, 310);
            graphics.DrawString(txtAmount.Text.Trim(), subtitle_font, Brushes.Black, 270, 305, stringFormat);
            
            graphics.DrawString("OutStanding Amount", subtitle_font, Brushes.Black, 20, 330);
            graphics.DrawString(lblBalanceAmt.Text.Trim(), subtitle_font, Brushes.Black, 270, 330, stringFormat);


            graphics.DrawLine(Pens.Black, 10, 350, 280, 350);
            graphics.DrawLine(Pens.Black, 10, 355, 280, 355);
            graphics.DrawString("------------       Thank You        ------------", small, Brushes.Black, 45, 365);
            graphics.DrawString("Powerd by Thiransolution.com", xsmall, Brushes.Black, 80, 385);

*/

            regular.Dispose();
            bold.Dispose();
            print_items.Clear();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void formClear()
        {
            dtp_date.Text = DateTime.Now.Date.ToString();
            cmbUserName.SelectedValue = 0;
            label8.Text = "";
            cmbCourse.SelectedValue = 0;
            txtfee.Text = "";
            txtDuration.Text = "";
            chkCase.Checked = false;
            chkBank.Checked = false;
            chkCheque.Checked = false;
            chkOnline.Checked = false;
            txtAmount.Text = "";
            chkCompleted.Checked = false;
            lblFeeDetailId.Text = "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            formClear();
        }
    }
}
