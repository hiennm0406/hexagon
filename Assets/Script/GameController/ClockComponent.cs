
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockComponent : MonoBehaviour
{
    public Image image;
    public List<Sprite> listImage;

    public void TurnClock(int number)
    {
        if (number >= listImage.Count)
        {
            number = listImage.Count - 1;
        }
        image.sprite = listImage[number];
    }
}
