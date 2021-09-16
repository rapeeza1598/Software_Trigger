using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO.Ports;
using System.Device.Location;

namespace Software_Trigger
{
    public partial class Form1 : Form
    {
        private int _ticks;
        private int tick_count;
        public string[] Data_Read { get; private set; }

        private double rs232_lat;
        private double rs232_long;
        private double lat_start;
        private double long_start;
        private double lat_end;
        private double long_end;
        private int time_check;

        public Form1()
        {
            InitializeComponent();
            
            try
            {
                serialPort_rs232.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open rs232 port");
            }
        }

        private void Start_Time_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.Equals(time_start.Text, "Start"))
                {

                    time_start.Text = "Stop";
                    Disable_textbox_time();
                    try
                    {
                        lat_start = Double.Parse(time_lat_start.Text);
                        long_start = Double.Parse(time_log_start.Text);

                        lat_end = Double.Parse(time_lat_end.Text);
                        long_end = Double.Parse(time_log_end.Text);

                        time_check = int.Parse(set_time_interval_box.Text);
                        timer1.Interval = time_check * 1000;
                        timer1.Start();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.ToString());
                        Enabled_textbox_time();
                        time_start.Text = "Start";
                        timer1.Stop();
                    }
                }
                else
                {
                    time_start.Text = "Start";

                    try
                    {
                        serialPort_rs232.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    Enabled_textbox_time();
                    timer1.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Enabled_textbox_time()
        {
            time_lat_start.Enabled = true;
            time_log_start.Enabled = true;
            time_lat_end.Enabled = true;
            time_log_end.Enabled = true;
            set_time_interval_box.Enabled = false;
        }

        private void Disable_textbox_time()
        {
            time_lat_start.Enabled = false;
            time_log_start.Enabled = false;
            time_lat_end.Enabled = false;
            time_log_end.Enabled = false;
            set_time_interval_box.Enabled = false;
        }

        /*TimeMode*/
        private void DataReceivedHandler_time(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string read_rs232;
            read_rs232 = sp.ReadLine();

            Data_Read = read_rs232.Split(',');

            rs232_lat = Double.Parse(Data_Read[2]);
            rs232_long = Double.Parse(Data_Read[4]);

            Console.WriteLine(Double.Parse(Data_Read[2]));
            Console.WriteLine(Double.Parse(Data_Read[4]));

            serialPort_rs232.Write(read_rs232);
            var point_start = new GeoCoordinate(latitude: lat_start, longitude: long_start);
            var point_check = new GeoCoordinate(latitude: rs232_lat, longitude: rs232_long);
            var point_end = new GeoCoordinate(latitude: lat_end, longitude: long_end);

            if (point_start.GetDistanceTo(point_check) < 300)
            {
                /*timer1.Start();*/
            }

            if (point_end.GetDistanceTo(point_check) < 300)
            {
                /*timer1.Stop();*/
            }

            /*Console.WriteLine(point_start.GetDistanceTo(point_check));*/
        }
        /*Mode Distance*/
        private void DataReceivedHandler_distance(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string read_rs232;
                read_rs232 = sp.ReadLine();

                Data_Read = read_rs232.Split(',');

                rs232_lat = Double.Parse(Data_Read[2]);
                rs232_long = Double.Parse(Data_Read[4]);

                Console.WriteLine(Double.Parse(Data_Read[2]));
                Console.WriteLine(Double.Parse(Data_Read[4]));
                serialPort_rs232.Write(read_rs232);

/*                var point_start = new GeoCoordinate(latitude: lat_start, longitude: long_end);
                var point_check = new GeoCoordinate(latitude: rs232_lat, longitude: rs232_long);

                if (point_start.GetDistanceTo(point_check) < 20)
                {
                    Trigger_Camera();
                }

                Console.WriteLine(point_start.GetDistanceTo(point_check));*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void Trigger_Camera()
        {
            try
            {
                serialPort_arduino.Open();
                serialPort_arduino.Write("1");
                serialPort_arduino.Close();
            }
            catch(Exception er)
            {
                Console.WriteLine("Can not write port");
            }
                
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _ticks++;
            Console.WriteLine(_ticks);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                /*https://stackoverflow.com/questions/16215741/c-sharp-read-only-serial-port-when-data-comes*/
                serialPort_rs232.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler_time);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open rs232 port");
            }
        }
    }
}
