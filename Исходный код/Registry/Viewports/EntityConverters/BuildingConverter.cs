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
            row["id_building"] = ViewportHelper.ValueOrDBNull(building.IdBuilding);
            row["id_state"] = ViewportHelper.ValueOrDBNull(building.IdState);
            row["id_structure_type"] = ViewportHelper.ValueOrDBNull(building.IdStructureType);
            row["id_street"] = ViewportHelper.ValueOrDBNull(building.IdStreet);
            row["house"] = ViewportHelper.ValueOrDBNull(building.House);
            row["floors"] = ViewportHelper.ValueOrDBNull(building.Floors);
            row["num_premises"] = ViewportHelper.ValueOrDBNull(building.NumPremises);
            row["num_rooms"] = ViewportHelper.ValueOrDBNull(building.NumRooms);
            row["num_apartments"] = ViewportHelper.ValueOrDBNull(building.NumApartments);
            row["num_shared_apartments"] = ViewportHelper.ValueOrDBNull(building.NumSharedApartments);
            row["cadastral_num"] = ViewportHelper.ValueOrDBNull(building.CadastralNum);
            row["cadastral_cost"] = ViewportHelper.ValueOrDBNull(building.CadastralCost);
            row["balance_cost"] = ViewportHelper.ValueOrDBNull(building.BalanceCost);
            row["description"] = ViewportHelper.ValueOrDBNull(building.Description);
            row["startup_year"] = ViewportHelper.ValueOrDBNull(building.StartupYear);
            row["improvement"] = ViewportHelper.ValueOrDBNull(building.Improvement);
            row["elevator"] = ViewportHelper.ValueOrDBNull(building.Elevator);
            row["rubbish_chute"] = ViewportHelper.ValueOrDBNull(building.RubbishChute);
            row["living_area"] = ViewportHelper.ValueOrDBNull(building.LivingArea);
            row["total_area"] = ViewportHelper.ValueOrDBNull(building.TotalArea);
            row["wear"] = ViewportHelper.ValueOrDBNull(building.Wear);
            row["state_date"] = ViewportHelper.ValueOrDBNull(building.StateDate);
            row["plumbing"] = ViewportHelper.ValueOrDBNull(building.Plumbing);
            row["hot_water_supply"] = ViewportHelper.ValueOrDBNull(building.HotWaterSupply);
            row["canalization"] = ViewportHelper.ValueOrDBNull(building.Canalization);
            row["electricity"] = ViewportHelper.ValueOrDBNull(building.Electricity);
            row["radio_network"] = ViewportHelper.ValueOrDBNull(building.RadioNetwork);
            row["id_heating_type"] = ViewportHelper.ValueOrDBNull(building.IdHeatingType);
            row["BTI_rooms"] = ViewportHelper.ValueOrDBNull(building.RoomsBTI);
            row["housing_cooperative"] = ViewportHelper.ValueOrDBNull(building.HousingCooperative);
            row.EndEdit();
        }
    }
}
