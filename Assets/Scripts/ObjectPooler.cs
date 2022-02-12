using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    #region  SingletonImplementation

    private static ObjectPooler _instance;
    
        public static ObjectPooler Instance {
            get { return _instance; }
        }

    #endregion

    #region Private Fields

    //we will get the required object from single pool, with "key" set by 'PoolableObject'
    private Dictionary<string, Queue<GameObject> > Pool;

    #endregion

    #region Public Fields

    public List<PoolableObject> objectsToPool;
    public GameObject poolContainer;

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(_instance);
        } else {
            _instance = this;
            DontDestroyOnLoad(_instance);
            InitializeVariables();
            CreatePool(); //Awake is called between scene loadings so we dont want to create a duplication of our pool. 
                          //This is why we will only create pool if this singleton is THE singleton
        }
        
    }
    #endregion

    #region Pool Methods
    public GameObject GetObjectFromPool(string key) {
        if (!Pool.ContainsKey(key)) {
            Debug.LogError("Null Pooled Object Requested! KEY: " +key);
            return null;
        }

        if (Pool[key].Count <= 0) {

            foreach (PoolableObject poolable in objectsToPool) {
                if (poolable.ItemName == key) {
                    if (!poolable.IsExpandablePool) {
                        Debug.Log("Non-expandable pool consumed, KEY: "+key);
                        return null;
                        
                    }
                }
            }
            
            ExpandPool(key);
            #if UNITY_EDITOR
            Debug.LogWarning("Pool is empty for the key: " + key);
            #endif
        }
        
        #if UNITY_EDITOR
        Debug.Log("POOL will return the object with KEY: " + key + "    NAME: "+ Pool[key].Peek().name +"   LEFT:"+(Pool[key].Count-1));
        #endif
        return Pool[key].Dequeue();
    }

    public void ReturnObjectToPool(string key, GameObject poolObj) {
        if (!Pool.ContainsKey(key)) {
            Debug.LogError("There is no such KEY: "+key +", object cant return to pool.");
            return;
        }
        poolObj.SetActive(false);
        Pool[key].Enqueue(poolObj);
        #if UNITY_EDITOR
        Debug.Log("Object returned to the pool with KEY: " + key + "    NAME: "+ Pool[key].Peek().name +"   LEFT:"+(Pool[key].Count));
        #endif
    }
    void ExpandPool(string key="") {
        
        //By default, we will expand the pool for each object with 5 new elements
        if (key == "") {
            foreach (PoolableObject poolable in objectsToPool) {
                if (!poolable.IsExpandablePool) {
                    continue; //we dont want some objects to expand, like, Defence Items since they are specified with const numbers.
                }
                for (int i = 0; i < 5; i++) {
                    GameObject poolObj = (GameObject) Instantiate(poolable.ObjectToPool, poolContainer.transform);
                    poolObj.SetActive(false);
                    Pool[poolable.ItemName].Enqueue(poolObj);
                }
            }
        }
        else {
            
            //This is not likely to happen since we already know the key from scriptable objects, but just to stay on safe side 
            //I implemented this.
            if (!Pool.ContainsKey(key)) {
                Debug.LogError("Null Key!");
                return;
            }
            
            //First wee need to find the object of corresponding key,
            //this will done by searching through objectsToPool list
            foreach (PoolableObject po in objectsToPool) {
                if (!po.IsExpandablePool) {
                    continue; //we dont want some objects to expand, like, Defence Items since they are specified with const numbers.
                }
                if (key == po.ItemName) {
                    GameObject objToInstantiate = po.ObjectToPool;
                    for (int i = 0; i < 10; i++) {
                        GameObject poolObj = (GameObject) Instantiate(objToInstantiate, poolContainer.transform);
                        poolObj.SetActive(false);
                        Pool[key].Enqueue(poolObj);
                    }
                    
                    //Now we will return since no more search needed
                    //This is to optimise best-case time complexity
                    return;
                }
            }
            
        }
        
    }

    void CreatePool() {
        Queue<GameObject> tempPool = new Queue<GameObject>();
        foreach (PoolableObject poolable in objectsToPool) {
            tempPool = new Queue<GameObject>();

            //If someone adds same object multiple times for pooling
            //We will pool the object but also give a warning to editor.
            if (Pool.ContainsKey(poolable.ItemName)) {
                for (int i = 0; i < poolable.AmountToPool; i++) {
                    GameObject poolObj = (GameObject) Instantiate(poolable.ObjectToPool,poolContainer.transform);
                    poolObj.SetActive(false);
                    Pool[poolable.ItemName].Enqueue(poolObj);
                }
                
                //Debug.Log can be memory consuming, so we will give it only for the editor.
                #if UNITY_EDITOR
                Debug.LogWarning("Item with duplicated key value added to object pool.");
                #endif
                 
                continue;
            }
            for (int i = 0; i < poolable.AmountToPool; i++) {
                GameObject poolObj = (GameObject) Instantiate(poolable.ObjectToPool,poolContainer.transform);
                poolObj.SetActive(false);
                tempPool.Enqueue(poolObj);
            }
            
            Pool.Add(poolable.ItemName,tempPool);
            
        }
    }
    #endregion

    #region Custom Methods

    void InitializeVariables() {
        Pool = new Dictionary<string, Queue<GameObject>>();
    }

    #endregion
    
    
}
