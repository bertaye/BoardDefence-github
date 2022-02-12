using UnityEngine;

public interface IBullet
{
    public TurretData turretData { get; set; }
    void FireBullet(Vector3 target);
    void GoBackToPool();

}
