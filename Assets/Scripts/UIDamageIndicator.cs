using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDamageIndicator : MonoBehaviour
{
    public TMP_Text damageText;
    public float moveSpeed;

    private RectTransform myRect;

    public float lifetime = 3f;
    void Start()
    {
        Destroy(gameObject, lifetime);

        myRect = GetComponent<RectTransform>();
        
    }

    
    void Update()
    {
        myRect.anchoredPosition += new Vector2(0f, -moveSpeed * Time.deltaTime);
        
    }
}
