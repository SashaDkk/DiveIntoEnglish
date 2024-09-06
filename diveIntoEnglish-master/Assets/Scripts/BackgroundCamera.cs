using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCamera : MonoBehaviour
{
    public float ScrollSpeed;

    private SpriteRenderer _backgroundRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _backgroundRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var currentOffset = _backgroundRenderer.material.mainTextureOffset;
        _backgroundRenderer.material.mainTextureOffset = new Vector2(currentOffset.x + ScrollSpeed, 0);
    }
}
