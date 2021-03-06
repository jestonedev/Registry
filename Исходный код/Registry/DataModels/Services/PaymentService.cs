﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Registry.Entities;
using Registry.DataModels.DataModels;

namespace Registry.DataModels.Services
{
    public sealed class PaymentService
    {
        public static IEnumerable<int> GetAccountIdsByPremiseFilter(string whereStatement)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT * FROM payments_account_premises_assoc WHERE {0}", whereStatement);
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsBySubPremiseFilter(string whereStatement)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT * FROM payments_account_sub_premises_assoc WHERE {0}", whereStatement);
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsByStreet(string idStreet)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT id_account
                                    FROM payments_accounts pa
                                    WHERE pa.id_account IN (
                                      SELECT v.id_account
                                      FROM
                                      (SELECT sp.id_premises, paspa.id_account
                                      FROM payments_account_sub_premises_assoc paspa
                                      INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                      UNION ALL
                                      SELECT papa.id_premises, papa.id_account
                                      FROM payments_account_premises_assoc papa) v
                                      INNER JOIN premises p ON p.id_premises = v.id_premises
                                      INNER JOIN buildings b ON p.id_building = b.id_building
                                      WHERE b.id_street = ?)";
                command.Parameters.Add(DBConnection.CreateParameter("id_street", idStreet));
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsByRegion(string idRegion)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT id_account
                                    FROM payments_accounts pa
                                    WHERE pa.id_account IN (
                                      SELECT v.id_account
                                      FROM
                                      (SELECT sp.id_premises, paspa.id_account
                                      FROM payments_account_sub_premises_assoc paspa
                                      INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                      UNION ALL
                                      SELECT papa.id_premises, papa.id_account
                                      FROM payments_account_premises_assoc papa) v
                                      INNER JOIN premises p ON p.id_premises = v.id_premises
                                      INNER JOIN buildings b ON p.id_building = b.id_building
                                      WHERE b.id_street LIKE ?)";
                command.Parameters.Add(DBConnection.CreateParameter("id_street", idRegion+"%"));
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsByHouse(string house)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT id_account
                                    FROM payments_accounts pa
                                    WHERE pa.id_account IN (
                                      SELECT v.id_account
                                      FROM
                                      (SELECT sp.id_premises, paspa.id_account
                                      FROM payments_account_sub_premises_assoc paspa
                                      INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                      UNION ALL
                                      SELECT papa.id_premises, papa.id_account
                                      FROM payments_account_premises_assoc papa) v
                                      INNER JOIN premises p ON p.id_premises = v.id_premises
                                      INNER JOIN buildings b ON p.id_building = b.id_building
                                      WHERE b.house = ?)";
                command.Parameters.Add(DBConnection.CreateParameter("house", house));
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsByPremiseNumber(string premiseNumber)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT id_account
                                    FROM payments_accounts pa
                                    WHERE pa.id_account IN (
                                      SELECT v.id_account
                                      FROM
                                      (SELECT sp.id_premises, paspa.id_account
                                      FROM payments_account_sub_premises_assoc paspa
                                      INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                      UNION ALL
                                      SELECT papa.id_premises, papa.id_account
                                      FROM payments_account_premises_assoc papa) v
                                      INNER JOIN premises p ON p.id_premises = v.id_premises
                                      WHERE p.premises_num = ?)";
                command.Parameters.Add(DBConnection.CreateParameter("premises_num", premiseNumber));
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsByPaymentFilter(string whereStatement)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT id_account FROM payments WHERE {0}", whereStatement);
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account")).Distinct();
            }
        }

        public static IEnumerable<int> GetPremisesIdsByAccountFilter(string whereStatement)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format(@"SELECT v.id_premises, v.object_type
                    FROM (
                    SELECT papa.id_premises, 1 as object_type, pa.id_account
                    FROM payments_accounts pa
                        INNER JOIN payments_account_premises_assoc papa ON pa.id_account = papa.id_account
                    UNION ALL
                    SELECT sp.id_premises, 2, pa.id_account
                    FROM payments_accounts pa
                        INNER JOIN payments_account_sub_premises_assoc paspa ON pa.id_account = paspa.id_account
                        INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises) v
                    WHERE {0}", whereStatement);
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_premises"));
            }
        }

        public static int? GetJudgeByIdAccount(int idAccount)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT jba.id_judge
                                        FROM (
                                        SELECT p.id_building, papa.id_account
                                        FROM payments_account_premises_assoc papa
                                          INNER JOIN premises p ON papa.id_premises = p.id_premises
                                        WHERE p.deleted <> 1
                                        UNION ALL
                                        SELECT p.id_building, paspa.id_account
                                        FROM payments_account_sub_premises_assoc paspa
                                          INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                          INNER JOIN premises p ON sp.id_premises = p.id_premises
                                        WHERE sp.deleted <> 1) v
                                          INNER JOIN judges_buildings_assoc jba ON v.id_building = jba.id_building
                                        WHERE v.id_account = ?
                                        LIMIT 1";
                command.Parameters.Add(DBConnection.CreateParameter("id_account", idAccount));
                return connection.SqlSelectTable("judge", command).AsEnumerable().Select(row => row.Field<int?>("id_judge")).FirstOrDefault();
            }
        }

        public static IEnumerable<ClaimPerson> GetClaimPersonsByIdAccount(int idAccount)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT tp.surname, tp.name, tp.patronymic, tp.date_of_birth, 
                                            IF(tp.id_kinship = 1, 1, 0) AS is_claimer
                                        FROM (
                                        SELECT * FROM (
                                        SELECT tp.id_process, tp.registration_num, tp.registration_date, papa.id_account
                                        FROM payments_account_premises_assoc papa
                                            INNER JOIN premises p ON papa.id_premises = p.id_premises
                                            INNER JOIN tenancy_premises_assoc tpa ON p.id_premises = tpa.id_premises
                                            INNER JOIN tenancy_processes tp ON tpa.id_process = tp.id_process
                                        WHERE p.deleted <> 1 AND tp.deleted <> 1 AND tpa.deleted <> 1
                                        UNION ALL
                                        SELECT tp.id_process, tp.registration_num, tp.registration_date, paspa.id_account
                                        FROM payments_account_sub_premises_assoc paspa
                                            INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                            INNER JOIN tenancy_sub_premises_assoc tspa ON sp.id_sub_premises = tspa.id_sub_premises
                                            INNER JOIN tenancy_processes tp ON tspa.id_process = tp.id_process
                                        WHERE sp.deleted <> 1 AND tp.deleted <> 1 AND tspa.deleted <> 1) v
                                        WHERE v.id_account = ? AND (v.registration_num IS NULL OR v.registration_num NOT LIKE '%н')
                                        ORDER BY v.registration_date DESC
                                        LIMIT 1) v INNER JOIN tenancy_persons tp
                                            ON v.id_process = tp.id_process
                                        WHERE tp.deleted <> 1";
                command.Parameters.Add(DBConnection.CreateParameter("id_account", idAccount));
                return connection.SqlSelectTable("persons", command).AsEnumerable().Select(row => 
                    new ClaimPerson
                    {
                        Surname = row.Field<string>("surname"),
                        Name = row.Field<string>("name"),
                        Patronymic = row.Field<string>("patronymic"),
                        DateOfBirth = row.Field<DateTime?>("date_of_birth"),
                        IsClaimer = row.Field<long>("is_claimer") == 1,
                    });
            }
        }

        

        public static void UpdateJudgeInfoByIdAccount(int? idAccount, int? idJudge)
        {
            var dataModel = EntityDataModel<JudgeBuildingAssoc>.GetInstance();
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT v.id_building
                                        FROM (
                                        SELECT p.id_building, papa.id_account
                                        FROM payments_account_premises_assoc papa
                                            INNER JOIN premises p ON papa.id_premises = p.id_premises
                                        WHERE p.deleted <> 1
                                        UNION ALL
                                        SELECT p.id_building, paspa.id_account
                                        FROM payments_account_sub_premises_assoc paspa
                                            INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                            INNER JOIN premises p ON sp.id_premises = p.id_premises
                                        WHERE sp.deleted <> 1) v
                                        WHERE v.id_account = ?
                                        LIMIT 1";
                command.Parameters.Add(DBConnection.CreateParameter("id_account", idAccount));
                var idBuilding = connection.SqlSelectTable("building", command).AsEnumerable().Select(
                    row => row.Field<int?>("id_building")).FirstOrDefault();
                if (idBuilding == null)
                {
                    return;
                }
                var judgeBuildingAssoc = dataModel.FilterDeletedRows()
                    .FirstOrDefault(r => r.Field<int>("id_building") == idBuilding);
                if (judgeBuildingAssoc != null)
                {
                    if (EntityDataModel<JudgeBuildingAssoc>.GetInstance().Update(new JudgeBuildingAssoc
                    {
                        IdAssoc = (int) judgeBuildingAssoc["id_assoc"],
                        IdBuilding = idBuilding,
                        IdJudge = idJudge
                    }) == -1)
                    {
                        return;
                    }
                    judgeBuildingAssoc["id_judge"] = idJudge;
                    judgeBuildingAssoc.EndEdit();
                }
                else
                {
                    // Insert
                    var id = EntityDataModel<JudgeBuildingAssoc>.GetInstance().Insert(new JudgeBuildingAssoc {
                        IdBuilding = idBuilding,
                        IdJudge = idJudge
                    });
                    if (id == -1)
                    {
                        return;
                    }
                    dataModel.Select().Rows.Add(id, idJudge, idBuilding);
                }
            }

        }

        public static DataTable GetBalanceInfoOnDate(IEnumerable<int> idAccounts, int year, int month)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText =
                    string.Format(@"SELECT p.id_account, p.balance_output_tenancy, p.balance_output_dgi,  p.balance_output_padun,  
                        p.balance_output_pkk, p.balance_output_penalties
                        FROM payments p
                        WHERE p.id_account IN (0{0}) AND MONTH(p.date) = ? AND YEAR(p.date) = ?",
                        idAccounts.Select(v => v.ToString()).Aggregate(
                            (acc, v) => acc + "," + v));
                command.Parameters.Add(DBConnection.CreateParameter("month", month));
                command.Parameters.Add(DBConnection.CreateParameter("year", year));
                return connection.SqlSelectTable("balance_info", command);
            }
        }
    }
}
