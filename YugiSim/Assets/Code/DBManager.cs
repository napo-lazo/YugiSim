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

        public struct COLLECTION
        {
            public const string TABLE_NAME = "Collection";
            public struct COLUMNS
            {
                public const string CARDID = "CardId";
                public const string SETCODE = "SetCode";
                public const string QUANTITY = "Quantity";
            }
        }

        public const string SET = "YugiohSet";
    }

    private const string dbName = "URI=file:YugiSim.db";

    static public Dictionary<string, List<int>> GetCardsFromSet(string setCode)
    {
        Dictionary<string, List<int>> cardIdsByRarity = new Dictionary<string, List<int>>();

        using (SqliteConnection connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT {DBConstants.CARD.COLUMNS.ID}, {DBConstants.CARD_SET.COLUMNS.RARITY} FROM {DBConstants.CARD.TABLE_NAME} JOIN Card_Set on Card.Id = Card_Set.CardId WHERE Card_Set.SetCode = '{setCode}'";
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

    static public void InsertCardsIntoCollection(string setCode, List<int> uniqueCards)
    {
        using (SqliteConnection connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"INSERT INTO {DBConstants.COLLECTION.TABLE_NAME} VALUES";

                foreach(int card in uniqueCards)
                {
                    cmd.CommandText += $" ({card}, '{setCode}', 1),";
                }

                cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 1);

                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    static public void UpdateCardsQuantityFromCollection(string setCode, Dictionary<int, int> cards, int amount)
    {
        using (SqliteConnection connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                string cardList = "(";

                foreach (KeyValuePair<int, int> card in cards)
                {
                    if (card.Value == amount)
                        cardList += $"{card.Key}, ";
                }

                cardList = cardList.Remove(cardList.Length - 2);
                cardList += ")";

                cmd.CommandText = $"UPDATE {DBConstants.COLLECTION.TABLE_NAME} SET {DBConstants.COLLECTION.COLUMNS.QUANTITY} = {DBConstants.COLLECTION.COLUMNS.QUANTITY} + {amount} WHERE {DBConstants.COLLECTION.COLUMNS.SETCODE} = '{setCode}' AND {DBConstants.COLLECTION.COLUMNS.CARDID} IN {cardList}";
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    static public Dictionary<int, int> GetListedCardsFromCollectionBySet(string setCode, List<int> cardIds)
    {
        Dictionary<int, int> cardsInCollection = new Dictionary<int, int>();

        using (SqliteConnection connection = new SqliteConnection(dbName))
        {
            connection.Open();
            
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                string cardsToSearch = string.Join(",", cardIds);
                cmd.CommandText = $"SELECT {DBConstants.COLLECTION.COLUMNS.CARDID}, {DBConstants.COLLECTION.COLUMNS.QUANTITY} FROM {DBConstants.COLLECTION.TABLE_NAME} WHERE {DBConstants.COLLECTION.COLUMNS.SETCODE} == '{setCode}' AND {DBConstants.COLLECTION.COLUMNS.CARDID} IN ({cardsToSearch})";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int cardId = Convert.ToInt32(reader[DBConstants.COLLECTION.COLUMNS.CARDID]);

                        if (!cardsInCollection.ContainsKey(cardId))
                            cardsInCollection[cardId] = Convert.ToInt32(reader[DBConstants.COLLECTION.COLUMNS.QUANTITY]);
                    }
                }
            }

            connection.Close();
        }

        return cardsInCollection;
    }
}
