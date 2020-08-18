using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
{
    public string initMessage = "";
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

    // Start is called before the first frame update
    void Start()
    {
        initMessage = initMessage + '\n' + '\n';
        StartCoroutine(BuildText(ReformatString(initMessage)));
    }

    // Update is called once per frame
    void Update()
    {

        // Speeds up text animation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            typingSpeed = typingSpeed / typingSpeedMultiplier;
        } else if (Input.GetKeyUp(KeyCode.Space))
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

    // Makes text appear one char at a time
    private IEnumerator BuildText(string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            textComponent.text = textComponent.text.Substring(0, textComponent.text.Length) + message[i];
            yield return new WaitForSeconds(typingSpeed);
        }
        togglePlayerTyping();
    }

    // Changes /n to a newline char so paragraphs can be written from unity
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

            } else
            {
                newText = string.Concat(newText, text[i]);
            }
        }
        return newText;
    }


    // Listens for user inputs
    private void playerTyping()
    {
        char keyPressed = '\0';

        // Cycle through keys A-Z and check if pressed
        // I chose this method rather than an InputField simply because this gave me more control
        for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
            {
                keyPressed = (char) i;
            }
        }

        // End command by hitting return
        if (Input.GetKeyDown(KeyCode.Return))
        {
            keyPressed = '\n';
            togglePlayerTyping();

        } else if (Input.GetKeyDown(KeyCode.Space))
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

        if (args[0] == "")
        {
            togglePlayerTyping();
            return;
        }

        string functionName = args[0];

        textComponent.text = string.Concat(textComponent.text, String.Format("The command '{0}' is unrecognised. Please enter 'help' for available commands\n\n", functionName));


        togglePlayerTyping();
    }


    // Toggles if player can type and also adds arrow at the start of typeable lines
    private void togglePlayerTyping()
    {
        if (!allowPlayerTyping)
        {
            textComponent.text = string.Concat(textComponent.text, newLine);
            allowPlayerTyping = true;
        } else
        {
            allowPlayerTyping = false;
        }
    }


}
