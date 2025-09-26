using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rMap
{
    public partial class LogDetailView : Form
    {
        public static LogDetailView Instance = new LogDetailView();

        public LogDetailView()
        {
            InitializeComponent();

            Hide();
        }

        public void AddInfo(string info)
        {
            if (txtInfo.Text != "")
                txtInfo.Text += "\r\n\r\n";

            txtInfo.Text += info;

            if (!Visible)
                Show();
        }

        private void LogDetailView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                txtInfo.Clear();
                e.Cancel = true;
                Hide();
            }
        }
    }
}
