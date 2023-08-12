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

        public static async Task<Dictionary<int, TimeItem>> GetFreeEntries(DateOnly date)
        {
            // Id, item
            // id - порядковый номер юнита в БД

            Dictionary<int,TimeItem> free_entries = new Dictionary<int, TimeItem> ();   
       
            //Забираем только уже свободные юниты
            string select = $"SELECT * FROM \"{date.ToString()}\" WHERE count < 3;";

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


                            free_entries.Add(time_item.Id,time_item);
                            
                        }


                      
                    }
                }
            };

            return free_entries;
        }
        public async Task AddWorkLoad(DateOnly date)
        {
            string update = $"UPDATE \"{date.ToString()}\" SET count = count + 1 WHERE id = {this.Id}";

            using (var conn = new NpgsqlConnection(Strings.Tokens.SqlConnection))
            {
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand(update, conn))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

    }
}
