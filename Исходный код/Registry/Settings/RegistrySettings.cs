using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Registry
{
    public static class RegistrySettings
    {

        public static string ConnectionString {
            get {
                return Settings.Properties.Settings.Default.ConnectionString;
            }
            set
            {
                Settings.Properties.Settings.Default.ConnectionString = value;
            }
        }

        public static string LDAPUserName
        {
            get
            {
                return Settings.Properties.Settings.Default.LDAPUserName;
            }
            set
            {
                Settings.Properties.Settings.Default.LDAPUserName = value;
            }
        }

        public static string LDAPPassword
        {
            get
            {
                string encrypted_password = Settings.Properties.Settings.Default.LDAPEncryptedPassword;
                string decrypted_password = encrypted_password;
                return decrypted_password;
            }
            set
            {
                Settings.Properties.Settings.Default.LDAPEncryptedPassword = value;
            }
        }

        public static string ActivityManagerPath
        {
            get
            {
                return Settings.Properties.Settings.Default.ActivityManagerPath;
            }
            set
            {
                Settings.Properties.Settings.Default.ActivityManagerPath = value;
            }
        }

        public static string ActivityManagerOutputCodepage
        {
            get
            {
                return Settings.Properties.Settings.Default.ActivityManagerOutputCodepage;
            }
            set
            {
                Settings.Properties.Settings.Default.ActivityManagerOutputCodepage = value;
            }
        }

        public static string ActivityManagerConfigsPath
        {
            get
            {
                return Settings.Properties.Settings.Default.ActivityManagerConfigsPath;
            }
            set
            {
                Settings.Properties.Settings.Default.ActivityManagerConfigsPath = value;
            }
        }

        public static int MaxDBConnectionCount
        {
            get
            {
                try
                {
                    return Settings.Properties.Settings.Default.MaxDBConnectionCount;
                }
                catch
                {
                    return 10;
                }
            }
            set
            {
                Settings.Properties.Settings.Default.MaxDBConnectionCount = value;
            }
        }

        public static void Save()
        {
            Settings.Properties.Settings.Default.Save();
        }
    }
}
