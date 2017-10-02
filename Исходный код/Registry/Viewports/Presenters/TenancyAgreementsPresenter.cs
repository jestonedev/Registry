using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Declensions.Unicode;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ModalEditors;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class TenancyAgreementsPresenter: Presenter
    {

        public enum ExtModificationTypes
        {
            CommercialProlong,
            SpecialProlong,
            ChangeTenant
        }

        public sealed class ExtModificationParameter
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        // Auto modify properties
        private List<TenancyPerson> ExcludePersons { get; set; }
        private List<TenancyPerson> IncludePersons { get; set; }
        private List<Dictionary<ExtModificationTypes, List<ExtModificationParameter>>> Modifications { get; set; }

        public TenancyAgreementsPresenter() : base(new TenancyAgreementsViewModel(), null, null)
        {
            Modifications = new List<Dictionary<ExtModificationTypes, List<ExtModificationParameter>>>();
            IncludePersons = new List<TenancyPerson>();
            ExcludePersons = new List<TenancyPerson>();
        }

        public void InitExtendedViewModelItems(string staticFilter)
        {
            if (ParentRow == null)
            {
                throw new ViewportException("Не указан ParentRow");
            }
            ViewModel.AddViewModeItem("persons_exclude", new ViewModelItem(EntityDataModel<TenancyPerson>.GetInstance()));
            ViewModel.AddViewModeItem("persons_change_tenant", new ViewModelItem(EntityDataModel<TenancyPerson>.GetInstance()));
            ViewModel.AddViewModeItem("tenant_change_tenant", new ViewModelItem(EntityDataModel<TenancyPerson>.GetInstance()));
            ViewModel["persons_exclude"].BindingSource.Filter = staticFilter;
            ViewModel["persons_change_tenant"].BindingSource.Filter = staticFilter + " AND (id_kinship <> 1 OR exclude_date is not null)";
            ViewModel["tenant_change_tenant"].BindingSource.Filter = staticFilter + " AND (id_kinship = 1 AND exclude_date is null)";
        }

        public string WarrantStringById(int idWarrant)
        {
            var bindingSource = ViewModel["warrants"].BindingSource;
            var rowIndex = bindingSource.Find("id_warrant", idWarrant);
            if (rowIndex == -1)
                return null;
            var row = (DataRowView)bindingSource[rowIndex];
            var registrationDate = Convert.ToDateTime(row["registration_date"], CultureInfo.InvariantCulture);
            var registrationNum = row["registration_num"].ToString();
            return string.Format(CultureInfo.InvariantCulture, "№ {0} от {1}",
                registrationNum, registrationDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
        }

        public bool DeleteRecord()
        {
            var row = ViewModel["general"].CurrentRow;
            var columnName = ViewModel["general"].PrimaryKeyFirst;
            if (ViewModel["general"].Model.Delete((int)row[columnName]) == -1)
                return false;
            row.Delete();
            return true;
        }

        public bool InsertRecord(TenancyAgreement tenancyAgreement)
        {
            var idAgreement = ViewModel["general"].Model.Insert(tenancyAgreement);
            if (idAgreement == -1)
            {
                return false;
            }
            tenancyAgreement.IdAgreement = idAgreement;
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<TenancyAgreement>.FillRow(tenancyAgreement, row);
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        public bool UpdateRecord(TenancyAgreement tenancyAgreement)
        {
            if (tenancyAgreement.IdAgreement == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить соглашение без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(tenancyAgreement) == -1)
                return false;
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<TenancyAgreement>.FillRow(tenancyAgreement, row);
            return true;
        }

        private void Prolong(List<ExtModificationParameter> parameters)
        {
            var beginDate = parameters.Where(v => v.Name == "ProlongFrom").Select(v => (DateTime?)v.Value).FirstOrDefault();
            var endDate = parameters.Where(v => v.Name == "ProlongTo").Select(v => (DateTime?)v.Value).FirstOrDefault();
            var untilDismissal = parameters.Where(v => v.Name == "UntilDismissal").Select(v => (bool?)v.Value).FirstOrDefault() ?? false;

            var rentPeriod = new TenancyRentPeriod
            {
                IdProcess = (int)ParentRow["id_process"],
                BeginDate = ViewportHelper.ValueOrNull<DateTime>(ParentRow, "begin_date"),
                EndDate = ViewportHelper.ValueOrNull<DateTime>(ParentRow, "end_date"),
                UntilDismissal = ViewportHelper.ValueOrNull<bool>(ParentRow, "until_dismissal"),
            };
            var rentPeriods = EntityDataModel<TenancyRentPeriod>.GetInstance();
            rentPeriods.EditingNewRecord = true;
            var idRentPeriod = rentPeriods.Insert(rentPeriod);
            if (idRentPeriod == -1) return;
            rentPeriod.IdRentPeriod = idRentPeriod;
            rentPeriods.Select().Rows.Add(idRentPeriod, rentPeriod.IdProcess, rentPeriod.BeginDate, rentPeriod.EndDate, rentPeriod.UntilDismissal);
            rentPeriods.EditingNewRecord = false;

            var tenancyProcess = EntityConverter<TenancyProcess>.FromRow(ParentRow);
            tenancyProcess.BeginDate = beginDate;
            tenancyProcess.EndDate = endDate;
            tenancyProcess.UntilDismissal = untilDismissal;
            var tenancyProcesses = EntityDataModel<TenancyProcess>.GetInstance();
            tenancyProcesses.EditingNewRecord = true;
            if (tenancyProcesses.Update(tenancyProcess) == -1)
            {
                return;
            }
            ParentRow["begin_date"] = ViewportHelper.ValueOrDbNull(beginDate);
            ParentRow["end_date"] = ViewportHelper.ValueOrDbNull(endDate);
            ParentRow["until_dismissal"] = untilDismissal;
            tenancyProcesses.EditingNewRecord = false;
        }

        private void ExtModificationsExecute()
        {
            foreach (var modification in Modifications)
            {
                foreach (var modificationPair in modification)
                {
                    switch (modificationPair.Key)
                    {
                        case ExtModificationTypes.ChangeTenant:
                            ChangeTenant(modificationPair.Value);
                            break;
                        case ExtModificationTypes.CommercialProlong:
                        case ExtModificationTypes.SpecialProlong:
                            Prolong(modificationPair.Value);
                            break;
                    }
                }
            }
            ClearModifyState();
        }

        private static void ChangeTenant(List<ExtModificationParameter> parameters)
        {
            var idOldTenant = parameters.Where(v => v.Name == "IdOldTenant").Select(v => (int?)v.Value).FirstOrDefault();
            var idKinshipOldTenant = parameters.Where(v => v.Name == "IdKinshipOldTenant").Select(v => (int?)v.Value).FirstOrDefault();
            var idNewTenant = parameters.Where(v => v.Name == "IdNewTenant").Select(v => (int?)v.Value).FirstOrDefault();
            var excludeTenant = parameters.Where(v => v.Name == "ExcludeTenant").Select(v => (bool?)v.Value).FirstOrDefault();
            if (idOldTenant == null || idNewTenant == null) return;
            var oldTenantRow =
                EntityDataModel<TenancyPerson>.GetInstance()
                    .FilterDeletedRows().FirstOrDefault(v => v.Field<int>("id_person") == idOldTenant.Value);
            var newTenantRow =
                EntityDataModel<TenancyPerson>.GetInstance()
                    .FilterDeletedRows().FirstOrDefault(v => v.Field<int>("id_person") == idNewTenant.Value);
            var oldTenant = oldTenantRow != null ? EntityConverter<TenancyPerson>.FromRow(oldTenantRow) : null;
            var newTenant = newTenantRow != null ? EntityConverter<TenancyPerson>.FromRow(newTenantRow) : null;

            if (oldTenant == null || newTenant == null)
            {
                MessageBox.Show(@"Произошла непредвиденная ошибка при изменении данных об участниках найма",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            if (excludeTenant == true)
            {
                using (var form = new PersonExcludeDateForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        oldTenant.ExcludeDate = form.ExcludeDate;
                    }
                }
            }
            else
            {
                oldTenant.IdKinship = idKinshipOldTenant;
            }
            var affected = EntityDataModel<TenancyPerson>.GetInstance().Update(oldTenant);
            if (affected == -1)
            {
                return;
            }
            oldTenantRow.BeginEdit();
            oldTenantRow["id_kinship"] = ViewportHelper.ValueOrDbNull(oldTenant.IdKinship);
            oldTenantRow["exclude_date"] = ViewportHelper.ValueOrDbNull(oldTenant.ExcludeDate);
            oldTenantRow.EndEdit();

            newTenant.IdKinship = 1;
            newTenant.ExcludeDate = null;
            affected = EntityDataModel<TenancyPerson>.GetInstance().Update(newTenant);
            if (affected == -1)
            {
                return;
            }
            newTenantRow.BeginEdit();
            newTenantRow["id_kinship"] = ViewportHelper.ValueOrDbNull(newTenant.IdKinship);
            newTenantRow["exclude_date"] = ViewportHelper.ValueOrDbNull(newTenant.ExcludeDate);
            newTenantRow.EndEdit();
        }

        public void ClearModifyState()
        {

            IncludePersons.Clear();
            ExcludePersons.Clear();
            Modifications.Clear();
        }

        private static int GetNextPointHeaderNumber(string content)
        {
            var contentList = content.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            var lastValue = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                var match = Regex.Match(contentList[i], "^\u200B?\\s*([0-9]+)\\s*[)]");
                if (match.Success && match.Groups.Count > 1)
                {
                    int.TryParse(match.Groups[1].Value, out lastValue);
                }
            }
            return lastValue + 1;
        }

        private static bool IsHeaderInserted(string content, string headerWildcard)
        {
            var contentList = content.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            return contentList.Any(line => Regex.IsMatch(line, headerWildcard));
        }

        private static int GetIndexForInsert(string content, string headerWildcard)
        {
            var contentList = content.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            var headerIndex = -1;
            var lastPointIndex = -1;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], headerWildcard))
                {
                    headerIndex = i;
                }
                else
                    if (headerIndex != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ исключить|\u200B.*пункт .+ дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        lastPointIndex = i;
                        break;
                    }
            }
            return lastPointIndex;
        }

        public string ExplainContentModifier(string content, string generalPoint, string point, string explainContent)
        {
            if (!string.IsNullOrEmpty(point.Trim()) && !string.IsNullOrEmpty(generalPoint.Trim()))
            {
                point = string.Format("подпункт {0} пункта {1}", point.Trim(), generalPoint.Trim());
            }
            else if (!string.IsNullOrEmpty(point.Trim()))
            {
                point = string.Format("подпункт {0}", point.Trim());
            }
            else
            {
                point = string.Format("пункт {0}", generalPoint.Trim());
            }
            var element = string.Format(CultureInfo.InvariantCulture, "\u200B{2}) изложить {0} в новой редакции:\r\n«{1}».", point, explainContent.Trim(), GetNextPointHeaderNumber(content));
            return content + Environment.NewLine + element;
        }

        public string IncludePersonsContentModifier(string content, string generalPoint, string point, string surname, string name,
            string patronymic, string kinship, DateTime dateOfBirth)
        {
            var contentList = content.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            var snp = (surname + " " + name + " " + patronymic).Trim();
            var element = string.Format(CultureInfo.InvariantCulture, "«{0}. {1}, {2} - {3} г.р.»;", point,
                snp, kinship, dateOfBirth.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            if (kinship == "наниматель")
            {
                var gender = Declension.GetGender(patronymic);
                contentList.Add(string.Format("\u200B{4}) считать по договору № {0} от {1} нанимателем - «{2} - {3} г.р.»;",
                    ParentRow["registration_num"],
                    GetFormatedRegistrationDate(),
                    gender == Gender.NotDefind ? snp : Declension.GetSNPDeclension(snp, gender, DeclensionCase.Vinit),
                    dateOfBirth.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    GetNextPointHeaderNumber(content)));
            }
            else
            {
                var headerWildcard = string.Format("^\u200B.*пункт {0} договора дополнить", generalPoint);
                if (!IsHeaderInserted(content, headerWildcard))
                {
                    contentList.Add(string.Format("\u200B{2}) пункт {0} договора дополнить подпунктом {1} следующего содержания:",
                        generalPoint, point, GetNextPointHeaderNumber(content)));
                }

                var lastPointIndex = GetIndexForInsert(content, headerWildcard);
                if (lastPointIndex == -1)
                    contentList.Add(element);
                else
                    contentList.Insert(lastPointIndex, element);
            }
            return contentList.Aggregate((v, acc) => v + "\r\n" + acc);
        }


        public string ExcludePersonsContentModifier(string content, string generalPoint, string point)
        {
            var tenancyPerson = ViewModel["persons_exclude"].CurrentRow;

            var kinship = tenancyPerson["id_kinship"] != DBNull.Value ?
                ((DataRowView)ViewModel["kinships"].BindingSource[
                ViewModel["kinships"].BindingSource.Find("id_kinship", tenancyPerson["id_kinship"])])["kinship"].ToString() : "";
            var element = string.Format(CultureInfo.InvariantCulture, "«{0}. {1} {2} {3} - {4}, {5} г.р.»;", point,
                tenancyPerson["surname"],
                tenancyPerson["name"],
                tenancyPerson["patronymic"],
                kinship,
                tenancyPerson["date_of_birth"] != DBNull.Value ?
                    Convert.ToDateTime(tenancyPerson["date_of_birth"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");
            var contentList = content.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            var headerWildcard = string.Format("^\u200B.*из пункта {0} договора исключить", generalPoint);
            if (!IsHeaderInserted(content, headerWildcard))
            {
                contentList.Add(string.Format("\u200B{2}) из пункта {0} договора исключить подпункт {1} следующего содержания:",
                    generalPoint, point, GetNextPointHeaderNumber(content)));
            }
            var lastPointIndex = GetIndexForInsert(content, headerWildcard);
            if (lastPointIndex == -1)
                contentList.Add(element);
            else
                contentList.Insert(lastPointIndex, element);
            return contentList.Aggregate((v, acc) => v + "\r\n" + acc);
        }

        public string ChangeTenancyStringBuilder()
        {
            var tenantChangeTenant = ViewModel["tenant_change_tenant"].BindingSource;
            var personsChangeTenant = ViewModel["persons_change_tenant"].BindingSource;
            var registrationNumber = ParentRow["registration_num"];

            var result = string.Format(CultureInfo.InvariantCulture,
                "1.1. По настоящему Соглашению Стороны по договору № {0} от {1} {2} найма жилого помещения, расположенного по адресу: Российская Федерация, {3}, ",
                registrationNumber, GetFormatedRegistrationDate(), GetTenancyRent(), GetTenancyAddress());

            // Исключаем старого нанимателя
            var oldTenantRow = (DataRowView)tenantChangeTenant[0];
            var snp = oldTenantRow["surname"] + @" " + oldTenantRow["name"] + @" " + oldTenantRow["patronymic"];
            var sPatronymic = oldTenantRow["patronymic"].ToString();
            var gender = Declension.GetGender(sPatronymic);
            result += string.Format("в связи c ________________________________________ нанимателя «{0}», договорились:",
                gender == Gender.NotDefind ? snp : Declension.GetSNPDeclension(snp, gender, DeclensionCase.Rodit));

            // Включаем нового нанимателя
            var newTenantRow = (DataRowView)personsChangeTenant[personsChangeTenant.Position];
            var newTenantSurname = (string)newTenantRow["surname"];
            var newTenantName = (string)newTenantRow["name"];
            var newTenantPatronymic = (string)newTenantRow["patronymic"];
            var newTenantSnp = newTenantSurname + " " + newTenantName + " " + newTenantPatronymic;
            gender = Declension.GetGender(newTenantPatronymic);
            result += Environment.NewLine +
                string.Format("\u200B1) считать стороной по договору - нанимателем - «{0}, {1} г.р.»;",
                gender == Gender.NotDefind ? newTenantSnp : Declension.GetSNPDeclension(newTenantSnp, gender, DeclensionCase.Vinit),
                newTenantRow["date_of_birth"] != DBNull.Value ? ((DateTime)newTenantRow["date_of_birth"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");
            return result;
        }

        public string PaymentStringBuilder(string content)
        {
            switch ((int)ParentRow["id_rent_type"])
            {
                case 3:
                    return "\u200B" + GetNextPointHeaderNumber(content) + @") изложить подпункт «з» пункта ___ в следующей редакции:
«з) вносить плату:
• за пользование жилым помещением (плата за наем) на расчетный счет Наймодателя в размере, установленном муниципальным правовым актом города Братска в соответствии с Жилищным кодексом Российской Федерации; 
• за содержание и ремонт жилого помещения лицам, осуществляющим соответствующий вид деятельности, в размере установленном муниципальным правовым актом города Братска в соответствии с Жилищным кодексом Российской Федерации;
• за коммунальные услуги лицам, осуществляющим соответствующий вид деятельности, в соответствии с тарифами, установленными в соответствии законодательством Российской Федерации.
Плата за жилое помещение и коммунальные услуги вносится ежемесячно до десятого числа месяца, следующего за истекшим месяцем, если иной срок не установлен договором управления многоквартирным домом либо решением общего собрания членов товарищества собственников жилья, жилищного кооператива или иного специализированного потребительского кооператива, созданного в целях удовлетворения потребностей граждан в жилье в соответствии с федеральным законом о таком кооперативе.
Плата за жилое помещение и коммунальные услуги вносится Нанимателем на основании платежных документов.
В случае невнесения в установленный срок платы за жилое помещение и (или) коммунальные услуги Наниматель уплачивает Наймодателю пени в размере, установленном Жилищным кодексом Российской Федерации, что не освобождает Нанимателя от уплаты причитающихся платежей».";
                case 1:
                case 2:
                    return "\u200B" + GetNextPointHeaderNumber(content) + @") главу ___ изложить в следующей редакции:
Наниматель вносит плату:
• за пользование жилым помещением (плата за наем) на расчетный счет Наймодателя в размере, установленном решением органа местного самоуправления в соответствии с Жилищным кодексом Российской Федерации;
• за содержание и ремонт жилого помещения лицам, осуществляющим соответствующий вид деятельности, в размере установленном решением органа местного самоуправления, в соответствии с Жилищным кодексом Российской Федерации;
• за коммунальные услуги лицам, осуществляющим соответствующий вид деятельности, в соответствии с тарифами, установленными в соответствии законодательством Российской Федерации.
Плата за жилое помещение и коммунальные услуги вносится ежемесячно до десятого числа месяца, следующего за истекшим месяцем, если иной срок не установлен договором управления многоквартирным домом либо решением общего собрания членов товарищества собственников жилья, жилищного кооператива или иного специализированного потребительского кооператива, созданного в целях удовлетворения потребностей граждан в жилье в соответствии с федеральным законом о таком кооперативе.
Плата за жилое помещение и коммунальные услуги вносится Нанимателем на основании платежных документов.
Наймодатель вправе в одностороннем порядке изменить размер платы за пользование жилым помещением (плата за наем) и за содержание и ремонт жилого помещения в случае внесения изменения в муниципальный правовой акт города Братска, устанавливающего размер платы за жилое помещение.";
            }
            return "";
        }

        public string ProlongSpecialStringBuilder(string content, 
            DateTime? prolongFrom, DateTime? prolongTo, bool untilDismissal, string prolongPoint, string prolongGeneralPoint)
        {
            var rentPeriodStr = "";
            if (prolongFrom != null)
            {
                rentPeriodStr += "с " + prolongFrom.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
            if (untilDismissal)
            {
                if (!string.IsNullOrEmpty(rentPeriodStr))
                    rentPeriodStr += " ";
                rentPeriodStr += "на период трудовых отношений";
            }
            else if (prolongTo != null)
            {
                if (!string.IsNullOrEmpty(rentPeriodStr))
                    rentPeriodStr += " ";
                rentPeriodStr += "по " + prolongTo.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            }

            var contentList = content.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            const string headerWildcard = "^\u200B.*изложить";
            if (!IsHeaderInserted(content, headerWildcard))
            {
                contentList.Add(string.Format("\u200B{0}) изложить в новой редакции:", GetNextPointHeaderNumber(content)));
            }
            var element = string.Format("подпункт {0} пункта {1}: \"Срок найма жилого помещения устанавливается {2}\".",
                    prolongPoint, prolongGeneralPoint, rentPeriodStr);

            var lastPointIndex = GetIndexForInsert(content, headerWildcard);
            if (lastPointIndex == -1)
                contentList.Add(element);
            else
                contentList.Insert(lastPointIndex, element);

            return contentList.Aggregate((v, acc) => v + "\r\n" + acc);
        }

        public string ProlongCommercialStringBuilder(
            DateTime? prolongFrom, DateTime? prolongTo, bool untilDismissal, DateTime requestDate,
            string prolongGeneralPoint)
        {
            var registrationNumber = ParentRow["registration_num"];

            var rentPeriodStr = "";
            if (prolongFrom != null)
            {
                rentPeriodStr += "с " + prolongFrom.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
            if (untilDismissal)
            {
                if (!string.IsNullOrEmpty(rentPeriodStr))
                    rentPeriodStr += " ";
                rentPeriodStr += "на период трудовых отношений";
            }
            else if (prolongTo != null)
            {
                if (!string.IsNullOrEmpty(rentPeriodStr))
                    rentPeriodStr += " ";
                rentPeriodStr += "по " + prolongTo.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
            if (!untilDismissal)
            {
                rentPeriodStr = "на период " + rentPeriodStr;
            }
            var result = GetDefaultAgreementPoint();
            result +=
                string.Format("\r\n\u200B1) на основании {0} от {1} продлить срок действия  договора  № {2} от {3} {4} найма жилого помещения {5}.",
                ViewModel["tenancy_prolong_reason_types"].CurrentRow["reason_template_genetive"],
                requestDate.ToString("dd.MM.yyyy"),
                registrationNumber, GetFormatedRegistrationDate(), GetTenancyRent(), rentPeriodStr);
            if (!string.IsNullOrEmpty(prolongGeneralPoint))
            {
                result += string.Format("\r\n\u200B2) пункт {0} исключить.", prolongGeneralPoint);
            }
            return result;
        }

        internal void AddProlongModification(DateTime? prolongFrom, DateTime? prolongTo, bool untilDismissal, ExtModificationTypes prolongType)
        {
            ClearModifyState();
            Modifications.Add(new Dictionary<ExtModificationTypes, List<ExtModificationParameter>>
            {
                {
                    prolongType, new List<ExtModificationParameter>
                    {
                        new ExtModificationParameter
                        {
                            Name = "ProlongFrom",
                            Value = prolongFrom
                        },
                        new ExtModificationParameter
                        {
                            Name = "ProlongTo",
                            Value = prolongTo
                        },
                        new ExtModificationParameter
                        {
                            Name = "UntilDismissal",
                            Value = untilDismissal
                        }
                    }
                }
            });
        }

        internal void AddChangeTenantModification(bool excludeTenant, int? idKinshipOldTenant)
        {
            ClearModifyState();
            var tenantChangeTenant = ViewModel["tenant_change_tenant"].BindingSource;
            var personsChangeTenant = ViewModel["persons_change_tenant"].BindingSource;
            var oldTenantRow = (DataRowView)tenantChangeTenant[0];
            var newTenantRow = (DataRowView)personsChangeTenant[personsChangeTenant.Position];

            Modifications.Add(
                new Dictionary<ExtModificationTypes, List<ExtModificationParameter>>
            {
                { ExtModificationTypes.ChangeTenant, new List<ExtModificationParameter>
                    {
                        new ExtModificationParameter
                        {
                            Name = "ExcludeTenant",
                            Value = excludeTenant
                        },
                        new ExtModificationParameter
                        {
                            Name = "IdNewTenant",
                            Value = (int)newTenantRow["id_person"]
                        },
                        new ExtModificationParameter
                        {
                            Name = "IdOldTenant",
                            Value = (int?)oldTenantRow["id_person"]
                        },
                        new ExtModificationParameter
                        {
                            Name = "IdKinshipOldTenant",
                            Value = idKinshipOldTenant
                        }
                    } 
                }
            });
        }

        internal void AddExcludePersonModification()
        {
            var tenancyPerson = ViewModel["persons_exclude"].CurrentRow;
            ExcludePersons.Add(new TenancyPerson
            {
                IdProcess = (int?)ParentRow["id_process"],
                IdPerson = (int?)tenancyPerson["id_person"],
                Surname = tenancyPerson["surname"].ToString(),
                Name = tenancyPerson["name"].ToString(),
                Patronymic = tenancyPerson["patronymic"].ToString(),
                DateOfBirth = (DateTime?)(tenancyPerson["date_of_birth"] == DBNull.Value ? null : tenancyPerson["date_of_birth"]),
                IdKinship = (int?)tenancyPerson["id_kinship"]
            });
        }

        internal void IncludePersonModification(string surname, string name, string patronymic, DateTime dateOfBirth, int? idKinship)
        {
            IncludePersons.Add(new TenancyPerson
            {
                IdProcess = (int?)ParentRow["id_process"],
                Surname = surname,
                Name = name,
                Patronymic = patronymic,
                DateOfBirth = dateOfBirth,
                IdKinship = idKinship
            });
        }

        internal void ApplyModifications()
        {
            // Обновление участников найма после сохранения соглашения
            if (IncludePersons.Count > 0)
            {
                new TenancyAgreementOnSavePersonManager(IncludePersons,
                    TenancyAgreementOnSavePersonManager.PersonsOperationType.IncludePersons).ShowDialog();
            }
            if (ExcludePersons.Count > 0)
            {
                new TenancyAgreementOnSavePersonManager(ExcludePersons,
                    TenancyAgreementOnSavePersonManager.PersonsOperationType.ExcludePersons).ShowDialog();
            }
            // Дополнительные обновления
            ExtModificationsExecute();
        }

        internal string TerminateStringBuilder(string terminateReason, DateTime terminateDate)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "1.1. По настоящему Соглашению Стороны договорились расторгнуть с {3} договор № {0} от {1} {4} найма жилого помещения, расположенного по адресу: Российская Федерация, {5}, (далее - договор) по {2}.\r\n" +
                "1.2. Обязательства, возникшие из указанного договора до момента расторжения, подлежат исполнению в соответствии с указанным договором. Стороны не имеют взаимных претензий по исполнению условий договора № {0} от {1}.",
                ParentRow["registration_num"],
                GetFormatedRegistrationDate(),
                terminateReason.StartsWith("по ") ? terminateReason.Substring(3).Trim() : terminateReason.Trim(),
                terminateDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                GetTenancyRent(),
                GetTenancyAddress());
        }

        internal string GetDefaultAgreementPoint()
        {           
            return string.Format(CultureInfo.InvariantCulture,
                "1.1. По настоящему Соглашению Стороны по договору № {0} от {1} {2} найма жилого помещения, расположенного по адресу: Российская Федерация, {3}, договорились:",
                ParentRow["registration_num"],
                GetFormatedRegistrationDate(),
                GetTenancyRent(),
                GetTenancyAddress());
        }

        private string GetFormatedRegistrationDate()
        {
            return ParentRow["registration_date"] != DBNull.Value
                ? Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture)
                    .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
                : "";
        }

        private string GetTenancyRent()
        {
            var rentRow = ViewModel["rent_types"].DataSource.Rows.Find(ParentRow["id_rent_type"]);
            var rent = "";
            if (rentRow != null)
            {
                rent = rentRow["rent_type_genetive"].ToString();
            }
            return rent;
        }

        private string GetTenancyAddress()
        {
            var premisesInfoRow = (from row in ViewModel["tenancy_premises_info"].Model.FilterDeletedRows()
                                   where row.Field<int>("id_process") == (int?)ParentRow["id_process"]
                                   select row).FirstOrDefault();
            var address = "";
            if (premisesInfoRow != null)
            {
                address = "Иркутская область, город Братск, " + premisesInfoRow["address"].ToString()
                    .Replace("жилрайон.", "жилой район")
                    .Replace("ул.", "улица")
                    .Replace("пр-кт.", "проспект")
                    .Replace("б-р.", "бульвар")
                    .Replace("туп.", "тупик")
                    .Replace("д.", "дом")
                    .Replace("кв.", "квартира")
                    .Replace("пом.", "помещение")
                    .Replace("ком.", "комната(ы)");
            }
            return address;
        }
    }
}
