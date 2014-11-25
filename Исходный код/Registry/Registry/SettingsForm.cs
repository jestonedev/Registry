using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            textBoxLDAPPassword.Text = RegistrySettings.LDAPPassword;
            textBoxLDAPUserName.Text = RegistrySettings.LDAPUserName;
            numericUpDownMaxDBConnectionCount.Value = RegistrySettings.MaxDBConnectionCount;
            numericUpDownDataModelsCallbackUpdateTimeout.Value = RegistrySettings.DataModelsCallbackUpdateTimeout;
            numericUpDownCalcDataModelsUpdateTimeout.Value = RegistrySettings.CalcDataModelsUpdateTimeout;
            checkBoxUseLDAP.Checked = RegistrySettings.UseLDAP;
        }

        private void vButton2_Click(object sender, EventArgs e)
        {
            RegistrySettings.ActivityManagerConfigsPath = textBoxActivityManagerConfigsPath.Text;
            RegistrySettings.ActivityManagerOutputCodePage = textBoxActivityManagerOutputCodepage.Text;
            RegistrySettings.ActivityManagerPath = textBoxActivityManagerPath.Text;
            RegistrySettings.ConnectionString = textBoxConnectionString.Text;
            RegistrySettings.LDAPPassword = textBoxLDAPPassword.Text;
            RegistrySettings.LDAPUserName = textBoxLDAPUserName.Text;
            RegistrySettings.MaxDBConnectionCount = Convert.ToInt32(numericUpDownMaxDBConnectionCount.Value);
            RegistrySettings.DataModelsCallbackUpdateTimeout = Convert.ToInt32(numericUpDownDataModelsCallbackUpdateTimeout.Value);
            RegistrySettings.CalcDataModelsUpdateTimeout = Convert.ToInt32(numericUpDownCalcDataModelsUpdateTimeout.Value);
            RegistrySettings.UseLDAP = checkBoxUseLDAP.Checked;
            RegistrySettings.Save();
            Close();
        }
    }
}
