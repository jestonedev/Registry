using System;
using System.Windows.Forms;
using Settings;

namespace Registry
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            textBoxActivityManagerConfigsPath.Text = RegistrySettings.ActivityManagerConfigsPath;
            textBoxActivityManagerOutputCodepage.Text = RegistrySettings.ActivityManagerOutputCodePage;
            textBoxActivityManagerPath.Text = RegistrySettings.ActivityManagerPath;
            textBoxConnectionString.Text = RegistrySettings.ConnectionString;
            textBoxLDAPPassword.Text = RegistrySettings.LdapPassword;
            textBoxLDAPUserName.Text = RegistrySettings.LdapUserName;
            numericUpDownMaxDBConnectionCount.Value = RegistrySettings.MaxDbConnectionCount;
            numericUpDownDataModelsCallbackUpdateTimeout.Value = RegistrySettings.DataModelsCallbackUpdateTimeout;
            numericUpDownCalcDataModelsUpdateTimeout.Value = RegistrySettings.CalcDataModelsUpdateTimeout;
            checkBoxUseLDAP.Checked = RegistrySettings.UseLdap;
        }

        private void vButton2_Click(object sender, EventArgs e)
        {
            RegistrySettings.ActivityManagerConfigsPath = textBoxActivityManagerConfigsPath.Text;
            RegistrySettings.ActivityManagerOutputCodePage = textBoxActivityManagerOutputCodepage.Text;
            RegistrySettings.ActivityManagerPath = textBoxActivityManagerPath.Text;
            RegistrySettings.ConnectionString = textBoxConnectionString.Text;
            RegistrySettings.LdapPassword = textBoxLDAPPassword.Text;
            RegistrySettings.LdapUserName = textBoxLDAPUserName.Text;
            RegistrySettings.MaxDbConnectionCount = Convert.ToInt32(numericUpDownMaxDBConnectionCount.Value);
            RegistrySettings.DataModelsCallbackUpdateTimeout = Convert.ToInt32(numericUpDownDataModelsCallbackUpdateTimeout.Value);
            RegistrySettings.CalcDataModelsUpdateTimeout = Convert.ToInt32(numericUpDownCalcDataModelsUpdateTimeout.Value);
            RegistrySettings.UseLdap = checkBoxUseLDAP.Checked;
            RegistrySettings.Save();
            Close();
        }
    }
}
