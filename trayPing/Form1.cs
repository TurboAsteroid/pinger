using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using System.IO;

namespace trayPing
{
    public partial class Form1 : Form
    {
        private void changeStatus(System.String status)
        {
            switch (status)
            {
                case "Error":
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                    notifyIcon1.Icon = Properties.Resources.red;
                    break;
                case "Success":
                    notifyIcon1.Text = "Все сервера доступны";
                    notifyIcon1.Icon = Properties.Resources.green;
                    break;
                case "Start":
                    button2.Text = "Stop";
                    break;
                default:
                    notifyIcon1.Text = "Waiting for ping start";
                    notifyIcon1.Icon = Properties.Resources.gray;
                    button2.Text = "Ping";
                    break;
            }
        }

        private void pingList()
        {
            foreach (ListViewItem element in listView1.Items)
            {
                bool result = tryping(element.SubItems[0].Text);
            }
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            bool pinging = false;
            System.String host = e.UserState.ToString();
            if (e.Cancelled)
            {
                //MessageBox.Show("Cancelled", "Cancelled");
            }
            else if (e.Error != null)
            {
                //MessageBox.Show(e.Error.ToString(), "Error");
            }
            else if (e.Reply.Status == IPStatus.Success)
            {
                //MessageBox.Show(host, "good");
                pinging = true;
            }
            else
            {
                //MessageBox.Show("alarm", "bad");
            }

            foreach (ListViewItem element in listView1.Items)
            {
                if (element.SubItems[0].Text == host)
                {
                    element.SubItems[1].Text = "" + pinging;

                    if (!pinging)
                    {
                        //notifyIcon1.BalloonTipTitle = "Внимание!";
                        //notifyIcon1.BalloonTipText = "Нет связи с " + host + "!";
                        //notifyIcon1.ShowBalloonTip(1000);
                    }
                }
            }
        }

        private bool tryping(System.String host)
        {
            bool pinging = false;
            
            try
            {
                Ping pingSender = new Ping();

                AutoResetEvent waiter = new AutoResetEvent(false);
                pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
                pingSender.SendAsync(host, host);
            }
            catch
            {
                //return false;
            }
            

            return pinging;
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                changeStatus("Default");
                timer1.Stop();
                timer2.Stop();
            }
            else
            {
                changeStatus("Start");
                pingList();
                timer1.Start();
                timer2.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pingList();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = textBox1.Text;
            lvi.SubItems.Add("");
            listView1.Items.Add(lvi);
        }

        private void removeFromListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0) {
                listView1.Items.Remove(listView1.SelectedItems[0]);
            }
        }

        private void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
            } else
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Minimized;
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           try {
                var logFile = File.ReadAllLines("\\\\elem-dc0\\SYSVOL\\elem.ru\\scripts\\pinger\\pinger.cfg");
                List<string> LogList = new List<string>(logFile);

                foreach (System.String host in LogList)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = host;
                    lvi.SubItems.Add("");
                    listView1.Items.Add(lvi);
                }

                this.Hide();
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Minimized;
                changeStatus("Start");
                pingList();
                timer1.Start();
                timer2.Start();
            }
            catch {
                
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //notifyIcon1.BalloonTipTitle = "Программа была спрятана";
                //notifyIcon1.BalloonTipText = "Обратите внимание что программа была спрятана в трей и продолжит свою работу.";
                //notifyIcon1.ShowBalloonTip(1000);
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            bool status = true;
            System.String noping = "Нет связи с ";
            foreach (ListViewItem element in listView1.Items)
            {
                if (element.SubItems[1].Text == "False")
                {
                    status = false;
                    noping += element.SubItems[0].Text + ", ";
                }
            }
            if (status) {
                changeStatus("Success");
            } else
            {
                changeStatus("Error");
                notifyIcon1.Text = noping;
            }
        }
    }
}
