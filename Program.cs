
global using MigrationBot;
global using Npgsql;
global using MigrationBot.Data;
global using MigrationBot.Models;
global using MigrationBot.Types;
global using Google.Apis.Auth.OAuth2;
global using Google.Apis.Services;
global using Google.Apis.Sheets.v4;
global using Google.Apis.Sheets.v4.Data;
global using Microsoft.VisualBasic;
global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.EntityFrameworkCore;
global using Telegram.Bot;
global using Telegram.Bot.Types.ReplyMarkups;
global using static MigrationBot.Types.Enums;
global using Telegram.Bot.Types;
global using Telegram.Bot.Types.Enums;
global using Telegram.Bot.Polling;

//Functions.CreateDateTables();
//Functions.DropDateTables();
//Functions.AppendSheets();

//Functions.DropSheets();

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




