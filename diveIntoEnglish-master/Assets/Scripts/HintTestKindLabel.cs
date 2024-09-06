using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTestKindLabel : MonoBehaviour
{
    /// <summary>
    /// Синглтон
    /// </summary>
    public static HintTestKindLabel Single;

    // Start is called before the first frame update
    void Start()
    {
        Single = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAnimationDone()
    {
        gameObject.SetActive(false);
        GamePlay.Single.NotifyHintWasShown();
    }
}
