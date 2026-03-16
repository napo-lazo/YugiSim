using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

static public class DBManager
{
    private struct DBConstants
    {
        public const string CARD = "Card";
        public const string CARD_SET = "Card_Set";
        public const string SET = "YugiohSet";
    }

    private const string dbName = "URI=file:YugiSim.db";

    static public List<int> GetCardsFromSet(string setName)
    {
        List<int> cardIds = new List<int>();

        using (SqliteConnection connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT Id FROM {DBConstants.CARD} JOIN Card_Set on Card.Id = Card_Set.CardId WHERE Card_Set.SetCode = '{setName}'";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cardIds.Add(Convert.ToInt32(reader["Id"]));
                    }
                }
            }

            connection.Close();
        }

        return cardIds;
    }
}
