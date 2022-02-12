using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputHandler : MonoBehaviour {

    #region Private Fields

    [SerializeField] private LayerMask layerMask; //This is to filter RaycastHit with only Grid Objects
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject selectedTurret;
    private GameObject selectedGrid;
    private TurretButton lastTurretButton;
    private string lastTurretKey;
    #endregion

    #region Public Fiels

    public GameObject SelectedTurret {
            get { return selectedTurret; }
            set { selectedTurret = value; }
        } //This is the object user wants to place on grid

    #endregion

    #region MonoBehaviour Callbacks
    private void Awake() {    
    
        if (mainCam == null) {
            #if UNITY_EDITOR
            /*
            //Since we are pooling our objects, there will be too many
            //objects in scene. Camera.main works as GameObject.Find, so 
            //this will be a problem for our performance. 
            //This is why we will log a warning, to remember to attach camera.*/
            Debug.LogWarning("Main Camera did not attached!");
            #endif
            mainCam = Camera.main;
        }
        layerMask = LayerMask.GetMask("Grid");
    }
    // Update is called once per frame
    void Update()
    {
        ReadInput();
    }
    

    #endregion

    #region Custom Public Methods

    public void TurretSelectButton(TurretButton selectedTurretButton) {
        
        //If last selection button still exists and different from the new one, we must return our pooled object
        //and set free to previous button
        if (lastTurretButton != selectedTurretButton && lastTurretButton!=null) {
            lastTurretButton.SetFree();
            ObjectPooler.Instance.ReturnObjectToPool(lastTurretKey,selectedTurret);
        }
        selectedTurret = ObjectPooler.Instance.GetObjectFromPool(selectedTurretButton.PooledTurretData.ItemName);
        lastTurretButton = selectedTurretButton; //With this, we can control easily if user changes his/her turret choice
        lastTurretKey = selectedTurretButton.PooledTurretData.ItemName;
    }

    #endregion

    #region Custom Private Methods
    void ReadInput() {
        
        //We want to cast a ray only if user selected a turret from UI Buttons. 
        if (selectedTurret==null)
            return;
        var ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,layerMask)) {
            if (hit.collider.gameObject.GetComponent<ISelectableGrid>() != null) {
                if (!hit.collider.gameObject.GetComponent<PlaceableGrid>().IsAvailable) {
                    return;
                }
                selectedGrid = hit.collider.gameObject;
                ShowObject(selectedGrid.transform.position + Vector3.up*selectedGrid.transform.localScale.y/2);
                
                //If user clicked to mouse button while hovering on a selectable grid,
                //We will place the object
                if (Input.GetMouseButtonDown(0)) {
                    PlaceTurret(selectedGrid);
                }
            }
        }
        else {
            HideObject();
        }
    }
    
    /// <summary>
    /// This function will show the  turret on the current grid user is hovering.
    /// </summary>
    /// <param name="position"></param>
    void ShowObject(Vector3 position) {
        if (selectedTurret.activeInHierarchy) {
            return;
        }
        selectedTurret.transform.position = position;
        selectedTurret.SetActive(true);
    }

    /// <summary>
    /// This function stops showing the turret on any grid.
    /// </summary>
    void HideObject() {

        if (!selectedTurret.activeInHierarchy) {
            return;
        }
        selectedTurret.SetActive(false);
    }

    void PlaceTurret(GameObject placedGrid) {
        selectedTurret.transform.position = placedGrid.transform.position + Vector3.up*placedGrid.transform.localScale.y/2;
        placedGrid.GetComponent<PlaceableGrid>().IsAvailable = false;
        placedGrid.GetComponent<PlaceableGrid>().OnTurretPlacement();
        lastTurretButton.TurretPlaced();
        selectedTurret.GetComponent<TurretController>().ActivateTurret();
        lastTurretButton = null;
        selectedTurret = null; //to prevent any unintended modification, we will free the object
    }
    

    #endregion
    
    
    

    
}
