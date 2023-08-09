using MigrationBot.Models;
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
                    if ((bool)(user?.Comand.Contains("Ask"))) await ExecuteSetters(message, chatId, bot, user);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            });
        }
        private static async Task ExecuteSetters(string message, long chatId, TelegramBotClient bot, MyUser user)
        {

            if(user.Comand == "AskFioRu")
            {
                await AskFio_Ru(message, chatId, bot, user);
            }
            if (user.Comand == "AskFioEn") 
            {
                await AskFio_En(message, chatId, bot, user);
            }

        }

        private static async Task Start(string message, long chatId, TelegramBotClient bot, MyUser user)
        {
            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.StartMessege);

            user.Comand = "AskFioRu";

            await user.SaveUser();

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskFioRu);
            
        }
        private static async Task AskFio_Ru(string message, long chatId, TelegramBotClient bot, MyUser user)
        {
            user.FioRu = message;
            user.Comand = "AskFioEn";

            await user.SaveUser();


            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskFioEn);
        }
        private static async Task AskFio_En(string message, long chatId, TelegramBotClient bot, MyUser user)
        {
            user.FioEn = message;
            user.Comand = "";

            await user.SaveUser();
        }
    }
}
