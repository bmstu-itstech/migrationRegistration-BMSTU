using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationBot.Models
{
    internal class MyUser
    {
        public string? FioEn { get; set; }

        public int? Country { get; set; }

        public int? Service { get; set; }

        public string? Comand { get; set; }

        public DateTime? Entry { get; set; }

        public long ChatId { get; set; }

        public string? FioRu { get; set; }

        public MyUser(long chatId)
        {
            this.ChatId = chatId;
        }
        public MyUser()
        {

        }

        private User ConvertToSqlUser()
        {
            return new User()
            {
                ChatId = this.ChatId,
                Country = this.Country,
                Service = this.Service,
                Comand = this.Comand,
                Entry = this.Entry,
                FioEn = this.FioEn,
                FioRu = this.FioRu
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
                        User user = this.ConvertToSqlUser();

                        await db.Users.AddAsync(user);

                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException ex)
                    {
                        await this.UpdateUser();
                        //Дубликат по записи 
                      //  Console.WriteLine(ex);
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
                    var user = db.Users.Where(x => x.ChatId == this.ChatId).FirstOrDefault();

                    user.ChatId = this.ChatId;
                    user.Country = this.Country;
                    user.Service = this.Service;
                    user.Comand = this.Comand;
                    user.Entry = this.Entry;
                    user.FioEn = this.FioEn;
                    user.FioRu = this.FioRu;


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
                    var my_user = new MyUser();

                    await my_user.Save();

                    return my_user;
                }
                  

                return new MyUser()
                {
                    ChatId = user.ChatId,
                    FioEn = user.FioEn,
                    FioRu = user.FioRu,
                    Comand = user.Comand,
                    Country = user.Country,
                    Service = user.Service,
                    Entry = user.Entry,
                };
            }
        }

    }
}
