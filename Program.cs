
using MigrationBot;
using Npgsql;
using MigrationBot.Data;
using MigrationBot.Models;

//Functions.DropDateTables();
//Functions.CreateDateTables();

string fio = "Митрошкин Alex";

Console.WriteLine(Functions.isRuFioValid(fio));
Console.WriteLine(Functions.isEnFioValid(fio));

while (true)
{
    try
    {

        // await Bot.Start();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);

        throw;
    }
}


