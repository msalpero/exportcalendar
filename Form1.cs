using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Exchange.WebServices.Auth;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using CsvHelper.Configuration;
using System.Reflection;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
        }
        string mbx =string.Empty;
        bool defaultcred = true;
        string acc = "acc";
        string pas = "pas";
        SearchFilter SFM = null;
        DateTime startDate = DateTime.Now.AddDays(-365);
        DateTime endDate = DateTime.Now.AddDays(365);
        DataTable dtmain = new DataTable();
        Stopwatch stopwatch = Stopwatch.StartNew();
        string delimiter = ",";
        public static string fnStringConverterCodepage(string sText, string sCodepageIn = "Windows-1254", string sCodepageOut = "Windows-1254")
        {
            string sResultado = string.Empty;
            try
            {
                byte[] tempBytes;
                tempBytes = System.Text.Encoding.GetEncoding(sCodepageIn).GetBytes(sText);
                sResultado = System.Text.Encoding.GetEncoding(sCodepageOut).GetString(tempBytes);
            }
            catch (Exception)
            {
                sResultado = "";
            }
            return sResultado;
        }
        public ExchangeService GetBinding()
        {
            WebCredentials cred = new WebCredentials(acc, pas);
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            service.Url = new Uri(textBox1.Text, UriKind.Absolute);
            service.UseDefaultCredentials = defaultcred;
            if (defaultcred == false)
            {
                service.Credentials = cred;
            }
            else
            {

            }
            service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, mbx);
            return service;
        }
        public SearchFilter SF1()
        {
            SearchFilter filter = new SearchFilter.SearchFilterCollection(LogicalOperator.And,
            new SearchFilter.IsEqualTo(ItemSchema.ItemClass, "IPM.Appointment"),
            new SearchFilter.IsEqualTo(AppointmentSchema.MyResponseType, "Organizer"),
            new SearchFilter.IsEqualTo(AppointmentSchema.Location, "Microsoft Teams Meeting"),
            new SearchFilter.IsGreaterThanOrEqualTo(AppointmentSchema.Start, startDate),
            new SearchFilter.IsLessThan(AppointmentSchema.Start, endDate)
            );
            return filter;
        }
        public SearchFilter SF2()
        {
            SearchFilter filter = new SearchFilter.SearchFilterCollection(LogicalOperator.And,
             new SearchFilter.IsEqualTo(ItemSchema.ItemClass, "IPM.Appointment"),
             new SearchFilter.IsEqualTo(AppointmentSchema.MyResponseType, "Organizer"),
             new SearchFilter.IsGreaterThanOrEqualTo(AppointmentSchema.Start, startDate),
             new SearchFilter.IsLessThan(AppointmentSchema.Start, endDate)
             );
            return filter;
        }
        public SearchFilter SF3()
        {
            SearchFilter filter = new SearchFilter.SearchFilterCollection(LogicalOperator.And,
            new SearchFilter.IsEqualTo(ItemSchema.ItemClass, "IPM.Appointment"),
            new SearchFilter.IsGreaterThanOrEqualTo(AppointmentSchema.Start, startDate),
            new SearchFilter.IsLessThan(AppointmentSchema.Start, endDate)
            );
            return filter;
        }
        public SearchFilter SF4()
        {
            SearchFilter filter = new SearchFilter.SearchFilterCollection(LogicalOperator.And,
            new SearchFilter.IsEqualTo(ItemSchema.ItemClass, "IPM.Appointment"),
            new SearchFilter.IsEqualTo(AppointmentSchema.Location, "Microsoft Teams Meeting"),
            new SearchFilter.IsGreaterThanOrEqualTo(AppointmentSchema.Start, startDate),
            new SearchFilter.IsLessThan(AppointmentSchema.Start, endDate)
            );
            return filter;
        }
        public FindItemsResults<Item> Find(string mbxid)
        {
            Guid MyPropertySetId = new Guid("{00020329-0000-0000-C000-000000000046}");
            ExtendedPropertyDefinition exdef = new ExtendedPropertyDefinition(MyPropertySetId, "SkypeTeamsMeetingUrl", MapiPropertyType.String);
            PropertySet props = new PropertySet(BasePropertySet.FirstClassProperties, exdef);
            ItemView Iview = new ItemView(5000);
            Iview.PropertySet = props;
            FolderId folderid = new FolderId(WellKnownFolderName.Calendar, mbxid);
            CalendarFolder calfolder = CalendarFolder.Bind(GetBinding(), folderid);
            var result = calfolder.FindItems(SFM, Iview);
            return result;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SFM = SF3();
            listView1.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            header.Text = "Calendar Export Logging";
            header.Name = "col1";
            listView1.Columns.Add(header);
            listView1.Scrollable = true;
            listView1.AutoResizeColumn(0,ColumnHeaderAutoResizeStyle.HeaderSize);
            comboBox1.Text = "--Select--";

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                defaultcred = false;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                listView1.Items.Add("EWS Impersination is active : " + defaultcred);


            }
            else
            {
                defaultcred = true;
                acc = string.Empty;
                pas = string.Empty;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                listView1.Items.Add("EWS Impersination is deactive : " + defaultcred);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            acc = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            pas = textBox3.Text;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                dateTimePicker1.Enabled = true;
                startDate = dateTimePicker1.Value;
            }
            else
            {
                dateTimePicker1.Enabled = false;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                textBox5.Enabled = true;
                button1.Enabled = true;
                checkBox7.Enabled = false;

            }
            else
            {
                textBox5.Enabled = false;
                button1.Enabled = false;
                dtmain.Clear();
                checkBox7.Enabled = true;

            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                dateTimePicker2.Enabled = true;
                endDate = dateTimePicker2.Value;
            }
            else
            {
                dateTimePicker2.Enabled = false;
            }

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            startDate = dateTimePicker1.Value;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            endDate = dateTimePicker2.Value;
        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                if (checkBox6.Checked)
                {
                    SFM = SF1();
                }
                else
                {
                    SFM = SF4();
                }
            }
            else
            {
                if (checkBox6.Checked)
                {
                    SFM = SF4();
                }
                else
                {
                    SFM = SF3();
                }

            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                if (checkBox2.Checked)
                {
                    SFM = SF1();
                }
                else
                {
                    SFM = SF2();
                }
            }
            else
            {
                if (checkBox2.Checked)
                {
                    SFM = SF2();
                }
                else
                {
                    SFM = SF3();
                }

            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                textBox4.Enabled = true;
                button2.Enabled = true;
                checkBox4.Enabled = false;
            }
            else
            {
                dtmain.Clear();
                textBox4.Enabled = false;
                button2.Enabled = false;
                checkBox4.Enabled = true;
            }
        }
        private static DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();

            try
            {

                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }

                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch
            {
            }
            return csvData;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string filename = string.Empty;
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                textBox4.Text = openFileDialog1.FileName;
                DataTable csvData = GetDataTabletFromCSVFile(filename);
                dtmain = csvData;
            }
            else if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Dosya secilmedi..!");
            }

        }
        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("Mailbox   : " + mbx);
                txtWriter.WriteLine("Error     : {0} ", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (dtmain.Rows.Count == 0)
            {
                MessageBox.Show("You must select at least one mailbox.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                Guid MyPropertySetId = new Guid("{00020329-0000-0000-C000-000000000046}");
                ExtendedPropertyDefinition exdef = new ExtendedPropertyDefinition(MyPropertySetId, "SkypeTeamsMeetingUrl", MapiPropertyType.String);
                PropertySet props = new PropertySet(BasePropertySet.FirstClassProperties, exdef);
                ItemView Iview = new ItemView(5000);
                stopwatch.Start();
                string m_exePath = string.Empty;

                foreach (DataRow dataRow in dtmain.Rows)
                {
                    try
                    {
                        foreach (var item in dataRow.ItemArray)
                        {
                            mbx = item.ToString();
                            var result = Find(item.ToString());
                            foreach (var appi in result.OfType<Appointment>())
                            {

                                listView1.Items.Add("Calendar export : " + item.ToString());
                                appi.Load(props);
                                string appendText = string.Empty;
                                string path = @"C:\Output\CalendarFind.csv";

                                if (!File.Exists(path))
                                {
                                    string createText = "Organizer" + delimiter + "Email" + delimiter + "MyResponse" + delimiter + "Subject" + delimiter + "Type" + delimiter + "Required Attendes" + delimiter + "Location" + delimiter + "Start" + delimiter + "End" + delimiter + "Optional Attendes" + delimiter + "TeamsUrl" + delimiter + "Recurrence" + delimiter + "Duration" + Environment.NewLine;
                                    File.WriteAllText(path, createText, Encoding.UTF8);
                                }
                                var regatten = string.Empty;
                                if (appi.RequiredAttendees.Count > 0)
                                {
                                    foreach (Attendee att in appi.RequiredAttendees)
                                    {
                                        regatten = regatten + att.Address + ";";
                                    }                                    
                                }
                                else 
                                {
                                    regatten = "** No required attendees **";
                                }
                                var regopt = string.Empty;
                                if (appi.OptionalAttendees.Count > 0)
                                {
                                    foreach (Attendee ato in appi.OptionalAttendees)
                                    {
                                        regopt = regopt + ato.Address + ";";
                                    }
                                }
                                else
                                {
                                    regopt = "** No optional attendees **";
                                }
                                var skype = string.Empty;
                                if (appi.ExtendedProperties.Count > 0)
                                {
                                    foreach (ExtendedProperty extendedProperty in appi.ExtendedProperties)
                                    {
                                        skype = skype + extendedProperty.Value;
                                    }
                                }
                                string turdata = string.Empty;
                                string durdata = string.Empty;
                                if (appi.Subject == null)
                                {
                                   turdata = "** No meeting subject..**";
                                }
                                else
                                {
                                    turdata = fnStringConverterCodepage(appi.Subject);
                                }
                                if (appi.Duration.ToString() == null)
                                {
                                    durdata = "00:00";
                                }
                                else
                                {
                                    durdata = appi.Duration.ToString();
                                }
                                string location = string.Empty;
                                if (appi.Location.ToString() == null)
                                {
                                    location = "** No location information **";
                                }
                                appendText = appi.Organizer + delimiter + mbx + delimiter + appi.MyResponseType + delimiter + turdata + delimiter + appi.AppointmentType + delimiter + regatten + delimiter + location + delimiter + appi.Start + delimiter + appi.End + delimiter + regopt + delimiter + skype + delimiter + appi.Recurrence + delimiter + durdata + Environment.NewLine;
                                File.AppendAllText(path, appendText);

                            }
                        }

                    }

                    catch (Exception Ex)
                    {
                        m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        try
                        {
                            using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                            {
                                Log(Ex.Message, w);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                stopwatch.Stop();
                listView1.Items.Add("Elapsed time : " + stopwatch.Elapsed);
                stopwatch.Reset();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataColumn dc = new DataColumn("mbxname", typeof(String));
            dtmain.Columns.Add(dc);
            dtmain.Rows.Add("Mailbox");
            dtmain.Rows.Add(textBox5.Text);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                comboBox1.Enabled = true;
            }
            else
            {
                comboBox1.Enabled = false;
            }
            
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            delimiter = comboBox1.SelectedItem.ToString();
        }
    }
}

