
namespace MigrationBot
{
    internal class Bot
    {
        private static string token = Data.Strings.Tokens.BotToken;
          internal static long Admin_Chat = 479020307;
        //debug == true => windows
        // debug == false => UNIX
        public static bool DEBUG = true;
       //internal static long Admin_Chat = 477686161;

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
