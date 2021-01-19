using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 For each concrete command we need to implement these 2 methods
 */
public abstract class Command
{
    public abstract void Execute();
    public abstract void ExecuteUndo();
}
