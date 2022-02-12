using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class MaxAndCurrentAmounts {
    public MaxAndCurrentAmounts(int _max, int _current) {
        maxAmount = _max;
        currentAmount = _current;
    }
    public int maxAmount { get; set; }
    public int currentAmount { get; set; }
}
public class EnemySpawner : MonoBehaviour {

    #region PrivateFields
    [SerializeField] private GridPreferences gridPreferences;

    [SerializeField] private LevelData thisLevel;
    
    [SerializeField] private float timeBetweenSpawns;
    //We will hold enemies and spawned amounts of them in here
    private Dictionary<PoolableObject,MaxAndCurrentAmounts> enemiesSpawned;
    private WaitForSeconds spawnDelayer;
    

    #endregion


    #region PublicFields

    /// <summary>
    /// With this list, we can easily check if any active enemies left
    /// and determine if player is won
    /// </summary>
    public List<GameObject> activeEnemies = new List<GameObject>();

    #endregion
    
    
    #region  SingletonImplementation

    private static EnemySpawner _instance;
    
    public static EnemySpawner Instance {
        get { return _instance; }
    }

    #endregion

    #region MonoBehaviour Callbacks
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(_instance);
        } else {
            _instance = this;
        }
        InitializeVariables();
    }

    private void Start() {
        StartCoroutine(SpawnNumerator());
    }
    

    #endregion

    #region Custom Methods
 /// <summary>
    /// This function is responsible from checking the spawned object counts and
    /// returning an available key for spawning. If there is no available key,
    /// it will return "" value which we will check inside numerator.
    /// </summary>
    /// <returns></returns>
    string RequestRandomPoolable() {
        string returnKey="";
        if (enemiesSpawned.Count <= 0) {
            return returnKey;
        }
            
        
        int random = UnityEngine.Random.Range(0, enemiesSpawned.Count);
        enemiesSpawned.ElementAt(random).Value.currentAmount++;
        returnKey = enemiesSpawned.ElementAt(random).Key.ItemName;
        
        //Following if statements check if we reached the maxAllowedAmount of spawn of this object
        if (enemiesSpawned.ElementAt(random).Value.currentAmount >= enemiesSpawned.ElementAt(random).Value.maxAmount) {
            enemiesSpawned.Remove(enemiesSpawned.ElementAt(random).Key);
        }
        return returnKey;
    }

    Vector3 RequestRandomPosition() {

        int randomX = UnityEngine.Random.Range(0, gridPreferences.Columns); //0, 1, 2, 3 for default case
        Debug.Log("RandomX: "+randomX);
        float x = (gridPreferences.SpacingX / 2 + (randomX - gridPreferences.Columns/2)*gridPreferences.SpacingX ) ; //to distribute equally around X axis!
        float y = 0.5f; //this is a predefined value
        float z = ( (int) (gridPreferences.Rows-1)/2)*gridPreferences.SpacingZ + gridPreferences.SpacingZ/2;

        return new Vector3(x, y, z);

    }

    void InitializeVariables() {
        if(timeBetweenSpawns<=0)
            timeBetweenSpawns = 1f;
        spawnDelayer = new WaitForSeconds(timeBetweenSpawns); //to reduce garbage collection
        enemiesSpawned = new Dictionary<PoolableObject, MaxAndCurrentAmounts>();
        foreach (LevelElement levelElement in thisLevel.ObjectsAndAmounts) {
            //Following if statement will pull Enemy objects from this level
            if (levelElement._poolableObject.ObjectToPool.GetComponent<IEnemy>() != null) {
                MaxAndCurrentAmounts tempClass = new MaxAndCurrentAmounts(levelElement.maxAmount, 0);
                enemiesSpawned.Add(levelElement._poolableObject,tempClass);//initially we didnt spawn any of these objects
            }
            
        }
    }
    
    IEnumerator SpawnNumerator() {
        while (true) {
            string key = RequestRandomPoolable();
            if (key == "")
                break;
            GameObject enemyGo;
            enemyGo = ObjectPooler.Instance.GetObjectFromPool(key);
            enemyGo.transform.position = RequestRandomPosition();
            Debug.Log("ENEMY START POS: "+enemyGo.transform.position);
            enemyGo.SetActive(true);
            activeEnemies.Add(enemyGo);
            yield return spawnDelayer;
        }

        yield return null;
    }
    

    #endregion

   
}
