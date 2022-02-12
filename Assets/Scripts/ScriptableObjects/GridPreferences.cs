using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Grid Preferences", menuName = "Custom Elements/New Grid Preferences", order = 1)]
public class GridPreferences : ScriptableObject
{
   [Tooltip("This is the grid object to be instantiated.")]
   [SerializeField] private GameObject gridBlock;

   [InspectorName("Row Count")]
   [SerializeField] private int rows;

   [InspectorName("Column Count")]
   [SerializeField] private int columns;
   
   [SerializeField] private float spacingX = 1.25f;

   [SerializeField] private float spacingZ = 1.25f;
   
   
   
   
   
   

  public GameObject GridBlock {
      get { return gridBlock; }
  }
  
  public int Rows {
      get { return rows; }
  }

  public int Columns {
      get { return columns; }
  }
  
  public float SpacingX {
      get { return spacingX; }
  }

  public float SpacingZ {
      get { return spacingZ; }
  }
   
}
