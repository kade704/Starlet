using UnityEngine;

[RequireComponent(typeof(Module))]
public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 23.0f;
    [SerializeField] private float speed = 20.0f;
    [SerializeField] private float distance = 30.0f;
    [SerializeField] private BulletType bulletType = BulletType.Normal;
    [SerializeField] private Color trailColor = Color.white;
    [SerializeField] private float reactionForce = 110.0f;
    [SerializeField] private float energyUse = 50;
    [SerializeField] private SignalProperty signalInput;
    [SerializeField] private Transform firePoint;

    private GameObject bulletPrefab;
    private Module module;

    private void Awake()
    {
        module = GetComponent<Module>();
        module.RegisterProperty(signalInput);

        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        firePoint = transform.Find("FirePoint");
    }

    private void LateUpdate()
    {
        if (!module.IsRunning()) return;

        var signalValue = module.spacecraft.GetSignalValue(signalInput.value);

        var energyEnough = module.spacecraft.HasEnergy(energyUse);

        if (signalValue >= 1 && energyEnough)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (module.IsCoolTimeWaiting()) return;
        module.ResetCoolTime();

        module.spacecraft.UseEnergy(energyUse);

        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Bullet>();
        bullet.Initialize(bulletType, damage, speed, distance, trailColor);
        Destroy(bullet.gameObject, 5f);
        
        module.spacecraft.rigidbody.AddForceAtPosition(-transform.up * reactionForce, transform.position);
    }
}
