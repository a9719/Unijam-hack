using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class SpaceBattle : MonoBehaviour
{
    static GameObject battleScene;
    static Terminal terminal;
    static Text textObject;
    static Dictionary<string, string> battleData = new Dictionary<string, string>();

    public static void InitSpaceBattle(XmlNode battle, Text text)
    {
        if (battleScene == null)
        {
            battleScene = new GameObject();
            battleScene.AddComponent<SpaceBattle>();
        }

        textObject = text;
        
        terminal = textObject.GetComponent<Terminal>();

        textObject.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void Win()
    {

    }

    void Lose()
    {

    }

    void End()
    {
        battleScene = null;
        textObject.enabled = true;

    }
}
