using MigrationBot.Data;
using Npgsql;
using Npgsql.Internal.TypeHandlers.DateTimeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationBot.Models
{
    internal class TimeUnit
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

        public static async Task<Dictionary<int, TimeUnit>> GetFreeEntries(DateOnly date)
        {
            // Id, item
            // id - порядковый номер юнита в БД

            Dictionary<int, TimeUnit> free_entries = new Dictionary<int, TimeUnit>();

            //Забираем только уже свободные юниты
            string select = $"SELECT * FROM \"{date.ToString()}\" WHERE count < 3;";

            using (var conn = new NpgsqlConnection(Strings.Tokens.SqlConnection))
            {
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand(select, conn))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            var time_item = new TimeUnit();

                            time_item.Id = reader.GetInt32(0);
                            time_item.Time = reader.GetString(1);
                            time_item.Count = reader.GetInt32(2);


                            free_entries.Add(time_item.Id, time_item);

                        }



                    }
                }
            };

            return free_entries;
        }
        public static async Task TookEntryPlace(DateOnly date,TimeSpan time)
        {
            string update = $"UPDATE \"{date.ToString()}\" SET count = count + 1 WHERE time = {time.ToString()}";

            using (var conn = new NpgsqlConnection(Strings.Tokens.SqlConnection))
            {
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand(update, conn))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public static async Task FreeEntryPlace(DateOnly date, TimeSpan time)
        {
            string update = $"UPDATE \"{date.ToString()}\" SET count = count - 1 WHERE time = {time.ToString()}";

            using (var conn = new NpgsqlConnection(Strings.Tokens.SqlConnection))
            {
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand(update, conn))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task<TimeUnit> GetTimeUnit(DateOnly date,TimeSpan time)
        {
            string select = $"SELECT * FROM \"{date.ToString()}\" WHERE time = {time.ToString()}";

            using (var conn = new NpgsqlConnection(Strings.Tokens.SqlConnection))
            {
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand(select, conn))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            var time_unit = new TimeUnit();

                            time_unit.Id = reader.GetInt32(0);
                            time_unit.Time = reader.GetString(1);
                            time_unit.Count = reader.GetInt32(2);


                            return time_unit;
                        }

                    }
                }
            };

            return null;

        }

    }
}
