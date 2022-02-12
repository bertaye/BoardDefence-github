using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BulletController : MonoBehaviour,IBullet {
    
    public TurretData turretData { get; set; }
    [SerializeField] private float speed=3f;
    private Coroutine fireCoroutine;

    #region MonoBehaviour Callbacks

    void Start() {
        
        /*
       //We will keep our pooled objects across scenes
       //So if they are active when new scene loaded, we will make sure they will return back to pool.
       //And since start is called only once for a lifetime of an object
       //we will subscribe inside start*/
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "DestroyCollided") {
            GoBackToPool();
            return;
        }

        if (collision.gameObject.GetComponent<IEnemy>() != null) {
            collision.gameObject.GetComponent<IEnemy>().TakeDamage(turretData.Damage);
            GoBackToPool();
            return;
        }
    }
    
    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        if (gameObject.activeSelf) {
            GoBackToPool();
        }
    }
    #endregion
    

    #region Custom Methods
    public void FireBullet(Vector3 target) {
        fireCoroutine = StartCoroutine(FireBulletNumerator((target - gameObject.transform.position)));
    }

    public void GoBackToPool() {
        StopCoroutine(fireCoroutine);
        ObjectPooler.Instance.ReturnObjectToPool(turretData.TurretBullet.ItemName,gameObject);
    }

    IEnumerator FireBulletNumerator(Vector3 direction) {
        while (true) {
            transform.Translate(direction*Time.deltaTime*speed, Space.World);
            yield return null;
        }
    }
    #endregion
    

    
}
