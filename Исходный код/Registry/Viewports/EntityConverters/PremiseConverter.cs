using System;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class PremiseConverter
    {
        public static void FillRow(Premise premise, DataRowView row)
        {
            row.BeginEdit();
            row["id_premises"] = ViewportHelper.ValueOrDbNull(premise.IdPremises);
            row["id_building"] = ViewportHelper.ValueOrDbNull(premise.IdBuilding);
            row["id_state"] = ViewportHelper.ValueOrDbNull(premise.IdState);
            row["premises_num"] = ViewportHelper.ValueOrDbNull(premise.PremisesNum);
            row["total_area"] = ViewportHelper.ValueOrDbNull(premise.TotalArea);
            row["living_area"] = ViewportHelper.ValueOrDbNull(premise.LivingArea);
            row["height"] = ViewportHelper.ValueOrDbNull(premise.Height);
            row["num_rooms"] = ViewportHelper.ValueOrDbNull(premise.NumRooms);
            row["num_beds"] = ViewportHelper.ValueOrDbNull(premise.NumBeds);
            row["id_premises_type"] = ViewportHelper.ValueOrDbNull(premise.IdPremisesType);
            row["id_premises_kind"] = ViewportHelper.ValueOrDbNull(premise.IdPremisesKind);
            row["floor"] = ViewportHelper.ValueOrDbNull(premise.Floor);
            row["cadastral_num"] = ViewportHelper.ValueOrDbNull(premise.CadastralNum);
            row["cadastral_cost"] = ViewportHelper.ValueOrDbNull(premise.CadastralCost);
            row["balance_cost"] = ViewportHelper.ValueOrDbNull(premise.BalanceCost);
            row["description"] = ViewportHelper.ValueOrDbNull(premise.Description);
            row["reg_date"] = ViewportHelper.ValueOrDbNull(premise.RegDate);
            row["is_memorial"] = ViewportHelper.ValueOrDbNull(premise.IsMemorial);
            row["account"] = ViewportHelper.ValueOrDbNull(premise.Account);
            row["state_date"] = ViewportHelper.ValueOrDbNull(premise.StateDate);
            row.EndEdit();
        }

        public static Premise FromRow(DataRow row)
        {
            return new Premise
            {
                IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                IdBuilding = ViewportHelper.ValueOrNull<int>(row, "id_building"),
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                PremisesNum = ViewportHelper.ValueOrNull(row, "premises_num"),
                LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area"),
                TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area"),
                Height = ViewportHelper.ValueOrNull<double>(row, "height"),
                NumRooms = ViewportHelper.ValueOrNull<short>(row, "num_rooms"),
                NumBeds = ViewportHelper.ValueOrNull<short>(row, "num_beds"),
                IdPremisesType = ViewportHelper.ValueOrNull<int>(row, "id_premises_type"),
                IdPremisesKind = ViewportHelper.ValueOrNull<int>(row, "id_premises_kind"),
                Floor = ViewportHelper.ValueOrNull<short>(row, "floor"),
                CadastralNum = ViewportHelper.ValueOrNull(row, "cadastral_num"),
                CadastralCost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost"),
                BalanceCost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                IsMemorial = ViewportHelper.ValueOrNull<bool>(row, "is_memorial"),
                Account = ViewportHelper.ValueOrNull(row, "account"),
                RegDate = ViewportHelper.ValueOrNull<DateTime>(row, "reg_date"),
                StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date")
            };
        }

        public static Premise FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }
    }
}
