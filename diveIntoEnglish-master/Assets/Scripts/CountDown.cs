using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    /// <summary>
    /// Синглтон
    /// </summary>
    public static CountDown Single;

    // Start is called before the first frame update
    void Start()
    {
        Single = this;
    }

    /// <summary>
    /// Сработал шаг анимации
    /// </summary>
    public void NotifyCountDownStep()
    {
        GamePlay.Single.NotifyCountDownStep();
    }

    /// <summary>
    /// Показать кадр
    /// </summary>
    /// <param name="value"></param>
    public void PlayCountdownFrame(int value)
    {
        gameObject.GetComponent<Text>().text = value.ToString();
        gameObject.SetActive(true);
        var animator = gameObject.GetComponent<Animator>();
        animator.SetTrigger("PlayFrame");
        animator.enabled = true;
    }
}
