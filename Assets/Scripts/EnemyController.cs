using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EnemyController : MonoBehaviour, IEnemy {

    #region PrivateFields

    [SerializeField] private EnemyData thisData;
    [SerializeField] private GridPreferences gridPreferences;
    private float speed;
    private float health;
    private Coroutine moveCoroutine;
    /// <summary>
    /// This is how we will stop our enemy, after collision we simply will start checking closeness to the collided turret object
    /// and if it is smaller or equal than spacingZ, we will stop the object.
    /// </summary>
    [SerializeField] private Transform collidedTransform;
    #endregion

    #region Interface Implementation

    public void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) {
            StopCoroutine(moveCoroutine);
            /*we only remove the enemy from activeEnemies list
            //if turret shots it. So, if none are left
            //player cleared the level. */
            EnemySpawner.Instance.activeEnemies.Remove(gameObject);
            if (EnemySpawner.Instance.activeEnemies.Count <= 0) {
                GameManager.Instance.GameOver(true);
            }
            GoBackToPool();
        }
        
    }

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake() {
        InitializeVariables();
    }

    void OnEnable() {
        moveCoroutine = StartCoroutine(MoveEnemyNumerator());
    }
    
    void Start() {
        /*
        //We will keep our pooled objects across scenes
        //So if they are active when new scene loaded, we will make sure they will return back to pool.
        //And since start is called only once for a lifetime of an object
        //we will subscribe inside start*/
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        if (gameObject.activeSelf) {
            GoBackToPool();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<ITurret>() != null) {
            collidedTransform = collision.gameObject.transform;
            return;
        }

        if (collision.gameObject.CompareTag("DestroyCollided")) {
            //We collided with boundary object, so this means enemy succeeded to reach to base.
            if(moveCoroutine!=null)
                StopCoroutine(moveCoroutine);
            GoBackToPool();
            EnemyWin();
            
        }
    }
    private void OnDisable(){
        collidedTransform = null; //we should free up this, otherwise it will stop at wrong position
    }

    #endregion

    #region Custom Methods

    void EnemyWin() {
        GameManager.Instance.GameOver(false);
    }

    void InitializeVariables() {
        speed = thisData.Speed;
        health = thisData.Health;
        collidedTransform = null;
    }

    void GoBackToPool() {
        ObjectPooler.Instance.ReturnObjectToPool(thisData.EnemyPoolable.ItemName,gameObject);
    }

    bool CheckIfShouldStop() {
        if (collidedTransform == null)
            return false;
        bool xPointCheck = gameObject.transform.position.x == collidedTransform.position.x;
        bool distanceCheck =(gameObject.transform.position.z - collidedTransform.transform.position.z) <=
                            gridPreferences.SpacingZ;
        return (xPointCheck && distanceCheck);
    }

    IEnumerator MoveEnemyNumerator() {
        while (!CheckIfShouldStop()) {
            gameObject.transform.Translate(Vector3.back*Time.deltaTime*speed,Space.World);
            yield return null;
        }

        yield return null;
    }

    #endregion

    


}
