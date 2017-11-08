using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class TenancyPresenter: Presenter
    {
        public TenancyPresenter() : base(new TenancyViewModel(), new SimpleSearchTenancyForm(), new ExtendedSearchTenancyForm())
        {
            ViewModel["executors"].BindingSource.Filter = "is_inactive = 0";
            ViewModel["tenancy_processes_tenancy_rent_periods_history"].BindingSource.Sort = "begin_date DESC";
        }

        public void AddAssocViewModelItem()
        {
            switch (ParentType)
            {
                case ParentTypeEnum.SubPremises:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<TenancySubPremisesAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_sub_premises = " + ParentRow["id_sub_premises"];
                    break;
                case ParentTypeEnum.Premises:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<TenancyPremisesAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_premises = " + ParentRow["id_premises"];
                    break;
                case ParentTypeEnum.Building:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<TenancyBuildingAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_building = " + ParentRow["id_building"];
                    break;
            }
        }

        public string GetStaticFilter()
        {
            IEnumerable<int> ids;
            if (ParentRow == null)
                return "";
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    ids = TenancyService.TenancyProcessIDsByBuildingId(Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.Premises:
                    ids = TenancyService.TenancyProcessIDsByPremisesId(Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.SubPremises:
                    ids = TenancyService.TenancyProcessIDsBySubPremisesId(Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture));
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
            var filter = "";
            if (ids == null) return filter;
            filter = "id_process IN (0";
            foreach (var id in ids)
                filter += id.ToString(CultureInfo.InvariantCulture) + ",";
            filter = filter.TrimEnd(',') + ")";
            return filter;
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

        public void FiltersRebuild()
        {
            var row = ViewModel["general"].CurrentRow;
            ViewModel["tenancy_premises_info"].BindingSource.Filter = row != null ? "id_process = 0" + row["id_process"] : "id_process = 0";
        }

        internal bool DeleteRecord()
        {
            var row = ViewModel["general"].CurrentRow;
            var columnName = ViewModel["general"].PrimaryKeyFirst;
            return ViewModel["general"].Delete((int)row[columnName]);
        }

        internal bool InsertRecord(TenancyProcess tenancy, int? idCopyProcess)
        {
            var id = ViewModel["general"].Model.Insert(tenancy);
            if (id == -1)
            {
                return false;
            }
            tenancy.IdProcess = id;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, tenancy.IdProcess);
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<TenancyProcess>.FillRow(tenancy, row);
            // Если производится копирование, а не создание новой записи, то надо скопировать участников найма и нанимаемое жилье
            if (idCopyProcess != null)
            {
                if (!CopyTenancyProcessRelData(id, idCopyProcess.Value))
                    MessageBox.Show(@"Произошла ошибка во время копирования данных", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
                if (ParentRow != null)
                {
                    int idAssoc;
                    var assoc = ViewModel["assoc"].Model;
                    assoc.EditingNewRecord = true;
                    switch (ParentType)
                    {
                        case ParentTypeEnum.Building:
                            var tBuilding = new TenancyBuildingAssoc
                            {
                                IdProcess = id,
                                IdBuilding = Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture),
                                RentLivingArea = null,
                                RentTotalArea = null
                            };
                            idAssoc = assoc.Insert(tBuilding);
                            if (idAssoc == -1)
                                return false;
                            tBuilding.IdAssoc = idAssoc;
                            assoc.Select().Rows.Add(idAssoc, tBuilding.IdBuilding,
                                tBuilding.IdProcess, tBuilding.RentTotalArea, tBuilding.RentLivingArea, 0);
                            break;
                        case ParentTypeEnum.Premises:
                            var tPremises = new TenancyPremisesAssoc
                            {
                                IdProcess = id,
                                IdPremises = Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture),
                                RentLivingArea = null,
                                RentTotalArea = null
                            };
                            idAssoc = assoc.Insert(tPremises);
                            if (idAssoc == -1)
                                return false;
                            tPremises.IdAssoc = idAssoc;
                            assoc.Select().Rows.Add(idAssoc, tPremises.IdPremises,
                                tPremises.IdProcess, tPremises.RentTotalArea, tPremises.RentLivingArea, 0);
                            break;
                        case ParentTypeEnum.SubPremises:
                            var tSubPremises = new TenancySubPremisesAssoc
                            {
                                IdProcess = id,
                                IdSubPremises = Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture),
                                RentTotalArea = null
                            };
                            idAssoc = assoc.Insert(tSubPremises);
                            if (idAssoc == -1)
                                return false;
                            tSubPremises.IdAssoc = idAssoc;
                            assoc.Select().Rows.Add(idAssoc, tSubPremises.IdSubPremises,
                                tSubPremises.IdProcess, tSubPremises.RentTotalArea, 0);
                            break;
                        default: throw new ViewportException("Неизвестный тип родительского объекта");
                    }
                    assoc.EditingNewRecord = false;
                }
            FiltersRebuild();
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        // Метод копирует зависимые данные по процессу найма
        private bool CopyTenancyProcessRelData(int idNewProcess, int idCopyProcess)
        {
            return CopyPersons(idNewProcess, idCopyProcess) && 
                CopyBuildingsAssoc(idNewProcess, idCopyProcess) && 
                CopyPremisesAssoc(idNewProcess, idCopyProcess) &&
                CopySubPremisesAssoc(idNewProcess, idCopyProcess);
        }

        private static bool CopySubPremisesAssoc(int idNewProcess, int idCopyProcess)
        {
            var tenancySubPremisesAssoc = EntityDataModel<TenancySubPremisesAssoc>.GetInstance();
            var subPremises = from row in tenancySubPremisesAssoc.FilterDeletedRows()
                where row.Field<int>("id_process") == idCopyProcess
                select row;
            tenancySubPremisesAssoc.EditingNewRecord = true;
            foreach (var row in subPremises.ToList())
            {
                var obj = new TenancySubPremisesAssoc
                {
                    IdSubPremises = row.Field<int?>("id_sub_premises"),
                    IdProcess = idNewProcess,
                    RentTotalArea = row.Field<double?>("rent_total_area")
                };
                var idAssoc = tenancySubPremisesAssoc.Insert(obj);
                if (idAssoc == -1)
                {
                    tenancySubPremisesAssoc.EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = idAssoc;
                tenancySubPremisesAssoc.Select().Rows.Add(obj.IdAssoc, obj.IdSubPremises, obj.IdProcess,
                    obj.RentTotalArea);
            }
            tenancySubPremisesAssoc.EditingNewRecord = false;
            return true;
        }

        private static bool CopyPremisesAssoc(int idNewProcess, int idCopyProcess)
        {
            var tenancyPremisesAssoc = EntityDataModel<TenancyPremisesAssoc>.GetInstance();
            var premises = from row in tenancyPremisesAssoc.FilterDeletedRows()
                where row.Field<int>("id_process") == idCopyProcess
                select row;
            tenancyPremisesAssoc.EditingNewRecord = true;
            foreach (var row in premises.ToList())
            {
                var obj = new TenancyPremisesAssoc
                {
                    IdPremises = row.Field<int?>("id_premises"),
                    IdProcess = idNewProcess,
                    RentLivingArea = row.Field<double?>("rent_living_area"),
                    RentTotalArea = row.Field<double?>("rent_total_area")
                };
                var idAssoc = tenancyPremisesAssoc.Insert(obj);
                if (idAssoc == -1)
                {
                    tenancyPremisesAssoc.EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = idAssoc;
                tenancyPremisesAssoc.Select().Rows.Add(obj.IdAssoc, obj.IdPremises, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            tenancyPremisesAssoc.EditingNewRecord = false;
            return true;
        }

        private static bool CopyBuildingsAssoc(int idNewProcess, int idCopyProcess)
        {
            var tenancyBuildingsAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance();
            var buildings = from row in tenancyBuildingsAssoc.FilterDeletedRows()
                where row.Field<int>("id_process") == idCopyProcess
                select row;
            tenancyBuildingsAssoc.EditingNewRecord = true;
            foreach (var row in buildings.ToList())
            {
                var obj = new TenancyBuildingAssoc
                {
                    IdBuilding = row.Field<int?>("id_building"),
                    IdProcess = idNewProcess,
                    RentLivingArea = row.Field<double?>("rent_living_area"),
                    RentTotalArea = row.Field<double?>("rent_total_area")
                };
                var idAssoc = tenancyBuildingsAssoc.Insert(obj);
                if (idAssoc == -1)
                {
                    tenancyBuildingsAssoc.EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = idAssoc;
                tenancyBuildingsAssoc.Select().Rows.Add(obj.IdAssoc, obj.IdBuilding, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            tenancyBuildingsAssoc.EditingNewRecord = false;
            return true;
        }

        private bool CopyPersons(int idNewProcess, int idCopyProcess)
        {
            var persons = from personsRow in ViewModel["tenancy_processes_tenancy_persons"].Model.FilterDeletedRows()
                where personsRow.Field<int>("id_process") == idCopyProcess
                select personsRow;
            ViewModel["tenancy_processes_tenancy_persons"].Model.EditingNewRecord = true;
            foreach (var personRow in persons.ToList())
            {
                var person = EntityConverter<TenancyPerson>.FromRow(personRow);
                person.IdProcess = idNewProcess;
                person.RegistrationDate = null;
                var idPerson = ViewModel["tenancy_processes_tenancy_persons"].Model.Insert(person);
                if (idPerson == -1)
                {
                    ViewModel["tenancy_processes_tenancy_persons"].Model.EditingNewRecord = false;
                    return false;
                }
                person.IdPerson = idPerson;
                var personBinding = new BindingSource
                {
                    DataSource = DataStorage.DataSet,
                    DataMember = "tenancy_persons"
                };
                var newPersonRow = (DataRowView) personBinding.AddNew();
                EntityConverter<TenancyPerson>.FillRow(person, newPersonRow);
            }
            ViewModel["tenancy_processes_tenancy_persons"].Model.EditingNewRecord = false;
            return true;
        }

        internal bool UpdateRecord(TenancyProcess tenancy)
        {
            if (tenancy.IdProcess == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить процесс найма без внутренного номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(tenancy) == -1)
                return false;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, tenancy.IdProcess);
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<TenancyProcess>.FillRow(tenancy, row);
            return true;
        }

        private static void RebuildFilterAfterSave(IBindingListView bindingSource, int? idPremise)
        {
            var filter = "";
            if (!string.IsNullOrEmpty(bindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", idPremise);
            bindingSource.Filter += filter;
        }

        public void AddRentPeriod(DateTime? beginDate, DateTime? endDate, bool? untilDismissal)
        {
            var row = ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var rentPeriodsViewModel = ViewModel["tenancy_processes_tenancy_rent_periods_history"];
            rentPeriodsViewModel.Model.EditingNewRecord = true;
            var rentPeriod = new TenancyRentPeriod
            {
                IdProcess = (int)row["id_process"],
                BeginDate = beginDate,
                EndDate = endDate,
                UntilDismissal = untilDismissal
            };
            var idRentPeriod = rentPeriodsViewModel.Model.Insert(rentPeriod);
            if (idRentPeriod == -1) return;
            rentPeriod.IdRentPeriod = idRentPeriod;
            rentPeriodsViewModel.DataSource.Rows.Add(
                idRentPeriod, rentPeriod.IdProcess, rentPeriod.BeginDate, rentPeriod.EndDate, rentPeriod.UntilDismissal);
            rentPeriodsViewModel.Model.EditingNewRecord = false;
        }

        internal void DeleteCurrentRentPeriod()
        {
            if (ViewModel["general"].CurrentRow == null)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить этот период найма?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var rentPeriodsViewModel = ViewModel["tenancy_processes_tenancy_rent_periods_history"];
            var row = rentPeriodsViewModel.CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран период найма для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var idRentPeriod = (int)row["id_rent_period"];
            if (rentPeriodsViewModel.Model.Delete(idRentPeriod) == -1)
                return;
            rentPeriodsViewModel.DataSource.Rows.Find(idRentPeriod).Delete();
        }

        public void SwapRentPeriod()
        {
            var row = ViewModel["general"].CurrentRow;
            if (row == null)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var rentPeriod = new TenancyRentPeriod
            {
                IdProcess = (int)row["id_process"],
                BeginDate = ViewportHelper.ValueOrNull<DateTime>(row, "begin_date"),
                EndDate = ViewportHelper.ValueOrNull<DateTime>(row, "end_date"),
                UntilDismissal = ViewportHelper.ValueOrNull<bool>(row, "until_dismissal"),
            };
            var rentPeriodsViewModel = ViewModel["tenancy_processes_tenancy_rent_periods_history"];
            rentPeriodsViewModel.Model.EditingNewRecord = true;
            var idRentPeriod = rentPeriodsViewModel.Model.Insert(rentPeriod);
            if (idRentPeriod == -1) return;
            rentPeriod.IdRentPeriod = idRentPeriod;
            rentPeriodsViewModel.DataSource.Rows.Add(
                idRentPeriod, rentPeriod.IdProcess, rentPeriod.BeginDate, rentPeriod.EndDate, rentPeriod.UntilDismissal);
            rentPeriodsViewModel.Model.EditingNewRecord = false;
        }

        public bool HasContractDuplicates()
        {
            var currentRow = ViewModel["general"].CurrentRow;
            if (currentRow == null) return false;
            if (currentRow["id_rent_type"] == DBNull.Value || 
                currentRow["id_process"] == DBNull.Value ||
                (currentRow["registration_num"] != DBNull.Value && currentRow["registration_num"].ToString().EndsWith("н"))) return false;
            var tenant = (from processRow in ViewModel["general"].Model.FilterDeletedRows()
                join personsRow in EntityDataModel<TenancyPerson>.GetInstance().FilterDeletedRows()
                    on processRow.Field<int?>("id_process") equals personsRow.Field<int?>("id_process")
                where processRow.Field<int?>("id_process") == (int?) currentRow["id_process"] &&
                      personsRow.Field<int?>("id_kinship") == 1 && personsRow["exclude_date"] == DBNull.Value
                select personsRow).FirstOrDefault();
            if (tenant == null) return false;
            var result = from processRow in ViewModel["general"].Model.FilterDeletedRows()
                join personsRow in EntityDataModel<TenancyPerson>.GetInstance().FilterDeletedRows()
                    on processRow.Field<int?>("id_process") equals personsRow.Field<int?>("id_process")
                where personsRow.Field<int?>("id_kinship") == 1 &&
                      personsRow["exclude_date"] == DBNull.Value &&
                      personsRow.Field<string>("surname").Trim() == tenant.Field<string>("surname").Trim() &&
                      personsRow.Field<string>("name").Trim() == tenant.Field<string>("name").Trim() &&
                      ((personsRow.Field<string>("patronymic") == null && tenant.Field<string>("patronymic") == null) || 
                       (personsRow.Field<string>("patronymic") != null && tenant.Field<string>("patronymic") != null &&
                        personsRow.Field<string>("patronymic").Trim() == tenant.Field<string>("patronymic").Trim())) &&
                      personsRow.Field<DateTime?>("date_of_birth") == tenant.Field<DateTime?>("date_of_birth") &&
                      processRow.Field<int?>("id_process") != (int?) currentRow["id_process"] &&
                      (processRow.Field<string>("registration_num") == null || !processRow.Field<string>("registration_num").EndsWith("н"))
                select processRow;
            return result.Any();
        }
    }
}
