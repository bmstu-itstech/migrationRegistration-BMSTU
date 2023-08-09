using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

using MigrationBot.Data;
namespace MigrationBot
{
    internal class Bot
    {
        private static string token = Data.Strings.Tokens.BotToken;

        private static TelegramBotClient bot = new TelegramBotClient(token);

        public static async Task Start()
        {

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);


            while (true)
            {
                try
                {
                    var cancellationToken = CancellationToken.None;
                    var receiverOptions = new ReceiverOptions
                    {
                        AllowedUpdates = { }, // receive all update types
                    };

                    var updateReceiver = new QueuedUpdateReceiver(bot, receiverOptions);

                    try
                    {
                        await foreach (Update update in updateReceiver.WithCancellation(cancellationToken))
                        {

                            _ = Task.Run(() =>
                            {

                                _ = UpdateHandlers.HandleUpdateAsync(update, bot);
                                return Task.CompletedTask;
                            });

                        }
                    }
                    catch (OperationCanceledException exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                   
                }
            }
        }

    }
}
