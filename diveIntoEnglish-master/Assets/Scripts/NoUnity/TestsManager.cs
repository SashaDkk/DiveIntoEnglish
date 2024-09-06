using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Assets.Scripts.NoUnity
{
    /// <summary>
    /// Один вопрос теста
    /// </summary>
    internal class TestQuestion
    {
        /// <summary>
        /// Слово, которое подлежит переводу
        /// </summary>
        public readonly string Word;

        /// <summary>
        /// Ответы на слово
        /// </summary>
        public readonly (string value, string valueTranslate)[] Answers;

        /// <summary>
        /// Индекс правильного ответа
        /// </summary>
        private readonly int _rightIndex;

        /// <summary>
        /// Ответ
        /// </summary>
        private int? _answerIndex; 

        /// <summary>
        /// Зарегистировать ответ
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void RegisterAnswer(int value)
        {
            _answerIndex = value;
        }

        /// <summary>
        /// Индекс правильного ответа
        /// </summary>
        public int RightIndex => _rightIndex;

        /// <summary>
        /// Ответ правильный
        /// </summary>
        public bool AnswerIsOk => _answerIndex == _rightIndex;

        /// <summary>
        /// Правильный ответ
        /// </summary>
        public string RightAnswer => Answers[_rightIndex].value;

        /// <summary>
        /// Индекс выбранного ответа
        /// </summary>
        public int? SelectedAnswerIndex => _answerIndex;

        /// <summary>
        /// Выбранный ответ
        /// </summary>
        public (string value, string valueTranslate) SelectedAnswer
        {
            get
            {
                if (_answerIndex == null)
                    throw new Exception("Обращение к индексу ответа до его инициализации!");
                return Answers[_answerIndex.Value];
            }

        }

        /// <summary>
        /// Создание вопроса
        /// </summary>
        /// <param name="word"></param>
        /// <param name="answer"></param>
        /// <param name="trashAnswers"></param>
        public TestQuestion([NotNull] string word, [NotNull]string answer, [NotNull](string value, string valueTranslate)[] trashAnswers)
        {
            Word = word;
            Answers = new (string, string)[trashAnswers.Length + 1];
            _rightIndex = Helpers.Rnd.Next(Answers.Length);
            for (int i = 0; i < Answers.Length; i++)
            {
                if (i < _rightIndex)
                    Answers[i] = trashAnswers[i];
                if (i == _rightIndex)
                    Answers[i] = (answer, word);
                if (i > _rightIndex)
                    Answers[i] = trashAnswers[i - 1];
            }
        }
    }
    
    /// <summary>
    /// Одно тестирование
    /// </summary>
    internal class Test
    {
        /// <summary>
        /// Тип теста
        /// </summary>
        public readonly TestKind Kind;
        
        /// <summary>
        /// Заголовок теста
        /// </summary>
        public readonly string Caption;
        
        /// <summary>
        /// Индекс текущего вопроса
        /// </summary>
        private int _questionIndex;
        
        /// <summary>
        /// Вопросы
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly TestQuestion[] _questions;

        /// <summary>
        /// Получить текущий вопрос
        /// </summary>
        [NotNull]
        public TestQuestion CurrentQuestion => _questions[_questionIndex];

        /// <summary>
        /// Число вопросов
        /// </summary>
        public int QuestionsCount => _questions.Length;

        /// <summary>
        /// Число правильных ответов
        /// </summary>
        public int GoodCount => _questions.Count(x => x.AnswerIsOk);

        /// <summary>
        /// Перейти к следующему вопросу
        /// </summary>
        /// <returns></returns>
        public void Next()
        {
            _questionIndex++;
        }

        /// <summary>
        /// Тест пройден
        /// </summary>
        public bool Done => _questionIndex > _questions.Length - 1;

        /// <summary>
        /// Индекс текущего вопроса
        /// </summary>
        public int QuestionIndex => _questionIndex;

        /// <summary>
        /// Создание
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="questions"></param>
        public Test(TestKind kind, [NotNull]string caption, [NotNull, ItemNotNull] TestQuestion[] questions)
        {
            Kind = kind;
            Caption = caption;
            _questions = questions;
        }
    }

    /// <summary>
    /// Одна глава исходных данных
    /// </summary>
    internal class TestChapter
    {
        /// <summary>
        /// Наименование главы
        /// </summary>
        [NotNull]
        public readonly string Caption;

        /// <summary>
        /// Набор пар слово - перевод
        /// </summary>
        [NotNull]
        public readonly (string eng, string rus)[] Pairs;

        /// <summary>
        /// Создать тест по конкретной части
        /// </summary>
        /// <returns></returns>
        public Test CreateTest(TestKind kind)
        {
            var questions = new List<TestQuestion>();
            foreach (var (eng, rus) in Pairs.RandomOrder())
            {
                var word = kind == TestKind.WordIsEnglish ? eng : rus;
                var answer = kind == TestKind.WordIsEnglish ? rus : eng;
                var randomAnswers = new HashSet<(string, string)>();
                while (randomAnswers.Count < 3)
                {
                    var newWord = this.GetRandomWord(kind);
                    if (newWord.value == answer)
                        continue;
                    randomAnswers.Add(newWord);
                }
                questions.Add(new TestQuestion(word, answer, randomAnswers.ToArray()));
            }
            return new Test(kind, Caption, questions.ToArray());
        }

        /// <summary>
        /// Создание 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="pairs"></param>
        public TestChapter([NotNull] string caption, params (string eng, string rus)[] pairs)
        {
            Caption = caption;
            Pairs = pairs.ToArray();
        }
    }

    /// <summary>
    /// Одна книга исходных данных
    /// Книга = класс (наверное, всегда)
    /// </summary>
    internal class TestBook
    {
        /// <summary>
        /// Текущая глава
        /// </summary>
        [NotNull]
        public TestChapter CurrentChapter => Chapters[_currentChapterIndex];

        /// <summary>
        /// Индекс текущей главы
        /// </summary>
        private int _currentChapterIndex;

        /// <summary>
        /// Индекс текущей главы
        /// </summary>
        public int CurrentChapterIndex => _currentChapterIndex;

        /// <summary>
        /// Установить текущую главу по наименованию
        /// </summary>
        /// <param name="chapterCaption"></param>
        /// <param name="saveOnDisk"></param>
        public void SetCurrentChapterByCaption([NotNull]string chapterCaption)
        {
            _currentChapterIndex = 0;
            for(var i = 0; i < Chapters.Length; i ++)
            {
                if (Chapters[i].Caption == chapterCaption)
                {
                    _currentChapterIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Установить текущую главу по номеру
        /// </summary>
        /// <param name="value"></param>
        public void SetCurrentChapterIndex(int value)
        {
            if (value > -1 && value < Chapters.Length)
                _currentChapterIndex = value;
        }

        /// <summary>
        /// Наименование книги
        /// </summary>
        [NotNull]
        public readonly string Caption;

        /// <summary>
        /// Главы
        /// </summary>
        [NotNull, ItemNotNull]
        public readonly TestChapter[] Chapters;

        /// <summary>
        /// Создать тест по всем частям
        /// </summary>
        /// <returns></returns>
        public Test CreateTestAllChapters(TestKind kind)
        {
            var questions = new List<TestQuestion>();
            var chaptersToUse = Chapters
                .Reverse()
                .SkipWhile(x => x != CurrentChapter);
            foreach (var (eng, rus) in chaptersToUse.SelectMany(x => x.Pairs).RandomOrder())
            {
                var word = kind == TestKind.WordIsEnglish ? eng : rus;
                var answer = kind == TestKind.WordIsEnglish ? rus : eng;
                var randomAnswers = new HashSet<(string, string)>();
                while (randomAnswers.Count < 3)
                {
                    var newWord = this.GetRandomWord(kind);
                    if (newWord.value == answer)
                        continue;
                    randomAnswers.Add(newWord);
                }
                questions.Add(new TestQuestion(word, answer, randomAnswers.ToArray()));
            }
            return new Test(kind, "Все слова", questions.ToArray());
        }

        /// <summary>
        /// Создать тест по случайным частям
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Test CreateTestLastChapters(int count, TestKind kind)
        {
            var questions = new HashSet<TestQuestion>();
            var chaptersToUse = Chapters
                .Reverse()
                .SkipWhile(x => x != CurrentChapter)
                .Take(count)
                .ToArray();
            foreach (var (eng, rus) in chaptersToUse.SelectMany(x => x.Pairs).RandomOrder())
            {
                var word = kind == TestKind.WordIsEnglish ? eng : rus;
                var answer = kind == TestKind.WordIsEnglish ? rus : eng;
                var randomAnswers = new HashSet<(string, string)>();
                while (randomAnswers.Count < 3)
                {
                    var newWord = this.GetRandomWord(kind);
                    if (newWord.value == answer)
                        continue;
                    randomAnswers.Add(newWord);
                }
                questions.Add(new TestQuestion(word, answer, randomAnswers.ToArray()));
            }
            // вопросов может оказаться меньше
            return new Test(kind, $"Последние {chaptersToUse.Length} главы", questions.ToArray());
        }

        /// <summary>
        /// Создать тест по случайным частям
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Test CreateTestRandomWords(int count, TestKind kind)
        {
            var questions = new HashSet<TestQuestion>();
            var chaptersToUse = Chapters
                .Reverse()
                .SkipWhile(x => x != CurrentChapter);
            foreach (var (eng, rus) in chaptersToUse.SelectMany(x => x.Pairs).RandomOrder().Take(count))
            {
                var word = kind == TestKind.WordIsEnglish ? eng : rus;
                var answer = kind == TestKind.WordIsEnglish ? rus : eng;
                var randomAnswers = new HashSet<(string, string)>();
                while (randomAnswers.Count < 3)
                {
                    var newWord = this.GetRandomWord(kind);
                    if (newWord.value == answer)
                        continue;
                    randomAnswers.Add(newWord);
                }
                questions.Add(new TestQuestion(word, answer, randomAnswers.ToArray()));
            }
            // вопросов может оказаться меньше
            return new Test(kind, $"Случайные {questions.Count} слов", questions.ToArray());
        }

        /// <summary>
        /// Все пары слов
        /// </summary>
        private readonly Lazy<(string eng, string rus)[]> _allPairs;

        /// <summary>
        /// Получить все пары слов
        /// </summary>
        public (string eng, string rus)[] AllPairs => _allPairs.Value;

        /// <summary>
        /// Создание
        /// </summary>
        public TestBook([NotNull]string caption, [NotNull, ItemNotNull]TestChapter[] chapters)
        {
            Caption = caption;
            Chapters = chapters;
            _allPairs = new Lazy<(string eng, string rus)[]>(() =>
            {
                var result = new List<(string eng, string rus)>();
                foreach (var chapter in chapters)
                    result.AddRange(chapter.Pairs);
                return result.ToArray();
            });
        }
    }

    /// <summary>
    /// Тип теста
    /// </summary>
    internal enum TestKind
    {
        /// <summary>
        /// Слово на аинглийском
        /// </summary>
        WordIsEnglish,

        /// <summary>
        /// Слово на русском
        /// </summary>
        WordIsRussian
    }
    
    /// <summary>
    /// Средства тестирования
    /// </summary>
    internal class TestsManager
    {
        /// <summary>
        /// Название файла текущей книги
        /// </summary>
        [NotNull]
        private const string CURRENT_BOOK_FILE = "currentBook.txt";

        /// <summary>
        /// Текущая глава
        /// </summary>
        [NotNull]
        public const string CURRENT_CHAPTER_FILE = "currentChapter.txt";

        /// <summary>
        /// Текущая глава
        /// </summary>
        [NotNull]
        public const string CURRENT_TEST_KIND_FILE = "currentTestKind.txt";

        /// <summary>
        /// Единственный экзеимпляр
        /// </summary>
        private static TestsManager _single;

        /// <summary>
        /// Единственный экзеимпляр
        /// </summary>
        [NotNull]
        public static TestsManager Single => _single;

        /// <summary>
        /// Выполнить начальную инициализацию
        /// </summary>
        public static void Initialize()
        {
            if (_single == null)
                _single = new TestsManager();
        }

        /// <summary>
        /// Индекс выбранной книги
        /// </summary>
        private int _currentBookIndex;

        /// <summary>
        /// Индекс выбранной книги
        /// </summary>
        public int CurrentBookIndex => _currentBookIndex;

        /// <summary>
        /// Все книги
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly TestBook[] _allBooks;

        /// <summary>
        /// Все главы
        /// </summary>
        [NotNull, ItemNotNull]
        public TestBook[] AllBooks => _allBooks;

        /// <summary>
        /// Получить текущую книгу
        /// </summary>
        [NotNull]
        public TestBook CurrentBook => AllBooks[_currentBookIndex];

        /// <summary>
        /// Установить текущую книгу
        /// </summary>
        /// <param name="bookCaption"></param>
        /// <param name="saveOnDisk"></param>
        public void SetCurrentBookByCaption([NotNull]string bookCaption)
        {
            _currentBookIndex = 0;
            for (var i = 0; i < AllBooks.Length; i++)
            {
                if (AllBooks[i].Caption == bookCaption)
                {
                    _currentBookIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Установить индекс книги (класса)
        /// </summary>
        /// <param name="value"></param>
        public void SetCurrentBookIndex(int value)
        {
            if (value > -1 && value < AllBooks.Length)
                _currentBookIndex = value;
        }

        /// <summary>
        /// Конструктор закрыт, т.к. явное создание этого объекта запрещено
        /// </summary>
        private TestsManager()
        {
            _allBooks = ChaptersContent.LoadAllBooks().ToArray();
        }
    }
}