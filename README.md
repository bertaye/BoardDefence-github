# DESIGNER'S MANUAL
# **HOW TO PLAY GAME**
 ```
  Open StartScene, hit Play button in editor.
```
## **HOW TO ADD A NEW ENEMY/TOWER(turret)?**
```
  You simply go to Game Data/Enemies folder; create New EnemyData; then do the same for creating a PoolableObject of the Enemy.
  Then open de ObjectPooler prefab, add the PoolableObject of the Enemy. 
  Follow same steps for the Tower(Turret).
 ```
## **HOW TO DESIGN LEVELS**
``` 
  You can duplicate "BaseScene" scene in  Scenes folder; than go to Game Data/LevelData directory and create New LevelData;
  add the objects and amounts for that level to this data. Then, drag and drop this data object to your new scenes following objects
      -UI & Input/Canvas/LayoutGroup/Turret_1 
      -UI & Input/Canvas/LayoutGroup/Turret_2
      -UI & Input/Canvas/LayoutGroup/Turret_3
      -EnemySpawner
  After that dont forget to add your level scene to the Scenes in Build panel.
```  
  
## **HOW TO CREATE NEW LEVELS WITH DIFFERENT GRID**
  ```
    Go to Game Data/Grid folder; create new GridPreferences; customize your preferences. Then,
    Duplicate the "BaseScene" scene; select GridCreator object from hierarchy, on the inspector drag and drop your new
    GridPreferences to GridCreator script and activate it; DONT FORGET to deactivate it after your new grid is created
    and deleting the old grids!
    Then, drag and drop your new GridPreferences to Enemy and Turret prefabs; so that Turrets can create
    their colliders for that grid prefs and
    Enemies can know where to stop.
    PS: You should use same GridPreferences through all game.
 ```
## **HOW TO CHANGE TURRET BULLETS**
 ```
    Create new PoolableObject inside Game Data/PoolableObjects; customize your new bullet; drag and drop this bullet 
    to corresponding TurretData objects
    under Game Data/Turrets folder.
 ```
## **OTHER CUSTOMIZATIONS**
 ```
  -You can change each bullets' speed from their prefabs. Just find the "BulletController" script and adjust speed.
  
  -You can change shooting frequency of each turret, just open that turrets' prefab and find the "TurretController" script, adjust TimeBetweenShots.
```
