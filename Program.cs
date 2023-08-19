
using MigrationBot;
using Npgsql;
using MigrationBot.Data;
using MigrationBot.Models;
using MigrationBot.Types;

 //Functions.CreateDateTables();
//Functions.DropDateTables();
Functions.AppendSheets();


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


