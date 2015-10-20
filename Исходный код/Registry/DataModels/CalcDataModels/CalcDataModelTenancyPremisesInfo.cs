﻿using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelTenancyPremisesInfo : CalcDataModel
    {
        private const string TableName = "tenancy_premises_info";

        private CalcDataModelTenancyPremisesInfo()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.TenancySubPremisesAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.KladrStreetsDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.BuildingsDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.PremisesDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.SubPremisesDataModel).Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("address").DataType = typeof(string);
            table.Columns.Add("total_area").DataType = typeof(double);
            table.Columns.Add("living_area").DataType = typeof(double);
            table.Columns.Add("rent_area").DataType = typeof(double);
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelTenancyPremisesInfo");
            // Вычисляем агрегационную информацию
            var assocSubPremises = DataModel.GetInstance(DataModelType.TenancySubPremisesAssocDataModel).FilterDeletedRows();
            var assocPremises = DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).FilterDeletedRows();
            var assocBuildings = DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel).FilterDeletedRows();
            var kladrStreet = DataModel.GetInstance(DataModelType.KladrStreetsDataModel).FilterDeletedRows();
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();

            var aSubPremises = from assocSubPremisesRow in assocSubPremises
                                    join subPremisesRow in subPremises
                                    on assocSubPremisesRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                                    join premisesRow in premises
                                    on subPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                    join buildingsRow in buildings
                                    on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    join kladrStreetRow in kladrStreet
                                    on buildingsRow.Field<string>("id_street") equals kladrStreetRow.Field<string>("id_street")
                                    select new
                                    {
                                        id_process = assocSubPremisesRow.Field<int>("id_process"),
                                        address = kladrStreetRow.Field<string>("street_name") + ", дом " + buildingsRow.Field<string>("house") +
                                            (premisesRow.Field<int>("id_premises_type") == 2 ? " ком. " : 
                                            (premisesRow.Field<int>("id_premises_type") == 4 ? " пом. " : " кв. ")) + premisesRow.Field<string>("premises_num") +
                                            " ком. " + subPremisesRow.Field<string>("sub_premises_num"),
                                        total_area = subPremisesRow.Field<double>("total_area"),
                                        living_area = subPremisesRow.Field<double>("living_area"),
                                        rent_area = assocSubPremisesRow.Field<double?>("rent_total_area")
                                    };
            var aPremises = from assocPremisesRow in assocPremises
                                    join premisesRow in premises
                                    on assocPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                    join buildingsRow in buildings
                                    on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    join kladrStreetRow in kladrStreet
                                    on buildingsRow.Field<string>("id_street") equals kladrStreetRow.Field<string>("id_street")
                                    select new
                                    {
                                        id_process = assocPremisesRow.Field<int>("id_process"),
                                        address = kladrStreetRow.Field<string>("street_name") + ", дом " + buildingsRow.Field<string>("house") +
                                            (premisesRow.Field<int>("id_premises_type") == 2 ? " ком. " :
                                            (premisesRow.Field<int>("id_premises_type") == 4 ? " пом. " : " кв. ")) + premisesRow.Field<string>("premises_num"),
                                        total_area = premisesRow.Field<double>("total_area"),
                                        living_area = premisesRow.Field<double>("living_area"),
                                        rent_area = assocPremisesRow.Field<double?>("rent_total_area")
                                    };
            var aBuildings = from assocBuildingsRow in assocBuildings
                             join buildingsRow in buildings
                             on assocBuildingsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                             join kladrStreetRow in kladrStreet
                             on buildingsRow.Field<string>("id_street") equals kladrStreetRow.Field<string>("id_street")
                             select new
                             {
                                 id_process = assocBuildingsRow.Field<int>("id_process"),
                                 address = kladrStreetRow.Field<string>("street_name") + ", дом " + buildingsRow.Field<string>("house"),
                                 total_area = buildingsRow.Field<double>("total_area"),
                                 living_area = buildingsRow.Field<double>("living_area"),
                                 rent_area = assocBuildingsRow.Field<double?>("rent_total_area")
                             };
            var result = aBuildings.Union(aPremises).Union(aSubPremises);

            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_process, x.address, x.total_area, x.living_area, x.rent_area);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelTenancyPremisesInfo GetInstance()
        {
            return new CalcDataModelTenancyPremisesInfo();
        }
    }
}
