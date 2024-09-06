using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using JetBrains.Annotations;

namespace Assets.Scripts.NoUnity
{
    /// <summary>
    /// Утилиты
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// Определение размера главного экрана
        /// </summary>
        /// <returns></returns>
        public static Vector2 CalculateScreenSizeInWorldCoords()
        {
            var cam = Camera.main;
            var p1 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            var p2 = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
            var p3 = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
            var width = (p2 - p1).magnitude;
            var height = (p3 - p2).magnitude;
            return new Vector2(width, height);
        }

        /// <summary>
        /// Сообщить о необходимости двинуться на позицию
        /// </summary>
        /// <param name="index"></param>
        public static float IndexToPosition(int index)
        {
            var btnHeight = (GamePlay.Single.ScreenSize.y * 0.6f) / 4f;
            var bottmMargin = btnHeight * 0.21f;
            var topOffset = index * (btnHeight + bottmMargin) + GamePlay.Single.ScreenSize.y * 0.21f;
            return GamePlay.Single.ScreenSize.y / 2f - topOffset;
        }

        /// <summary>
        /// Средства получения случайного числа
        /// </summary>
        [NotNull]
        public static readonly System.Random Rnd = new System.Random();

        /// <summary>
        /// Перемешать порядок следования последовательности
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<T> RandomOrder<T>([NotNull]this IEnumerable<T> source)
        {
            return source.Select(x => (Rnd.Next(Int32.MaxValue), x))
                .OrderBy(x => x.Item1)
                .Select(x => x.Item2);
        }

        /// <summary>
        /// Получить произвольное слово по всем главам в целом
        /// </summary>
        /// <param name="testBook"></param>
        /// <param name="testKind"></param>
        /// <returns></returns>
        public static (string value, string valueTranslate) GetRandomWord([NotNull] this TestBook testBook, TestKind testKind)
        {
            var pair = testBook.AllPairs[Rnd.Next(testBook.AllPairs.Length)];
            return testKind == TestKind.WordIsEnglish ? (pair.rus, pair.eng) : (pair.eng, pair.rus);
        }

        /// <summary>
        /// Получить произвольное слово по всем главам в целом
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="testKind"></param>
        /// <returns></returns>
        public static (string value, string valueTranslate) GetRandomWord([NotNull] this TestChapter chapter, TestKind testKind)
        {
            var pair = chapter.Pairs[Rnd.Next(chapter.Pairs.Length)];
            return testKind == TestKind.WordIsEnglish ? (pair.rus, pair.eng) : (pair.eng, pair.rus);
        }
    }
}
