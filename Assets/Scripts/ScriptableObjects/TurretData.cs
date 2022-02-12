using UnityEngine;

public enum AttackDirections {
    Forward,
    All
}

/// <summary>
/// This Data object will be used only for TurretController.cs class
/// That class will behave according to these settings.
/// To instantiate turrets, we will still need Turret Poolable Objects!
/// </summary>
[CreateAssetMenu(fileName = "Turret Data", menuName = "Custom Elements/New Turret Data", order = 2)]
public class TurretData : ScriptableObject {
    
    [SerializeField] private float damage;
    
    [Tooltip("Attack range (in blocks).")]
    [SerializeField] private int range;
    
    [Tooltip("Allowed time between two consecutive placement of the turret (in seconds).")]
    [SerializeField] private float cooldownTime;
    
    [SerializeField] private AttackDirections attackDirection;

    //This is added just to make game to more easy to modify. 
    //With this data, game designer can set the bullet data of the object and 
    //we can easily get the pooled object by name(we are taking the name from the turretBullet) from ObjectPooler.
    [SerializeField] private PoolableObject turretBullet;

    [SerializeField] private PoolableObject turretPoolable;
    
    #region Properties
    
    public float Damage {
        get { return damage; }
    }
    
    public int Range {
        get { return range; }
    }
    
    public float CoolDownTime {
        get { return cooldownTime; }
    }
    
    public AttackDirections AttackDirection {
        get { return attackDirection; }
    }

    public PoolableObject TurretBullet {
        get{return turretBullet;}
    }
    
    public PoolableObject TurretPoolable {
        get { return turretPoolable; }
    }

    #endregion
}
