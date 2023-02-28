#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DS.Data.Error
{
    public class DSErrorData
    {
        public Color color { get; set; }

        public DSErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            color = new Color32(
                (byte) Random.Range(65, 256),
                (byte) Random.Range(50, 176),
                (byte) Random.Range(50, 176), 
                255
                );
        }
    }
}

#endif