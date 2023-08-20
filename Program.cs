
using MigrationBot;
using Npgsql;
using MigrationBot.Data;
using MigrationBot.Models;
using MigrationBot.Types;

//Functions.CreateDateTables();
//Functions.DropDateTables();
//Functions.AppendSheets();
//Functions.DropSheets();

var gw = new GoogleSheetWorker();

DateOnly dt = new DateOnly(2023, 8, 22);
await gw.CleanSheet(dt);

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


