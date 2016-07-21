using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class PremiseConverter
    {
        public static void FillRow(Premise premise, DataRowView row)
        {
            row.BeginEdit();
            row["id_premises"] = ViewportHelper.ValueOrDBNull(premise.IdPremises);
            row["id_building"] = ViewportHelper.ValueOrDBNull(premise.IdBuilding);
            row["id_state"] = ViewportHelper.ValueOrDBNull(premise.IdState);
            row["premises_num"] = ViewportHelper.ValueOrDBNull(premise.PremisesNum);
            row["total_area"] = ViewportHelper.ValueOrDBNull(premise.TotalArea);
            row["living_area"] = ViewportHelper.ValueOrDBNull(premise.LivingArea);
            row["height"] = ViewportHelper.ValueOrDBNull(premise.Height);
            row["num_rooms"] = ViewportHelper.ValueOrDBNull(premise.NumRooms);
            row["num_beds"] = ViewportHelper.ValueOrDBNull(premise.NumBeds);
            row["id_premises_type"] = ViewportHelper.ValueOrDBNull(premise.IdPremisesType);
            row["id_premises_kind"] = ViewportHelper.ValueOrDBNull(premise.IdPremisesKind);
            row["floor"] = ViewportHelper.ValueOrDBNull(premise.Floor);
            row["cadastral_num"] = ViewportHelper.ValueOrDBNull(premise.CadastralNum);
            row["cadastral_cost"] = ViewportHelper.ValueOrDBNull(premise.CadastralCost);
            row["balance_cost"] = ViewportHelper.ValueOrDBNull(premise.BalanceCost);
            row["description"] = ViewportHelper.ValueOrDBNull(premise.Description);
            row["reg_date"] = ViewportHelper.ValueOrDBNull(premise.RegDate);
            row["is_memorial"] = ViewportHelper.ValueOrDBNull(premise.IsMemorial);
            row["account"] = ViewportHelper.ValueOrDBNull(premise.Account);
            row["state_date"] = ViewportHelper.ValueOrDBNull(premise.StateDate);
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
