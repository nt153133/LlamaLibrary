using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ff14bot.Enums;
using ff14bot.Managers;

namespace LlamaLibrary.Helpers
{
    public static class Translator
    {
        public static readonly Language Language;

        public static string SummoningBell => Summoning_Bell[Language];

        static Translator()
        {
            Language = (Language) typeof(DataManager).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                .First(i => i.FieldType == typeof(Language)).GetValue(null);
        }

        static Dictionary<Language, string> Summoning_Bell = new Dictionary<Language, string>
        {
            { Language.Eng, "Summoning Bell"},
            { Language.Jap, "リテイナーベル"},
            { Language.Fre, "Sonnette"},
            { Language.Ger, "Krämerklingel"},
            { Language.Chn, "传唤铃" }
        };
        
        //Addon # 2385
        static Dictionary<Language, string> Addon2385 = new Dictionary<Language, string>
        {
            { Language.Eng, "View venture report. (Complete)"},
            { Language.Jap, "リテイナーベンチャーの確認　[完了]"},
            { Language.Fre, "Voir le rapport de la tâche terminée"},
            { Language.Ger, "Abgeschlossene Unternehmung einsehen"},
            { Language.Chn, "查看雇员探险情况　[结束]" }
        };

        //Addon # 2386
        static Dictionary<Language, string> Addon2386 = new Dictionary<Language, string>
        {
            { Language.Eng, "Assign venture."},
            { Language.Jap, "リテイナーベンチャーの依頼"},
            { Language.Fre, "Liste des tâches"},
            { Language.Ger, "Mit Unternehmung beauftragen"},
            { Language.Chn, "委托雇员进行探险" }
        };

        //Addon # 2387
        static Dictionary<Language, string> Addon2387 = new Dictionary<Language, string>
        {
            { Language.Eng, "Assign venture. (In progress)"},
            { Language.Jap, "リテイナーベンチャーの依頼　[依頼中]"},
            { Language.Fre, "Liste des tâches [Tâche en cours]"},
            { Language.Ger, "Mit Unternehmung beauftragen (Gehilfe beschäftigt)"},
            { Language.Chn, "委托雇员进行探险　[进行中]" }
        };

        //Addon # 12590
        static Dictionary<Language, string> Addon12590 = new Dictionary<Language, string>
        {
            { Language.Eng, "None in progress"},
            { Language.Jap, "依頼なし"},
            { Language.Fre, "Aucune"},
            { Language.Ger, "Keine Unternehmung"},
            { Language.Chn, "没有探险委托" }
        };

        //Addon # 12591
        static Dictionary<Language, string> Addon12591 = new Dictionary<Language, string>
        {
            { Language.Eng, "Complete in "},
            { Language.Jap, "残り時間"},
            { Language.Fre, "Fin de la tâche dans "},
            { Language.Ger, "Noch "},
            { Language.Chn, "剩余时间" }
        };

        //Addon # 12592
        static Dictionary<Language, string> Addon12592 = new Dictionary<Language, string>
        {
            { Language.Eng, "Complete"},
            { Language.Jap, "完了"},
            { Language.Fre, "Terminée"},
            { Language.Ger, "Abgeschlossen"},
            { Language.Chn, "结束" }
        };
    }
}