using MigrationBot.Models;
using MigrationBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static MigrationBot.Types.Enums;

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
                if (query.Contains("setCountry")) await SetCountry(query, chatId, bot, user, change_flag);
                else if (query.Contains("setService")) await SetService(query, chatId, bot, user, change_flag);
                else if (query.Contains("DateSelection")) await SelectDate(query, chatId, bot, user, change_flag);
                else if (query.Contains("SelectDate")) await SelectHour(query, chatId, bot, user, change_flag);
                else if (query.Contains("SelectHour")) await SelectTime(query, chatId, bot, user, change_flag);
                else if (query.Contains("SelectTime")) await SetUserEntry(query, chatId, bot, user, change_flag);
                else if (query.Contains("Change")) await Changers(query, chatId, bot, user);
                else if (query == "Registration_End") await ProveRegistration(query, chatId, bot, user);
                else if (query.Contains("МoveEntry")) await MoveEntry(user, bot, change_flag);
                else if (query.Contains("RejectEntry")) await RejectEntry(user, bot);
                else if (query.Contains("Admin_answer")) await Admin_answer(query, bot, user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }
        private static async Task SetCountry(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            int selection = int.Parse(query.Split(' ')[1]);

            user.Country = (Countries)selection;



            if (edit_flag)
            {
                if (user.Country == Countries.OTHER)
                    user.Comand = "ChangeCountryStr";

                await user.Save();

                if (user.Country != Countries.OTHER)
                {
                    var keybord = Functions.GenerateDateKeyBoard(user, week_number: 1);
                    await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskCountryStr);

                }
            }
            else
            {
                if (user.Country != Countries.OTHER)
                    user.Comand = "AskArivalDate";
                else
                    user.Comand = "AskCountryStr";

                await user.Save();

                if (user.Country != Countries.OTHER)
                    await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskArivalDate);
                else
                {
                    await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskCountryStr);
                }



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

            if (edit_flag == false)
            {
                await EndReg(chatId, bot, user);


            }
            else
            {
                var entry = new MyEntry()
                {
                    UserId = chatId,
                    Date = (DateTime)user.Entry,
                };

                await entry.Add();
                await entry.Enroll(user);


                await GenerateLastMessage(user, bot);
            }

        }
        private static async Task SelectDate(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            int week_number = int.Parse(query.Split(' ')[1]);

            var keybord = Functions.GenerateDateKeyBoard(user, week_number, edit_flag);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);
        }
        private static async Task SelectHour(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            DateOnly selected_date = DateOnly.Parse(query.Split(' ')[1]);
            int week_number = int.Parse(query.Split(' ')[2]);

            var keybord = Functions.GenerateHourSelectionKeyBoard(selected_date, week_number, edit_flag);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskHourForEntry, replyMarkup: keybord);

        }
        private static async Task SelectTime(string query, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            TimeSpan selected_hour = TimeSpan.Parse(query.Split(' ')[1]);
            DateOnly selected_date = DateOnly.Parse(query.Split(' ')[2]);
            int week_number = int.Parse(query.Split(' ')[3]);

            var keybord = await Functions.GenerateTimeSelectionKeyBoard(user, selected_date, selected_hour, week_number, edit_flag);


            string msg = Data.Strings.Messeges.AskTimeForEntry;

            if (keybord.InlineKeyboard.Count() == 1)
            {
                msg = "К сожалению, запись на это время не доступна";
            }

            await bot.SendTextMessageAsync(chatId, msg, replyMarkup: keybord);

        }

        internal static async Task EndReg(long chatId, TelegramBotClient bot, MyUser user)
        {
            user.Comand = "";

            await user.Save();

            string mess = "Убедитесь, что указанные ниже данные верны:\n\n" +
             $"🔷 Фио (ru): {user.FioRu}\n\n" +
             $"🔷 Фио (en): {user.FioEn}\n\n" +
             $"🔷 Страна: {Enums.Countries_byId[(int)user.Country]}\n\n" +
             $"🔷 Услуга: {Enums.Services_byId[(int)user.Service]}\n\n" +
             $"🔷 Дата прибытия: {user.ArrivalDate:D}\n\n" +
             $"🔷 Дата записи: {user.Entry:f}";


            await bot.SendTextMessageAsync(chatId, mess, replyMarkup: Data.KeyBoards.EndRegKeyBoard);
        }

        private static async Task Changers(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            if (query == "ChangeFio_Ru")
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
            if (query == "ChangeCountry")
            {
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskCountry, replyMarkup: Data.KeyBoards.CountrySelection_Change);

            }
            if (query == "ChangeArivalDate")
            {
                user.Comand = "ChangeArivalDate";

                await user.Save();

                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskArivalDate);

            }
            if (query == "ChangeService")
            {
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskService, replyMarkup: Data.KeyBoards.ServiceSelection_Change);

            }
        }

        private static async Task GenerateLastMessage(MyUser user, TelegramBotClient bot)
        {
            string last_message = $"Вы записаны на {user.Entry:f}\nКабинет 403ю находится по адресу: г. Москва, ул. 2-я Бауманская, д.5, стр.1. ( поднимаетесь на 4 этаж и идёте в южное крыло до конца) \r\n\r\nСоветуем прийти за полчаса до вашего времени, чтобы не пропустить свою очередь. Если вы не сможете прийти просим вас написать об этом";

            await bot.SendTextMessageAsync(user.ChatId, last_message, replyMarkup: Data.KeyBoards.EntryKeyBoard);

        }

        private static async Task ProveRegistration(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            var entry = new MyEntry()
            {
                UserId = chatId,
                Date = (DateTime)user.Entry,
            };

            await entry.Add();
            await entry.Enroll(user);


            await GenerateLastMessage(user, bot);

        }

        private static async Task MoveEntry(MyUser user, TelegramBotClient bot, bool edit_flag = false)
        {
            try
            {
                MyEntry entry = await MyEntry.GetEntry(user.ChatId);

                if (entry is not null)
                {
                    await entry.UnEnroll(user);


                }
            }
            catch (Exception)
            {

            }
            finally
            {
                var keybord = Functions.GenerateDateKeyBoard(user, 1, edit_flag);

                await bot.SendTextMessageAsync(user.ChatId, Data.Strings.Messeges.RejectEntry);
                await bot.SendTextMessageAsync(user.ChatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);

            }
        }
        internal static async Task RejectEntry(MyUser user, TelegramBotClient bot)
        {

            MyEntry entry = await MyEntry.GetEntry(user.ChatId);

            if (entry is not null)
            {
                await entry.UnEnroll(user);

                await bot.SendTextMessageAsync(user.ChatId, Data.Strings.Messeges.Contact_with_admin);

                user.Comand = "ContactAdmin";
                await user.Save();
            }

        }
        private static async Task Admin_answer(string query,  TelegramBotClient bot, MyUser user)
        {
            long user_chat_id = long.Parse(query.Split(' ')[1]);

            user.Comand = $"AdminAnswer {user_chat_id}";
            await user.Save();

            await bot.SendTextMessageAsync(user.ChatId, "Напишите ваш ответ пожалуйста");

        }
    }
}
