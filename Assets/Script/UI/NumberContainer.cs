using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberContainer : MonoBehaviour
{
    public static NumberContainer Instance;
    public List<Sprite> listNumber;

    private void Awake()
    {
        Instance = this;
    }

    public Sprite getNumber(int i)
    {
        return listNumber[i];
    }
}
