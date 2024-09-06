using Assets.Scripts.NoUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    /// <summary>
    /// Синглтон
    /// </summary>
    public static Submarine Single;
    
    // Start is called before the first frame update
    void Start()
    {
        Single = this;
        gameObject.transform.position = new Vector3(-8, 0, 0);
    }

    /// <summary>
    /// Субмарина приехала на старт
    /// </summary>
    public void OnSubmarineEnter()
    {
        GamePlay.Single.NotifyStartGame();
    }

    /// <summary>
    /// Столкновение
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GamePlay.Single.NotifySubmarineCollide(collision.gameObject);
    }

    /// <summary>
    /// Мигалка при повреждениях
    /// </summary>
    private int _damageCounter;

    /// <summary>
    /// Автоотключение состояния повреждения
    /// </summary>
    public void OnDamageTimeOut()
    {
        _damageCounter++;
        if (_damageCounter > 10)
        {
            _damageCounter = 0;
            GetComponent<Animator>().SetBool("Damaged", false);
        }
    }
}
