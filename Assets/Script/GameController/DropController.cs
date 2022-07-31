using Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    public static DropController Instance;
    Hexagon hexGrid;
    public List<HexNumber> list;
    public List<Hexgrid> randomNotFillGrid;
    int randNumber;
    public List<int> listNumber = new List<int>();
    private bool _halfOne;


    private void Awake()
    {
        Instance = this;
        listNumber.Add(1);
        listNumber.Add(1);
        listNumber.Add(2);
        listNumber.Add(2);
        listNumber.Add(4);
    }

    /*
     * Start the GAME
     */
    public void StartGame()
    {
        hexGrid = Hexagon.Instance;
        // Khởi tạo 5 ô
        for (int i = 0; i < 5; i++)
        {
            SummonTheHex();
        }
        StartCoroutine(Hexagon.Instance.GetHighScore());
        StartCoroutine(Hexagon.Instance.DropToBottom());
        _halfOne = false;
    }

    public void create()
    {
        // check row have slot
        bool lose = true;
        foreach (Hexgrid hex in hexGrid.Grid)
        {
            if (hex.owner == null)
            {
                lose = false;
                break;
            }
        }
        if (lose)
        {
            Hexagon.Instance.LoseGame();
        }
        else
        {
            bool mutate = false;
            bool bigMutate = false;
            if (Hexagon.Instance.Time >= 9)
            {
                mutate = true;
                Hexagon.Instance.BigTime++;
                if(Hexagon.Instance.BigTime >= 9)
                {
                    bigMutate = true;
                    Hexagon.Instance.BigTime = Hexagon.Instance.BigTime % 9;
                }
                Hexagon.Instance.Time = Hexagon.Instance.Time % 9;

            }

            int rand = Random.Range(2, 3);
            for (int i = 0; i < rand; i++)
            {
                SummonTheHex(mutate, bigMutate);
                if (mutate)
                {
                    mutate = false;
                    bigMutate = false;
                }
            }
        }
    }

    public void SummonTheHex(bool isNotNormal = false, bool isBigBoss = false)
    {
        randomNotFillGrid.Clear();
        list.Clear();
        foreach (Hexgrid hex in hexGrid.Grid)
        {
            if(hex.owner == null)
            {
                randomNotFillGrid.Add(hex);
            }
            else
            {
                if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                {
                    list.Add(hex.owner);
                }
            }
        }
        if (randomNotFillGrid.Count != 0)
        {
            int rand = Random.Range(0, randomNotFillGrid.Count);

            Vector3 spawn = randomNotFillGrid[rand].transform.position;
            GameObject obj = ObjectPooler.SharedInstance.GetPooledObject("Hexagon");
            if (obj != null)
            {
                HexNumber created = obj.GetComponent<HexNumber>();
                obj.transform.parent = randomNotFillGrid[rand].transform;
                obj.transform.localPosition = spawn;

                if (isBigBoss)
                {
                    created.TYPE = (GlobalStrings.TYPEHEXAGON)Random.Range(6,8);
                    randNumber = Random.Range(0, list.Count);
                    created.number = list[randNumber].number;
                }
                else
                if (isNotNormal)
                {
                    if (_halfOne)
                    {
                        created.TYPE = GlobalStrings.TYPEHEXAGON.HALFOFONE;
                        _halfOne = false;
                    }
                    else
                    {
                        created.TYPE = (GlobalStrings.TYPEHEXAGON)Random.Range(1, 5);
                        if (created.TYPE == GlobalStrings.TYPEHEXAGON.HALFOFONE)
                        {
                            _halfOne = true;
                        }
                    }
                    randNumber = Random.Range(0, list.Count);
                    created.number = list[randNumber].number;
                }
                else
                {
                    created.TYPE = GlobalStrings.TYPEHEXAGON.NORMAL;
                    randNumber = Random.Range(0, listNumber.Count);
                    created.number = listNumber[randNumber];
                }
                created.HexInit();

                created.grid = randomNotFillGrid[rand];
                randomNotFillGrid[rand].owner = created;

                obj.SetActive(true);
                StartCoroutine(created.MoveToNewSpot());
            }
        }
    }
}
