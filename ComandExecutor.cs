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
            else if(user.Comand == "AskArivalDate")
            {
                await AskArivalDate(message, chatId, bot, user);
            }

        }

        private static async Task Start(string message, long chatId, TelegramBotClient bot, MyUser user)
        {
            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.StartMessege);

            user.Comand = "AskFioRu";

            var keybord = Functions.GenerateEntryKeyBoard(user,1);

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);

            await user.Save();

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskFioRu);

        }
        private static async Task AskFio_Ru(string message, long chatId, TelegramBotClient bot, MyUser user)
        {
            user.FioRu = message;
            user.Comand = "AskFioEn";

            await user.Save();

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskFioEn);
        }
        private static async Task AskFio_En(string message, long chatId, TelegramBotClient bot, MyUser user)
        {
            user.FioEn = message;

            await user.Save();

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskCountry,replyMarkup: Data.KeyBords.CountrySelection);
        }
        private static async Task AskArivalDate(string message, long chatId, TelegramBotClient bot, MyUser user)
        {
            try
            {
                var arival_date = DateOnly.Parse(message);

                //Дата прибытия не может быть раньше, чем сегодняшняя
                if (arival_date < DateOnly.FromDateTime(DateTime.Now))
                    throw new Exception();

                user.ArrivalDate = arival_date;
                await user.Save();

                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskService, replyMarkup: Data.KeyBords.ServiceSelection);


            }
            catch (Exception)
            {
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.InputErorr);
            }
        }
    }
}
