using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class will handle cooldown of the turrets/defense items
/// </summary>
public class TurretButton : MonoBehaviour, ITurretButton {
    
    #region Private Fields

    [SerializeField] private Image fillImage;
    [SerializeField] private TurretData turretData; //To get the cooldown time
    [SerializeField] private PoolableObject pooledTurretData;
    [SerializeField] private LevelData levelData;
    [SerializeField] private TMPro.TMP_Text leftCountText;
    private float timePassed = 0f;
    private int currentSpawnedTurret = 0; //This value will be used to limit turret count, as restricted within LevelData 
    private int maxAllowedTurret;
    #endregion

    #region Public Fields
    public PoolableObject PooledTurretData {
        get { return pooledTurretData; }
    }

    #endregion

    #region MonoBehaviour Callbacks

    private void Start() {
        InitializeVariables();
    }

    #endregion

    #region Custom Public Methods
    
    //Freeze will be called immediately after button is clicked.
    public void Freeze() {
        timePassed = 0f;
        gameObject.GetComponent<Button>().enabled = false;
        fillImage.fillAmount = 1;
    }

    //TurretPlaced will be called from PlayerInputHandler
    //if this turred is placed to a grid
    public void TurretPlaced() {
        currentSpawnedTurret++;
        UpdateText();
        if (currentSpawnedTurret == maxAllowedTurret) {
            Freeze();
            return;
        }
        StartCoroutine(CooldownEffect(turretData.CoolDownTime));
    }
    
    //SetFree function will be mainly called if user switches to another turret
    //before placing this turret
    public void SetFree() {
        if (currentSpawnedTurret == maxAllowedTurret) {
            return; //if we reach the limit, button cant set free
        }
        gameObject.GetComponent<Button>().enabled = true;
        fillImage.fillAmount = 0;
    }

    #endregion

    #region Custom Private Methods
    void InitializeVariables() {
        foreach (LevelElement le in levelData.ObjectsAndAmounts) {
            if (le._poolableObject.ItemName == pooledTurretData.ItemName) {
                maxAllowedTurret = le.maxAmount;
                break; //No need to search deeper, to reduce time complexity
            }
        }
        leftCountText.text = "x" + (maxAllowedTurret - currentSpawnedTurret).ToString();
    }

    void UpdateText() {
        leftCountText.text = "x" + (maxAllowedTurret - currentSpawnedTurret).ToString();
    }
    

    #endregion

    #region Coroutines

    private IEnumerator CooldownEffect(float time) {
        Freeze(); //We dont want user to be able to press the button while cooldown is on progress.
        while (timePassed < time) {
            fillImage.fillAmount = 1 - (timePassed) / time;
            timePassed += Time.deltaTime;
            yield return null;
        }
        gameObject.GetComponent<Button>().enabled = true;
        SetFree();
    }

    #endregion
    

    
}
