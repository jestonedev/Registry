using System;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class BuildingConverter
    {
        public static Building FromRow(DataRow row)
        {
            return new Building
            {
                IdBuilding = ViewportHelper.ValueOrNull<int>(row, "id_building"),
                IdStreet = ViewportHelper.ValueOrNull(row, "id_street"),
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                IdStructureType = ViewportHelper.ValueOrNull<int>(row, "id_structure_type"),
                House = ViewportHelper.ValueOrNull(row, "house"),
                Floors = ViewportHelper.ValueOrNull<short>(row, "floors"),
                NumPremises = ViewportHelper.ValueOrNull<int>(row, "num_premises"),
                NumRooms = ViewportHelper.ValueOrNull<int>(row, "num_rooms"),
                NumApartments = ViewportHelper.ValueOrNull<int>(row, "num_apartments"),
                NumSharedApartments = ViewportHelper.ValueOrNull<int>(row, "num_shared_apartments"),
                LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area"),
                TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area"),
                CadastralNum = ViewportHelper.ValueOrNull(row, "cadastral_num"),
                CadastralCost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost"),
                BalanceCost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                StartupYear = ViewportHelper.ValueOrNull<int>(row, "startup_year"),
                Improvement = ViewportHelper.ValueOrNull<bool>(row, "improvement"),
                Elevator = ViewportHelper.ValueOrNull<bool>(row, "elevator"),
                RubbishChute = ViewportHelper.ValueOrNull<bool>(row, "rubbish_chute"),
                Wear = ViewportHelper.ValueOrNull<double>(row, "wear"),
                StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date"),
                Plumbing = ViewportHelper.ValueOrNull<bool>(row, "plumbing"),
                HotWaterSupply = ViewportHelper.ValueOrNull<bool>(row, "hot_water_supply"),
                Canalization = ViewportHelper.ValueOrNull<bool>(row, "canalization"),
                Electricity = ViewportHelper.ValueOrNull<bool>(row, "electricity"),
                RadioNetwork = ViewportHelper.ValueOrNull<bool>(row, "radio_network"),
                IdHeatingType = ViewportHelper.ValueOrNull<int>(row, "id_heating_type"),
                RoomsBTI = ViewportHelper.ValueOrNull(row, "BTI_rooms"),
                HousingCooperative = ViewportHelper.ValueOrNull(row, "housing_cooperative")
            };
        }

        public static Building FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static void FillRow(Building building, DataRowView row)
        {
            row.BeginEdit();
            row["id_building"] = ViewportHelper.ValueOrDbNull(building.IdBuilding);
            row["id_state"] = ViewportHelper.ValueOrDbNull(building.IdState);
            row["id_structure_type"] = ViewportHelper.ValueOrDbNull(building.IdStructureType);
            row["id_street"] = ViewportHelper.ValueOrDbNull(building.IdStreet);
            row["house"] = ViewportHelper.ValueOrDbNull(building.House);
            row["floors"] = ViewportHelper.ValueOrDbNull(building.Floors);
            row["num_premises"] = ViewportHelper.ValueOrDbNull(building.NumPremises);
            row["num_rooms"] = ViewportHelper.ValueOrDbNull(building.NumRooms);
            row["num_apartments"] = ViewportHelper.ValueOrDbNull(building.NumApartments);
            row["num_shared_apartments"] = ViewportHelper.ValueOrDbNull(building.NumSharedApartments);
            row["cadastral_num"] = ViewportHelper.ValueOrDbNull(building.CadastralNum);
            row["cadastral_cost"] = ViewportHelper.ValueOrDbNull(building.CadastralCost);
            row["balance_cost"] = ViewportHelper.ValueOrDbNull(building.BalanceCost);
            row["description"] = ViewportHelper.ValueOrDbNull(building.Description);
            row["startup_year"] = ViewportHelper.ValueOrDbNull(building.StartupYear);
            row["improvement"] = ViewportHelper.ValueOrDbNull(building.Improvement);
            row["elevator"] = ViewportHelper.ValueOrDbNull(building.Elevator);
            row["rubbish_chute"] = ViewportHelper.ValueOrDbNull(building.RubbishChute);
            row["living_area"] = ViewportHelper.ValueOrDbNull(building.LivingArea);
            row["total_area"] = ViewportHelper.ValueOrDbNull(building.TotalArea);
            row["wear"] = ViewportHelper.ValueOrDbNull(building.Wear);
            row["state_date"] = ViewportHelper.ValueOrDbNull(building.StateDate);
            row["plumbing"] = ViewportHelper.ValueOrDbNull(building.Plumbing);
            row["hot_water_supply"] = ViewportHelper.ValueOrDbNull(building.HotWaterSupply);
            row["canalization"] = ViewportHelper.ValueOrDbNull(building.Canalization);
            row["electricity"] = ViewportHelper.ValueOrDbNull(building.Electricity);
            row["radio_network"] = ViewportHelper.ValueOrDbNull(building.RadioNetwork);
            row["id_heating_type"] = ViewportHelper.ValueOrDbNull(building.IdHeatingType);
            row["BTI_rooms"] = ViewportHelper.ValueOrDbNull(building.RoomsBTI);
            row["housing_cooperative"] = ViewportHelper.ValueOrDbNull(building.HousingCooperative);
            row.EndEdit();
        }
    }
}
