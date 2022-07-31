using UnityEngine;

public class Credit : MonoBehaviour
{
    private float offset;
    public float speed = 29.0f;
    public GUIStyle style;
    public Rect viewArea;

    private void Start()
    {
        if (this.viewArea.width == 0.0f)
        {
            this.viewArea = new Rect(Screen.width * 0.3f, 0f, Screen.width, Screen.height);
        }
        this.offset = this.viewArea.height*0.8f;
    }
    private void OnEnable()
    {
        this.offset = this.viewArea.height * 0.8f;
    }

    private void Update()
    {
        this.offset -= Time.deltaTime * this.speed;
    }

    private void OnGUI()
    {
        GUI.BeginGroup(this.viewArea);

        var position = new Rect(0, this.offset, this.viewArea.width, this.viewArea.height);
        var text = @" 

Developer by Octolust
28-04-2022
Any suggest or feedback please send to [octolust.creator@gmail.com]
Thanks you.




Fluffing a Duck Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 3.0 License
http://creativecommons.org/licenses/by/3.0/
Music promoted by https://www.chosic.com/free-music/all/.";

        GUI.Label(position, text, this.style);

        GUI.EndGroup();
    }
}