using System;
using System.Data.SQLite; 
using System.Threading.Tasks;
using Deedle;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Data;
using System.Diagnostics;

namespace DSharpAPP.db
{
	public class SQL
	{
        static SQLiteConnection _connection = new SQLiteConnection("Data Source=./Data/MySQL.db");

        public static void init()
        {
            _connection.Open();
        }

        public static int Check(string author) // Проверяем процент духоты для author
        {
            var command = _connection.CreateCommand();
            int authorProcent = 0; 

            command.CommandText = $"SELECT procent FROM BotuserData WHERE  author = '{author}'";  
            var reader = command.ExecuteReader();

            if (reader.HasRows)  
            {
                while (reader.Read())   
                {
                    authorProcent = reader.GetInt32(0);
                }
            }

            reader.Close(); 
            return authorProcent;
        }

        public static string[] Checking(string author) // Выводим всю информацию для author
        {
            var command = _connection.CreateCommand();
            string[] authorInfo = new string[]{"", "", "", ""};

            command.CommandText = $"SELECT * FROM BotuserData WHERE  author = '{author}'";
            var reader = command.ExecuteReader();

            if (reader.HasRows)     // если есть данные
            {
                while (reader.Read())   // построчно считываем данные
                {
                    authorInfo[0]  = (string)reader.GetValue(1);
                    authorInfo[1]  = (string)reader.GetValue(2);
                    authorInfo[2]  = reader.GetValue(3).ToString()!;
                    var curseValue  = (string)reader.GetValue(4);

                    authorInfo[3] = string.IsNullOrWhiteSpace(curseValue) ? "--" : curseValue;
                }
            }   

            reader.Close();  // Закрываем ридер
            return authorInfo;
        }

        public static string[] PreyFound(string prey) // Находим всю информацию для prey(жертвы SWAP)
        {
            var command = _connection.CreateCommand();
            string[] preyerInfo = new string[]{"", "", "", ""};

            command.CommandText = $"SELECT * FROM BotuserData WHERE  authorid = '{prey}'";
            var reader = command.ExecuteReader();

            if (reader.HasRows) 
            {
                while (reader.Read())   
                {
                    preyerInfo[0]  = (string)reader.GetValue(1);
                    preyerInfo[1]  = (string)reader.GetValue(2);
                    preyerInfo[2]  = reader.GetValue(3).ToString()!;
                    preyerInfo[3]  = (string)reader.GetValue(4);
                }
            }   

            reader.Close(); 
            return preyerInfo;
        }

        public static List<string[]> Select() // Находим всю информацию для команды List
        {
            List<string[]> allInfo = new List<string[]>();
            var command = _connection.CreateCommand();
            
            command.CommandText = "SELECT * FROM BotuserData";
            var reader = command.ExecuteReader();
       
            if (reader.HasRows) 
            {
                while (reader.Read())    
                {
                    var author  = reader.GetValue(1).ToString()!;
                    var authorid  = reader.GetValue(2).ToString()!;
                    var procent  = reader.GetValue(3).ToString()!;
                    var curseValue  = reader.GetValue(4).ToString()!;
                    
                    var curse = string.IsNullOrWhiteSpace(curseValue) ? "--" : curseValue;

                    string[] infoArray = new string[] {author, procent, curse};
                    allInfo.Add(infoArray);
                }
            }
                    
            reader.Close(); 
            return allInfo;    
        }

        public static void AddProcent(string author, string authorid, int rate) // Добавляем в ТАБЛИЦУ данные об author после вызова STUFFY
        { 
            var command = _connection.CreateCommand();
            command.CommandText = $"INSERT INTO BotuserData (author, authorid, procent, curse) VALUES ('{author}', '{authorid}', {rate}, '')";
            var reader = command.ExecuteReader();

            reader.Close(); 
        }

        public static void AddCurse(string author, string who)  // Добавляем обзывалку для author
        {
            var command = _connection.CreateCommand();
            command.CommandText =  $"UPDATE BotuserData SET curse = '{who}' WHERE  authorid = '{author}'";
            var reader = command.ExecuteReader();

            reader.Close(); 
        }

        public static List<string> SelectMoney()  // Находим вчерашний курс валют
        {
            List<string> allInfo = new List<string>();
            var command = _connection.CreateCommand();
            
            command.CommandText = "SELECT * FROM moneyRates";
            var reader = command.ExecuteReader();
       
            if (reader.HasRows) 
            {
                while (reader.Read())    
                {
                    var  currencyValue  = reader.GetValue(2).ToString()!;

                    allInfo.Add(currencyValue);
                }
            }

            reader.Close(); 
            return allInfo;    
        }

        public static void AddMoney(decimal[] newRate)  // Обновляем курс валют на сегодняшний
        {
            var command = _connection.CreateCommand();
            string[] myCurrency = new string[]{"eur", "usd", "cny", "uah", "kzt"};
    
            for (int i = 0; i < 5; i++)
            {
                string formattedRate = newRate[i].ToString("0.00", CultureInfo.GetCultureInfo("en-US"));
                command.CommandText =  $"UPDATE moneyRates SET value = {formattedRate}  WHERE  currency = '{myCurrency[i]}'";
                var reader = command.ExecuteReader();
                reader.Close(); 
            }
        }

        public static void UpdateInventery(string author, string loot) // Обновляем инвентарь жертвы "author" после вручения ему подарка
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"UPDATE Allusers SET inventory = inventory ||  '_{loot}' WHERE  author = '{author}'";
            var reader = command.ExecuteReader();
            reader.Close();    
        }

        public static void Updatebalanse(string author, double salary) // Обновляем баланс "author" после покупки чего-либо
        {
            var command = _connection.CreateCommand();
            string formattedSalary = salary.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));
            
            command.CommandText = $"UPDATE Allusers SET balanse = balanse + {formattedSalary} WHERE  author = '{author}'";
            var reader = command.ExecuteReader();
            reader.Close();    
        }

        public static string[] CheckBalanse(string author) // Обновляем баланс "author" хватает ли денег на покупку  чего-либо
        {
            string[] MyBalanse = new string [2];
            var command = _connection.CreateCommand();
            command.CommandText = $"SELECT balanse, inventory FROM Allusers WHERE  author = '{author}'";
            var reader = command.ExecuteReader();
            
            if (reader.HasRows) 
            {
                while (reader.Read())    
                {
                    var balanse = reader.GetValue(0).ToString()!;
                    var inventory = reader.GetValue(1).ToString()!.Replace("_", "\n");

                    MyBalanse[0] = balanse;
                    MyBalanse[1] = inventory;
                }
            }
            
            reader.Close();
            return MyBalanse;
        }

        public static string[] PriceMap(string goods) // Проверяем цену  каждого из ивентов "goods"
        {
            var command = _connection.CreateCommand();
            string[] priceList = new string[3];
            
            command.CommandText = $"SELECT * FROM CoffeeMachine WHERE  goods = '{goods}'";
            var reader = command.ExecuteReader();
            
            if (reader.HasRows) 
            {
                while (reader.Read())    
                {
                    var product = reader.GetValue(1).ToString()!;
                    var price = reader.GetValue(2).ToString()!;
                    var url = reader.GetValue(3).ToString()!;

                    priceList[0] = product;
                    priceList[1] = price;
                    priceList[2] = url;
                }
            }
            
            reader.Close();
            return priceList;
        }

        public static void AddConnectInfo(string userId, string guildId)
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"INSERT INTO connection_info(user_id, guild_id, conn_data) VALUES('{userId}', '{guildId}', strftime('%d-%m-%Y', 'now'))";
            var reader = command.ExecuteReader();
            reader.Close();
        }

        public static void UodateConnectInfo(string userId, string guildId)
        {
            var command = _connection.CreateCommand();
            
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@guildId", guildId);

            command.CommandText = "UPDATE connection_info SET conn_data = strftime('%d-%m-%Y', 'now') WHERE user_id = @userId AND guild_id = @guildId";

            var reader = command.ExecuteReader();
            reader.Close();
        }

        public static List<string[]>  GetConnectInfo(string guildId)
        {
            var command = _connection.CreateCommand();

            List<string[]> connect_info = new List<string[]>();

            command.CommandText = $"SELECT user_id, conn_data FROM connection_info WHERE guild_id = '{guildId}'";
            var reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    int userIdOrdinal = reader.GetOrdinal("user_id");
                    int connDataOrdinal = reader.GetOrdinal("conn_data");

                    string userId = reader.GetString(userIdOrdinal);
                    string dateString = reader.GetString(connDataOrdinal);

                    string[] currencyValue = { userId, dateString };
                    connect_info.Add(currencyValue);
                }
            }

            reader.Close();
            return connect_info;
        }
    }
}
