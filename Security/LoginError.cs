using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rMap.Security
{
    public partial class LoginError : Form
    {
        public LoginError()
        {
            InitializeComponent();
        }

        public string Error
        {
            get { return lblError.Text; }
            set { lblError.Text = value; }
        }
    }
}
