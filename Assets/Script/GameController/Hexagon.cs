using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Input = InputWrapper.Input;
using System;
using UnityEngine.UI;

public class Hexagon : MonoBehaviour
{
    public static Hexagon Instance;
    private AudioSource source;

    public Dictionary<int, Dictionary<int, Dictionary<int, Hexgrid>>> GridControl;
    public Dictionary<int, List<Hexgrid>> ListLine;
    public List<Hexgrid> Grid;
    public GUISkin skin;
    public GUIStyle style;

    public NumberController highScoreShower;
    public int hScore;
    public NumberController scoreShower;
    public int score;
    public NumberController move;

    public GameObject MenuPanel;
    public GameObject LosePanel;
    public GameObject LoadingScreen;
    public List<GameObject> ButtonStart;

    public ClockComponent clock;

    public InterstitialAds ads;
    public RewardAds rewardAds;

    public int Move;

    public int Time;
    public int BigTime;

    public GameObject flipGainPanel;
    public GameObject flipConfirmPanel;
    public GameObject flipWatchAds;
    public GameObject flipRefuse;

    public enum Pivot
    {
        X,
        Y,
        Z,
    }
    public Pivot pivot;
    public bool Revert = false;
    public bool isRotating;
    public float timeRoll = 0.2f;
    private bool pause;

    private int lastDay;
    private int flipCount;
    public Text flipText;
    public bool Flip;

    private bool viewAdsAlready = false;

    private Vector2 startPoint, endPoint;

    public bool Pause { get => pause; set => pause = value; }
    public bool ViewAdsAlready { get => viewAdsAlready; set => viewAdsAlready = value; }

    private void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
        GridControl = new Dictionary<int, Dictionary<int, Dictionary<int, Hexgrid>>>();
        Grid = new List<Hexgrid>();
        ListLine = new Dictionary<int, List<Hexgrid>>();
        for (int i = 1; i < 8; i++)
        {
            ListLine.Add(i, new List<Hexgrid>());
        }

        foreach (Transform child in transform)
        {
            Hexgrid hex = child.GetComponent<Hexgrid>();
            if (!hex)
                continue;
            if (!GridControl.ContainsKey(hex.X))
            {
                GridControl.Add(hex.X, new Dictionary<int, Dictionary<int, Hexgrid>>());
            }
            if (!GridControl[hex.X].ContainsKey(hex.Y))
            {
                GridControl[hex.X].Add(hex.Y, new Dictionary<int, Hexgrid>());
            }
            GridControl[hex.X][hex.Y].Add(hex.Z, hex);
            Grid.Add(hex);
        }
        pivot = Pivot.X;
        Revert = false;
        isRotating = true;
        score = 0;
        Time = 0;
        pause = true;
        Flip = false;
        LosePanel.SetActive(false);
    }

    private void OnEnable()
    {
        GlobalEvent.IncreaseTurn.AddListener(CountTurnIncrease);
        GlobalEvent.EndTurn.AddListener(CountTurnIncrease);
        GlobalEvent.WaitPlayerMove.AddListener(WaitPlayer);
    }

    private void OnDisable()
    {
        GlobalEvent.IncreaseTurn.RemoveAllListeners();
        GlobalEvent.EndTurn.RemoveAllListeners();
        GlobalEvent.WaitPlayerMove.RemoveAllListeners();
    }

    private void Start()
    {
        ReadyStart();
    }


    private void Update()
    {
        if (isRotating || pause)
        {
            return;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startPoint = Input.GetTouch(0).position;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endPoint = Input.GetTouch(0).position;


            if ((endPoint.x < startPoint.x) && (endPoint.x < Screen.width / 3)  && (Mathf.Abs(startPoint.y - endPoint.y) < Screen.height / 5))
            {
                StartCoroutine(TurnLeft());
            }

            if ((endPoint.x > startPoint.x) && (endPoint.x > Screen.width * 2 / 3)  && (Mathf.Abs(startPoint.y - endPoint.y) < Screen.height / 5))
            {
                StartCoroutine(TurnRight());
            }
        }
    }


    public void StartNewGame()
    {
        foreach (Hexgrid hex in Grid)
        {
            if (hex.owner != null)
            {
                hex.owner.gameObject.SetActive(false);
                hex.owner = null;
            }
        }
        Move = 0;
        score = 0;
        Time = 0;
        BigTime = 0;
        GetScore();
        move.ShowNumber(Move);
        pause = false;
        DropController.Instance.StartGame();
        clock.TurnClock(0);
        viewAdsAlready = false;
        MenuPanel.SetActive(false);
        LosePanel.SetActive(false);
    }

    public void ReadyStart()
    {
        Debug.Log("Already load");

        foreach (GameObject item in ButtonStart)
        {
            item.SetActive(true);
        }
        // get the gift
        lastDay = PlayerPrefs.GetInt("lastDays", lastDay);
        flipCount = PlayerPrefs.GetInt("flipCount", flipCount);

        if (DateTime.Now.DayOfYear != lastDay )
        {
            flipGainPanel.SetActive(true);
            PlayerPrefs.SetInt("lastDays", DateTime.Now.DayOfYear);
            flipCount++;
            PlayerPrefs.SetInt("flipCount", flipCount);
            flipText.text = "" + flipCount;
        }

        LoadingScreen.SetActive(false);
    }

    public void LoseGame()
    {
        pause = true;
        ads.ShowAd();
        LosePanel.SetActive(true);
        MenuPanel.SetActive(false);
    }

    public void ReturnMenu()
    {
        LosePanel.SetActive(false);
        MenuPanel.SetActive(true);
    }


    public Drop GetLine()
    {
        Drop drop = GetRowCompare();
        for (int i = 1; i < 8; i++)
        {
            ListLine[i].Clear();
            if (drop.pivot == Pivot.X)
            {
                foreach (Hexgrid hex in Grid)
                {
                    if (hex.X == i)
                    {
                        ListLine[i].Add(hex);
                    }
                }
            }
            else if (drop.pivot == Pivot.Y)
            {
                foreach (Hexgrid hex in Grid)
                {
                    if (hex.Y == i)
                    {
                        ListLine[i].Add(hex);
                    }
                }
            }
            else if (drop.pivot == Pivot.Z)
            {
                foreach (Hexgrid hex in Grid)
                {
                    if (hex.Z == i)
                    {
                        ListLine[i].Add(hex);
                    }
                }
            }
        }
        return drop;
    }

    public IEnumerator GetHighScore()
    {
        yield return new WaitForFixedUpdate();
        hScore = PlayerPrefs.GetInt("highscore", hScore);
        highScoreShower.ShowNumber(hScore);
    }


    // DROP TỪ TRÊN XUỐNG, KHÔNG HỢP NHẤT Ô - SỬ DỤNG CHO NEW TURN
    public IEnumerator DropToBottom()
    {
        Drop drop = GetLine();
        for (int i = 1; i < 8; i++)
        {
            if (drop.calc == Pivot.X)
            {
                if (drop.Revert)
                {
                    ListLine[i] = ListLine[i].OrderByDescending(w => w.X).ToList();
                } else
                    ListLine[i] = ListLine[i].OrderBy(w => w.X).ToList();
            }
            else if (drop.calc == Pivot.Y)
            {
                if (drop.Revert)
                {
                    ListLine[i] = ListLine[i].OrderByDescending(w => w.Y).ToList();
                }
                else
                    ListLine[i] = ListLine[i].OrderBy(w => w.Y).ToList();
            }
            else if (drop.calc == Pivot.Z)
            {
                if (drop.Revert)
                {
                    ListLine[i] = ListLine[i].OrderByDescending(w => w.Z).ToList();
                }
                else
                    ListLine[i] = ListLine[i].OrderBy(w => w.Z).ToList();
            }

            for (int x = 0; x < ListLine[i].Count; x++)
            {
                if (ListLine[i][x].owner == null) {
                    for (int z = x + 1; z < ListLine[i].Count; z++)
                    {
                        if (ListLine[i][z].owner != null) // found
                        {
                            // if this node FREZZE - Reset
                            if (ListLine[i][z].owner.TYPE == GlobalStrings.TYPEHEXAGON.FREEZE)
                            {
                                break;
                            }

                            ListLine[i][x].owner = ListLine[i][z].owner;
                            ListLine[i][x].owner.grid = ListLine[i][x];
                            ListLine[i][z].owner = null;
                            ListLine[i][x].owner.transform.parent = ListLine[i][x].transform;
                            StartCoroutine(ListLine[i][x].owner.MoveToNewSpot());
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        GlobalEvent.WaitPlayerMove.Invoke();
    }


    // HỢP NHẤT CÁC Ô KHI RƠI TỪ TRÊN CAO XUỐNG - SỬ DỤNG SAU KHI XOAY
    public IEnumerator MergeToBottom()
    {
        Drop drop = GetLine();
        for (int i = 1; i < 8; i++)
        {
            if (drop.calc == Pivot.X)
            {
                if (drop.Revert)
                {
                    ListLine[i] = ListLine[i].OrderByDescending(w => w.X).ToList();
                }
                else
                    ListLine[i] = ListLine[i].OrderBy(w => w.X).ToList();
            }
            else if (drop.calc == Pivot.Y)
            {
                if (drop.Revert)
                {
                    ListLine[i] = ListLine[i].OrderByDescending(w => w.Y).ToList();
                }
                else
                    ListLine[i] = ListLine[i].OrderBy(w => w.Y).ToList();
            }
            else if (drop.calc == Pivot.Z)
            {
                if (drop.Revert)
                {
                    ListLine[i] = ListLine[i].OrderByDescending(w => w.Z).ToList();
                }
                else
                    ListLine[i] = ListLine[i].OrderBy(w => w.Z).ToList();
            }

            HexNumber nowCheck = null;
            // loop every node
            for (int x = 0; x < ListLine[i].Count; x++)
            {
                if (ListLine[i][x].owner == null) // if node null, continuos loop
                {
                    for (int z = x + 1; z < ListLine[i].Count; z++)
                    {
                        if (ListLine[i][z].owner != null) // find a node not null behide
                        {
                            // if this node FREZZE - Reset

                            if (ListLine[i][z].owner.TYPE == GlobalStrings.TYPEHEXAGON.FREEZE)
                            {
                                nowCheck = null;
                                break;
                            }

                            // if not have NOWCHECK or number in NOWCHECK different, drop this node to null node
                            if (nowCheck == null || nowCheck.number != ListLine[i][z].owner.number)
                            {
                                ListLine[i][x].owner = ListLine[i][z].owner;
                                ListLine[i][x].owner.grid = ListLine[i][x];
                                ListLine[i][z].owner = null;
                                nowCheck = ListLine[i][x].owner;
                                nowCheck.transform.parent = ListLine[i][x].transform;
                                StartCoroutine(ListLine[i][x].owner.MoveToNewSpot());
                            }
                            // if have NOWCHECK and same number with NOWCHECK, MERGE with NOWCHECK
                            else
                            {
                                // drop to nowcheck position
                                HexNumber target = nowCheck;
                                Hexgrid grid = target.grid;
                                grid.owner = ListLine[i][z].owner;
                                grid.owner.grid = grid;
                                ListLine[i][z].owner = null;
                                grid.owner.transform.parent = grid.transform;
                                StartCoroutine(grid.owner.MergeToNewSpot(target));
                                x--;
                                nowCheck = null;
                            }
                            break;
                        }
                    }
                }
                else // if not null
                {
                    // If FREEZE, it NOWCHECK now.
                    if (ListLine[i][x].owner.TYPE == GlobalStrings.TYPEHEXAGON.FREEZE)
                    {
                        nowCheck = ListLine[i][x].owner;
                    }
                    // if not FREEZE, check NOWCHECK. IF not NOWCHECK, OR if number different with NOWCHECK, it NOWCHECK now
                    else if (nowCheck == null || nowCheck.number != ListLine[i][x].owner.number)
                    {
                        nowCheck = ListLine[i][x].owner;
                    }
                    // MERGE with NOWCHECK
                    else if (nowCheck.number == ListLine[i][x].owner.number)
                    {
                        HexNumber target = nowCheck;
                        Hexgrid grid = target.grid;
                        grid.owner = ListLine[i][x].owner;
                        grid.owner.grid = grid;
                        ListLine[i][x].owner = null;
                        grid.owner.transform.parent = grid.transform;
                        StartCoroutine(grid.owner.MergeToNewSpot(target));
                        x--;
                        nowCheck = null;
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);

        // Tạo mới và drop vào bàn
        GlobalEvent.EndTurn.Invoke();
        yield return new WaitForSeconds(0.1f);
        
        
        DropController.Instance.create();
        yield return StartCoroutine(DropToBottom());
    }

    public IEnumerator flipBoard()
    {
        //flip the board
        flipText.text = "" + flipCount;
        PlayerRoll();
        Quaternion fromPos = transform.rotation;
        Quaternion toPos = Quaternion.Euler(transform.eulerAngles + (new Vector3(180, 0, 0)));
        float timePassed = 0.0f; //Time passed since the start of the linear interpolation. Starting at 0, it increases until it reaches 1. All values are rendered.

        while (timePassed < timeRoll) //While the time passes is less than 1 (the maximum of a linear interpolation)
        {
            timePassed += UnityEngine.Time.deltaTime; //Increase the timePassed with the time passed since the last frame; the time is first normalized
            transform.rotation = Quaternion.Slerp(fromPos, toPos, timePassed * (1f / timeRoll)); //Set the pathblock rotation to a new value defined by linear interpolation
            yield return null; //Stop the function, finish Update() and return to this while loop; this will cause all slerp() values to render, resulting in a smooth animation
        }
        transform.rotation = toPos;
       
        Revert = !Revert;
        Flip = !Flip;
        StartCoroutine(MergeToBottom());
    }

    public void checkFlip()
    {
        if(flipCount > 0)
        {
            flipCount--;
            StartCoroutine(flipBoard());
            PlayerPrefs.SetInt("flipCount", flipCount);
        }
        else
        {
            pause = true;
            flipConfirmPanel.SetActive(true);
            if (!viewAdsAlready)
            {
                flipRefuse.SetActive(false);
                flipWatchAds.SetActive(true);
            }
            else
            {
                flipWatchAds.SetActive(false);
                flipRefuse.SetActive(true);
            }
        }
    }

    public void WaitPlayer()
    {
        isRotating = false;
        move.ShowNumber(Move);
        clock.TurnClock(Time);
    }

    public void AddPoint(int i)
    {
        score += i;
    }

    public IEnumerator TurnRight()
    {
        PlayerRoll();
        Quaternion fromPos = transform.rotation;
        Quaternion toPos;
        if (!Flip)
        {
            toPos = Quaternion.Euler(transform.eulerAngles + (new Vector3(0, 0, -60)));
        }
        else
        {
            toPos = Quaternion.Euler(transform.eulerAngles + (new Vector3(0, 0, +60)));
        }
        float timePassed = 0.0f; //Time passed since the start of the linear interpolation. Starting at 0, it increases until it reaches 1. All values are rendered.

        while (timePassed < timeRoll) //While the time passes is less than 1 (the maximum of a linear interpolation)
        {
            timePassed += UnityEngine.Time.deltaTime; //Increase the timePassed with the time passed since the last frame; the time is first normalized
            transform.rotation = Quaternion.Slerp(fromPos, toPos, timePassed * (1f / timeRoll)); //Set the pathblock rotation to a new value defined by linear interpolation
            yield return null; //Stop the function, finish Update() and return to this while loop; this will cause all slerp() values to render, resulting in a smooth animation
        }
        transform.rotation = toPos;
        if (!Flip)
        {
            if (pivot == Pivot.X)
            {
                pivot = Pivot.Z;
            }
            else if (pivot == Pivot.Y)
            {
                pivot = Pivot.X;
            }
            else if (pivot == Pivot.Z)
            {
                pivot = Pivot.Y;
                Revert = !Revert;
            }
        }
        else
        {
            if (pivot == Pivot.X)
            {
                pivot = Pivot.Y;
            }
            else if (pivot == Pivot.Y)
            {
                pivot = Pivot.Z;
                Revert = !Revert;
            }
            else if (pivot == Pivot.Z)
            {
                pivot = Pivot.X;
            }
        }
        StartCoroutine(MergeToBottom());
    }

    public IEnumerator TurnLeft()
    {
        PlayerRoll();
        Quaternion fromPos = transform.rotation;
        Quaternion toPos;
        if (!Flip)
        {
            toPos = Quaternion.Euler(transform.eulerAngles + (new Vector3(0, 0, +60)));
        }
        else
        {
            toPos = Quaternion.Euler(transform.eulerAngles + (new Vector3(0, 0, -60)));
        }
    
        float timePassed = 0.0f; //Time passed since the start of the linear interpolation. Starting at 0, it increases until it reaches 1. All values are rendered.

        while (timePassed < timeRoll) //While the time passes is less than 1 (the maximum of a linear interpolation)
        {
            timePassed += UnityEngine.Time.deltaTime; //Increase the timePassed with the time passed since the last frame; the time is first normalized
            transform.rotation = Quaternion.Slerp(fromPos, toPos, timePassed * (1f / timeRoll)); //Set the pathblock rotation to a new value defined by linear interpolation
            yield return null; //Stop the function, finish Update() and return to this while loop; this will cause all slerp() values to render, resulting in a smooth animation
        }
        transform.rotation = toPos;

        if (!Flip)
        {
            if (pivot == Pivot.X)
            {
                pivot = Pivot.Y;
            }
            else if (pivot == Pivot.Y)
            {
                pivot = Pivot.Z;
                Revert = !Revert;
            }
            else if (pivot == Pivot.Z)
            {
                pivot = Pivot.X;
            }
        }
        else
        {
            if (pivot == Pivot.X)
            {
                pivot = Pivot.Z;
            }
            else if (pivot == Pivot.Y)
            {
                pivot = Pivot.X;
               
            }
            else if (pivot == Pivot.Z)
            {
                pivot = Pivot.Y;
                Revert = !Revert;
            }
        }
        StartCoroutine(MergeToBottom());
    }

    public void PlayerRoll()
    {
        isRotating = true;
        source.Play();
        Move++;
    }

    public Drop GetRowCompare()
    {
        if (pivot == Pivot.X)
        {
            return new Drop(Pivot.X, Pivot.Z, Revert);
        }
        else if (pivot == Pivot.Y)
        {
            return new Drop(Pivot.Y, Pivot.Z, Revert);
        }
        else if (pivot == Pivot.Z)
        {
            return new Drop(Pivot.Z, Pivot.X, !Revert);
        }
        return null;
    }

    public void GetScore()
    {
        scoreShower.ShowNumber(score);
        if(score > hScore)
        {
            hScore = score;
            PlayerPrefs.SetInt("highscore", hScore);
            highScoreShower.ShowNumber(hScore);
        }
    }

    private void CountTurnIncrease()
    {
        Time++;
        clock.TurnClock(Time);
    }
}
