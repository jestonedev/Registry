using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Reporting
{
    public partial class ActPremiseExtInfoForm : Form
    {
        public bool HasAqueduct
        {
            get { return checkBoxHasAqueduct.Checked; }
        }

        public bool HasHotWater
        {
            get { return checkBoxHasHotWater.Checked; }
        }

        public bool HasSewerage
        {
            get { return checkBoxHasSewerage.Checked; }
        }

        public bool HasLighting
        {
            get { return checkBoxHasLighting.Checked; }
        }

        public bool HasChute
        {
            get { return checkBoxHasChute.Checked; }
        }

        public bool HasRadio
        {
            get { return checkBoxHasRadio.Checked; }
        }

        public bool HasHeating
        {
            get { return checkBoxHasHeating.Checked; }
        }

        public int HeatingType
        {
            get {
                if (radioButtonStoveHeating.Checked)
                    return 1;
                else
                    if (radioButtonLocalHeating.Checked)
                        return 2;
                    else
                        if (radioButtonCentralHeating.Checked)
                            return 3;
                        else 
                            return 0;

            }
        }

        public ActPremiseExtInfoForm()
        {
            InitializeComponent();
        }

        private void checkBoxHasHeating_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonCentralHeating.Enabled = radioButtonLocalHeating.Enabled = radioButtonStoveHeating.Enabled = checkBoxHasHeating.Checked;
        }
    }
}
