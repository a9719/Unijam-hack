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

    // Dictionary will contain:
    // "win_destination" for the scene to go to after a win
    // "lose_destination" for the scene to go to after a loss
    // "opponent" contains the type of opponent the player is facing. Could be pirates or a character.

    public static void InitSpaceBattle(XmlNode battle, Text text)
    {
        if (battleScene == null)
        {
            battleScene = new GameObject("SpaceBattleController");
            battleScene.AddComponent<SpaceBattle>();
        }

        textObject = text;
        
        terminal = textObject.GetComponent<Terminal>();
        textObject.enabled = false;

        for (int i = 0; i < battle.Attributes.Count; i++)
        {
            battleData.Add(battle.Attributes[i].Name, battle.Attributes[i].Value);
        }
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
        terminal.LoadScene(battleData["win_destination"]);
        End();
    }

    void Lose()
    {
        terminal.LoadScene(battleData["lose_destination"]);
        End();
    }

    void End()
    {
        textObject.enabled = true;
        Destroy(battleScene);
    }
}
