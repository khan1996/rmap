using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rMap
{
    [System.ComponentModel.DefaultEvent("Changed")]
    public partial class CryptoPanel : UserControl
    {
        private static Color OkColor = SystemColors.Window;
        private static Color ErrorColor = Color.LightPink;

        public event Action<CryptoPanel, byte[]> Changed;

        public CryptoPanel()
        {
            InitializeComponent();
            cmbType.SelectedIndex = 0;
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            ValidateInner();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateInner();
        }

        private bool ValidateCnt()
        {
            try
            {
                byte[] d = Crypto;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ValidateInner()
        {
            bool ok = ValidateCnt();
            txtValue.BackColor = ok ? OkColor : ErrorColor;

            if (ok && Changed != null)
                Changed(this, Crypto);
        }

        public byte[] Crypto
        {
            get
            {
                if (string.IsNullOrWhiteSpace(txtValue.Text))
                    return new byte[] { };

                if (CryptoType == 0)
                {
                    byte[] d = Encoding.ASCII.GetBytes(txtValue.Text.Trim());
                    string test = Encoding.ASCII.GetString(d);

                    if (txtValue.Text.Trim() != test)
                        throw new Exception("Unknown characters used");

                    return d;
                }
                else
                {
                    string line = txtValue.Text.Trim();

                    if (line.Length > 2 && line.Substring(0, 2).ToLower() == "0x")
                        line = line.Substring(2);

                    byte[] d;
                    if (line.Length > 2 && line[2] == ' ')
                        d = line.Split(' ').Select(x => Convert.ToByte(x, 16)).ToArray();
                    else
                    {
                        if (line.Length % 2 != 0)
                            line = "0" + line;

                        d = new byte[line.Length / 2];
                        for (int i = 0; i < line.Length / 2; i++)
                            d[i] = Convert.ToByte(line.Substring(i * 2, 2), 16);
                    }
                    return d;
                }
            }
            set
            {
                if (value == null)
                    txtValue.Text = null;
                else if (CryptoType == 0)
                {
                    txtValue.Text = Encoding.ASCII.GetString(value);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < value.Length; i++)
                    {
                        sb.Append(string.Format("{0:X2}", value[i]));

                        if (i != value.Length - 1)
                            sb.Append(' ');
                    }
                    txtValue.Text = sb.ToString();
                }
            }
        }

        public static byte[] GetFromSerialized(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 1)
                return null;
            else if (value.Length < 2 || value[1] != ',')
                return Encoding.ASCII.GetBytes(value);
            else
                return Convert.FromBase64String(value.Substring(2));
        }

        public string CryptoSerialized
        {
            get
            {
                return CryptoType.ToString() + "," + Convert.ToBase64String(Crypto);
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 1)
                {
                    CryptoType = 0;
                    Crypto = null;
                }
                else
                {
                    if (value.Length > 1 && value[1] != ',')
                    {
                        CryptoType = 0;
                        Crypto = Encoding.ASCII.GetBytes(value);
                    }
                    else
                    {
                        CryptoType = int.Parse(value.Substring(0, 1));
                        Crypto = Convert.FromBase64String(value.Substring(2));
                    }
                }
            }
        }

        /// <summary>
        /// 0 - string
        /// 1 - hex
        /// </summary>
        public int CryptoType
        {
            get
            {
                return cmbType.SelectedIndex < 0 ? 0 : cmbType.SelectedIndex;
            }
            set
            {
                cmbType.SelectedIndex = value;
            }
        }
    }
}
