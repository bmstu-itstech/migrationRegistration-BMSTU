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
                if (query.Contains("setCountry")) await SetCountry(query);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }
        private static async Task SetCountry(string query)
        {
            int selection = int.Parse(query.Split(' ')[1]);
        }
    }
}
