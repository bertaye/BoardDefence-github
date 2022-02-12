using UnityEngine;

public class BoundaryManager : MonoBehaviour {

    #region CONSTANTS
    
    //this offset is set to prevent collision with pool objects
    private const float BOUNDARY_OFFSET = 0f;

    #endregion

    #region Private Fields

    private GameObject topCenter, bottomCenter, leftCenter, rightCenter, groundCenter;
    private float sizeX, sizeY, ratio;

    private float _xMax, _zMax;

    #endregion

    #region Read-only Properties

    public float xMax {
        get { return _xMax - BOUNDARY_OFFSET; }
    }
    public float zMax {
        get { return _zMax - BOUNDARY_OFFSET; }
    }

    #endregion
    // Start is called before the first frame update
    void Start() {
        
        CreateColliders();
        
        CalculateScreenBounds();

        PlaceColliders();
        
    }

    void CreateColliders() {
        topCenter = new GameObject();
        bottomCenter = new GameObject();
        leftCenter = new GameObject();
        rightCenter = new GameObject();
        groundCenter = new GameObject();
        
        topCenter.transform.SetParent(gameObject.transform);
        bottomCenter.transform.SetParent(gameObject.transform);
        leftCenter.transform.SetParent(gameObject.transform);
        rightCenter.transform.SetParent(gameObject.transform);
        groundCenter.transform.SetParent(gameObject.transform);

        topCenter.tag = "DestroyCollided";
        bottomCenter.tag = "DestroyCollided";
        leftCenter.tag = "DestroyCollided";
        rightCenter.tag = "DestroyCollided";
        groundCenter.tag = "DestroyCollided";

        topCenter.layer = LayerMask.NameToLayer("Ignore Raycast");
        bottomCenter.layer = LayerMask.NameToLayer("Ignore Raycast");
        leftCenter.layer = LayerMask.NameToLayer("Ignore Raycast");
        rightCenter.layer = LayerMask.NameToLayer("Ignore Raycast");
        groundCenter.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        topCenter.AddComponent<BoxCollider>();
        bottomCenter.AddComponent<BoxCollider>();
        leftCenter.AddComponent<BoxCollider>();
        rightCenter.AddComponent<BoxCollider>();
        groundCenter.AddComponent<BoxCollider>();
    }
    void CalculateScreenBounds() {
        sizeY = Camera.main.orthographicSize * 2;
        ratio = (float) Screen.width / (float) Screen.height;
        sizeX = sizeY * ratio;
        _xMax = sizeX / 2;
        _zMax = sizeY / 2;
        _xMax += BOUNDARY_OFFSET;
        _zMax += BOUNDARY_OFFSET;
    }

    void PlaceColliders() {
        topCenter.transform.position = new Vector3(0, 0, _zMax);
        topCenter.transform.localScale = new Vector3 (_xMax*2,3,1);

        bottomCenter.transform.position = new Vector3(0, 0, -_zMax);
        bottomCenter.transform.localScale = new Vector3(_xMax * 2, 3, 1);

        rightCenter.transform.position = new Vector3(_xMax, 0, 0);
        rightCenter.transform.localScale = new Vector3(1, 3, _zMax * 2);
        
        leftCenter.transform.position = new Vector3(-_xMax, 0, 0);
        leftCenter.transform.localScale = new Vector3(1, 3, _zMax * 2);
        
        groundCenter.transform.position = new Vector3(0, -3/2, 0);
        groundCenter.transform.localScale = new Vector3(xMax*2, 1, _zMax * 2);
    }
}