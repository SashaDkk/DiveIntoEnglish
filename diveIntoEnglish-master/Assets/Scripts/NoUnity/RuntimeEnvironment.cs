using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.NoUnity
{
    /// <summary>
    /// Окружение приложения
    /// (переменные для обмена между всеми Activity)
    /// </summary>
    internal static class RuntimeEnvironment
    {
        /// <summary>
        /// Данные для сохранения
        /// Инициализируются и сохраняются снаружи
        /// </summary>
        public static SavingData SavingData { get; set; }
    }
}