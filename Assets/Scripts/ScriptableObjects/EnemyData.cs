using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Custom Elements/New Enemy Data", order = 5)]
public class EnemyData : ScriptableObject {
    
    [SerializeField] private float health;
    
    [Tooltip("Speed of the enemy object (blocks/sec)")]
    [SerializeField] private float speed;

    [SerializeField] private PoolableObject enemyPoolable;

    #region Properties

    public float Health {
        get { return health; }
    }
    
    public float Speed {
        get { return speed; }
    }
    
    public PoolableObject EnemyPoolable {
        get { return enemyPoolable; }
    }

    #endregion
}
