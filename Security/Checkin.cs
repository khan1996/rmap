using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Net;
using System.Web;
using AlphA;

namespace rMap.Security
{
    class Checkin
    {
        /*
		 0 => "General error", 
		 1 => "Forum name not set", 
		 2 => "Invalid forum user or password", 
		 3 => "Application not found",
		 4 => "Application usage not allowed",
		 5 => "PC not same",
		 6 => "IP not same",
		 7 => "Password required",
		 8 => "All accounts locked",
		 9 => "IP locked under another account",
		10 => "PC locked under another account",
		11 => "Update needed"
         */

        private static int[] fatal_error = new []{ 0, 3, 6 };
        private static int[] expired_errors = new[] { 4 };
        private static VirtualMachine last_vm = null;
        private static bool firstTimeRun = true;

        public static DateTime? ExpiresAt;
        public static bool TimeAccount;
        public static long RemainingTimeOnAcc;
        public static bool IsTrial;
        private static IDrop drop;

        public static void Init()
        {
            drop = DropOpener.Load(Properties.Resources.hyljatud_jpg);
        }

        public static Form Load(string[] args)
        {
            Zalla.ISceneLoader loader = GetLoader();
            DisposeVM();

            if (loader == null)
                return null;

            return new rMapForm(args);
        }

        public static void DisposeVM()
        {
            if (last_vm == null)
                return;

            last_vm.Dispose();
            last_vm = null;
        }

        public static Zalla.ISceneLoader GetLoader()
        {
            Dictionary<string, string> keys = new Dictionary<string, string>();
            keys.Add("app", "rMap");
            keys.Add("version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            keys.Add("forum_password", "");
            keys.Add("forum_name", rMap.Properties.Settings.Default.Username);

            Dictionary<string, string> outValues = new Dictionary<string, string>();
            int error_code = 0;
            string user = null, pass = null, error = null;

            while (true)
            {
                if (string.IsNullOrEmpty(rMap.Properties.Settings.Default.Username))
                {
                    if (!RequestLogin(ref user, ref pass, error))
                        return null;

                    keys["forum_password"] = pass;
                    keys["forum_name"] = user;
                }

                bool success = false;
                try
                {
                    success = RequestSecurity(keys, out outValues, out error, out error_code);
                }
                catch (Exception ex)
                {
                    error_code = 0;
                    error = "Error connecting the auth server. Don't close this. Try again later. - " + ex.ToString();
                }

                if (success)
                    break;

                if (fatal_error.Contains(error_code))
                {
                    LoginError err = new LoginError();
                    err.Error = error;
                    err.ShowDialog();
                    return null;
                }
                else if (expired_errors.Contains(error_code) && !firstTimeRun)
                {
                    DialogResult res = MessageBox.Show("License expired. If you wish to continue extend your subscription and click OK. Otherways click Cancel.", "rMap", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    if (res == DialogResult.Cancel)
                        return null;
                }
                else
                {
                    rMap.Properties.Settings.Default.Username = null;
                }
            }

            firstTimeRun = false;

            ExpiresAt = outValues["ExpiresAt"].ToLong() > 0 ? (DateTime?)UnixTimeStampToDateTime(outValues["ExpiresAt"].ToLong()) : null;
            TimeAccount = outValues["GTAcc"].ToBool();
            RemainingTimeOnAcc = outValues["FTime"].ToLong();
            IsTrial = outValues["Trial"].ToBool();

            rMap.Properties.Settings.Default.Username = keys["forum_name"];
            rMap.Properties.Settings.Default.Save();

            last_vm = new VirtualMachine() { Code = outValues["Loader"] };
            return last_vm.Load<Zalla.ISceneLoader>("rMap.Zalla.SceneLoader");
        }

        private static bool RequestSecurity(Dictionary<string, string> keys, out Dictionary<string, string> ret, out string error, out int error_code)
        {
            var post = drop.Implode(keys);

            WebClient wc = new WebClient();

            byte[] status = wc.UploadValues(drop.GetUrl(), post);

            if (status.StartsWith("OK:"))
            {
                error = null;
                error_code = 0;

                byte[] bit = new byte[status.Length - 3];
                Array.Copy(status, 3, bit, 0, status.Length - 3);
                ret = drop.Explode(bit);

                return true;
            }
            else if (status.StartsWith("ERR:"))
            {
                string[] splits = Encoding.ASCII.GetString(status).Split(':');

                if (splits.Length != 3)
                    throw new Exception("Error in error message");

                error = splits[2];
                error_code = int.Parse(splits[1]);
                ret = null;

                return false;
            }
            else
                throw new Exception("Unknown reply");
        }

        private static bool RequestLogin(ref string user, ref string pass, string error = null)
        {
            Login frm = new Login();

            frm.Username = user;
            frm.Password = pass;
            frm.Error = error;

            if (frm.ShowDialog() == DialogResult.OK)
            {
                user = frm.Username;
                pass = frm.Password;

                return true;
            }
            else
                return false;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimeStamp).ToLocalTime();
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
