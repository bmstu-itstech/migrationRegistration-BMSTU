
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

        public static async Task<Dictionary<int, TimeUnit>> GetFreeEntries(DateOnly date, Services service)
        {
            // Id, item
            // id - порядковый номер юнита в БД

            Dictionary<int, TimeUnit> free_entries = new Dictionary<int, TimeUnit>();

            //Забираем только уже свободные юниты

            //08/21/2023 mm/dd/yyyy -> dd:mm:yyyy

            string curr_date = $"0{date.Day}.0{date.Month}";

            if (date.Month >= 10 && date.Day >= 10)
                curr_date = $"{date.Day}.{date.Month}";
            if (date.Month >= 10 && !(date.Day >= 10))
                curr_date = $"0{date.Day}.{date.Month}";
            if (!(date.Month >= 10) && date.Day >= 10)
                curr_date = $"{date.Day}.0{date.Month}";



            string table_name = $"{curr_date}.{date.Year}";

            if (service != Services.DOCUMENTS)
            {
                using (MigroBotContext db = new MigroBotContext())
                {

                    var entries = db.Entries.Where(entry => entry.Date.Value.Day == date.Day && entry.Date.Value.Month == date.Month).
                    Where(entry => entry.Service != (int)Services.DOCUMENTS).ToList();

                    if (entries.Count() >= 20)
                        return free_entries;

                }
            }
            string select = $"SELECT * FROM \"{table_name}\" WHERE count < 3;";

            using (var conn = new NpgsqlConnection(Data.Strings.Tokens.SqlConnection))
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
        public static async Task TookEntryPlace(DateOnly date, TimeSpan time)
        {
            string curr_date = $"0{date.Day}.0{date.Month}";

            if (date.Month >= 10 && date.Day >= 10)
                curr_date = $"{date.Day}.{date.Month}";
            if (date.Month >= 10 && !(date.Day >= 10))
                curr_date = $"0{date.Day}.{date.Month}";
            if (!(date.Month >= 10) && date.Day >= 10)
                curr_date = $"{date.Day}.0{date.Month}";

            string update = $"UPDATE \"{curr_date}.{date.Year}\" SET count = count + 1 WHERE time = '{time.ToString()}'";


            using (var conn = new NpgsqlConnection(Data.Strings.Tokens.SqlConnection))
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
            string curr_date = $"0{date.Day}.0{date.Month}";

            if (date.Month >= 10 && date.Day >= 10)
                curr_date = $"{date.Day}.{date.Month}";
            if (date.Month >= 10 && !(date.Day >= 10))
                curr_date = $"0{date.Day}.{date.Month}";
            if (!(date.Month >= 10) && date.Day >= 10)
                curr_date = $"{date.Day}.0{date.Month}";

            string update = $"UPDATE \"{curr_date}.{date.Year}\" SET count = count - 1 WHERE time = '{time.ToString()}'";

            using (var conn = new NpgsqlConnection(Data.Strings.Tokens.SqlConnection))
            {
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand(update, conn))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task<TimeUnit> GetTimeUnit(DateOnly date, TimeSpan time)
        {
            string curr_date = $"0{date.Day}.0{date.Month}";

            if (date.Month >= 10 && date.Day >= 10)
                curr_date = $"{date.Day}.{date.Month}";
            if (date.Month >= 10 && !(date.Day >= 10))
                curr_date = $"0{date.Day}.{date.Month}";
            if (!(date.Month >= 10) && date.Day >= 10)
                curr_date = $"{date.Day}.0{date.Month}";

            string select = $"SELECT * FROM \"{curr_date}.{date.Year}\" WHERE time = {time.ToString()}";

            using (var conn = new NpgsqlConnection(Data.Strings.Tokens.SqlConnection))
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
