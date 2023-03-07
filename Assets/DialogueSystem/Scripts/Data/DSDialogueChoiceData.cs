using System;
using UnityEngine;

namespace DS.Data
{
    using ScriptableObjects;
    [Serializable]
    public class DSDialogueChoiceData
    {
        [field: SerializeField] public string Text;// { get; set; }
        [field: SerializeField] public string nextID;// NextDialogue;// { get; set; }
    }
}

