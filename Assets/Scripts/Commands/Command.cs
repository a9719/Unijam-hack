using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface Command
{

    // Downcasting so we have an extra type of object we can do things with :)

    // Runs command
    void Run(Terminal terminal);

    // Overrides ToString function
    string ToString();
}
