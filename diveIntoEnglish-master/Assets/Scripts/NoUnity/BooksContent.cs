using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.NoUnity
{
    /// <summary>
    /// Контент глав
    /// </summary>
    internal static class ChaptersContent
    {        
        /// <summary>
        /// Распарсить контент, получив главы
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [NotNull]
        private static IEnumerable<TestBook> ParseBooks([NotNull]string content)
        {
            const string bookSeparator = "---->";
            var bookCaption = string.Empty;
            var chapterCaption = string.Empty;
            var chapterWords = new List<(string eng, string rus)>();
            var bookChapters = new List<TestChapter>();
            foreach (var line in content.Split('\n').Skip(1).Select(x => x.Trim()))
            {
                if (line.StartsWith(";"))
                    continue;
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith(bookSeparator))
                {
                    if (chapterWords.Any() && !string.IsNullOrEmpty(chapterCaption))
                        bookChapters.Add(new TestChapter(chapterCaption, chapterWords.ToArray()));
                    if (bookChapters.Any())
                        yield return new TestBook(bookCaption, bookChapters.ToArray());
                    bookCaption = line.Substring(bookSeparator.Length).Trim();
                    bookChapters.Clear();
                    chapterWords.Clear();
                }

                var sepIndex = line.IndexOf('=');
                if (sepIndex == -1)
                {
                    if (chapterWords.Any() && !string.IsNullOrEmpty(chapterCaption))
                    {
                        bookChapters.Add(new TestChapter(chapterCaption, chapterWords.ToArray()));
                        chapterWords.Clear();
                    }
                    chapterCaption = line;
                }
                else
                {
                    var engWord = line.Substring(0, sepIndex).Trim();
                    var rusWord = line.Substring(sepIndex + 1).Trim();
                    if (!string.IsNullOrEmpty(engWord) && !string.IsNullOrEmpty(rusWord))
                        chapterWords.Add((engWord, rusWord));
                }
            }
            if (chapterWords.Any() && !string.IsNullOrEmpty(chapterCaption))
            {
                bookChapters.Add(new TestChapter(chapterCaption, chapterWords.ToArray()));
                yield return new TestBook(bookCaption, bookChapters.ToArray());
            }
        }

        /// <summary>
        /// Загрузить все главы
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestBook> LoadAllBooks()
        {
            return ParseBooks(((TextAsset)Resources.Load("Words")).text);
        }
    }
}