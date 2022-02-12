using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// This class will be used for creating grids
/// If one want, with the help of this class s/he may
/// Change the row and/or column count and thus modify the game
/// </summary>
[ExecuteInEditMode]
public class GridCreator : MonoBehaviour {
    [SerializeField] private GridPreferences gridPrefs;

    [SerializeField] private GameObject GridContainer;
    // Start is called before the first frame update
    void Start() {
          CreateGrids();
    }

    void CreateGrids() {
        GameObject tempGridBlock;
        Vector3 tempInstantiatePosition = Vector3.zero;
        for (int z = 0; z < gridPrefs.Rows; z++) {
            for (int x = 0; x < gridPrefs.Columns; x++) {
                
                tempGridBlock = Instantiate(gridPrefs.GridBlock,GridContainer.transform);
                tempGridBlock.transform.position = new Vector3(
                    ((x - gridPrefs.Columns / 2) * gridPrefs.SpacingX + gridPrefs.SpacingX / 2),
                    0f,
                    ((z - gridPrefs.Rows / 2) * gridPrefs.SpacingZ + gridPrefs.SpacingZ / 2));

                if (z < 4) {
                    tempGridBlock.AddComponent<PlaceableGrid>();
                }

                tempGridBlock.layer = LayerMask.NameToLayer("Grid");

            }
        } 
    }
}
