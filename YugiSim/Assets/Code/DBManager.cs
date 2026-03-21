using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

static public class DBManager
{
    private struct DBConstants
    {
        public struct CARD
        {
            public const string TABLE_NAME = "Card";
            public struct COLUMNS
            {
                public const string ID = "Id";
            }
        }

        public struct CARD_SET
        {
            public const string TABLE_NAME = "Card_Set";
            public struct COLUMNS
            {
                public const string RARITY = "Rarity";
            }
        }

        public const string SET = "YugiohSet";
    }

    private const string dbName = "URI=file:YugiSim.db";

    static public Dictionary<string, List<int>> GetCardsFromSet(string setName)
    {
        Dictionary<string, List<int>> cardIdsByRarity = new Dictionary<string, List<int>>();

        using (SqliteConnection connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT {DBConstants.CARD.COLUMNS.ID}, {DBConstants.CARD_SET.COLUMNS.RARITY} FROM {DBConstants.CARD.TABLE_NAME} JOIN Card_Set on Card.Id = Card_Set.CardId WHERE Card_Set.SetCode = '{setName}'";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string rarity = Convert.ToString(reader[DBConstants.CARD_SET.COLUMNS.RARITY]);

                        if (!cardIdsByRarity.ContainsKey(rarity))
                            cardIdsByRarity[rarity] = new List<int>();

                        cardIdsByRarity[rarity].Add(Convert.ToInt32(reader[DBConstants.CARD.COLUMNS.ID]));
                    }
                }
            }

            connection.Close();
        }

        return cardIdsByRarity;
    }
}
