using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="defaultDialogue", menuName = "dialogueData")]
public class Dialogue : ScriptableObject
{
    [TextArea] public List<string> textBody;
               public List<string> textSpeaker;
}
