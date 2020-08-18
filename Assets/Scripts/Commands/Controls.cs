using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour, Command
{
    private readonly string DESCRIPTION = Terminal.AdjoinTextWithPadding("<Controls>", "- Displays controls\n");
    private readonly string CONTROLS = Terminal.AdjoinTextWithPadding("Space", "- speed up auto-typing text\nArrow Keys (Or W,A,S,D) - Move in space battles");

    public void Run(Terminal terminal)
    {
        StartCoroutine(terminal.BuildText(CONTROLS));
    }

    override
    public string ToString()
    {
        return DESCRIPTION;
    }
}
