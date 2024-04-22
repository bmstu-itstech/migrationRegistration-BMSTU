﻿
namespace MigrationBot.Types
{
    internal class MyEntry
    {
        public long UserId { get; set; }

        public DateTime Date { get; set; }

        public int Service { get; set; }

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
        public static async Task<MyEntry> GetEntry(long user_id) 
        {
            using (MigroBotContext db = new MigroBotContext())
            {
                var entry = db.Entries.Where(x => x.UserId == user_id).FirstOrDefault();

                return new MyEntry
                {
                    UserId = entry.UserId,
                    Date = (DateTime)entry.Date,
                    Service = (int)entry.Service
                };

            }
        }

        private Entry ConvertToSqlEnrty()
        {
            return new Entry()
            {
                UserId = UserId,
                Date = Date,
                Service = Service
            };
        }
        private static MyEntry ConvertFromSqlEnrty(Entry entry)
        {
            return new MyEntry()
            {
                UserId = entry.UserId,
                Date = (DateTime)entry.Date,
                Service = (int)entry.Service    
            };
        }
        public async Task Enroll(MyUser user)
        {
            // Меняем значения в временной таблице
            DateOnly date = DateOnly.FromDateTime(this.Date);
            TimeSpan time = new TimeSpan(this.Date.Hour, this.Date.Minute, this.Date.Second);

            int time_block_count = Functions.GetServiceDuration(user) / 5;

            for (int i = 0; i < time_block_count; i++)
            {
                await TimeUnit.TookEntryPlace(date, time);
                time = time.Add(new TimeSpan(0, 5, 0));
            }

            var gw = new GoogleSheetWorker();
            await gw.UpdateEntries(DateOnly.FromDateTime(this.Date));
        }
        public async Task UnEnroll(MyUser user)
        {
            // Меняем значения в временной таблице
            DateOnly date = DateOnly.FromDateTime(this.Date);
            TimeSpan time = new TimeSpan(this.Date.Hour, this.Date.Minute, this.Date.Second);

            int time_block_count = Functions.GetServiceDuration(user) / 5;
            for (int i = 0; i < time_block_count; i++)
            {
                await TimeUnit.FreeEntryPlace(date, time);
                time = time.Add(new TimeSpan(0, 5, 0));

            }
            using (MigroBotContext db = new MigroBotContext())
            {
                db.Entries.Remove(this.ConvertToSqlEnrty());
                await db.SaveChangesAsync();

            }
            var gw = new GoogleSheetWorker();

            await gw.UpdateEntries(DateOnly.FromDateTime(this.Date));

        }

        public static List<MyEntry> GetEntriesByDate(DateOnly date)
        {
            using (MigroBotContext db = new MigroBotContext())
            {

                var entries = db.Entries.Where(x => x.Date.Value.Day == date.Day && x.Date.Value.Month == date.Month)
                .Select(ConvertFromSqlEnrty).OrderBy(x => x.Date).ToList();

                return entries;
            }

        }

    }
}
