using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutUiBehaviour : MonoBehaviour
{
    /// <summary>
    /// Панель возврата
    /// </summary>
    public FaderPanel FaderPanel;

    /// <summary>
    /// Звук клика по кнопке
    /// </summary>
    public AudioSource BubbleClick;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Клик по Кнопке закрытия
    /// </summary>
    public void BtnCloseClick()
    {
        BubbleClick.Play();
        FaderPanel.FadeOutByHand();
    }
}
