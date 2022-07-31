using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Ô lục giác mang số - thứ sẽ rơi mỗi lượt
public class HexNumber : MonoBehaviour
{
    public float number;
    public GUIStyle style;
    public Hexgrid grid;
    public bool Merge;
    public float SpeedDown;
    public GlobalStrings.TYPEHEXAGON TYPE;
    private SpriteRenderer spriteRenderer;

    public bool Smoke;
    private bool firstTurn;
    public int countSmoke;

    private void Awake()
    {
        Merge = false;
        SpeedDown = 50f;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        GlobalEvent.EndTurn.AddListener(RemoveSmoke);
    }

    private void OnDisable()
    {
        GlobalEvent.WaitPlayerMove.RemoveListener(NewTurnEvent);
        GlobalEvent.EndTurn.RemoveListener(RemoveSmoke);
    }

  
    public void HexInit()
    {
        Smoke = false;
       
        if (TYPE == GlobalStrings.TYPEHEXAGON.HALFOFONE)
        {
            spriteRenderer.color = Color.black;
            number = 1f / 2f;

        }else if (TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
        {
            spriteRenderer.color = Color.black;
            GlobalEvent.WaitPlayerMove.RemoveListener(NewTurnEvent);
        }
        else if (TYPE == GlobalStrings.TYPEHEXAGON.FREEZE)
        {
            spriteRenderer.color = new Color(0f, 0.5f, 1f, 1);
            GlobalEvent.WaitPlayerMove.RemoveListener(NewTurnEvent);
        }
        else if (TYPE == GlobalStrings.TYPEHEXAGON.ICECORE)
        {
            firstTurn = true;
            spriteRenderer.color = Color.blue;
            GlobalEvent.WaitPlayerMove.AddListener(NewTurnEvent);
        }
        else if (TYPE == GlobalStrings.TYPEHEXAGON.TIME)
        {
            spriteRenderer.color = new Color(1f, 0.8f, 0f, 1);
            GlobalEvent.EndTurn.AddListener(NewTurnEvent);
        }
        else if (TYPE == GlobalStrings.TYPEHEXAGON.SMOKEROOT)
        {
            GlobalEvent.WaitPlayerMove.AddListener(NewTurnEvent);
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1);
            firstTurn = true;
        }
        else if (TYPE == GlobalStrings.TYPEHEXAGON.SMOKE)
        {
            spriteRenderer.color = Color.white;
            Smoke = true;
            GlobalEvent.WaitPlayerMove.RemoveListener(NewTurnEvent);
        }
        else if (TYPE == GlobalStrings.TYPEHEXAGON.SHAPESHIFTER)
        {
            GlobalEvent.WaitPlayerMove.AddListener(NewTurnEvent);
            spriteRenderer.color = Color.red;
        }
        else if (TYPE == GlobalStrings.TYPEHEXAGON.CLOUDING)
        {
            spriteRenderer.color = Color.gray;
            GlobalEvent.WaitPlayerMove.AddListener(NewTurnEvent);
        }
    }


    internal IEnumerator MoveToNewSpot()
    {
        Vector3 startingPos = transform.localPosition;
        Vector3 finalPos = Vector3.zero;
        float step = (SpeedDown / (startingPos - finalPos).magnitude) * Time.fixedDeltaTime;

        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            transform.localPosition = Vector3.Lerp(startingPos, finalPos, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        transform.localPosition = Vector3.zero;
    }


    internal IEnumerator MergeToNewSpot(HexNumber nowCheck)
    {
        Vector3 startingPos = transform.localPosition;
        Vector3 finalPos = Vector3.zero;
        float step = (SpeedDown / (startingPos - finalPos).magnitude) * Time.fixedDeltaTime;
        float t = 0;
       
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            transform.localPosition = Vector3.Lerp(startingPos, finalPos, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
       
        number += nowCheck.number;
        
      
        Hexagon.Instance.AddPoint((int)number);

        TYPE = GlobalStrings.TYPEHEXAGON.NORMAL;
        HexInit();
        nowCheck.gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
        Hexagon.Instance.GetScore();
    }

    public void NewTurnEvent()
    {
        if (TYPE == GlobalStrings.TYPEHEXAGON.CLOUDING)
        {
            List<HexNumber> listGrid = new List<HexNumber>();
            // create smoke
            Debug.Log("count");
            countSmoke++;
            foreach (Hexgrid hex in Hexagon.Instance.Grid)
            {
                // get nearly X => get Z - 1 and + 1
                if (hex.X == grid.X)
                {
                    if (hex.Z == grid.Z - 1 && hex.owner != null)
                    {
                        hex.owner.countSmoke++;
                    }
                    if (hex.Z == grid.Z + 1 && hex.owner != null)
                    {
                        hex.owner.countSmoke++;
                    }
                }

                // get nearly Y => get Z - 1 and + 1
                if (hex.Y == grid.Y)
                {
                    if (hex.Z == grid.Z - 1 && hex.owner != null)
                    {
                        hex.owner.countSmoke++;
                    }
                    if (hex.Z == grid.Z + 1 && hex.owner != null)
                    {
                        hex.owner.countSmoke++;
                    }
                }

                // get nearly Z => get X - 1 and + 1
                if (hex.Z == grid.Z)
                {
                    if (hex.X == grid.X - 1 && hex.owner != null)
                    {
                        hex.owner.countSmoke++;
                    }
                    if (hex.X == grid.X + 1 && hex.owner != null)
                    {
                        hex.owner.countSmoke++;
                    }
                }
            }
        }
        else if (TYPE == GlobalStrings.TYPEHEXAGON.SMOKEROOT)
        {
            if (!firstTurn)
            {
                List<HexNumber> listGrid = new List<HexNumber>();
                // create smoke
                foreach (Hexgrid hex in Hexagon.Instance.Grid)
                {
                    // get nearly X => get Z - 1 and + 1
                    if (hex.X == grid.X)
                    {
                        if (hex.Z == grid.Z - 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                        if (hex.Z == grid.Z + 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                    }

                    // get nearly Y => get Z - 1 and + 1
                    if (hex.Y == grid.Y)
                    {
                        if (hex.Z == grid.Z - 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                        if (hex.Z == grid.Z + 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                    }

                    // get nearly Z => get X - 1 and + 1
                    if (hex.Z == grid.Z)
                    {
                        if (hex.X == grid.X - 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                        if (hex.X == grid.X + 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                    }
                }

                if (listGrid.Count > 0)
                {
                    HexNumber target = listGrid[Random.Range(0, listGrid.Count - 1)];
                    target.TYPE = GlobalStrings.TYPEHEXAGON.SMOKE;
                    target.HexInit();
                }
            }
            else
            {
                firstTurn = false;
            }
        }else if(TYPE == GlobalStrings.TYPEHEXAGON.SHAPESHIFTER)
        {
            List<HexNumber> list = new List<HexNumber>();
            foreach (Hexgrid hex in Hexagon.Instance.Grid)
            {
                if (hex.owner != null && hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                {
                     list.Add(hex.owner);
                }
            }
            int randNumber = Random.Range(0, list.Count);
            // duyệt lấy list HexNumber, random 1 value bất kỳ trong list
            number = list[randNumber].number;
        }else if (TYPE == GlobalStrings.TYPEHEXAGON.TIME)
        {
            // nếu là time hex - tăng 1 turn
            GlobalEvent.IncreaseTurn.Invoke();
        }
        else if (TYPE == GlobalStrings.TYPEHEXAGON.ICECORE)
        {
            if (!firstTurn)
            {
                List<HexNumber> listGrid = new List<HexNumber>();
                // create smoke
                foreach (Hexgrid hex in Hexagon.Instance.Grid)
                {
                    // get nearly X => get Z - 1 and + 1
                    if (hex.X == grid.X)
                    {
                        if (hex.Z == grid.Z - 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                        if (hex.Z == grid.Z + 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                    }

                    // get nearly Y => get Z - 1 and + 1
                    if (hex.Y == grid.Y)
                    {
                        if (hex.Z == grid.Z - 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                        if (hex.Z == grid.Z + 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                    }

                    // get nearly Z => get X - 1 and + 1
                    if (hex.Z == grid.Z)
                    {
                        if (hex.X == grid.X - 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                        if (hex.X == grid.X + 1 && hex.owner != null)
                        {
                            if (hex.owner.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL)
                                listGrid.Add(hex.owner);
                        }
                    }
                }

                if (listGrid.Count > 0)
                {
                    HexNumber target = listGrid[Random.Range(0, listGrid.Count - 1)];
                    target.TYPE = GlobalStrings.TYPEHEXAGON.FREEZE;
                    target.HexInit();
                }
            }
            else
            {
                firstTurn = false;
            }
        }
    }

    public void RemoveSmoke()
    {
        countSmoke = 0;
    }

    void OnGUI()
    {
        // Make a group on the center of the screen
        // All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
        // We'll make a box so you can see where the group is on-screen.
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        screenPosition.y = Screen.height - screenPosition.y;
       
        if (!Hexagon.Instance.Pause)
        {
            if (TYPE == GlobalStrings.TYPEHEXAGON.HALFOFONE)
            {
                GUI.TextField(new Rect(screenPosition.x - 10, screenPosition.y - 5, 20, 10), "1/2", style);
            }else
            if (!Smoke && countSmoke == 0)
            {
                GUI.TextField(new Rect(screenPosition.x - 10, screenPosition.y - 5, 20, 10), number.ToString(), style);
            }
            
        }
    }
}
