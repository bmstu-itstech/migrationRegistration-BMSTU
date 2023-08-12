using MigrationBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MigrationBot
{
    internal class QueryExecutor
    {
        public static async Task Execute(string query, long chatId, TelegramBotClient bot)
        {
            MyUser user = await MyUser.GetUser(chatId);
            try
            {
                if (query.Contains("setCountry")) await SetCountry(query, chatId, bot, user);
                else if (query.Contains("setService")) await SetService(query, chatId, bot, user);
                else if (query.Contains("DateSelection")) await SelectDate(query, chatId, bot, user);
                else if (query.Contains("SelectDate")) await SelectHour(query, chatId, bot, user);
                else if (query.Contains("SelectHour")) await SelectTime(query, chatId, bot, user);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }
        private static async Task SetCountry(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            int selection = int.Parse(query.Split(' ')[1]);

            user.Country = (Enums.Countries)selection;
            user.Comand = "AskArivalDate";

            await user.Save();

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskArivalDate);

        }
        private static async Task SetService(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            int selection = int.Parse(query.Split(' ')[1]);

            user.Service = (Enums.Services)selection;
            await user.Save();

            var keybord = Functions.GenerateDateKeyBoard(user,1);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry,replyMarkup:keybord);
        }
        private static async Task SelectDate(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            int week_number = int.Parse(query.Split(' ')[1]);
          
            var keybord = Functions.GenerateDateKeyBoard(user, week_number);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);
        }
        private static async Task SelectHour(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            DateOnly selected_date = DateOnly.Parse(query.Split(' ')[1]);
            int week_number = int.Parse(query.Split(' ')[2]);

            var keybord = Functions.GenerateHourSelectionKeyBoard(selected_date, week_number);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskHourForEntry, replyMarkup: keybord);

        }
        private static async Task SelectTime(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            TimeSpan selected_hour = TimeSpan.Parse(query.Split(' ')[1]);
            DateOnly selected_date = DateOnly.Parse(query.Split(' ')[2]);
            int week_number = int.Parse(query.Split(' ')[3]);

            var keybord = await Functions.GenerateTimeSelectionKeuBoard(user,selected_date,selected_hour, week_number);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskTimeForEntry, replyMarkup: keybord);

        }
    }
}
