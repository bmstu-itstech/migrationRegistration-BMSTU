using MigrationBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MigrationBot
{
    internal class ComandExecutor
    {

        public static Task Execute(string message, long chatId, TelegramBotClient bot)
        {
            return Task.Run(async () =>
            {
                MyUser user = await MyUser.GetUser(chatId);
                try
                {
                    if (message == "/start") await Start(message, chatId, bot, user);
                    else if ((bool)(user?.Comand?.Contains("Ask"))) await ExecuteSetters(message, chatId, bot, user);
                    else if (user.Comand.Contains("Change")) await ExecuteChangers(message, chatId, bot, user);
                    else if (message.Contains("/remove") && message.Contains("-r")) await Functions.RemoveEntryFor(long.Parse(message.Split(' ')[2]), bot);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            });
        }
        private static async Task ExecuteSetters(string message, long chatId, TelegramBotClient bot, MyUser user)
        {

            if (user.Comand == "AskFioRu")
            {
                await AskFio_Ru(message, chatId, bot, user);
            }
            else if (user.Comand == "AskFioEn")
            {
                await AskFio_En(message, chatId, bot, user);
            }
            else if (user.Comand == "AskArivalDate")
            {
                await AskArivalDate(message, chatId, bot, user);
            }

        }
        private static async Task ExecuteChangers(string message, long chatId, TelegramBotClient bot, MyUser user)
        {
            if (user.Comand == "ChangeFioRu")
            {
                await AskFio_Ru(message, chatId, bot, user, true);
            }
            else if (user.Comand == "ChangeFioEn")
            {
                await AskFio_En(message, chatId, bot, user, true);
            }
            else if (user.Comand == "ChangeArivalDate")
            {
                await AskArivalDate(message, chatId, bot, user, true);
            }
        }

        private static async Task Start(string message, long chatId, TelegramBotClient bot, MyUser user)
        {





            bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.StartMessege);

            user.Comand = "AskFioRu";

            await user.Save();


            bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskFioRu);

        }
        internal static async Task AskFio_Ru(string message, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            try
            {


                if (!Functions.isRuFioValid(message))
                    throw new Exception();

                user.FioRu = message;
                user.Comand = "AskFioEn";

                await user.Save();



                if (edit_flag == true)
                    await QueryExecutor.EndReg(chatId, bot, user);
                else
                {
                    await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskFioEn);

                }
            }
            catch (Exception)
            {

                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.InputErorr);
            }
        }
        internal static async Task AskFio_En(string message, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            try
            {
                if (!Functions.isEnFioValid(message))
                    throw new Exception();
                user.FioEn = message;

                await user.Save();

                if (edit_flag)
                    await QueryExecutor.EndReg(chatId, bot, user);
                else
                    await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskCountry, replyMarkup: Data.KeyBoards.CountrySelection);
            }
            catch (Exception)
            {

                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.InputErorr);
            }

        }
        internal static async Task AskArivalDate(string message, long chatId, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            try
            {
                var arival_date = DateOnly.Parse(message);

                //Дата прибытия не может быть раньше, чем сегодняшняя
                if (arival_date < DateOnly.FromDateTime(DateTime.Now))
                    throw new Exception();

                user.ArrivalDate = arival_date;
                user.Comand = "";
                await user.Save();

                if (edit_flag)
                {
                    var keybord = Functions.GenerateDateKeyBoard(user, 1);

                    await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);
                }
                else
                    await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskService, replyMarkup: Data.KeyBoards.ServiceSelection);


            }
            catch (Exception)
            {
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.InputErorr);
            }
        }
    }
}
