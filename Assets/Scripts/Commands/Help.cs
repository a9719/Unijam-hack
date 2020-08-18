using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Help : MonoBehaviour, Command
{


    public string text = ""; // I allowed this to be changed with the thought that a difficult level could reply with "Run" or something creepy instead
    private readonly string DESCRIPTION = Terminal.AdjoinTextWithPadding("<Help>", "- Displays available commands\n"); // Description of command
   

    // Runs command
    public void Run(Terminal terminal)
    {

        if (text == string.Empty)
        {
            // Get array of all commands 
            // NOTE: this is in order of placement in unity
            Command[] commands = terminal.GetComponentsInParent<Command>();

            // Run through commands and add descriptions
            foreach (Command command in commands)
            {
                text += command.ToString();
            }
        }

        // Starts typing commands
        StartCoroutine(terminal.BuildText(text.Remove(text.Length - 1)));
    }


    // Overrides ToString function to display description instead
    override
    public string ToString()
    {
        return DESCRIPTION;
    }
}
