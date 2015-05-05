using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyActReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            using (ActPremiseExtInfoForm form = new ActPremiseExtInfoForm())
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\act.xml"));
                    arguments.Add("connectionString", RegistrySettings.ConnectionString);
                    arguments.Add("aqueduct", form.HasAqueduct ? "$u$водопроводом$/u$" : "водопроводом");
                    arguments.Add("hot_water", form.HasHotWater ? "$u$горячим водоснабжением$/u$" : "горячим водоснабжением");
                    arguments.Add("sewerage", form.HasSewerage ? "$u$канализацией$/u$" : "канализацией");
                    arguments.Add("lighting", form.HasLighting ? "$u$электроосвещением$/u$" : "электроосвещением");
                    arguments.Add("chute", form.HasChute ? "$u$мусоропроводом$/u$" : "мусоропроводом");
                    arguments.Add("radio", form.HasRadio ? "$u$радиотрансляционной сетью$/u$" : "радиотрансляционной сетью");
                    arguments.Add("heating", form.HasHeating ? "$u$отоплением$/u$" : "отоплением");
                    arguments.Add("stove_heating", form.HeatingType == 1 ? "$u$печным$/u$" : "печным");
                    arguments.Add("local_heating", form.HeatingType == 2 ? "$u$местным$/u$" : "местным");
                    arguments.Add("central_heating", form.HeatingType == 3 ? "$u$центральным$/u$" : "центральным");
                    base.Run(arguments);
                }
                else
                    base.Cancel();
            }
        }
    }
}
