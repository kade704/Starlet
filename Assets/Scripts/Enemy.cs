using Newtonsoft.Json.Linq;
using UnityEngine;

public enum EnemyType { Fighter, Fugitive }

[RequireComponent(typeof(Spacecraft))]
public class Enemy : MonoBehaviour
{
    private Spacecraft spacecraft;
    private Gun[] guns;
    private EnemyType type;
    private EnemyState enemyState;
    private EnemyState globalState;
    private Spacecraft target;
    public Spacecraft Target
    {
        get => target;
        set => target = value;
    }

    public EnemyType Type => type;
    public Spacecraft Spacecraft => spacecraft;

    private void Awake()
    {
        spacecraft = GetComponent<Spacecraft>();
        guns = GetComponentsInChildren<Gun>();
    }

    public void Initialize(EnemyType type, JArray spacecraftData, Vector2 position, Quaternion rotation)
    {
        this.type = type;
        this.spacecraft.Deserialize(spacecraftData);
        this.transform.position = position;
        this.transform.rotation = rotation;
    }

    private void Start()
    {
        globalState = new EnemyGlobalState(this);
        globalState.Begin();

        enemyState = new EnemyWanderState(this);
        enemyState.Begin();
    }

    private void Update()
    {
        var playerDist = Vector2.Distance(transform.position, Camera.main.transform.position);
        if (playerDist > 60)
        {
            Destroy(gameObject);
            return;
        }

        enemyState.Update();
        globalState.Update();
    }

    public EnemyState GetState()
    {
        return enemyState;
    }

    public void ChangeState(EnemyState newState)
    {
        enemyState.End();
        enemyState = newState;
        enemyState.Begin();
    }

    public void Aim(Vector2 targetPos, bool move)
    {
        var targetDir = (targetPos - (Vector2)transform.position).normalized;
        var targetDist = Vector2.Distance(transform.position, targetPos);
        var targetAngle = Vector2.SignedAngle(transform.up, targetDir);

        var ccw = Mathf.Clamp(targetAngle, 0.0f, 90.0f) / 90.0f;
        spacecraft.SetSignalValue("A", ccw);

        var cw = Mathf.Clamp(targetAngle, -90.0f, 0.0f) / -90.0f;
        spacecraft.SetSignalValue("B", cw);

        if (move)
        {
            var boost = Mathf.Abs(targetAngle) < Mathf.Clamp(targetDist * 10, 5, 50);
            spacecraft.SetSignalValue("C", boost ? 0.5f : 0);
            Debug.DrawLine(transform.position, targetPos, Color.white);
        }
    }
}