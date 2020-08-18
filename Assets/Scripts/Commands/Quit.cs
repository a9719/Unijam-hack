using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour, Command
{
    private string description = "<Quit> - Exits the game\n";

    public void Run(Terminal terminal)
    {
        Application.Quit(0);
    }

    override
    public string ToString()
    {
        return description;
    }
}
