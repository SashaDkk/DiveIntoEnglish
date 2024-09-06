using Assets.Scripts.NoUnity;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageResultUiBehaviour : MonoBehaviour
{
    /// <summary>
    /// Наименование книги
    /// </summary>
    public GameObject LevelCaption;

    /// <summary>
    /// Наименование уровня
    /// </summary>
    public GameObject StageCaption;
    
    /// <summary>
    /// Узел канвы
    /// </summary>
    public GameObject CanvasNode;

    /// <summary>
    /// Счет
    /// </summary>
    public GameObject CoinsValueNode;

    /// <summary>
    /// Кнопка перехода к следующему уровню
    /// </summary>
    public GameObject BtnNextNode;

    /// <summary>
    /// Панель выхода
    /// </summary>
    public FaderPanel FadePnl;

    /// <summary>
    /// Элемент "Новый рекорд"
    /// </summary>
    public GameObject NewRecordNode;

    /// <summary>
    /// Элемент "Открыт новый уровень"
    /// </summary>
    public GameObject NewStageNode;

    /// <summary>
    /// Хорошая музыка
    /// </summary>
    public AudioSource GoodMusic;

    /// <summary>
    /// Плохая музыка
    /// </summary>
    public AudioSource BadMusic;

    /// <summary>
    /// Клик по кнопке
    /// </summary>
    public AudioSource BubbleClick;

    /// <summary>
    /// Бонус лучшего результата
    /// </summary>
    public GameObject BestResult;

    /// <summary>
    /// Пузырьки сзади
    /// </summary>
    public GameObject BackgroundBubbles;

    // Start is called before the first frame update
    void Start()
    {
        CoinsValueNode.GetComponent<Text>().text = GamePlay.Single.CoinsValue.ToString();
        var curLevelInfo = RuntimeEnvironment.SavingData.GetLevelInfo(TestsManager.Single.CurrentBookIndex).GetStageInfo(TestsManager.Single.CurrentBook.CurrentChapterIndex);
        NewRecordNode.SetActive(curLevelInfo.NotifyCoins(GamePlay.Single.CoinsValue));
        var newStageLabelVisible = GamePlay.Single.HpValue > 0 && curLevelInfo.NotifySucceed();
        NewStageNode.SetActive(newStageLabelVisible);
        if (newStageLabelVisible && TestsManager.Single.CurrentBook.CurrentChapterIndex == TestsManager.Single.CurrentBook.Chapters.Length - 1)
            NewStageNode.GetComponent<Text>().text = "Уровень пройден!";
        RuntimeEnvironment.SavingData.Save();
        var nextLevelAllowed = TestsManager.Single.CurrentBook.CurrentChapterIndex < TestsManager.Single.CurrentBook.Chapters.Length - 1 && curLevelInfo.Succeed > 0;
        BtnNextNode.SetActive(nextLevelAllowed);
        LevelCaption.GetComponent<Text>().text = TestsManager.Single.CurrentBook.Caption;
        StageCaption.GetComponent<Text>().text = TestsManager.Single.CurrentBook.CurrentChapter.Caption;
        foreach (var animator in CanvasNode.GetComponentsInChildren<Animator>())
        {
            if (!animator.enabled)
                continue;
            animator.Update(Random.value);
            animator.speed = 0.8f;
        }

        if (GamePlay.Single.HpValue > 0)
        {
            GoodMusic.Play();
            var maxValue = GamePlay.Single.ActiveTest.QuestionsCount * 2 * (GamePlaySettings.TimeToAnswerSeconds - 2);
            for (var i = 0; i < GamePlaySettings.StartHp; i++)
                maxValue *= 2;
            if (GamePlay.Single.CoinsValue >= maxValue)
            {
                BestResult.SetActive(true);
                BackgroundBubbles.SetActive(false);
            }
        }
        else
            BadMusic.Play();
    }

    /// <summary>
    /// Клик по переходу на следующий уровень
    /// </summary>
    public void OnNextLevelBtnClick()
    {
        BubbleClick.Play();
        TestsManager.Single.CurrentBook.SetCurrentChapterIndex(TestsManager.Single.CurrentBook.CurrentChapterIndex + 1);
        RuntimeEnvironment.SavingData.CurrentStage = TestsManager.Single.CurrentBook.CurrentChapterIndex;
        RuntimeEnvironment.SavingData.Save();
        FadePnl.NextStageName = "StagePrestart";
        FadePnl.FadeOutByHand();
    }

    /// <summary>
    /// Клик по кнопке переиграть текущий уровень
    /// </summary>
    public void OnReplayBtnClick()
    {
        BubbleClick.Play();
        FadePnl.NextStageName = "StagePrestart";
        FadePnl.FadeOutByHand();
    }

    /// <summary>
    /// Клик по кнопке главного меню
    /// </summary>
    public void OnMainMenuBtnClick()
    {
        BubbleClick.Play();
        FadePnl.NextStageName = "MainMenu";
        FadePnl.FadeOutByHand();
    }
}
