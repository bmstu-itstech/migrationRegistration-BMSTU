


using MigrationBot.Data;

namespace MigrationBot
{
    internal class ComandExecutor
    {

        public static Task Execute(string message, long chatId, TelegramBotClient bot, Telegram.Bot.Types.Message mess)
        {
            return Task.Run(async () =>
            {

               // message = Functions.ReconstructQuery(message);

               // Console.WriteLine($"ХУЙ {message}");
                MyUser user = await MyUser.GetUser(chatId);
                try
                {
                    if (message.Contains("/start")) await Start(message, chatId, bot, user);
                    else if ((bool)(user?.Comand?.Contains("Ask"))) await ExecuteSetters(message, chatId, bot, user);
                    else if (user.Comand.Contains("Change")) await ExecuteChangers(message, chatId, bot, user);
                    else if (message.Contains("/remove") && message.Contains("-r")) await Functions.RemoveEntryFor(long.Parse(message.Split(' ')[2]), bot);
                    else if (user.Comand.Contains("ContactAdmin")) await ContactAdmin(message, bot, user, mess);
                    else if (user.Comand.Contains("AdminAnswer")) await AdminAnswer(message, bot, user, mess);
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
            else if (user.Comand == "AskCountryStr")
            {
                await AskCountryStr(message, bot, user);
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
            else if (user.Comand == "ChangeCountryStr")
            {
                await AskCountryStr(message, bot, user, true);

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
                message = message.Replace('/', '.');

                int day = int.Parse(message.Split('.')[0]);
                int moutn = int.Parse(message.Split('.')[1]);
                int year = int.Parse(message.Split('.')[2]);
                var arival_date = new DateOnly(year, moutn, day);

                // Нельзя прибыть раньше, чем можно встать на миграционный учёт
                var ts = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time")) - arival_date.ToDateTime(new TimeOnly(0, 0, 0));
                Console.WriteLine(ts.Days - Functions.GetUserMaxDays(user, date: arival_date));


                if (user.Service != Services.RENEWAL_REGISTRATION && user.Service!=Services.VISA_EXETENSIO && ts.Days > Functions.GetUserMaxDays(user))
                    throw new Exception();

                user.ArrivalDate = arival_date;
                user.Comand = "";
                await user.Save();

                var keybord = Functions.GenerateDateKeyBoard(user, 1);

                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await bot.SendTextMessageAsync(chatId, Data.Strings.Messeges.InputErorr);
            }
        }
        public static async Task AskCountryStr(string message, TelegramBotClient bot, MyUser user, bool edit_flag = false)
        {
            user.CountrStr = message;
            await user.Save();

            if (edit_flag)
            {
                var keybord = Functions.GenerateDateKeyBoard(user, week_number: 1);

                await bot.SendTextMessageAsync(user.ChatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);

            }
            else
            {
                await bot.SendTextMessageAsync(user.ChatId, Data.Strings.Messeges.AskService,replyMarkup:KeyBoards.ServiceSelection);

            }

        }


        private static async Task ContactAdmin(string message, TelegramBotClient bot, MyUser user, Telegram.Bot.Types.Message mess)
        {
            string builded_message = $"Отмена записи от @{mess.From.Username}\n" +
                $"#{DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"))).ToString().Replace('.', '_')}\n" +
                $"#{user.FioRu.Replace(' ', '_')}\n" +
                $"#{user.ChatId}\n" +
                $"--------------------------------------------------------" +
                $"\n\n" +
                $"{message}";


            var answer_keyBoard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Ответить",$"Admin_answer {user.ChatId}")
                }
            });

            await bot.SendTextMessageAsync(Bot.Admin_Chat, builded_message, replyMarkup: answer_keyBoard);

            await bot.SendTextMessageAsync(user.ChatId, Data.Strings.Messeges.Wait_For_admin_message);



        }
        // Тут админ отвечает пользователяю 
        private static async Task AdminAnswer(string message, TelegramBotClient bot, MyUser user, Telegram.Bot.Types.Message mess)
        {
            long user_chat_id = long.Parse(user.Comand.Split(' ')[1]);




            //Сообщение, которое отправил человек в предыдущий раз 


            string build_msg = $"Ответ администратора:\n" +
            $"{message}";


            await bot.SendTextMessageAsync(user_chat_id, build_msg);

        }
    }
}
