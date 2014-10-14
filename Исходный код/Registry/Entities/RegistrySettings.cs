using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.Entities.Properties;

namespace Registry.Entities
{
    public static class RegistrySettings
    {
        public static string ConnectionString {
            get {
                return Settings.Default["ConnectionString"].ToString();
            }
        }

        public static string LDAPUserName
        {
            get
            {
                return Settings.Default["LDAPUserName"].ToString();
            }
        }

        public static string LDAPEncryptedPassword
        {
            get
            {
                string encrypted_password = Settings.Default["LDAPEncryptedPassword"].ToString();
                string decrypted_password = encrypted_password;
                return decrypted_password;
            }
        }
    }
}
