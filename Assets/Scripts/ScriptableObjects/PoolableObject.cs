using UnityEngine;

[CreateAssetMenu(fileName = "Poolable Object", menuName = "Custom Elements/New Poolable Object", order = 3)]
public class PoolableObject : ScriptableObject {
    
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private string itemName;
    
    [Tooltip("If the object is not expandable, please make sure that 'Amount to Pool' is larger or equal to required max amount of the object!")]
    [SerializeField] private int amountToPool;
    
    [Tooltip("If the object is not expandable, please make sure that 'Amount to Pool' is larger or equal to required max amount of the object!")]
    [SerializeField] private bool isExpandablePool;
    
    #region Properties
    
    public GameObject ObjectToPool {
        get { return objectToPool; }
    }
    
    public string ItemName {
        get { return itemName; }
    }
    
    public int AmountToPool {
        get { return amountToPool; }
    }

    public bool IsExpandablePool {
        get { return isExpandablePool; }
    }
    
    #endregion
}
