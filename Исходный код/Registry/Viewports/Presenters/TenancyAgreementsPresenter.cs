using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Declensions.Unicode;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class TenancyAgreementsPresenter: Presenter
    {
        public TenancyAgreementsPresenter() : base(new TenancyAgreementsViewModel(), null, null)
        {
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

        public string ExplainContentModifier(string content, string generalPoint, string point, string explainContent)
        {
            var contentList = content.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            var headersCount = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B"))
                    headersCount++;
            }
            var headerIndex = -1;
            var lastPointIndex = -1;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B.*изложить"))
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
            var element = string.Format(CultureInfo.InvariantCulture, "{0}. {1}", point, explainContent.Trim());
            if (headerIndex == -1)
            {
                contentList.Add(string.Format("\u200B{0}) изложить в новой редакции:", ++headersCount));
            }
            if (lastPointIndex == -1)
                contentList.Add(element);
            else
                contentList.Insert(lastPointIndex, element);
            return contentList.Aggregate((v, acc) => v + "\r\n" + acc);
        }

        public string IncludePersonsContentModifier(string content, string generalPoint, string point, string surname, string name,
            string patronymic, string kinship, DateTime dateOfBirth)
        {
            var contentList = content.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            var headersCount = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B"))
                    headersCount++;
            }
            var headerIndex = -1;
            var lastPointIndex = -1;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], string.Format("^\u200B.*пункт {0} договора дополнить", generalPoint)))
                {
                    headerIndex = i;
                }
                else
                    if (headerIndex != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ договора исключить|\u200B.*пункт .+ договора дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        lastPointIndex = i;
                        break;
                    }
            }

            var snp = (surname + " " + name + " " + patronymic).Trim();
            var element = string.Format(CultureInfo.InvariantCulture, "«{0}. {1}, {2} - {3} г.р.»;", point,
                snp, kinship, dateOfBirth.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            if (kinship == "наниматель")
            {
                var gender = Declension.GetGender(patronymic);
                contentList.Add(string.Format("\u200B{4}) считать по договору № {0} от {1} нанимателем - «{2} - {3} г.р.»;",
                    ParentRow["registration_num"],
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "",
                        gender == Gender.NotDefind ? snp :
                            Declension.GetSNPDeclension(snp, gender, DeclensionCase.Vinit),
                        dateOfBirth.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                        ++headersCount));
            }
            else
            {
                if (headerIndex == -1)
                {
                    contentList.Add(string.Format("\u200B{2}) пункт {0} договора дополнить подпунктом {1} следующего содержания:",
                        generalPoint, point, ++headersCount));
                }
                if (lastPointIndex == -1)
                    contentList.Add(element);
                else
                    contentList.Insert(lastPointIndex, element);
            }
            return contentList.Aggregate((v, acc) => v + "\r\n" + acc);
        }


        public string ExcludePersonsContentModifier(string content, string generalPoint, string point)
        {
            var contentList = content.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            var headerIndex = -1;
            var lastPointIndex = -1;
            var headersCount = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B"))
                    headersCount++;
            }
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], string.Format("^\u200B.*из пункта {0} договора исключить",
                    generalPoint)))
                {
                    headerIndex = i;
                }
                else
                    if (headerIndex != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ договора исключить|\u200B.*пункт .+ договора дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        lastPointIndex = i;
                        break;
                    }
            }
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
            if (headerIndex == -1)
            {
                contentList.Add(string.Format("\u200B{2}) из пункта {0} договора исключить подпункт {1} следующего содержания:",
                    generalPoint, point, ++headersCount));
            }
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
            var regDateStr = ParentRow["registration_date"] != DBNull.Value
                ? Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture)
                .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
            var registrationNumber = ParentRow["registration_num"];

            var result = string.Format(CultureInfo.InvariantCulture,
                "1.1. По настоящему Соглашению Стороны по договору № {0} от {1}, ",
                regDateStr, registrationNumber);

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
                string.Format("\u200B1) считать стороной по договору - нанимателем - «{0}, {1}»;",
                gender == Gender.NotDefind ? newTenantSnp : Declension.GetSNPDeclension(newTenantSnp, gender, DeclensionCase.Vinit),
                newTenantRow["date_of_birth"] != DBNull.Value ? ((DateTime)newTenantRow["date_of_birth"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");
            return result;
        }

        public string PaymentStringBuilder(int pointNum)
        {
            switch ((int)ParentRow["id_rent_type"])
            {
                case 3:
                    return "\u200B" + pointNum + @") изложить подпункт ""з"" пункта ___ в следующей редакции:
«вносить плату:
• за пользование жилым помещением (плата за наем) на расчетный счет Наймодателя в размере, установленном муниципальным правовым актом города Братска в соответствии с Жилищным кодексом Российской Федерации; 
• за содержание и ремонт жилого помещения лицам, осуществляющим соответствующий вид деятельности, в размере установленном муниципальным правовым актом города Братска в соответствии с Жилищным кодексом Российской Федерации;
• за коммунальные услуги лицам, осуществляющим соответствующий вид деятельности, в соответствии с тарифами, установленными в соответствии законодательством Российской Федерации.
Плата за жилое помещение и коммунальные услуги вносится ежемесячно до десятого числа месяца, следующего за истекшим месяцем, если иной срок не установлен договором управления многоквартирным домом либо решением общего собрания членов товарищества собственников жилья, жилищного кооператива или иного специализированного потребительского кооператива, созданного в целях удовлетворения потребностей граждан в жилье в соответствии с федеральным законом о таком кооперативе.
Плата за жилое помещение и коммунальные услуги вносится Нанимателем на основании платежных документов.
В случае невнесения в установленный срок платы за жилое помещение и (или) коммунальные услуги Наниматель уплачивает Наймодателю пени в размере, установленном Жилищным кодексом Российской Федерации, что не освобождает Нанимателя от уплаты причитающихся платежей».";
                case 1:
                case 2:
                    return "\u200B" + pointNum + @") главу ___ изложить в следующей редакции:
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

        public string ProlongSpecialStringBuilder(
            DateTime? prolongFrom, DateTime? prolongTo, bool untilDismissal, string prolongPoint, string prolongGeneralPoint)
        {
            var regDateStr = ParentRow["registration_date"] != DBNull.Value
                ? Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture)
                .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
            var registrationNumber = ParentRow["registration_num"];

            var rentPeriodStr = "";
            if (prolongFrom != null)
            {
                rentPeriodStr += "с " + prolongFrom.Value.ToString("dd.MM.yyy", CultureInfo.InvariantCulture);
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
                rentPeriodStr += "по " + prolongTo.Value.ToString("dd.MM.yyy", CultureInfo.InvariantCulture);
            }
            return string.Format(@"1.1. По настоящему Соглашению Стороны по договору № {0} от {1} договорились:" +
                "\r\n\u200B1) изложить в новой редакции:" +
                "\r\nподпункт {2} пункта {3}: \"Срок найма жилого помещения устанавливается {4}\".",
                regDateStr, registrationNumber,
                prolongPoint, prolongGeneralPoint, rentPeriodStr);
        }

        public string ProlongCommercialStringBuilder(
            DateTime? prolongFrom, DateTime? prolongTo, bool untilDismissal, DateTime requestDate,
            string prolongGeneralPoint)
        {
            var regDateStr = ParentRow["registration_date"] != DBNull.Value
                ? Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture)
                .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
            var registrationNumber = ParentRow["registration_num"];

            var rentPeriodStr = "";
            if (prolongFrom != null)
            {
                rentPeriodStr += "с " + prolongFrom.Value.ToString("dd.MM.yyy", CultureInfo.InvariantCulture);
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
                rentPeriodStr += "по " + prolongTo.Value.ToString("dd.MM.yyy", CultureInfo.InvariantCulture);
            }
            if (!untilDismissal)
            {
                rentPeriodStr = "на период " + rentPeriodStr;
            }

            return string.Format(@"1.1. По настоящему Соглашению на основании личного заявления нанимателя от {3}. Стороны договорились:" +
                "\r\n\u200B1) продлить срок действия  договора  № {0} от {1} {2}." +
                "\r\n\u200B2) пункт {4} исключить.",
                registrationNumber, regDateStr, rentPeriodStr,
                requestDate.ToString("dd.MM.yyyy"), prolongGeneralPoint);
        }
    }
}
