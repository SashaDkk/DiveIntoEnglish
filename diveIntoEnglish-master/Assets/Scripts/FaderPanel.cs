using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaderPanel : MonoBehaviour
{
    /// <summary>
    /// Наименование следующего загружаемого уровня
    /// </summary>
    public string NextStageName;
    
    /// <summary>
    /// Уйти в закат после скольки секунд
    /// </summary>
    public float FadeAfter = 2;

    /// <summary>
    /// Загрузка закончена
    /// </summary>
    private bool _started;

    /// <summary>
    /// Признак ухода в закат
    /// </summary>
    private bool _fadeoutActived;
    
    /// <summary>
    /// Завершилось затемнение сцены
    /// </summary>
    public void OnFadeOut()
    {
        if (_fadeoutActived)
        {
            if (string.IsNullOrEmpty(NextStageName))
                Application.Quit();
            else
                SceneManager.LoadScene(NextStageName);
        }
    }

    /// <summary>
    /// Туман рассеялся
    /// </summary>
    public void OnFadeIn()
    {
        if (!_started)
        {
            _started = true;
            GetComponent<RectTransform>().SetAsFirstSibling();
        }
    }

    /// <summary>
    /// Отрисовка назад
    /// </summary>
    private void FadeOutInternal()
    {
        _fadeoutActived = true;
        GetComponent<RectTransform>().SetAsLastSibling();
        GetComponent<Animator>().SetBool("isHidden", true);
    }

    /// <summary>
    /// Вручную запустить завершение
    /// </summary>
    public void FadeOutByHand()
    {
        FadeOutInternal();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_fadeoutActived)
        {
            if (FadeAfter > 0 && Time.time > FadeAfter)
            {
                FadeOutInternal();
            }
        }
    }
}
