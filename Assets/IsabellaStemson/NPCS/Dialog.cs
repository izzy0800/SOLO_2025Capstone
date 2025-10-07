using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialog", menuName = "Dialog/Dialog")]
public class Dialog : ScriptableObject
{
    public DialogLine[] lines;
}

[System.Serializable]
public class DialogLine
{
    public string speakerName;
    [TextArea(3, 10)]
    public string text;
    public DialogChoice[] choices;
}

[System.Serializable]
public class DialogChoice
{
    public string text;
    public Dialog response;
}