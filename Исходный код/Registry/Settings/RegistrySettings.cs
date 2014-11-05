using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry
{
    public static class RegistrySettings
    {
        public static string ConnectionString {
            get {
                return Settings.Properties.Settings.Default.ConnectionString;
            }
        }

        public static string LDAPUserName
        {
            get
            {
                return Settings.Properties.Settings.Default.LDAPUserName;
            }
        }

        public static string LDAPEncryptedPassword
        {
            get
            {
                string encrypted_password = Settings.Properties.Settings.Default.LDAPEncryptedPassword;
                string decrypted_password = encrypted_password;
                return decrypted_password;
            }
        }

        public static string ActivityManagerPath
        {
            get
            {
                return Settings.Properties.Settings.Default.ActivityManagerPath;
            }
        }

        public static string ActivityManagerOutputCodepage
        {
            get
            {
                return Settings.Properties.Settings.Default.ActivityManagerOutputCodepage;
            }
        }

        public static string ActivityManagerConfigsPath
        {
            get
            {
                return Settings.Properties.Settings.Default.ActivityManagerConfigsPath;
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
        }
    }
}
