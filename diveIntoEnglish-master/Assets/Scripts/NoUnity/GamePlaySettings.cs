using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.NoUnity
{
    /// <summary>
    /// Настройки игрового процесса
    /// </summary>
    internal static class GamePlaySettings
    {
        /// <summary>
        /// Время на раздумье
        /// </summary>
        public static int TimeToAnswerSeconds = 5;

        /// <summary>
        /// Кол-во жизней при старте
        /// </summary>
        public static int StartHp = 3;
    }
}
