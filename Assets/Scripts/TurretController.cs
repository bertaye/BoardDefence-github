using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurretController : MonoBehaviour, ITurret
{
    #region Private Fields

    [SerializeField] private float timeBetweenShots;
    private WaitForSeconds shootDelayer; //we will decrease garbage collection significantly
    private Coroutine lookCoroutine; //to decrease memory usage and make our look behaviour more controllable
    private Coroutine shootCoroutine;
    private Quaternion initialGunRotation;
    private GameObject lastEnemyEnteredRange;
    private Queue<GameObject> enemiesInRange;
    [SerializeField] private TurretData thisTurretData;
    [SerializeField] private GridPreferences gridPreferences; //we will create a collider box for the turret with help of grid spacing
    [SerializeField] private GameObject gunObject; //This object will be used for following enemy
    [SerializeField] private Transform firePoint;
    private BoxCollider boxCollider;
    
    #endregion

    #region MonoBehaviour Callbacks

    private void Awake() {
        InitializeVariables();
    }

    // Start is called before the first frame update
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

    void GoBackToPool() {
        ObjectPooler.Instance.ReturnObjectToPool(thisTurretData.TurretPoolable.ItemName, gameObject);
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<IEnemy>() == null)
            return; //or we can use collision matrix to make sure no other  thing will collide with this object
                    //but in prototypes, this wouldnt be feasible for 'O' of the S.O.L.I.D principles, we would be close to improvement too.

        Debug.Log("Enemy Entered Collider!");
        
        if (enemiesInRange.Count == 0) {
            lookCoroutine = StartCoroutine(LookEnemyNumerator());
            shootCoroutine = StartCoroutine(ShootEnemeyNumerator());
        }
        enemiesInRange.Enqueue(collision.gameObject);


    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.GetComponent<IEnemy>() == null)
            return;

        enemiesInRange.Dequeue();
        if (enemiesInRange.Count <= 0) {
            StopCoroutine(lookCoroutine);
            StopCoroutine(shootCoroutine);
            gunObject.transform.rotation = initialGunRotation;
        }
        
    }

    #endregion

    public void ActivateTurret() {
        SetColliderSizeAndCenter();
    }
    
    /// <summary>
    /// This function will be used to set the collider
    /// so we can trigger attacking to enemy
    /// </summary>
    void SetColliderSizeAndCenter() {
        Vector3 gridScale = gridPreferences.GridBlock.transform.localScale;
        Vector3 localScale = gameObject.transform.localScale;
        float spacingX = gridPreferences.SpacingX;
        float spacingZ = gridPreferences.SpacingZ;
        
        
        //Since box collider transform measured in local space of object, we need to
        //divide everything to turret's localscale
        if (thisTurretData.AttackDirection == AttackDirections.Forward) {
            boxCollider.size = new Vector3((gridScale.x / localScale.x),
                                        (gridScale.y/localScale.y),
                                        ((thisTurretData.Range)*spacingZ + gridScale.z)/localScale.z );

            boxCollider.center = new Vector3((0f),
                                        (gridScale.y / localScale.y / 2),
                                        (((float) thisTurretData.Range / 2) * spacingZ/ localScale.z));
        }
        else if(thisTurretData.AttackDirection == AttackDirections.All) {
            boxCollider.size = new Vector3(((2*spacingX*thisTurretData.Range + gridScale.x)/localScale.x),
                                        (gridScale.y/localScale.y),
                                        ((2*spacingZ*thisTurretData.Range + gridScale.z)/localScale.z) );

            
            boxCollider.center = new Vector3((0f),
                                            (gridScale.y / localScale.y / 2),
                                            0);
        }
        
        
    }

    void InitializeVariables() {
        boxCollider = gameObject.GetComponent<BoxCollider>();
        if (gunObject == null) {
            gunObject = transform.GetChild(0).gameObject;
            #if UNITY_EDITOR
            //We will get the first child as gun object, but this is not reliable so we will give a log warning.
            Debug.LogWarning("Gun object of turret " + gameObject.name + " not attached!");
            #endif
        }

        if (firePoint == null) {
            firePoint = gunObject.transform;
            #if UNITY_EDITOR
            Debug.LogWarning("Fire Point of turret " + gameObject.name + " not attached!");
            #endif
        }
        initialGunRotation = gunObject.transform.rotation;
        enemiesInRange = new Queue<GameObject>();
        if (timeBetweenShots <= 0)
            timeBetweenShots = 1f;
        shootDelayer = new WaitForSeconds(timeBetweenShots);
        
    }

    void ShootEnemy() {
        if (enemiesInRange.Count <=0 || !enemiesInRange.Peek().activeInHierarchy)
            return;
        GameObject bullet = ObjectPooler.Instance.GetObjectFromPool(thisTurretData.TurretBullet.ItemName);
        bullet.SetActive(true);
        Quaternion pureRotation;
        pureRotation = gunObject.transform.rotation * Quaternion.Inverse(initialGunRotation); //simply we are subtracting the initial rotation offset
        bullet.transform.position = firePoint.position;
        bullet.transform.LookAt(enemiesInRange.Peek().transform);
        bullet.GetComponent<BulletController>().turretData = thisTurretData;
        bullet.GetComponent<BulletController>().FireBullet(enemiesInRange.Peek().transform.position);
    }
    void LookEnemy() {
        if (enemiesInRange.Count > 0) {

            //If the enemy object disabled and returned to pool
            //we need to remove it from our queue and stop looking to it.
            if (enemiesInRange.Peek().activeInHierarchy == false) {
                enemiesInRange.Dequeue();
                if (enemiesInRange.Count <= 0) {
                    StopCoroutine(lookCoroutine);
                    StopCoroutine(shootCoroutine);
                    gunObject.transform.rotation = initialGunRotation;
                    return;
                }
                    
            }
            
            gunObject.transform.LookAt(enemiesInRange.Peek().transform.position,Vector3.up);
            //if prefab's rotation set differently than we expect
            //we may end up with weird results, so we will add the initial rotation to stay on safe-side
            gunObject.transform.rotation *= initialGunRotation; 
        }
    }

    IEnumerator LookEnemyNumerator() {
        while (true) {
            LookEnemy();
            yield return null;
        }
    }

    IEnumerator ShootEnemeyNumerator() {
        while (true) {
            ShootEnemy();
            yield return shootDelayer;
        }
    }
}
