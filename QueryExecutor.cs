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
            bool change_flag = query.Contains("change:true");
            try
            {
                if (query.Contains("setCountry")) await SetCountry(query, chatId, bot, user,change_flag);
                else if (query.Contains("setService")) await SetService(query, chatId, bot, user, change_flag);
                else if (query.Contains("DateSelection")) await SelectDate(query, chatId, bot, user, change_flag);
                else if (query.Contains("SelectDate")) await SelectHour(query, chatId, bot, user, change_flag);
                else if (query.Contains("SelectHour")) await SelectTime(query, chatId, bot, user, change_flag);
                else if (query.Contains("SelectTime")) await SetUserEntry(query, chatId, bot, user, change_flag);
                else if (query.Contains("Change")) await Changers(query, chatId, bot, user);    


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }
        private static async Task SetCountry(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            int selection = int.Parse(query.Split(' ')[1]);

            user.Country = (Enums.Countries)selection;
            user.Comand = "AskArivalDate";

            await user.Save();

            if (edit_flag)
            {
                var keybord = Functions.GenerateDateKeyBoard(user, 1);

                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);
            }
            else
            {
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskArivalDate);

            }

        }
        private static async Task SetService(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            int selection = int.Parse(query.Split(' ')[1]);

            user.Service = (Enums.Services)selection;
            await user.Save();

            if (edit_flag)
            {
                
                var keybord = Functions.GenerateDateKeyBoard(user, 1);

                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);
            }
            else
            {
                var keybord = Functions.GenerateDateKeyBoard(user, 1);

                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);
            }
               
        }
        private static async Task SetUserEntry(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            DateOnly date = DateOnly.Parse(query.Split(' ')[1]);
            TimeSpan time = TimeSpan.Parse(query.Split(' ')[2]);

            DateTime user_entry = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);

            user.Entry = user_entry;

            await user.Save();

            await EndReg(chatId, bot, user);
        }
        private static async Task SelectDate(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            int week_number = int.Parse(query.Split(' ')[1]);

            var keybord = Functions.GenerateDateKeyBoard(user, week_number);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);
        }
        private static async Task SelectHour(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            DateOnly selected_date = DateOnly.Parse(query.Split(' ')[1]);
            int week_number = int.Parse(query.Split(' ')[2]);

            var keybord = Functions.GenerateHourSelectionKeyBoard(selected_date, week_number);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskHourForEntry, replyMarkup: keybord);

        }
        private static async Task SelectTime(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            TimeSpan selected_hour = TimeSpan.Parse(query.Split(' ')[1]);
            DateOnly selected_date = DateOnly.Parse(query.Split(' ')[2]);
            int week_number = int.Parse(query.Split(' ')[3]);

            var keybord = await Functions.GenerateTimeSelectionKeuBoard(user, selected_date, selected_hour, week_number);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskTimeForEntry, replyMarkup: keybord);

        }
        
        internal static async Task EndReg(long chatId, TelegramBotClient bot, MyUser user)
        {
            user.Comand = "";

            await user.Save();

            string mess = "Убедитесь, что указанные ниже данные верны:\n" +
             $"🔷 Фио (ru): {user.FioRu}\n" +
             $"🔷 Фио (en): {user.FioEn}\n" +
             $"🔷 Страна: {Enums.Countries_byId[(int)user.Country]}\n" +
             $"🔷 Услуга: {Enums.Services_byId[(int)user.Service]}\n" +
             $"🔷 Дата прибытия: {user.ArrivalDate:D}\n" +
             $"🔷 Дата записи: {user.Entry:f}\n";


            await bot.SendTextMessageAsync(chatId, mess, replyMarkup: Data.KeyBords.EndRegKeyBoard);
        }

        private static async Task Changers(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            if(query == "ChangeFio_Ru") 
            {
                user.Comand = "ChangeFioRu";
                await user.Save();
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskFioRu);
            }
            if (query == "ChangeFio_En")
            {
                user.Comand = "ChangeFioEn";
                await user.Save();
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskFioEn);
            }
            if(query == "ChangeCountry")
            {
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskCountry, replyMarkup: Data.KeyBords.CountrySelection_Change);

            }
            if (query == "ChangeArivalDate")
            {
                user.Comand = "ChangeArivalDate";

                await user.Save();

                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskArivalDate);

            }
            if(query == "ChangeService")
            {
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskService, replyMarkup: Data.KeyBords.ServiceSelection_Change);

            }
        }
    }
}
