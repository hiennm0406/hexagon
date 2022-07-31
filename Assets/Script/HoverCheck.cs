using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverCheck : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject selected;
    float timeLeft;
    private Hexagon hex;
    public Text text;
    public GameObject textfield;
    private bool waitClose;

    private void Start()
    {
        hex = Hexagon.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (hex.isRotating)
        {
            return;
        }

        // save data of clicked object and start count time
        if (Input.GetMouseButtonDown(0) && !hex.Pause)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                selected = hit.collider.gameObject;
                HexNumber hexNumber = selected.GetComponent<HexNumber>();
                if(hexNumber.TYPE == GlobalStrings.TYPEHEXAGON.NORMAL || hexNumber.TYPE == GlobalStrings.TYPEHEXAGON.HALFOFONE)
                {
                    selected = null;
                    return;
                }
                timeLeft = 1f;
            }
        }

        if (Input.GetMouseButton(0) && !hex.Pause)
        {
            if(selected == null)
            {
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if(selected != hit.collider.gameObject)
                {
                    selected = null;
                    return;
                }
            }

            timeLeft -= Time.deltaTime;
            Debug.Log(timeLeft);
            if (timeLeft <= 0)
            {
                HexNumber hexNumber = selected.GetComponent<HexNumber>();
                if (hexNumber.TYPE == GlobalStrings.TYPEHEXAGON.SMOKE)
                {
                    text.text = "<b><size=60>Smoke</size></b> \n The smoke is covering this place, you can't know what's behind it.";
                }
                else if (hexNumber.TYPE == GlobalStrings.TYPEHEXAGON.CLOUDING)
                {
                    text.text = "<b><size=60>Cloud</size></b> \n The cloud is covering this place and nearby, you can't know what's behind it.";
                }
                else if (hexNumber.TYPE == GlobalStrings.TYPEHEXAGON.FREEZE)
                {
                    text.text = "<b><size=60>Freeze</size></b> \n This place has been frozen in the eternal cold, nothing can break it.";
                }
                else if (hexNumber.TYPE == GlobalStrings.TYPEHEXAGON.ICECORE)
                {
                    text.text = "<b><size=60>Frozen Core</size></b> \n Dark matter from space, it freezes anything that comes near it.";
                }
                else if (hexNumber.TYPE == GlobalStrings.TYPEHEXAGON.SHAPESHIFTER)
                {
                    text.text = "<b><size=60>Shapeshifter</size></b> \n It lives!!! It transforms!";
                }
                else if (hexNumber.TYPE == GlobalStrings.TYPEHEXAGON.SMOKEROOT)
                {
                    text.text = "<b><size=60>Burn</size></b> \n The fire produces smoke, and it will spread until the end of the world.";
                }
                else if (hexNumber.TYPE == GlobalStrings.TYPEHEXAGON.TIME)
                {
                    text.text = "<b><size=60>Time</size></b> \n A new day has come, and many new days have passed.";
                }
                // show modal
                selected = null;
                waitClose = true;
                textfield.SetActive(true);
                hex.Pause = true;
                return;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (waitClose)
            {
                textfield.SetActive(false);
                hex.Pause = false;
                selected = null;
                waitClose = false;
            }
        }
    }
}
