using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class SubPremiseConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_sub_premises"], 
                row["id_premises"], 
                row["id_state"], 
                row["sub_premises_num"], 
                row["total_area"],
                row["living_area"],
                row["description"],
                row["state_date"],
                row["cadastral_num"],
                row["cadastral_cost"],
                row["balance_cost"],
                row["account"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static SubPremise FromRow(DataRow row)
        {
            return new SubPremise
            {
                IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises"),
                IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                SubPremisesNum = ViewportHelper.ValueOrNull(row, "sub_premises_num"),
                TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area"),
                LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date"),
                CadastralNum = ViewportHelper.ValueOrNull(row, "cadastral_num"),
                CadastralCost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost"),
                BalanceCost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost"),
                Account = ViewportHelper.ValueOrNull(row, "account")
            };
        }

        public static SubPremise FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static SubPremise FromRow(DataGridViewRow row)
        {
            return new SubPremise
            {
                IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises"),
                IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                SubPremisesNum = ViewportHelper.ValueOrNull(row, "sub_premises_num"),
                TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area"),
                LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date"),
                CadastralNum = ViewportHelper.ValueOrNull(row, "cadastral_num"),
                CadastralCost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost"),
                BalanceCost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost"),
                Account = ViewportHelper.ValueOrNull(row, "account")
            };
        }

        public static void FillRow(SubPremise subPremise, DataRow row)
        {
            row["id_sub_premises"] = ViewportHelper.ValueOrDbNull(subPremise.IdSubPremises);
            row["id_premises"] = ViewportHelper.ValueOrDbNull(subPremise.IdPremises);
            row["id_state"] = ViewportHelper.ValueOrDbNull(subPremise.IdState);
            row["sub_premises_num"] = ViewportHelper.ValueOrDbNull(subPremise.SubPremisesNum);
            row["total_area"] = ViewportHelper.ValueOrDbNull(subPremise.TotalArea);
            row["living_area"] = ViewportHelper.ValueOrDbNull(subPremise.LivingArea);
            row["description"] = ViewportHelper.ValueOrDbNull(subPremise.Description);
            row["state_date"] = ViewportHelper.ValueOrDbNull(subPremise.StateDate);
            row["cadastral_num"] = ViewportHelper.ValueOrDbNull(subPremise.CadastralNum);
            row["cadastral_cost"] = ViewportHelper.ValueOrDbNull(subPremise.CadastralCost);
            row["balance_cost"] = ViewportHelper.ValueOrDbNull(subPremise.BalanceCost);
            row["account"] = ViewportHelper.ValueOrDbNull(subPremise.Account);
        }
    }
}
