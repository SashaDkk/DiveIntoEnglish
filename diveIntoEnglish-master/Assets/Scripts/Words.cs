using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Words : MonoBehaviour
{
    /// <summary>
    /// Синглтон
    /// </summary>
    public static Words Single;

    /// <summary>
    /// Кнопки
    /// </summary>
    private Animator[] _buttonAnimatos;

    /// <summary>
    /// Количество слов
    /// </summary>
    /// <returns></returns>
    public int WordsCount => _buttonAnimatos.Length;

    // Start is called before the first frame update
    void Start()
    {
        Single = this;
        _buttonAnimatos = GetComponentsInChildren<Animator>();
    }

    /// <summary>
    /// Слов показано
    /// </summary>
    private int _wordsShown;

    /// <summary>
    /// Показать слова
    /// </summary>
    public void BeginShowWords()
    {
        for(var i = 0; i < WordsCount; i++)
            _buttonAnimatos[i].gameObject.GetComponentInChildren<Text>().text = GamePlay.Single.ActiveTest.CurrentQuestion.Answers[i].value;
        _wordsShown = 0;
        _buttonAnimatos[_wordsShown].enabled = true;
        _buttonAnimatos[_wordsShown].SetBool("isHidden", false);
    }

    public void NotifyWordEnter()
    {
        _wordsShown++;
        if (_wordsShown == _buttonAnimatos.Length)
            GamePlay.Single.NotifyAnswersShown();
        else
        {
            _buttonAnimatos[_wordsShown].enabled = true;
            _buttonAnimatos[_wordsShown].SetBool("isHidden", false);
        }
    }

    /// <summary>
    /// Запустить процесс сокрытия слов
    /// </summary>
    public void BeginHideWords()
    {
        _wordsShown = 0;
        _buttonAnimatos[_wordsShown].SetBool("isHidden", true);
    }

    /// <summary>
    /// Внутреннее уведомление о сокрытии слова
    /// </summary>
    public void NotifyWordLeave()
    {
        _wordsShown++;
        Debug.Log($"NotifyWordLeave() - {_wordsShown}");
        if (_wordsShown == _buttonAnimatos.Length)
        {
            GamePlay.Single.NotifyAnswersHide();
            Debug.Log($"_wordsShown == _buttonAnimatos.Length");
        }
        else
        {
            _buttonAnimatos[_wordsShown].SetBool("isHidden", true);
            Debug.Log($"_buttonAnimatos[_wordsShown].SetBool(\"isHidden\", true);");
        }
    }

    /// <summary>
    /// Выполнен клик по кнопке
    /// </summary>
    /// <param name="btnObject"></param>
    public void NotifyBtnClick(int index)
    {
        Debug.Log("NotifyBtnClick");
        GamePlay.Single.NotifyUserAnswer(index);
    }
}
