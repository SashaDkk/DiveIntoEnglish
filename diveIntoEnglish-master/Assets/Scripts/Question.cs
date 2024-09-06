using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    private Animator _animator;
    
    public static Question Single;
    
    // Start is called before the first frame update
    void Start()
    {
        Single = this;
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Начать анимирванное отображение вопроса
    /// </summary>
    public void BeginShowQuestion()
    {
        gameObject.GetComponentsInChildren<Text>().Single(x => x.fontStyle == FontStyle.Bold).text = 
            GamePlay.Single.ActiveTest.CurrentQuestion.Word;
        _animator.enabled = true;
        _animator.SetBool("isHidden", false);
    }

    /// <summary>
    /// Начать анимированное сокрытие вопроса
    /// </summary>
    public void BeginHideQuestion()
    {
        _animator.SetBool("isHidden", true);
    }

    /// <summary>
    /// Вопрос отобразился
    /// </summary>
    public void OnQuestionEnter()
    {
        GamePlay.Single.NotifyQuestionShown();
    }

    /// <summary>
    /// Вопрос скрылся
    /// </summary>
    public void OnQuestionLeave()
    {
        GamePlay.Single.NotifyQuestionHide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
