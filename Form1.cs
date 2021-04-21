using Prac4.FlyAPI;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Prac4
{
    public partial class Form1 : Form
    {
        FlightXML2SoapClient FlyInformator = new FlightXML2SoapClient();
        public Form1()
        {
            Cursor.Current = Cursors.AppStarting;
            InitializeComponent();
            FlyInformator.ClientCredentials.UserName.UserName = "DunaOne";
            FlyInformator.ClientCredentials.UserName.Password = "5e662b11de4745e744970c5b3fb94cfa578208f8";
            FlyInformator.SetMaximumResultSize(1);
            Cursor.Current = Cursors.Default;
        }

        public Image StringToImage(string base64String)
        {
            if (String.IsNullOrWhiteSpace(base64String))
                return null;

            var bytes = Convert.FromBase64String(base64String);
            var stream = new MemoryStream(bytes);
            return Image.FromStream(stream);

        }

        public void FlyInfoToString(FlightStruct flightStruct)
        {
            listView1.Items.Add("Aircraft Type: " + flightStruct.aircrafttype);
            listView1.Items.Add("Ident: " + flightStruct.ident);
            listView1.Items.Add("Destination: " + flightStruct.destination);
            listView1.Items.Add("Origin: " + flightStruct.origin);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                listView1.Items.Clear();
                FlyInfoToString(FlyInformator.FlightInfo(textBox1.Text, 1).flights[0]);
                pictureBox1.Image = StringToImage(FlyInformator.MapFlight(textBox1.Text, 500, 500));
            }
            catch (Exception exp)
            {
                MessageBox.Show("Некорректный ввод", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cursor.Current = Cursors.Default;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                TrackStruct[] tracks = FlyInformator.GetLastTrack(textBox1.Text);

                DateTime dt = TimeZoneInfo.ConvertTimeToUtc(dateTimePicker1.Value);
                DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan tsInterval = dt.Subtract(dt1970);
                Int32 iSeconds = Convert.ToInt32(tsInterval.TotalSeconds);

                for (int i = 1; i < tracks.Length; i++)
                {
                    if (tracks[i].timestamp > iSeconds && tracks[i - 1].timestamp < iSeconds)
                    {
                        listView1.Items.Add("Широта: " + tracks[i].latitude);
                        listView1.Items.Add("Долгота: " + tracks[i].longitude);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Некорректный ввод", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
