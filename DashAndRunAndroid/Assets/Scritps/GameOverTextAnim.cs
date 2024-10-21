using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverTextAnim : MonoBehaviour
{
    [SerializeField] private TMP_Text networkWarning;
    [SerializeField] private float colorChangeSpeed = 1f;
    private float hue;

    private void Awake()
    {
        networkWarning = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(ChangeOutlineColor());
    }

    private void OnDisable()
    {
        StopCoroutine(ChangeOutlineColor());
    }

    private IEnumerator ChangeOutlineColor()
    {
        while (true)
        {
            hue += Time.deltaTime * colorChangeSpeed;
            if (hue > 1) hue -= 1; // Loop back to 0
            Color outlineColor = Color.HSVToRGB(hue, 1, 1);
            networkWarning.outlineColor = outlineColor;

            yield return null; // Wait for the next frame
        }
    }
}