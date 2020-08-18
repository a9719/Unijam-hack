using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Terminal : MonoBehaviour
{
    public static readonly int TEXT_PADDING = 15;
    public static readonly string STORY_FILE = "Assets/Resources/StoryFile.xml";
    private readonly string FIRST_SCENE = "0";

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
    }

    // Code to load scene and start typing
    public IEnumerator LoadScene(string sceneID)
    {
        XmlNode sceneData = ParseXML.getScene(sceneID);
        currentScene = sceneData;
        XmlNodeList decisions = sceneData.SelectNodes(".//decision");

        string message = ReformatString(sceneData.FirstChild.InnerText);
        for (int i = 0; i < message.Length; i++)
        {
            textComponent.text = textComponent.text.Substring(0, textComponent.text.Length) + message[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        foreach(XmlNode node in decisions)
        {
            textComponent.text += ReformatString(String.Format("{0} - {1}", node.Attributes[0].Value, node.InnerText));
        }

        togglePlayerTyping();
    }


    // Code for decision to be carried out
    private void makeDecision(string decisionID)
    {
        
        XmlNodeList decisions = currentScene.SelectNodes(".//decision");
        foreach (XmlNode node in decisions)
        {
            if (decisionID == node.Attributes[0].Value)
            {
                togglePlayerTyping();
                textComponent.text = textComponent.text.Remove(textComponent.text.Length - 3) + "YOU: " + node.InnerText + "\n\n";
                StartCoroutine(LoadScene(node.Attributes[1].Value));
            }
        }
    }


    // Makes text appear one char at a time
    public IEnumerator BuildText(string message)
    {
        message = ReformatString(message);
        for (int i = 0; i < message.Length; i++)
        {
            textComponent.text = textComponent.text.Substring(0, textComponent.text.Length) + message[i];
            yield return new WaitForSeconds(typingSpeed);
        }
        togglePlayerTyping();
    }

    // Changes /n to a newline char so paragraphs can be written from unity and xml file
    private string ReformatString(string text)
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
        return newText + '\n' + '\n';
    }


    // Listens for user inputs
    private void playerTyping()
    {
        char keyPressed = '\0';

        for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; i++) {
            if (Input.GetKeyDown((KeyCode)i))
            {
                keyPressed = (char)i;
                makeDecision(keyPressed.ToString());
                return;
            }
        }


        // Cycle through keys A-Z and check if pressed
        // I chose this method rather than an InputField simply because this gave me more control
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
                ExecuteFunction(command);
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
    private void ExecuteFunction(string commandLine)
    {
        string[] args = commandLine.Split(' ');

        if (args[0] == string.Empty)
        {
            togglePlayerTyping();
            return;
        }

        string functionName = args[0];

        Command function = (Command)textComponent.gameObject.GetComponent(char.ToUpper(functionName[0]) + functionName.Substring(1));

        if (function != null)
        {
            function.Run(this);
            return;
        }
        else
        {
            textComponent.text = string.Concat(textComponent.text, String.Format("The command '{0}' is unrecognised. Please enter 'help' for available commands.\n\n", functionName));
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


