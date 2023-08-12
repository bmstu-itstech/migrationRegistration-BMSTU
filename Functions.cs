using MigrationBot.Models;
using MigrationBot.Types;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using static MigrationBot.Types.Enums;

namespace MigrationBot
{
    internal class Functions
    {

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
                days = 7;

            return days;
        }
        private static int GetServiceDuration(MyUser user)
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

        public static InlineKeyboardMarkup GenerateDateKeyBoard(MyUser user, int week_number)
        {
            int days = GetUserDays(user);
            int totaldays_skipped = (week_number - 1) * 7;

            DateOnly date = (DateOnly)user.ArrivalDate;

            date = date.AddDays((week_number - 1) * 7);



            var next = InlineKeyboardButton.WithCallbackData("➡️ Далее ➡️", $"DateSelection {week_number + 1}");
            var back = InlineKeyboardButton.WithCallbackData("⬅️ Назад ⬅️", $"DateSelection {week_number - 1}");

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

                button.Add(InlineKeyboardButton.WithCallbackData(curr_date, $"SelectDate {curr_date} {week_number}"));
                keybord_buttnons.Add(button);



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
        public static async Task<InlineKeyboardMarkup> GenerateTimeSelectionKeuBoard(MyUser user, DateOnly selected_date, TimeSpan selected_hour, int week_number)
        {
            var free_times = await FindFreeSeqence(user, selected_date);
            var free_times_in_hour = new List<TimeSpan>();

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
                    InlineKeyboardButton.WithCallbackData(time.ToString(@"hh\:mm"), $"SelectTime {time.ToString()}")
                };

                keyboard.Add(button);
            }

            List<InlineKeyboardButton> back = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("⬅️ Назад ⬅️", $"SelectDate {selected_date} {week_number}")
            };
            keyboard.Add(back);

            return new InlineKeyboardMarkup(keyboard);

    }
    public static InlineKeyboardMarkup GenerateHourSelectionKeyBoard(DateOnly selected_date, int week_number)
    {
        InlineKeyboardMarkup TimeSelection = new InlineKeyboardMarkup(new[]
        {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("10:00",$"SelectHour 10:00 {selected_date} {week_number}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("11:00",$"SelectHour 11:00 {selected_date} {week_number}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("12:00",$"SelectHour 12:00 {selected_date} {week_number}")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("13:00",$"SelectHour 13:00 {selected_date} {week_number}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("14:00",$"SelectHour 14:00 {selected_date} {week_number}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("15:00",$"SelectHour 15:00 {selected_date} {week_number}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("16:00",$"SelectHour 16:00 {selected_date} {week_number}")
                },
                 new[]
                {
                    InlineKeyboardButton.WithCallbackData("17:00",$"SelectHour 17:00 {selected_date} {week_number}")
                },
                  new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад ⬅️",$"DateSelection {week_number}")
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

        var free_entries = await TimeItem.GetFreeEntries(selected_date);

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
            {
                free_times.Add(TimeSpan.Parse(free_entries[i].Time));
                //если последовательность подходит, подэлементы не нужно проверять 

              //  i += time_block - 1;
                //// цикл загрузки временного отрезка очередным мигрантом 
                //for (int k = i; k < i + time_block; k++)
                //{
                //    await free_entries[k].AddWorkLoad(selected_date);
                //}



            }

        }


        return free_times;
    }

}
}
