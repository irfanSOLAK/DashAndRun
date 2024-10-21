using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputCanvas : MonoBehaviour
{
    [SerializeField] Color jumpAvailableColor;
    [SerializeField] Color oneJumpLeftColor;
    [SerializeField] Color noJumpColor;
    [SerializeField] GameObject jumpButton;


    private int jumpsLeft;

    public int JumpsLeft
    {
        get { return jumpsLeft; }
        set { jumpsLeft = value; SetButtonColor(); }
    }


    public bool IsPressedJump { get; set; } = false;
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        jumpButton.GetComponent<Image>().color = jumpAvailableColor;
    }

    public void Jump()
    {
        IsPressedJump = true; // jump butonuna atanmýþtýr
    }

    private void SetButtonColor()
    {
        switch (jumpsLeft)
        {
            case 2:
                jumpButton.GetComponent<Image>().color = jumpAvailableColor;
                break;
            case 1:
                jumpButton.GetComponent<Image>().color = oneJumpLeftColor;
                break;
            case 0:
                jumpButton.GetComponent<Image>().color = noJumpColor;
                break;
        }

    }
}
