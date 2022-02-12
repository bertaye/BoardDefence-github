using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Since Unity cannot serialize dictionaries, this one will be used to simplify our job.
/// We will reach the poolable object from here to limit its count on the game.
/// </summary>

[CreateAssetMenu(fileName = "Level Data", menuName = "Custom Elements/New Level Data", order = 4)]
public class LevelData : ScriptableObject {

    [Tooltip("Add only NON-REUSABLE elements.")]
    [SerializeField] private List<LevelElement> objectsAndAmounts;

    
    public List<LevelElement> ObjectsAndAmounts {
        get { return objectsAndAmounts;}
    }
}
