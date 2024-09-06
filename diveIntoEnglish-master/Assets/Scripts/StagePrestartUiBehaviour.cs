using Assets.Scripts.NoUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StagePrestartUiBehaviour : MonoBehaviour
{
    /// <summary>
    /// Звук клика
    /// </summary>
    public AudioSource BubbleClick;
    
    /// <summary>
    /// Панель контента
    /// </summary>
    public GameObject ScrollContent;
    
    /// <summary>
    /// Надпись названия уровня сложности
    /// </summary>
    public GameObject LevelCaptionNode;

    /// <summary>
    /// Надпись названия Stepа
    /// </summary>
    public GameObject StageCaptionNode;

    /// <summary>
    /// Отображалка числа жизней
    /// </summary>
    public GameObject HpNode;

    /// <summary>
    /// Надпись со словами
    /// </summary>
    public GameObject WordsTextNode;

    /// <summary>
    /// Панель перехода
    /// </summary>
    public FaderPanel FaderPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        LevelCaptionNode.GetComponent<Text>().text = TestsManager.Single.CurrentBook.Caption;
        StageCaptionNode.GetComponent<Text>().text = TestsManager.Single.CurrentBook.CurrentChapter.Caption;
        HpNode.GetComponent<Text>().text = $"x {GamePlaySettings.StartHp}";
        WordsTextNode.GetComponent<Text>().text = $"Слова (всего {TestsManager.Single.CurrentBook.CurrentChapter.Pairs.Length}):\n" +
            string.Join("\n", TestsManager.Single.CurrentBook.CurrentChapter.Pairs.Select(x => $"{x.rus} - {x.eng}"));
        var rowsCount = TestsManager.Single.CurrentBook.CurrentChapter.Pairs.Length + 1;
        var textRect = WordsTextNode.GetComponent<RectTransform>();
        const float rowSize = 25f;
        var heightToSet = rowsCount * rowSize + 10f;
        ScrollContent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightToSet);
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightToSet);
    }

    /// <summary>
    /// Клик по кнопке СТАРТ
    /// </summary>
    public void OnStartBtnClick()
    {
        BubbleClick.Play();
        FaderPanel.FadeOutByHand();
    }

    /// <summary>
    /// Нажатие на кнопку выход
    /// </summary>
    public void OnExitClick()
    {
        BubbleClick.Play();
        FaderPanel.NextStageName = "MainMenu";
        FaderPanel.FadeOutByHand();
    }
}
