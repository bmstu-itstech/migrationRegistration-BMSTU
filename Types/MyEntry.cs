using Microsoft.EntityFrameworkCore;
using MigrationBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationBot.Types
{
    internal class MyEntry
    {
        public long UserId { get; set; }

        public DateTime Date { get; set; }



        public Task Add()
        {
            return Task.Run(async () =>
            {
                using (MigroBotContext db = new MigroBotContext())
                {
                    try
                    {
                        Entry entry = ConvertToSqlEnrty();

                        await db.Entries.AddAsync(entry);

                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                      

                    }

                }
            });
        }
        public Task Update(DateTime new_time)
        {
            return Task.Run(async () =>
            {
                using (MigroBotContext db = new MigroBotContext())
                {
                    var entry = db.Entries.Where(x => x.UserId == this.UserId).FirstOrDefault();

                    entry.UserId = this.UserId;
                    entry.Date = new_time;

                    await db.SaveChangesAsync();
                }
            });
        }
        public static async Task<Entry> GetEntry(long user_id)
        {
            using (MigroBotContext db = new MigroBotContext())
            {
                var entry = db.Entries.Where(x => x.UserId == user_id).FirstOrDefault();

                return new Entry
                {
                    UserId = entry.UserId,
                    Date = entry.Date,
                };


            }
        }

        private Entry ConvertToSqlEnrty()
        {
            return new Entry()
            {
                UserId = UserId,
                Date = Date
            };
        }
    }
}
