using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationBot.Models
{
    internal class MyTimeUnit
    {
        // показывает время для записи
        public TimeOnly time { get; set; }

        //Показывает занято время или нет 
        public bool is_free { get; set; }
    }
}
