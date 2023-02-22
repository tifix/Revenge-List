using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    using Data;
    using Enumerations;
    public class DSDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] public string SpeakerName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<DSDialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DSDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }
        public void Init(string dialogueName, string text, string speaker, List<DSDialogueChoiceData> choices, DSDialogueType dialogueType, bool isStartingDialogue)
        {
            DialogueName = dialogueName;
            SpeakerName = speaker;
            Text = text;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
        }
    }  
}

