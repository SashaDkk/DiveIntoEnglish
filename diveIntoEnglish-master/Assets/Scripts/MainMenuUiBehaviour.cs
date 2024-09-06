using Assets.Scripts.NoUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUiBehaviour : MonoBehaviour
{
    /// <summary>
    /// Клик по кнопке
    /// </summary>
    public AudioSource BubbleClick;
    
    /// <summary>
    /// Элемент канвы
    /// </summary>
    public GameObject CanvasNode;

    /// <summary>
    /// Затеняющая панель
    /// </summary>
    public GameObject FaderPanel;

    /// <summary>
    /// Текст названия уровня сложности
    /// </summary>
    public GameObject LevelCaptionNode;

    /// <summary>
    /// Текст названия Stepа (главы)
    /// </summary>
    public GameObject StageCaptionNode;

    /// <summary>
    /// Кнопка "Предыдущий уровень"
    /// </summary>
    public GameObject BtnPrevStageNode;

    /// <summary>
    /// Кнопка "Следующий уровень"
    /// </summary>
    public GameObject BtnNextStageNode;

    /// <summary>
    /// Изображение замка
    /// </summary>
    public GameObject ImgLock;

    /// <summary>
    /// Изображение нормальной кнопки
    /// </summary>
    public GameObject EnabledBtnImage;

    /// <summary>
    /// Изображение отключенной кнопки
    /// </summary>
    public GameObject DisabledBtnImage;

    /// <summary>
    /// Изменить доступность кнопки
    /// </summary>
    /// <param name="btnObject"></param>
    /// <param name="value"></param>
    private void SetBtnEnabled(GameObject btnObject, bool value)
    {
        btnObject.GetComponent<Button>().enabled = value;
        btnObject.GetComponent<Animator>().enabled = value;
        var otherImage = value ? EnabledBtnImage : DisabledBtnImage;
        btnObject.GetComponentsInChildren<Image>().Skip(1).First().sprite = otherImage.GetComponent<Image>().sprite;
    }

    /// <summary>
    /// Обновить элементы управления
    /// </summary>
    private void UpdateControls()
    {
        LevelCaptionNode.GetComponent<Text>().text = TestsManager.Single.CurrentBook.Caption;
        StageCaptionNode.GetComponent<Text>().text = TestsManager.Single.CurrentBook.CurrentChapter.Caption;
        SetBtnEnabled(BtnPrevStageNode, TestsManager.Single.CurrentBook.CurrentChapterIndex > 0);
        if (TestsManager.Single.CurrentBook.CurrentChapterIndex == TestsManager.Single.CurrentBook.Chapters.Length - 1)
        {
            SetBtnEnabled(BtnNextStageNode, false);
            ImgLock.SetActive(false);
        }
        else
        {
            var stageInfo = RuntimeEnvironment.SavingData.GetLevelInfo(TestsManager.Single.CurrentBookIndex).GetStageInfo(TestsManager.Single.CurrentBook.CurrentChapterIndex);
            // Уровень заперт, пока не пройден текущий
            SetBtnEnabled(BtnNextStageNode, stageInfo.Succeed > 0);
            ImgLock.SetActive(stageInfo.Succeed == 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var animator in CanvasNode.GetComponentsInChildren<Animator>())
        {
            animator.Update(Random.value);
            animator.speed = 0.8f;
        }
        TestsManager.Initialize();
        RuntimeEnvironment.SavingData = SavingData.Load();
        TestsManager.Single.SetCurrentBookIndex(RuntimeEnvironment.SavingData.CurrentLevel);
        TestsManager.Single.CurrentBook.SetCurrentChapterIndex(RuntimeEnvironment.SavingData.CurrentStage);
        UpdateControls();
    }

    /// <summary>
    /// Клик по кнопке Старт!
    /// </summary>
    public void BtnStartClick()
    {
        BubbleClick.Play();
        FaderPanel.GetComponent<FaderPanel>().FadeOutByHand();
        RuntimeEnvironment.SavingData.CurrentLevel = TestsManager.Single.CurrentBookIndex;
        RuntimeEnvironment.SavingData.CurrentStage = TestsManager.Single.CurrentBook.CurrentChapterIndex;
        RuntimeEnvironment.SavingData.Save();
    }

    /// <summary>
    /// Клик на уровень назад
    /// </summary>
    public void BtnPrevLevelClick()
    {
        BubbleClick.Play();
        var newLevelIndex = TestsManager.Single.CurrentBookIndex - 1;
        if (newLevelIndex < 0)
            newLevelIndex = TestsManager.Single.AllBooks.Length - 1;
        TestsManager.Single.SetCurrentBookIndex(newLevelIndex);
        UpdateControls();
    }

    /// <summary>
    /// Клик на уровень вперед
    /// </summary>
    public void BtnNextLevelClick()
    {
        BubbleClick.Play();
        var newLevelIndex = TestsManager.Single.CurrentBookIndex + 1;
        if (newLevelIndex == TestsManager.Single.AllBooks.Length)
            newLevelIndex = 0;
        TestsManager.Single.SetCurrentBookIndex(newLevelIndex);
        UpdateControls();
    }

    /// <summary>
    /// Клик на главу назад
    /// </summary>
    public void BtnPrevChapterClick()
    {
        BubbleClick.Play();
        var newChapterIndex = TestsManager.Single.CurrentBook.CurrentChapterIndex - 1;
        if (newChapterIndex >= 0)
        {
            TestsManager.Single.CurrentBook.SetCurrentChapterIndex(newChapterIndex);
            UpdateControls();
        }
    }

    /// <summary>
    /// Клик на главу вперед
    /// </summary>
    public void BtnNextChapterClick()
    {
        BubbleClick.Play();
        var newChapterIndex = TestsManager.Single.CurrentBook.CurrentChapterIndex + 1;
        if (newChapterIndex < TestsManager.Single.CurrentBook.Chapters.Length)
        {
            TestsManager.Single.CurrentBook.SetCurrentChapterIndex(newChapterIndex);
            UpdateControls();
        }
    }

    /// <summary>
    /// Клик по кнопке Выход
    /// </summary>
    public void BtnExitClick()
    {
        BubbleClick.Play();
        FaderPanel.GetComponent<FaderPanel>().NextStageName = string.Empty;
        FaderPanel.GetComponent<FaderPanel>().FadeOutByHand();
    }

    /// <summary>
    /// Клик по кнопке "О программе"
    /// </summary>
    public void BtnAboutClick()
    {
        BubbleClick.Play();
        FaderPanel.GetComponent<FaderPanel>().NextStageName = "About";
        FaderPanel.GetComponent<FaderPanel>().FadeOutByHand();
    }
}
