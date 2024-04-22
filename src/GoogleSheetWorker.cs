﻿

namespace MigrationBot
{
    internal class GoogleSheetWorker
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "baumanbot";

        private static readonly string SpreadSheetId = Data.Strings.Tokens.GoogleToken;


        public SheetsService service;
        public GoogleCredential credential;

        public GoogleSheetWorker()
        {
            using (var stream = new FileStream("secrets.json", FileMode.Open, FileAccess.Read))
            {
                this.credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);

            }

            this.service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        public async Task UpdateEntries(DateOnly date)
        {
            await CleanSheet(date);

            string curr_date = $"0{date.Day}.0{date.Month}";

            if (date.Month >= 10 && date.Day >= 10)
                curr_date = $"{date.Day}.{date.Month}";
            if (date.Month >= 10 && !(date.Day >= 10))
                curr_date = $"0{date.Day}.{date.Month}";
            if (!(date.Month >= 10) && date.Day >= 10)
                curr_date = $"{date.Day}.0{date.Month}";

            var range = $"{curr_date}.{date.Year}!A:G";
            var valueRange = new ValueRange();

            var user_entries = MyEntry.GetEntriesByDate(date);
         


            List<object> header = new List<object> { "id", "ФИО (ru)", "ФИО (en)", "Дата прибытия", "Страна", "Услуга", "Время записи" };

            IList<IList<object>> values = new List<IList<object>>
            {
                header
            };

            foreach (var entry in user_entries)
            {
                var user = await MyUser.GetUser(entry.UserId);

                string country = Enums.Countries_byId[(int)user.Country];

                if (user.CountrStr != null)
                    country = user.CountrStr;
                List<object> objectList = new List<object>()
                {
                    user.ChatId,user.FioRu,
                    user.FioEn,
                    user.ArrivalDate,
                    country,
                    Enums.Services_byId[(int)user.Service],
                    user.Entry.Value.TimeOfDay.ToString()
                };

                values.Add(objectList);
            }

            valueRange.Values = values;

            var update = service.Spreadsheets.Values.Update(valueRange, SpreadSheetId, range);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            await update.ExecuteAsync();
        }

        public async Task CleanSheet(DateOnly date)
        {
            string curr_date = $"0{date.Day}.0{date.Month}";

            if (date.Month >= 10 && date.Day >= 10)
                curr_date = $"{date.Day}.{date.Month}";
            if (date.Month >= 10 && !(date.Day >= 10))
                curr_date = $"0{date.Day}.{date.Month}";
            if (!(date.Month >= 10) && date.Day >= 10)
                curr_date = $"{date.Day}.0{date.Month}";

            var range = $"{curr_date}.{date.Year}!A:G";
            var valueRange = new ValueRange();

            ClearValuesRequest request_body = new ClearValuesRequest();

            var appendRequest = service.Spreadsheets.Values.Clear(request_body, SpreadSheetId, range);

            await appendRequest.ExecuteAsync();
        }

        public async Task AddSheet(string _sheetName)
        {
            // Добавить новый лист
            var addSheetRequest = new AddSheetRequest();
            addSheetRequest.Properties = new SheetProperties();
            addSheetRequest.Properties.Title = _sheetName;
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = new List<Request>
            {
                new Request
                {
                    AddSheet = addSheetRequest
                }
            };

            // Создать запрос
            var batchUpdateRequest =
                service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, SpreadSheetId);

            // Выполнить запрос
            var response = await batchUpdateRequest.ExecuteAsync();

        }

        public async Task DropSheet()
        {
            var request = service.Spreadsheets.Get(SpreadSheetId);
            var resp = await request.ExecuteAsync();


            foreach (var sheet in resp.Sheets)
            {

                var delete_sheet = new DeleteSheetRequest();

                delete_sheet.SheetId = sheet.Properties.SheetId;
                BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
                batchUpdateSpreadsheetRequest.Requests = new List<Request>
                {
                    new Request
                    {
                        DeleteSheet = delete_sheet
                    }
                };


                var batchUpdateRequest =
                    service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, SpreadSheetId);

                if (sheet.Properties.Title != "Не удаляй меня")
                    await batchUpdateRequest.ExecuteAsync();



            }


        }
    }
}
