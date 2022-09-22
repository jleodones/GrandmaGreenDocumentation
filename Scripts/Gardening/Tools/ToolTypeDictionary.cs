using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GrandmaGreen/Tools/ToolTypeDictionary")]
public class ToolTypeDictionary : ScriptableObject
{
    public List<ToolType> toolData;

    public ToolType this[int i]
    {
        get { return toolData[i]; }
        set { toolData[i] = value; }
    }

}
