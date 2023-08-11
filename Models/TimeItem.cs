using MigrationBot.Data;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationBot.Models
{
    internal class TimeItem
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public int Count { get; set; }
        public bool isFree
        {
            get
            {
                return Count < 3 ? true : false;
            }
        }

        public static async Task<List<TimeItem>> GetFreeEntries(DateOnly date)
        {
            List <TimeItem> output = new List<TimeItem>();  

            string table_name = $"\"{date.ToString()}\"";
            Console.WriteLine(table_name);  
            string select = $"SELECT * FROM {table_name} WHERE count < 3;";

            using (var conn = new NpgsqlConnection(Strings.Tokens.SqlConnection))
            {
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand(select, conn))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {

                        while(await reader.ReadAsync())
                        {
                            var time_item = new TimeItem();
 
                            time_item.Id = reader.GetInt32(0);
                            time_item.Time = reader.GetString(1);
                            time_item.Count = reader.GetInt32(2);

                            output.Add(time_item);
                        }


                      
                    }
                }
            };

            return output;
        }

    }
}
