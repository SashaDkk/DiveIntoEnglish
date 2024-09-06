using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.NoUnity;
using UnityEngine.UI;

/// <summary>
/// Состояние опроса
/// </summary>
public enum GamePlayState
{
    Initialize,
    BeginShowQuestion,
    BeginShowAnswers,
    WaitUserAnswer,
    BeginShowQuestionResultAnimation,
}

public class GamePlay : MonoBehaviour
{
    /// <summary>
    /// Надпись о новом рекорде
    /// </summary>
    public GameObject NewRecordObject;
    
    /// <summary>
    /// Новый рекорд
    /// </summary>
    public AudioSource NewRecordSound;
    
    /// <summary>
    /// Звук коллекционироваия жизни
    /// </summary>
    public AudioSource CollectLifeSound;
    
    /// <summary>
    /// Клик
    /// </summary>
    public AudioSource BubbleClickSound;
    
    /// <summary>
    /// Звук взрыва
    /// </summary>
    public AudioSource ExplosionSound;

    /// <summary>
    /// Звук Монеты
    /// </summary>
    public AudioSource CoinSound;
    
    /// <summary>
    /// Бомба
    /// </summary>
    public GameObject MineNode;

    /// <summary>
    /// Монета
    /// </summary>
    public GameObject CoinNode;

    /// <summary>
    /// Жизнь
    /// </summary>
    public GameObject LifeNode;

    /// <summary>
    /// Взрыв пузырей
    /// </summary>
    public GameObject BubbleExplosion;

    /// <summary>
    /// Взрыв огня
    /// </summary>
    public GameObject FireExplosion;

    /// <summary>
    /// Отображалка счета
    /// </summary>
    public GameObject CoinLabelNode;

    /// <summary>
    /// Отображалка счета
    /// </summary>
    public GameObject HpLabelNode;

    /// <summary>
    /// Размер экрана
    /// </summary>
    private Vector2 _screenSize;

    /// <summary>
    /// Панель перехода
    /// </summary>
    public FaderPanel FaderPanel;

    /// <summary>
    /// Идикатор
    /// </summary>
    public GameObject ProgressPanel;

    /// <summary>
    /// Картина с лодкой на индикаторе
    /// </summary>
    public GameObject ProgressPanelImage;

    /// <summary>
    /// Двигатель прогресса
    /// </summary>
    private NodeMover _progressMover;

    /// <summary>
    /// Дельта каждого шага
    /// </summary>
    private float _progressDelta;

    /// <summary>
    /// Размер экрана
    /// </summary>
    public Vector2 ScreenSize => _screenSize;

    /// <summary>
    /// Индекс последнего выбранного ответа
    /// </summary>
    private int _lastAnswerIndex;

    /// <summary>
    /// Индекс последнего выбранного ответа
    /// </summary>
    public int LastAnswerIndex => _lastAnswerIndex;

    /// <summary>
    /// Синглтон
    /// </summary>
    public static GamePlay Single;

    /// <summary>
    /// Текущее состояние игрового процесса
    /// </summary>
    private GamePlayState _state = GamePlayState.Initialize;

    /// <summary>
    /// Ответы скрыты
    /// </summary>
    private bool _answersHidden;

    /// <summary>
    /// Мульт со столкновением показан
    /// </summary>
    private bool _movieShown;

    /// <summary>
    /// Значение обратного отсчета
    /// </summary>
    private int _countdownValue;

    /// <summary>
    /// Кол-во жизней
    /// </summary>
    private int _hpValue;

    /// <summary>
    /// Кол-во жизней
    /// </summary>
    public int HpValue => _hpValue;

    /// <summary>
    /// Количество собранных монет
    /// </summary>
    private int _counsValue;

    /// <summary>
    /// Количество собранных монет
    /// </summary>
    public int CoinsValue => _counsValue;

    /// <summary>
    /// Выполняемый тест
    /// </summary>
    internal Test ActiveTest;

    /// <summary>
    /// Текущее состояние игрового процесса
    /// </summary>
    public GamePlayState State => _state;

    /// <summary>
    /// Предыдущая информация об уровне
    /// </summary>
    private StageInfo _stageInfo;

    // Start is called before the first frame update
    void Start()
    {
        Single = this;
        ActiveTest = TestsManager.Single.CurrentBook.CurrentChapter.CreateTest(TestKind.WordIsEnglish);
        _screenSize = Helpers.CalculateScreenSizeInWorldCoords();
        _hpValue = GamePlaySettings.StartHp;
        _progressDelta = (ProgressPanel.GetComponent<RectTransform>().rect.width - ProgressPanelImage.GetComponent<RectTransform>().rect.width) / (ActiveTest.QuestionsCount - 1);
        _stageInfo = RuntimeEnvironment.SavingData.GetLevelInfo(TestsManager.Single.CurrentBookIndex).GetStageInfo(TestsManager.Single.CurrentBook.CurrentChapterIndex);
        CoinLabelNode.GetComponent<Text>().text = _stageInfo.MaxCoins == 0 ? "0" : $"0 / {_stageInfo.MaxCoins}";
        _lastAnswerIndex = 2;
    }

    /// <summary>
    /// Уведомление о завершении показа хинта
    /// Пора выпускать лодку
    /// </summary>
    public void NotifyHintWasShown()
    {
        if (Submarine.Single.GetComponent<Animator>().enabled)
            NotifyStartGame();
        else
        {
            Submarine.Single.gameObject.SetActive(true);
            Submarine.Single.GetComponent<Animator>().enabled = true;
        }
    }

    /// <summary>
    /// Уведомление о начале работы (лодка вышла)
    /// </summary>
    public void NotifyStartGame()
    {
        if (_state == GamePlayState.Initialize)
        {
            _state = GamePlayState.BeginShowQuestion;
            Question.Single.BeginShowQuestion();
        }
    }

    /// <summary>
    /// Уведомление о показе вопроса
    /// </summary>
    public void NotifyQuestionShown()
    {
        if (_state == GamePlayState.BeginShowQuestion)
        {
            _state = GamePlayState.BeginShowAnswers;
            Words.Single.BeginShowWords();
        }
    }

    /// <summary>
    /// Ответы показаны
    /// </summary>
    public void NotifyAnswersShown()
    {
        if (_state == GamePlayState.BeginShowAnswers)
        {
            _countdownValue = GamePlaySettings.TimeToAnswerSeconds;
            CountDown.Single.PlayCountdownFrame(_countdownValue);
            _state = GamePlayState.WaitUserAnswer;
        }
    }

    /// <summary>
    /// Перемещалка субмарины
    /// </summary>
    private NodeMover _submarineMover;

    /// <summary>
    /// Перемещалка мин и монет
    /// </summary>
    private readonly List<NodeMover> _objectMovers = new List<NodeMover>();

    /// <summary>
    /// Индекс позиции субмарины
    /// </summary>
    private int? _lastSubmarinePositionIndex;

    /// <summary>
    /// Сообщить об ответе пользователя
    /// </summary>
    public void NotifyUserAnswer(int index)
    {
        BubbleClickSound.Play();
        Debug.Log($"NotifyUserAnswer - {_state}");
        if (_state == GamePlayState.WaitUserAnswer)
        {
            _lastAnswerIndex = index;
            _answersHidden = false;
            _movieShown = false;
            _state = GamePlayState.BeginShowQuestionResultAnimation;
            Question.Single.BeginHideQuestion();
            _submarineMover = new NodeMover(Submarine.Single.gameObject, null, Helpers.IndexToPosition(index), 5);
            ActiveTest.CurrentQuestion.RegisterAnswer(index);
        }
        else
            Debug.Log($"!!!");
    }

    /// <summary>
    /// Субмарина встала на позицию
    /// </summary>
    private void NotifySubmarineMovedToPosition()
    {
        const float animationItemsSpeed = 10.0f;
        
        _lastSubmarinePositionIndex = ActiveTest.CurrentQuestion.SelectedAnswerIndex;
        // Рисование мин и монеток
        for (var i = 0; i < ActiveTest.CurrentQuestion.Answers.Length; i++)
        {
            var yPosition = Helpers.IndexToPosition(i);
            if (_countdownValue > 0 && i == ActiveTest.CurrentQuestion.RightIndex)
            {
                var coinSprite = CoinNode.GetComponent<SpriteRenderer>();
                var coinWidth = coinSprite.size.x;
                for (var j = 0; j < _countdownValue; j++)
                {
                    var coin = Instantiate(CoinNode);
                    coin.tag = i.ToString();
                    coin.transform.position = new Vector3(0.6f * ScreenSize.x + coinWidth * 1.1f * j, yPosition);
                    _objectMovers.Add(new NodeMover(coin, - 1.1f * ScreenSize.x / 2f, null, animationItemsSpeed));
                }
                // Если это последний вопрос уровня, рисуем жизни
                if (ActiveTest.QuestionIndex == ActiveTest.QuestionsCount - 1 && ActiveTest.Kind == TestKind.WordIsRussian)
                {
                    var liftSprite = LifeNode.GetComponent<SpriteRenderer>();
                    var lifeWidth = liftSprite.size.x;
                    for (var j = 0; j < _hpValue; j++)
                    {
                        var life = Instantiate(LifeNode);
                        life.tag = i.ToString();
                        life.transform.position = new Vector3(0.6f * ScreenSize.x + coinWidth * 1.1f * _countdownValue + lifeWidth * 1.2f * j, yPosition);
                        _objectMovers.Add(new NodeMover(life, -1.1f * ScreenSize.x / 2f, null, animationItemsSpeed));
                    }
                }
            }
            else
            {
                var mine = Instantiate(MineNode);
                mine.tag = i.ToString();
                mine.transform.position = new Vector3(0.6f * ScreenSize.x, yPosition);
                _objectMovers.Add(new NodeMover(mine, -1.1f * ScreenSize.x / 2f, null, animationItemsSpeed));
            }
        }
    }

    /// <summary>
    /// Уведомление о сокрытии вопроса
    /// </summary>
    public void NotifyQuestionHide()
    {
        if (_state == GamePlayState.BeginShowQuestionResultAnimation)
            Words.Single.BeginHideWords();
    }

    /// <summary>
    /// Проверить возможный переход к следующему вопросу
    /// </summary>
    private void NotifyPossibleGotoNextQuestion()
    {
        Debug.Log($"NotifyPossibleGotoNextQuestion, _state = {_state}, _answersHidden = {_answersHidden}, _movieShown = {_movieShown} ");
        if (_state == GamePlayState.BeginShowQuestionResultAnimation && _answersHidden && _movieShown)
        {
            _state = GamePlayState.BeginShowQuestion;
            Question.Single.BeginShowQuestion();
        }
    }

    /// <summary>
    /// Все ответы скрыты
    /// </summary>
    public void NotifyAnswersHide()
    {
        Debug.Log($"NotifyAnswersHide()  _state == BeginShowQuestionResultAnimation");
        if (_state == GamePlayState.BeginShowQuestionResultAnimation)
        {
            _answersHidden = true;
            NotifyPossibleGotoNextQuestion();
        }
    }

    /// <summary>
    /// Субманира столкнулась с врагом
    /// </summary>
    public void NotifySubmarineCollide(GameObject collidedObject)
    {
        if (_lastSubmarinePositionIndex == null || _lastSubmarinePositionIndex.Value.ToString() == collidedObject.tag)
        {
            // Двигаем индикатор
            var panelPosition = ProgressPanel.GetComponent<RectTransform>();
            _progressMover = new NodeMover(ProgressPanelImage, _progressDelta * (ActiveTest.QuestionIndex + 1), 0, 190 / ActiveTest.QuestionsCount);
            if (ActiveTest.CurrentQuestion.AnswerIsOk)
            {
                _objectMovers.RemoveAll(x => x.ObjectToMove == collidedObject);
                collidedObject.GetComponent<Animator>().enabled = true;
                var isCoin = collidedObject.gameObject.name.StartsWith("Coin");
                if (isCoin)
                    CoinSound.Play();
                else
                    CollectLifeSound.Play();
                _objectMovers.Add(new NodeMover(collidedObject, -0.8f * ScreenSize.x / 2f, -0.8f * ScreenSize.y / 2f, 5) 
                { 
                    Tag = () => 
                    {
                        var prevCounsValue = _counsValue;
                        _counsValue = isCoin ? _counsValue + 1 : _counsValue * 2;
                        if (_stageInfo.MaxCoins > 0 && prevCounsValue <= _stageInfo.MaxCoins && _counsValue > _stageInfo.MaxCoins)
                        {
                            NewRecordSound.Play();
                            NewRecordObject.GetComponent<Animator>().enabled = true;
                        }
                        CoinLabelNode.GetComponent<Text>().text = _stageInfo.MaxCoins == 0 || _counsValue > _stageInfo.MaxCoins ? _counsValue.ToString() : $"{_counsValue} / {_stageInfo.MaxCoins}";
                    }
                });
            }
            else
            {
                // Взрыв
                _objectMovers.RemoveAll(x => x.ObjectToMove == collidedObject);
                Destroy(collidedObject);
                var layerCounter = 0;
                foreach(var explosionPrefab in new[] { BubbleExplosion, FireExplosion })
                {
                    layerCounter++;
                    var explosion = Instantiate(explosionPrefab.GetComponent<ParticleSystem>(), 
                        Submarine.Single.transform.position, Quaternion.identity);
                    var explosionRenderer = explosion.gameObject.GetComponent<Renderer>();
                    explosionRenderer.sortingLayerName = "Player";
                    explosionRenderer.sortingOrder = -layerCounter;
                    Destroy(explosion.gameObject, explosion.main.startLifetime.constant);
                }
                _hpValue--;
                HpLabelNode.GetComponent<Text>().text = $"x {_hpValue}";
                ExplosionSound.Play();
                // Ранил
                if (_hpValue > 0)
                    Submarine.Single.GetComponent<Animator>().SetBool("Damaged", true);
                // Или убил
                else
                {
                    Submarine.Single.GetComponent<Animator>().SetBool("Dead", true);
                    Submarine.Single.GetComponent<Rigidbody2D>().gravityScale = 0.8f;
                    FaderPanel.FadeOutByHand();
                }
            }
        }
    }

    /// <summary>
    /// Анимационное отображение результата завершено
    /// </summary>
    private void NotifyAnimationResultDone()
    {
        if (_state == GamePlayState.BeginShowQuestionResultAnimation)
        {
            if (_hpValue > 0)
            {
                _movieShown = true;
                ActiveTest.Next();
                if (ActiveTest.Done)
                {
                    // Если это первая часть тестирования
                    if (ActiveTest.Kind == TestKind.WordIsEnglish)
                    {
                        ActiveTest = TestsManager.Single.CurrentBook.CurrentChapter.CreateTest(TestKind.WordIsRussian);
                        _state = GamePlayState.Initialize;
                        HintTestKindLabel.Single.GetComponent<Text>().text = "Перевод\nна английский";
                        HintTestKindLabel.Single.gameObject.SetActive(true);
                        ProgressPanelImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                        _progressMover = null;
                    }
                    // Иначе тестирование закончено
                    else
                        FaderPanel.FadeOutByHand();
                }
                else
                    NotifyPossibleGotoNextQuestion();
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (State == GamePlayState.BeginShowQuestionResultAnimation)
        {
            if (_submarineMover != null)
            {
                if (!_submarineMover.Update())
                {
                    _submarineMover = null;
                    NotifySubmarineMovedToPosition();
                }
            }
        }
        _progressMover?.Update();
        if (_objectMovers.Any())
        {
            for(var i = _objectMovers.Count - 1; i >= 0; i--)
            {
                var mover = _objectMovers[i];
                if (!mover.Update())
                {
                    _objectMovers.RemoveAt(i);
                    mover.Tag?.Invoke();
                    Destroy(mover.ObjectToMove);
                }
            }
            if (!_objectMovers.Any())
                NotifyAnimationResultDone();
        }
    }

    /// <summary>
    /// Уведомление о шаге обратного отсчета
    /// </summary>
    public void NotifyCountDownStep()
    {
        if (_state == GamePlayState.WaitUserAnswer)
        {
            _countdownValue--;
            if (_countdownValue > 0)
                CountDown.Single.PlayCountdownFrame(_countdownValue);
            else
            {
                _answersHidden = false;
                _movieShown = false;
                _state = GamePlayState.BeginShowQuestionResultAnimation;
                Question.Single.BeginHideQuestion();
                NotifySubmarineMovedToPosition();
            }
        }
    }

    /// <summary>
    /// Клик по кнопке Выход
    /// </summary>
    public void OnBtnExitClick()
    {
        BubbleClickSound.Play();
        FaderPanel.NextStageName = "MainMenu";
        FaderPanel.FadeOutByHand();
    }
}
