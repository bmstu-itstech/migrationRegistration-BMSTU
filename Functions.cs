

namespace MigrationBot
{
    internal static class Functions
    {

        public static int GetUserMaxDays(MyUser user)
        {
            int days = 0;
            if (user.Country == Countries.TJ || user.Country == Countries.UZ)
                days = 15;
            if (user.Country == Countries.KZ || user.Country == Countries.KG || user.Country == Countries.AM)
                days = 30;
            if (user.Country == Countries.UA || user.Country == Countries.BY)
                days = 90;
            if (user.Country == Countries.OTHER)
            {
                days = 9;

                if (user.ArrivalDate.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    days = 10;
                }
                if (user.ArrivalDate.Value.DayOfWeek == DayOfWeek.Saturday || user.ArrivalDate.Value.DayOfWeek == DayOfWeek.Friday)
                    days = 11;
              
            }

            return days;
        }
        private static int GetUserDays(MyUser user)
        {
            int days = 0;
            if (user.Country == Countries.TJ || user.Country == Countries.UZ)
                days = 15;
            if (user.Country == Countries.KZ || user.Country == Countries.KG || user.Country == Countries.AM)
                days = 30;
            if (user.Country == Countries.UA || user.Country == Countries.BY)
                days = 90;
            if (user.Country == Countries.OTHER)
            {
                days = 9;

                if (user.ArrivalDate.Value.DayOfWeek == DayOfWeek.Sunday)
                    days = 10;
                if (user.ArrivalDate.Value.DayOfWeek == DayOfWeek.Saturday || user.ArrivalDate.Value.DayOfWeek == DayOfWeek.Friday)
                    days = 11;
            }


            var user_start_count_date = DateTime.Parse(user.ArrivalDate.ToString());
            var date_time_now = DateTime.Now;



            TimeSpan ts = date_time_now - user_start_count_date;

            int day_passed_from_arrival = ts.Days;


            return days - day_passed_from_arrival;
        }
        public static int GetServiceDuration(MyUser user)
        {
            int service_duration = 0;

            if (user.Service == Services.VIZA)
                service_duration = 25;
            if (user.Service == Services.VIZA_INSURANCE)
                service_duration = 30;
            if (user.Service == Services.REGISTRATION)
                service_duration = 15;
            if (user.Service == Services.YEAR_REGISTRATION)
                service_duration = 20;
            if (user.Service == Services.INSURANCE)
                service_duration = 5;
            if (user.Service == Services.DOCUMENTS)
                service_duration = 10;
            if (user.Service == Services.ALL)
                service_duration = 45;

            return service_duration;
        }

        public static InlineKeyboardMarkup GenerateDateKeyBoard(MyUser user, int week_number, bool edit_flag = false)
        {
            int days = GetUserDays(user);
            int totaldays_skipped = (week_number - 1) * 7;

            DateOnly date = DateOnly.FromDateTime(DateTime.Now);

            date = date.AddDays((week_number - 1) * 7);

            string mode = "";
            if (edit_flag)
                mode = "change:true";

            var next = InlineKeyboardButton.WithCallbackData("➡️ Далее ➡️", $"DateSelection {week_number + 1} {mode}");
            var back = InlineKeyboardButton.WithCallbackData("⬅️ Назад ⬅️", $"DateSelection {week_number - 1} {mode}");

            List<List<InlineKeyboardButton>> keybord_buttnons = new List<List<InlineKeyboardButton>>();

            for (int j = 0; j < 7; j++)
            {

                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>();

                string curr_date = $"0{date.Day}.0{date.Month}";

                if (date.Month >= 10 && date.Day >= 10)
                    curr_date = $"{date.Day}.{date.Month}";
                if (date.Month >= 10 && !(date.Day >= 10))
                    curr_date = $"0{date.Day}.{date.Month}";
                if (!(date.Month >= 10) && date.Day >= 10)
                    curr_date = $"{date.Day}.0{date.Month}";

                button.Add(InlineKeyboardButton.WithCallbackData(curr_date, $"SelectDate {curr_date} {week_number} {mode}"));

                if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
                {
                    keybord_buttnons.Add(button);

                }
                // В Субботу и воскресенье приём не ведётся, поэтому после пятницы нужно пропустить не 1 день, а сразу 2
                if (date.DayOfWeek == DayOfWeek.Friday)
                {
                    date = date.AddDays(2);

                    totaldays_skipped += 2;
                }

                date = date.AddDays(1);
                totaldays_skipped++;

   

                if (days <= totaldays_skipped)
                    break;
            }

            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            bool can_go_next = days - totaldays_skipped > 0 ? true : false;
            bool can_go_back = week_number > 1 ? true : false;

            if (can_go_back)
            {
                buttons.Add(back);

            }
            if (can_go_next)
            {
                buttons.Add(next);
            }

            keybord_buttnons.Add(buttons);

            return new InlineKeyboardMarkup(keybord_buttnons);
        }
        public static async Task<InlineKeyboardMarkup> GenerateTimeSelectionKeyBoard(MyUser user, DateOnly selected_date, TimeSpan selected_hour, int week_number, bool edit_flag = false)
        {
            var free_times = await FindFreeSeqence(user, selected_date);
            var free_times_in_hour = new List<TimeSpan>();

            string mode = "";
            if (edit_flag)
                mode = "change:true";

            foreach (var time in free_times)
            {
                //Выбираем только те временные промежутки, которые находятся в интересующем нас часе
                if (time >= selected_hour && time < selected_hour.Add(new TimeSpan(1, 0, 0)))
                    free_times_in_hour.Add(time);
            }

            List<List<InlineKeyboardButton>> keyboard = new List<List<InlineKeyboardButton>>();



            foreach (var time in free_times_in_hour)
            {
                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(time.ToString(@"hh\:mm"), $"SelectTime {selected_date} {time.ToString()} {mode}")
                };

                keyboard.Add(button);
            }

            List<InlineKeyboardButton> back = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("⬅️ Назад ⬅️", $"SelectDate {selected_date} {week_number} {mode}")
            };
            keyboard.Add(back);

            return new InlineKeyboardMarkup(keyboard);

        }
        public static InlineKeyboardMarkup GenerateHourSelectionKeyBoard(DateOnly selected_date, int week_number, bool edit_flag = false)
        {
            string mode = "";
            if (edit_flag)
                mode = "change:true";

            InlineKeyboardMarkup TimeSelection = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("10:00",$"SelectHour 10:00 {selected_date} {week_number} {mode}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("11:00",$"SelectHour 11:00 {selected_date} {week_number} {mode}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("12:00",$"SelectHour 12:00 {selected_date} {week_number} {mode}")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("13:00",$"SelectHour 13:00 {selected_date} {week_number} {mode}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("14:00",$"SelectHour 14:00 {selected_date} {week_number} {mode}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("15:00",$"SelectHour 15:00 {selected_date} {week_number} {mode}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("16:00",$"SelectHour 16:00 {selected_date} {week_number} {mode}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("17:00",$"SelectHour 17:00 {selected_date} {week_number} {mode}")
                },
                  new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад ⬅️",$"DateSelection {week_number} {mode}")
                }
            });

            return TimeSelection;
        }

        private static async Task<List<TimeSpan>> FindFreeSeqence(MyUser user, DateOnly selected_date)
        {

            int service_duration = GetServiceDuration(user);
            List<TimeSpan> free_times = new List<TimeSpan>();

            // Смотрим, сколько временных юнитов нужно для данного мигранта 
            // Именно такой длины должна быть последовательность юнитов
            int time_block = service_duration / 5;

            var free_entries = await TimeUnit.GetFreeEntries(selected_date);

            // Цикл выделения подходящийх временных отрезков 
            for (int i = 1; i <= 90; i++)
            {

                if (!free_entries.Keys.Contains(i))
                    continue;

                bool curr_entry_flag = true;

                for (int j = i + 1; j < i + time_block; j++)
                {
                    curr_entry_flag = free_entries.Keys.Contains(j);

                    if (!curr_entry_flag)
                    {
                        break;
                    }
                }

                if (curr_entry_flag)
                    free_times.Add(TimeSpan.Parse(free_entries[i].Time));


            }

            return free_times;
        }


        public static async void CreateDateTables()
        {
            var date = DateTime.Now;

            for (int i = 0; i < 90; i++)
            {


                var time = new TimeSpan(10, 0, 0);
                var diner_start = new TimeSpan(12, 30, 0);
                var diner_end = new TimeSpan(13, 30, 0);



                string query = $"CREATE TABLE \"{date.ToShortDateString()}\"" +
                    $"(" +
                    $"id INTEGER PRIMARY KEY," +
                    $"time CHARACTER VARYING(255)," +
                    $"count INTEGER" +
                    $");";

                if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
                {

                    using (var conn = new NpgsqlConnection(Data.Strings.Tokens.SqlConnection))
                    {
                        await conn.OpenAsync();

                        using (var command = new NpgsqlCommand(query, conn))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                    };


                    int time_blocks_count = 90;

                    //Если это пятница, то приём ведётся до 16:45
                    if (date.DayOfWeek == DayOfWeek.Friday)
                        time_blocks_count = 81;
                    for (int j = 0; j <= time_blocks_count; j++)
                    {
                        var size = new TimeSpan(0, 5, 0);

                        int count = 0;

                        if (time >= diner_start && time <= diner_end)
                            count = 3;

                        string insert = $"INSERT INTO \"{date.ToShortDateString()}\" (id,time,count) VALUES({j + 1},'{time}',{count});";

                        using (var conn = new NpgsqlConnection(Data.Strings.Tokens.SqlConnection))
                        {
                            await conn.OpenAsync();

                            using (var command = new NpgsqlCommand(insert, conn))
                            {
                                await command.ExecuteNonQueryAsync();
                            }
                        };
                        time = time.Add(size);
                    }
                }
                date = date.AddDays(1);
            }
        }

        public static async void DropDateTables()
        {
            var date = DateTime.Now;

            for (int i = 0; i < 90; i++)
            {
                string drop = $"DROP TABLE \"{date.ToShortDateString()}\"";

                try
                {
                    using (var conn = new NpgsqlConnection(Data.Strings.Tokens.SqlConnection))
                    {
                        await conn.OpenAsync();

                        using (var command = new NpgsqlCommand(drop, conn))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                    };
                }
                catch (Exception)
                {

                }

                date = date.AddDays(1);

            }


        }

        public static async void AppendSheets()
        {
            var date = DateTime.Now;

            for (int i = 0; i < 90; i++)
            {
                if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
                {
                    var gw = new GoogleSheetWorker();

                    await gw.AddSheet(DateOnly.FromDateTime(date).ToString());
                }
                date = date.AddDays(1);
            }
        }

        public static async void DropSheets()
        {
            
            var gw = new GoogleSheetWorker();
            await gw.DropSheet();
        }

        public static bool isRuFioValid(string Fio)
        {
            if (Fio.Contains("'\'") || Fio.Contains("/") || Fio.Contains("'"))
                return false;
            if (Fio.Split(' ').Length == 2 || Fio.Split(' ').Length == 3)
            {
                Fio = Fio.ToLower();
                foreach (char el in Fio)
                {
                    if (char.IsDigit(el))
                        return false;

                    if (el != ' ')
                    {
                        if (((int)el >= 97 && (int)el <= 122))
                        {
                            return false;
                        }
                    }

                }
            }
            else
                return false;

            return true;

        }

        public static bool isEnFioValid(string Fio)
        {
            if (Fio.Contains("'\'") || Fio.Contains("/") || Fio.Contains("'"))
                return false;
            if (Fio.Split(' ').Length == 2 || Fio.Split(' ').Length == 3)
            {
                Fio = Fio.ToLower();

                foreach (char el in Fio)
                {
                    if (char.IsDigit(el))
                        return false;

                    if (el != ' ')
                    {

                        Console.Write((int)el + " ");
                        if (((int)el >= 'а' && (int)el <= 'я'))
                        {
                            return false;
                        }
                    }

                }
            }
            else
                return false;

            return true;

        }

        internal static async Task RemoveEntryFor(long user_chat, TelegramBotClient bot)
        {
            var entry = await MyEntry.GetEntry(user_chat);
            if (entry is not null)
            {
                await bot.SendTextMessageAsync(user_chat, Data.Strings.Messeges.Excuse_Message);


                var user = await MyUser.GetUser(user_chat);

                await entry.UnEnroll(user);

                var gw = new GoogleSheetWorker();
                gw.UpdateEntries(DateOnly.FromDateTime(entry.Date));

                var keybord = Functions.GenerateDateKeyBoard(user, 1, true);

                await bot.SendTextMessageAsync(user.ChatId, Data.Strings.Messeges.AskEntry, replyMarkup: keybord);
            }
        }
    }
}
