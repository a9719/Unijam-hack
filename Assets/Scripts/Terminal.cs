using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

public class Terminal : MonoBehaviour
{
    public static readonly int TEXT_PADDING = 15;
    public static readonly string DEFAULT_TEXT_COLOUR = "#ffffff";
    public static readonly string STORY_FILE = "Assets/Resources/TestStory.xml";
    private readonly string FIRST_SCENE = "0";

    private static readonly string SPACE_BATTLE_SCENE = "space_battle";


    public Text textComponent;
    public float typingSpeed = 0.15f;
    public float typingSpeedMultiplier = 5.0f;

    private string command = "";
    private string newLine = ">  ";
    private bool allowPlayerTyping = false;
    private float cursorBlinkInterval = 0.3f;
    private float cursorBlinkElapsed = 0.0f;
    private string[] cursorType = { "_", " " };
    private int currCursorType = 0;

    private XmlNode currentScene;

    // Start is called before the first frame update
    void Start()
    {
        ParseXML.SetFilePath(STORY_FILE);
        StartCoroutine(LoadScene(FIRST_SCENE));
    }

    // Update is called once per frame
    void Update()
    {

        // Speeds up text animation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            typingSpeed = typingSpeed / typingSpeedMultiplier;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            typingSpeed = typingSpeed * typingSpeedMultiplier;
        }

        // Allows player to use keyboard
        if (allowPlayerTyping)
        {
            playerTyping();
        }

        // Flashing cursor
        cursorBlinkElapsed += Time.deltaTime;
        if (cursorBlinkElapsed > cursorBlinkInterval)
        {
            cursorBlinkElapsed = 0;
            currCursorType = (currCursorType + 1) % cursorType.Length;
        }

        ;
    }

    // Code to load scene and start typing
    public IEnumerator LoadScene(string sceneID)
    {
        // Gets scene from XML file
        currentScene = ParseXML.getScene(sceneID);

        // Get decisions from scene
        XmlNodeList decisions = currentScene.SelectNodes(".//decision");
        XmlNodeList text = currentScene.ChildNodes;
        List<(string, string)> message = CompileText(text);

        
        // Auto-type main text
        foreach((string, string) tuple in message)
        {
            for (int i = 0; i < tuple.Item2.Length; i++)
            {
                // Add colour tags to include newly typed text
                if (i == 0) textComponent.text += String.Format("<color={0}>", tuple.Item1);
                else textComponent.text = textComponent.text.Remove(textComponent.text.Length - 8);

                textComponent.text = textComponent.text.Substring(0, textComponent.text.Length) + tuple.Item2[i] + "</color>";
                yield return new WaitForSeconds(typingSpeed);
            }
                
        }

        // Print decisions thereafter
        foreach(XmlNode node in decisions)
        {
            textComponent.text += ReformatString(String.Format("{0} - {1}", node.Attributes[0].Value, node.InnerText), 2);
            yield return new WaitForSeconds(typingSpeed);
        }

        togglePlayerTyping();
    }


    // Code for decision to be carried out
    private void makeDecision(string decisionID)
    {
        // Gets decision nodes
        XmlNodeList decisions = currentScene.SelectNodes(".//decision");

        foreach (XmlNode node in decisions)
        {
            // if decision exists, do following things
            if (decisionID == node.Attributes[0].Value)
            {
                togglePlayerTyping();

                XmlNode spaceBattle = node.SelectSingleNode(".//spacebattle");
                if (spaceBattle != null) {
                    SpaceBattle.InitSpaceBattle(spaceBattle, textComponent);
                    return;
                }
                
                // Shows that the player responded with the decision
                textComponent.text = textComponent.text.Remove(textComponent.text.Length - 3) + "YOU: " + node.FirstChild.InnerText + "\n\n\n";

                // Load next scene
                StartCoroutine(LoadScene(node.Attributes[1].Value));
            }
        }
    }


    // Makes text appear one char at a time
    public IEnumerator BuildText(string message)
    {
        message = ReformatString(message, 2);
        for (int i = 0; i < message.Length; i++)
        {
            textComponent.text = textComponent.text.Substring(0, textComponent.text.Length) + message[i];
            yield return new WaitForSeconds(typingSpeed);
        }
        togglePlayerTyping();
    }

    // Changes /n to a newline char so paragraphs can be written from unity and xml file
    private string ReformatString(string text, int newLines)
    {

        string newText = ""; // Initialise text

        for (int i = 0; i < text.Length; i++)
        {
            // Replaces /n with \n as unity's text editor is a bit funky. Just bear in mind which way the "fake" newline char has the slash
            if (i <= text.Length - 2 && string.Compare(text.Substring(i, 2), "/n") == 0)
            {
                newText = string.Concat(newText, "\n");
                if (i < text.Length)
                {
                    if (text[i + 2] == ' ') // Removes space after newline char if there is one
                    {
                        i++;
                    }
                }
                i++;

            }
            else
            {
                newText = string.Concat(newText, text[i]);
            }
        }

        for (int i = 0; i < newLines; i++) newText += '\n';

        return newText;
    }

    // Compiles text from xml (allows for text in paragraph to have different colours)
    private List<(string,string)> CompileText(XmlNodeList texts)
    {
        List<(string, string)> compiledText = new List<(string, string)>();
        foreach (XmlNode node in texts)
        {
            if (node.Name == "text")
            {
                if (node.Attributes.Count > 0)
                {
                    compiledText.Add((node.Attributes[0].Value, ReformatString(node.InnerText, 0)));
                }
                else
                {
                    compiledText.Add((DEFAULT_TEXT_COLOUR, ReformatString(node.InnerText, 0)));
                }
            }
        }

        // Adds new lines to final piece of text
        if (compiledText.Any())
        {
            (string, string) lastItem = compiledText.Last();
            compiledText.RemoveAt(compiledText.Count - 1);
            compiledText.Add((lastItem.Item1, ReformatString(lastItem.Item2, 2)));
        }
        
        

        return compiledText;
    }


    // Listens for user inputs
    private void playerTyping()
    {
        char keyPressed = '\0';

        // Number keys are used for decisions. Using this method, there can be no more than 10 decisions
        for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; i++) {
            if (Input.GetKeyDown((KeyCode)i))
            {
                // Run decision and return, the number is never printed and no decision is made if player chose 4 but there are 3 decisions
                keyPressed = (char)i;
                makeDecision(keyPressed.ToString());
                return;
            }
        }


        // Cycle through keys A-Z and check if pressed
        // I chose this method rather than an InputField simply because this gives more control about how the terminal responds to inputs
        for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
            {
                keyPressed = (char)i;
            }
        }

        // End command by hitting return
        if (Input.GetKeyDown(KeyCode.Return))
        {
            keyPressed = '\n';
            togglePlayerTyping();

        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            keyPressed = ' ';
        }

        // C# doesn't have a null char, so this will have to do
        // Because of the flashing cursor, new characters are added by first removing the cursor, adding the char, then adding the cursor again
        if (keyPressed != '\0')
        {
            textComponent.text = string.Concat(textComponent.text.Remove(textComponent.text.Length - 1), keyPressed);
            textComponent.text = string.Concat(textComponent.text, cursorType[currCursorType]);
            if (keyPressed == '\n')
            {
                textComponent.text = textComponent.text.Remove(textComponent.text.Length - 1);
                textComponent.text += keyPressed;
                ExecuteCommand(command);
                command = "";
                return;
            }
            command += keyPressed;
        }
        else
        {
            textComponent.text = string.Concat(textComponent.text.Remove(textComponent.text.Length - 1), cursorType[currCursorType]);
        }
    }


    // This will be used for executing functions
    private void ExecuteCommand(string commandLine)
    {
        string[] args = commandLine.Split(' ');

        if (args[0] == string.Empty)
        {
            togglePlayerTyping();
            return;
        }

        string commandName = args[0];

        Command command = (Command)textComponent.gameObject.GetComponent(char.ToUpper(commandName[0]) + commandName.Substring(1));

        if (command != null)
        {
            command.Run(this);
            return;
        }
        else
        {
            textComponent.text = string.Concat(textComponent.text, String.Format("The command '{0}' is unrecognised. Please enter 'help' for available commands.\n\n", commandName));
        }



        togglePlayerTyping();
    }


    // Toggles if player can type and also adds arrow at the start of typeable lines
    public void togglePlayerTyping()
    {
        if (!allowPlayerTyping)
        {
            textComponent.text = string.Concat(textComponent.text, newLine);
            allowPlayerTyping = true;
        }
        else
        {
            allowPlayerTyping = false;
        }
    }


    public static string AdjoinTextWithPadding(string s1, string s2)
    {
        return s1.PadRight(TEXT_PADDING) + s2;
    }


}


