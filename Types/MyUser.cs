using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MigrationBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationBot.Types
{
    internal class MyUser
    {
        public string? FioEn { get; set; }

        public Enums.Countries Country { get; set; }

        public Enums.Services Service { get; set; }

        public string? Comand { get; set; }

        public DateTime? Entry { get; set; }

        public long ChatId { get; set; }

        public string? FioRu { get; set; }

        public DateOnly? ArrivalDate { get; set; }

        public MyUser(long chatId)
        {
            ChatId = chatId;
        }
        public MyUser()
        {

        }

        private User ConvertToSqlUser()
        {
            return new User()
            {
                ChatId = ChatId,
                Country = (int?)Country,
                Service = (int?)Service,
                Comand = Comand,
                Entry = Entry,
                FioEn = FioEn,
                FioRu = FioRu,
                ArrivalDate = ArrivalDate
            };
        }

        public Task Save()
        {
            return Task.Run(async () =>
            {
                using (MigroBotContext db = new MigroBotContext())
                {
                    try
                    {
                        User user = ConvertToSqlUser();

                        await db.Users.AddAsync(user);

                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        await UpdateUser();

                    }

                }
            });
        }
        private Task UpdateUser()
        {
            return Task.Run(async () =>
            {
                using (MigroBotContext db = new MigroBotContext())
                {
                    var user = db.Users.Where(x => x.ChatId == ChatId).FirstOrDefault();

                    user.ChatId = ChatId;
                    user.Country = (int?)Country;
                    user.Service = (int?)Service;
                    user.Comand = Comand;
                    user.Entry = Entry;
                    user.FioEn = FioEn;
                    user.FioRu = FioRu;
                    user.ArrivalDate = ArrivalDate;

                    await db.SaveChangesAsync();
                }
            });
        }
        public static async Task<MyUser> GetUser(long chatId)
        {
            using (MigroBotContext db = new MigroBotContext())
            {

                var user = db.Users.Where(x => x.ChatId == chatId).FirstOrDefault();

                if (user == null)
                {
                    var my_user = new MyUser(chatId);

                    await my_user.Save();

                    return my_user;
                }


                return new MyUser()
                {
                    ChatId = user.ChatId,
                    FioEn = user.FioEn,
                    FioRu = user.FioRu,
                    Comand = user.Comand,
                    Country = (Enums.Countries)user.Country,
                    Service = (Enums.Services)user.Service,
                    Entry = user.Entry,
                    ArrivalDate = user.ArrivalDate
                };
            }
        }

    }
}
