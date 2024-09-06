using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word : MonoBehaviour
{
    /// <summary>
    /// Слово отображено
    /// </summary>
    public void OnWordEnter()
    {
        if (GamePlay.Single.State == GamePlayState.BeginShowAnswers)
            Words.Single.NotifyWordEnter();
    }

    /// <summary>
    /// Слово скрыто
    /// </summary>
    public void OnWordLeave()
    {
        Debug.Log($"OnWordLeave - {GamePlay.Single.State}");
        if (GamePlay.Single.State == GamePlayState.BeginShowQuestionResultAnimation)
            Words.Single.NotifyWordLeave();
        else
            Debug.Log($"ВОТ ОШИБКА!!");
    }
}
