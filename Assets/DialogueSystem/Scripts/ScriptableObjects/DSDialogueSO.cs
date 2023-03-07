using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    using Data;
    using Enumerations;
    [System.Serializable]
    public class DSDialogueSO
    {
        [field: SerializeField] public string DialogueName;// { get; set; }
        [field: SerializeField] public string nodeID;// { get; set; }
        [field: SerializeField] public string SpeakerName;// { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text;// { get; set; }
        [field: SerializeField] public string SpritePath;//{ get; set; }
        [field: SerializeField] public List<DSDialogueChoiceData> Choices;// { get; set; }
        [field: SerializeField] public DSDialogueType DialogueType;// { get; set; }
        [field: SerializeField] public bool IsStartingDialogue;// { get; set; }
        public void Init(string ID,string dialogueName, string text, string speaker, string spritePath, List<DSDialogueChoiceData> choices, DSDialogueType dialogueType, bool isStartingDialogue)
        {
            DialogueName = dialogueName;
            Text = text;
            SpeakerName = speaker;
            SpritePath = spritePath;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
        }
    }  
}

