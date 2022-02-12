using UnityEngine;
using DG.Tweening;
public class PlaceableGrid : MonoBehaviour,ISelectableGrid {
    
    #region PublicFields
    public bool IsAvailable {
            get { return isAvailable; }
            set { isAvailable = value; }
        }

    public Color InitialColor {
        get { return initialColor; }
    }
    #endregion

    #region Private Fields

    private bool isAvailable;
    private MaterialPropertyBlock materialPropertyBlock;
    private MeshRenderer meshRenderer;
    private Color initialColor;
    private Vector3 initialScale;

    #endregion

    #region MonoBehaviour Callbacks
    void Start()
    {
        InitializeVariables();
    }

    // Update is called once per frame
    private void OnMouseEnter() {
        if (isAvailable) {
            SetMaterialColor(Color.green);
        }
        else {
            SetMaterialColor(Color.red);
        }
            
        
    }

    private void OnMouseExit() {
        SetMaterialColor(initialColor);
    }
    

    #endregion

    #region Custom Public Methods

    public void OnTurretPlacement() {
        SetMaterialColor(initialColor);
        gameObject.transform.DOScale(initialScale + new Vector3(0.25f, 0f, 0.25f), 0.05f).OnComplete(
            () => { gameObject.transform.DOScale(initialScale, 0.05f); }
        );

    }

    #endregion

    #region Custom Private Methods

    void SetMaterialColor(Color color) {
        
        //This method of changing material color preferred because it reduces Material Memory used.
        materialPropertyBlock.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    void InitializeVariables() {
        isAvailable = true;
        initialScale = transform.localScale;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(materialPropertyBlock);
        initialColor = meshRenderer.material.color;
    }

    #endregion
    

    
}
