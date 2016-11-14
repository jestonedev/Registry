using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;

namespace Registry.DataModels.CalcDataModels
{
    internal class CalcDataModelTenancyPaymentsInfo: CalcDataModel
    {
        private static CalcDataModelTenancyPaymentsInfo _dataModel;

        private const string TableName = "tenancy_payments_info";

        private CalcDataModelTenancyPaymentsInfo()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(EntityDataModel<TenancySubPremisesAssoc>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<TenancyPremisesAssoc>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<TenancyBuildingAssoc>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<Building>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<Premise>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<SubPremise>.GetInstance().Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("id_sub_premises").DataType = typeof(int);
            table.Columns.Add("rent_area").DataType = typeof(double);
            table.Columns.Add("id_rent_category").DataType = typeof(int);
            table.Columns.Add("payment").DataType = typeof(decimal);
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelTenancyPremisesInfo");
            // Вычисляем агрегационную информацию
            var assocSubPremises = EntityDataModel<TenancySubPremisesAssoc>.GetInstance().FilterDeletedRows();
            var assocPremises = EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows();
            var assocBuildings = EntityDataModel<TenancyBuildingAssoc>.GetInstance().FilterDeletedRows();
            var buildings = EntityDataModel<Building>.GetInstance().FilterDeletedRows().ToList();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            var emergencyPremises = PremisesService.PremiseIDsByOwnershipType(2).Distinct().ToList();

            var aSubPremises = from assocSubPremisesRow in assocSubPremises
                                    join subPremisesRow in subPremises
                                    on assocSubPremisesRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                                    join premisesRow in premises
                                    on subPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                    join buildingsRow in buildings
                                    on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    select new
                                    {
                                        id_process = assocSubPremisesRow.Field<int>("id_process"),
                                        id_building = buildingsRow.Field<int>("id_building"),
                                        id_premises = premisesRow.Field<int?>("id_premises"),
                                        id_sub_premises = subPremisesRow.Field<int?>("id_sub_premises"),
                                        total_area = subPremisesRow.Field<double>("total_area"),
                                        rent_area = assocSubPremisesRow.Field<double?>("rent_total_area")
                                    };
            var aPremises = from assocPremisesRow in assocPremises
                                    join premisesRow in premises
                                    on assocPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                    join buildingsRow in buildings
                                    on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    select new
                                    {
                                        id_process = assocPremisesRow.Field<int>("id_process"),
                                        id_building = buildingsRow.Field<int>("id_building"),
                                        id_premises = premisesRow.Field<int?>("id_premises"),
                                        id_sub_premises = (int?)null,
                                        total_area = premisesRow.Field<double>("total_area"),
                                        rent_area = assocPremisesRow.Field<double?>("rent_total_area")
                                    };
            var aBuildings = from assocBuildingsRow in assocBuildings
                             join buildingsRow in buildings
                             on assocBuildingsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                             select new
                             {
                                 id_process = assocBuildingsRow.Field<int>("id_process"),
                                 id_building = buildingsRow.Field<int>("id_building"),
                                 id_premises = (int?)null,
                                 id_sub_premises = (int?)null,
                                 total_area = buildingsRow.Field<double>("total_area"),
                                 rent_area = assocBuildingsRow.Field<double?>("rent_total_area")
                             };
            var rentObjects = aBuildings.Union(aPremises).Union(aSubPremises);
            var result = from rentObjectsRow in rentObjects
                join buildingsRow in buildings
                    on rentObjectsRow.id_building equals buildingsRow.Field<int>("id_building")
                join emergencyPremisesRow in emergencyPremises
                    on rentObjectsRow.id_premises equals emergencyPremisesRow into epr
                from eprRow in epr.DefaultIfEmpty()
                select new
                {
                    rentObjectsRow.id_process,
                    rentObjectsRow.id_building,
                    rentObjectsRow.id_premises,
                    rentObjectsRow.id_sub_premises,
                    rent_area = rentObjectsRow.rent_area ?? rentObjectsRow.total_area,
                    id_rent_category = BuildingService.GetRentCategory(buildingsRow, eprRow != 0),
                    payment = (buildingsRow.Field<decimal>("rent_coefficient") != 0 ? buildingsRow.Field<decimal>("rent_coefficient") : 
                        BuildingService.GetRentCoefficient(BuildingService.GetRentCategory(buildingsRow, eprRow != 0))) * 
                            (decimal)(rentObjectsRow.rent_area ?? rentObjectsRow.total_area)
                };

            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_process, x.id_building, x.id_premises, x.id_sub_premises, x.rent_area, x.id_rent_category, x.payment);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelTenancyPaymentsInfo GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelTenancyPaymentsInfo());
        }
    }
}
