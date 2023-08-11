using MigrationBot.Types;
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

        public static InlineKeyboardMarkup GenerateEntryKeyBoard(MyUser user, int week_nuber)
        {
            int days = GetUserDays(user);
            int totaldays_skipped = week_nuber * 7;

            DateOnly date = (DateOnly)user.ArrivalDate;

            date = date.AddDays((week_nuber - 1) * 7);

            

            var next = InlineKeyboardButton.WithCallbackData("➡️ Далее ➡️", $"DateSelection {week_nuber + 1}");
            var back = InlineKeyboardButton.WithCallbackData("⬅️ Назад ⬅️", $"DateSelection {week_nuber - 1}");

            List<List<InlineKeyboardButton>> keybord_buttnons = new List<List<InlineKeyboardButton>>();

            for (int j = 0; j < 7; j++)
            {
                if (days <= totaldays_skipped)
                    break;

                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>();

                string curr_date = $"0{date.Day}.0{date.Month}";

                if (date.Month >= 10 && date.Day >= 10)
                    curr_date = $"{date.Day}.{date.Month}";
                if (date.Month >= 10 && !(date.Day >= 10))
                    curr_date = $"0{date.Day}.{date.Month}";
                if (!(date.Month >= 10) && date.Day >= 10)
                    curr_date = $"{date.Day}.0{date.Month}";

                button.Add(InlineKeyboardButton.WithCallbackData(curr_date, $"SelectDate {curr_date}"));
                keybord_buttnons.Add(button);



                date = date.AddDays(1);
                totaldays_skipped++;

            }

            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

            bool can_go_next = days - totaldays_skipped > 0 ? true : false;
            bool can_go_back = week_nuber > 1 ? true : false;

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
    }
}
