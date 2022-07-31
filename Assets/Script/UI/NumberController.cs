using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform imagePanel;
    List<Image> ListImage;
    public bool showZero;

    public void Start()
    {
        ListImage = new List<Image>();
        foreach (Transform child in imagePanel)
        {
            Image img = child.GetComponent<Image>();

            img.sprite = NumberContainer.Instance.getNumber(0);
            ListImage.Add(img);
        }
        if (!showZero)
        {
            for (int i = 0; i < ListImage.Count - 1; i++)
            {
                ListImage[i].gameObject.SetActive(false);
            }
        }

    }

    public void ShowNumber(int value)
    {
        string s = value.ToString();


        for (int i = 0; i < ListImage.Count; i++)
        {
            int y = i - (ListImage.Count - s.Length);

            if(y < 0)
            {
                if (showZero)
                {
                    if (!ListImage[i].gameObject.activeInHierarchy)
                    {
                        ListImage[i].gameObject.SetActive(true);
                    }
                    ListImage[i].sprite = NumberContainer.Instance.getNumber(0);
                }
                else
                {
                    ListImage[i].gameObject.SetActive(false);
                }
            }
            else
            {
                if (!ListImage[i].gameObject.activeInHierarchy)
                {
                    ListImage[i].gameObject.SetActive(true);
                }

                ListImage[i].sprite = NumberContainer.Instance.getNumber(int.Parse(""+s[y]));
            }
        }
    }
}
