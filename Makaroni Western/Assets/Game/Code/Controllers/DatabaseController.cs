using UnityEngine;
using System.Collections.Generic;
using SQLite;

public static class DatabaseController
{
    private static string databasePath = $"{Application.dataPath}/StreamingAssets/db.bytes";


    public static List<Results> ReadDatabase()
    {
        using (var db = new SQLiteConnection(databasePath))
        {
            IEnumerable<Results> list = db.Query<Results>("SELECT match_id, status, player_1, player_2, concentration_1, concentration_2, remaining_hp_1, remaining_hp_2, shots_number_1, shots_number_2 FROM match_result");
            List<Results> data = new List<Results>();

            foreach(Results rs in list)
            {
                data.Add(rs);
            }

            db.Close();
            return data;
        }

    }

    public static void AddRecord(Results record)
    {
        using (var db = new SQLiteConnection(databasePath))
        {
            string query = $"INSERT INTO match_result (status, player_1, player_2, concentration_1, concentration_2, remaining_hp_1, remaining_hp_2, shots_number_1, shots_number_2) VALUES ('{record.status}', '{record.player_1}', '{record.player_2}', '{record.concentration_1}', '{record.concentration_2}', '{record.remaining_hp_1}', '{record.remaining_hp_2}', '{record.shots_number_1}', '{record.shots_number_2}')";
            db.Execute(query);
            db.Close();
        }
    }

    public static void Clean()
    {
        using (var db = new SQLiteConnection(databasePath))
        {
            string query = $"DELETE FROM match_result;";
            db.Execute(query);

            query = "UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='match_result'";
            db.Execute(query);
            db.Close();
        }
    }
}

public class Results
{
    [PrimaryKey, AutoIncrement, Unique]
    public int match_id { get; set; }
    public string status { get; set; }
    public string player_1 { get; set; }
    public string player_2 { get; set; }
    public float concentration_1 { get; set; }
    public float concentration_2 { get; set; }
    public float remaining_hp_1 { get; set; }
    public float remaining_hp_2 { get; set; }
    public int shots_number_1 { get; set; }
    public int shots_number_2 { get; set; }

    public Results()
    {

    }

    public Results(NetworkCowboy p1, NetworkCowboy p2, string _status)
    {
        status = _status;

        player_1 = p1.playerName;
        player_2 = p2.playerName;

        concentration_1 = p1.concentration;
        concentration_2 = p2.concentration;

        if (p1.health < 0)
            remaining_hp_1 = 0;
        else
            remaining_hp_1 = p1.health;

        if (p2.health < 0)
            remaining_hp_2 = 0;
        else
            remaining_hp_2 = p2.health;

        shots_number_1 = p1.shots;
        shots_number_2 = p2.shots;
    }

    public Results(PlayerController p1, PlayerController p2, string _status)
    {
        status = _status;

        player_1 = p1.name;
        player_2 = p2.name;

        concentration_1 = p1.concentration;
        concentration_2 = p2.concentration;

        if (p1.health < 0)
            remaining_hp_1 = 0;
        else
            remaining_hp_1 = p1.health;

        if (p2.health < 0)
            remaining_hp_2 = 0;
        else
            remaining_hp_2 = p2.health;

        shots_number_1 = p1.shots;
        shots_number_2 = p2.shots;
    }
}
