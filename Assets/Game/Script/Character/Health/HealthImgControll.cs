using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthImgControll : MonoBehaviour
{
    [Header("Images")]
    public Sprite fullHealthImg, threeQuarterHealthImg, halfHealthImg, quarterHealthImg, emptyHealthImg;
    Image healthImg;

    private void Awake()
    {
        //
        healthImg = GetComponent<Image>();
    }
    // Switch health image based on the health status
    public void SetHealthImage(HealthStatus status)
    {
        switch (status)
        {
            case HealthStatus.Empty:
                healthImg.sprite = emptyHealthImg;
                break;
            case HealthStatus.ThreeQuarter:
                healthImg.sprite = threeQuarterHealthImg;
                break;
            case HealthStatus.Half:
                healthImg.sprite = halfHealthImg;
                break;
            case HealthStatus.Quarter:
                healthImg.sprite = quarterHealthImg;
                break;
            case HealthStatus.Full:
                healthImg.sprite = fullHealthImg;
                break;
        }
    }
}

public enum HealthStatus
{
    Empty = 0,
   Quarter = 1,
    Half = 2,
    ThreeQuarter = 3,
    Full = 4
}
