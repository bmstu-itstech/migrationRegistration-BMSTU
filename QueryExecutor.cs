using MigrationBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

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

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }
        private static async Task SetCountry(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            int selection = int.Parse(query.Split(' ')[1]);

            user.Country = selection;
            await user.Save();

            await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskService, replyMarkup: Data.KeyBords.ServiceSelection);
        }
        private static async Task SetService(string query, long chatId, TelegramBotClient bot, MyUser user)
        {
            int selection = int.Parse(query.Split(' ')[1]);

            user.Service = selection;
            await user.Save();
        }
    }
}
