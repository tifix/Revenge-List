using UnityEngine;

namespace DS.ScriptableObjects
{
    public class DSDialogueGroupSO
    {
        [field: SerializeField] public string GroupName;// { get; set; }
        public void Init(string groupName)
        {
            GroupName = groupName;
        }
    }
}

