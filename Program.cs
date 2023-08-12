
using MigrationBot;
using Npgsql;
using MigrationBot.Data;
using MigrationBot.Models;



CreateDateTables();
//DropDateTables();
while (true)
{
    try
    {

         await Bot.Start();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);

        throw;
    }
}

static async void CreateDateTables()
{
    var date = DateTime.Now;

    for (int i = 0; i < 1; i++)
    {


        var time = new TimeSpan(10, 0, 0);
        var diner_start = new TimeSpan(12, 30, 0);
        var diner_end = new TimeSpan(13, 30, 0);

        string query = $"CREATE TABLE \"{date.ToShortDateString()}\"" +
            $"(" +
            $"id INTEGER PRIMARY KEY," +
            $"time CHARACTER VARYING(255)," +
            $"count INTEGER" +
            $");";

        using (var conn = new NpgsqlConnection(Strings.Tokens.SqlConnection))
        {
            await conn.OpenAsync();

            using (var command = new NpgsqlCommand(query, conn))
            {
                await command.ExecuteNonQueryAsync();
            }
        };


        for (int j = 0; j <= 90; j++)
        {
            var size = new TimeSpan(0, 5, 0);

            int count = 0;

            if (time >= diner_start && time <= diner_end)
                count = 3;

            string insert = $"INSERT INTO \"{date.ToShortDateString()}\" (id,time,count) VALUES({j + 1},'{time}',{count});";

            using (var conn = new NpgsqlConnection(Strings.Tokens.SqlConnection))
            {
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand(insert, conn))
                {
                    await command.ExecuteNonQueryAsync();
                }
            };
            time = time.Add(size);
        }

        date = date.AddDays(1);
    }
}

static async void DropDateTables()
{
    var date = DateTime.Now;

    for(int i = 0; i < 1; i++)
    {
        string drop = $"DROP TABLE \"{date.ToShortDateString()}\"";
        using (var conn = new NpgsqlConnection(Strings.Tokens.SqlConnection))
        {
            await conn.OpenAsync();

            using (var command = new NpgsqlCommand(drop, conn))
            {
                await command.ExecuteNonQueryAsync();
            }
        };
        date = date.AddDays(1);

    }


}

