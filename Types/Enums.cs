using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationBot.Types
{
    internal class Enums
    {
        public enum Countries
        {
            TJ, 
            UZ,
            KZ,
            KG,
            AM,
            BY,
            UA,
            OTHER
        }
        public enum Services
        {
            VIZA,
            VIZA_INSURANCE,
            REGISTRATION,
            RENEWAL_REGISTRATION,
            INSURANCE,
            DOCUMENTS,
            ALL,
            VISA_EXETENSIO
        }

        public static Dictionary<int ,string> Countries_byId = new Dictionary<int, string>() 
        {
            {0,"🇹🇯 Таджикистан" },
            {1,"🇺🇿 Узбекистан" },
            {2,"🇰🇿 Казахстан" },
            {3,"🇰🇬 Киргизия" },
            {4,"🇦🇲 Армения" },
            {5,"🇧🇾 Беларусь" },
            {6,"🇺🇦 Украина" },
            {7,"Другие страны" },

        };
        public static Dictionary<int, string> Services_byId = new Dictionary<int, string>()
        {
            {0,"Виза" },
            {1,"Виза и страховка" },
            {2,"Регистрация (первичная) " },
            {3,"Продление регистрации" },
            {4,"Страховка" },
            {5,"Выдача документов" },
            {6,"Все услуги" },
            { 7,"Продление визы"}
            

        };
    }
}
