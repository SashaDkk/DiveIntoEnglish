using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NoUnity
{
    
    /// <summary>
    /// Информация о прохождении уровния
    /// </summary>
    public class StageInfo
    {
        /// <summary>
        /// Сколько раз уровень успешно пройден
        /// </summary>
        public int Succeed;

        /// <summary>
        /// Максимальный рекорд
        /// </summary>
        public int MaxCoins;

        /// <summary>
        /// Уведомить об успешном прохождении
        /// </summary>
        /// <returns>true - если открыт новый уровень!</returns>
        public bool NotifySucceed()
        {
            Succeed++;
            return Succeed == 1;
        }

        /// <summary>
        /// Сообщить о счете (если он больше максимального, он его перетрет)
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true - если рекорд, первая запись не является рекордом</returns>
        public bool NotifyCoins(int value)
        {
            if (MaxCoins < value)
            {
                var prevCoins = MaxCoins;
                MaxCoins = value;
                return prevCoins > 0;
            }
            return false;
        }

        /// <summary>
        /// Псевдоконструктор из строки
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StageInfo LoadFromString(string value)
        {
            var sepPos = value.IndexOf(',');
            if (sepPos == -1)
                return null;
            if (!int.TryParse(value.Substring(0, sepPos), out var tmpInt1))
                return null;
            if (!int.TryParse(value.Substring(sepPos + 1), out var tmpInt2))
                return null;
            return new StageInfo
            {
                Succeed = tmpInt1,
                MaxCoins = tmpInt2
            };
        }

        /// <summary>
        /// Сохранение в строку
        /// </summary>
        /// <returns></returns>
        public string SaveToString()
        {
            return $"{Succeed},{MaxCoins}";
        }
    }

    /// <summary>
    /// Информация об уровне
    /// </summary>
    public class LevelInfo
    {
        /// <summary>
        /// Информация о главах книги
        /// </summary>
        private readonly Dictionary<int, StageInfo> _stages = new Dictionary<int, StageInfo>();

        /// <summary>
        /// Псевдоконструктор из строки
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LevelInfo LoadFromString(string value)
        {
            var result = new LevelInfo();
            foreach(var item in value.Split('|'))
            {
                var sepPos = item.IndexOf(',');
                if (sepPos == -1)
                    continue;
                if (!int.TryParse(item.Substring(0, sepPos), out var levelIndex))
                    continue;
                var stageInfo = StageInfo.LoadFromString(item.Substring(sepPos + 1));
                if (stageInfo == null)
                    continue;
                result._stages[levelIndex] = stageInfo;
            }
            return result;
        }

        /// <summary>
        /// Сохранение в строку
        /// </summary>
        /// <returns></returns>
        public string SaveToString()
        {
            return string.Join("|", _stages.Select(x => $"{x.Key},{x.Value.SaveToString()}"));
        }

        /// <summary>
        /// Информация об уровне
        /// </summary>
        /// <param name="stageIndex"></param>
        /// <returns></returns>
        [NotNull]
        public StageInfo GetStageInfo(int stageIndex)
        {
            if (!_stages.TryGetValue(stageIndex, out var result))
            {
                result = new StageInfo();
                _stages[stageIndex] = result;
            }
            return result;
        }
    }

    /// <summary>
    /// Данные, которые должны сохраняться в устройстве
    /// </summary>
    internal class SavingData
    {
        /// <summary>
        /// Текущий уровень сложности (класс)
        /// </summary>
        public int CurrentLevel;

        /// <summary>
        /// Текущая глава
        /// </summary>
        public int CurrentStage;

        /// <summary>
        /// Информация об уровнях
        /// </summary>
        private readonly Dictionary<int, LevelInfo> _levelsInfo = new Dictionary<int, LevelInfo>();

        /// <summary>
        /// Заполучить информацию об уровне сложности 
        /// </summary>
        /// <returns></returns>
        public LevelInfo GetLevelInfo(int value)
        {
            if (!_levelsInfo.TryGetValue(value, out var result))
            {
                result = new LevelInfo();
                _levelsInfo[value] = result;
            }
            return result;
        }

        /// <summary>
        /// Загрузить из устройства
        /// </summary>
        /// <returns></returns>
        public static SavingData Load()
        {
            var result = new SavingData
            {
                CurrentLevel = PlayerPrefs.GetInt("CurrentLevel"),
                CurrentStage = PlayerPrefs.GetInt("CurrentStage")
            };
            foreach(var line in PlayerPrefs.GetString("Levels").Split('\n'))
            {
                var sepPos = line.IndexOf(',');
                if (sepPos == -1)
                    continue;
                if (!int.TryParse(line.Substring(0, sepPos), out var levelIndex))
                    continue;
                var levelInfo = LevelInfo.LoadFromString(line.Substring(sepPos + 1));
                if (levelInfo == null)
                    continue;
                result._levelsInfo[levelIndex] = levelInfo;
            }
            return result;
        }

        /// <summary>
        /// Сохранить данные
        /// </summary>
        public void Save()
        {
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
            PlayerPrefs.SetInt("CurrentStage", CurrentStage);
            var saveBuilder = new StringBuilder();
            foreach (var info in _levelsInfo)
                saveBuilder.AppendLine($"{info.Key},{info.Value.SaveToString()}");
            PlayerPrefs.SetString("Levels", saveBuilder.ToString());
        }
    }
}
