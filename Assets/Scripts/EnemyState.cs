using UnityEngine;

public class EnemyState
{
    public Enemy owner;

    public EnemyState(Enemy enemy)
    {
        owner = enemy;
    }

    public virtual void Begin() { }
    public virtual void Update() { }
    public virtual void End() { }
}

public class EnemyGlobalState : EnemyState
{
    public EnemyGlobalState(Enemy enemy) : base(enemy) { }

    public override void Update()
    {
        var layerMask = LayerMask.GetMask("Module", "Ore");

        var hit = Physics2D.Raycast(owner.transform.position, owner.transform.up, 20.0f, layerMask);
        if (hit.collider != null)
        {
            owner.Spacecraft.SetSignalValue("C", 0, true);
            owner.Spacecraft.SetSignalValue("D", 1, true);
        }
    }
}

public class EnemyWanderState : EnemyState
{
    private const float searchDuration = 1.0f;

    private Vector2 targetPos;
    private float nextSearchTime;
    public EnemyWanderState(Enemy enemy) : base(enemy) { }

    public override void Begin()
    {
        targetPos = GetAroundRandomPos();
        nextSearchTime = Time.time + searchDuration;
    }

    public override void Update()
    {
        if (Time.time > nextSearchTime)
        {
            nextSearchTime = Time.time + searchDuration;

            var newTarget = GetClosestTarget();
            if (newTarget != null)
            {
                if (owner.Type == EnemyType.Fighter)
                {
                    owner.Target = newTarget;
                    owner.ChangeState(new EnemySeekState(owner));
                }
                else if (owner.Type == EnemyType.Fugitive)
                {
                    owner.Target = newTarget;
                    owner.ChangeState(new EnemyFleeState(owner));
                }
            }

            return;
        }

        if (Vector2.Distance(owner.transform.position, targetPos) < 5.0f)
        {
            targetPos = GetAroundRandomPos();
        }
        owner.Aim(targetPos, true);
    }

    private Spacecraft GetClosestTarget()
    {
        Spacecraft closest = null;
        float closestDist = float.MaxValue;

        var spacecrafts = GameObject.FindObjectsOfType<Spacecraft>();
        foreach (var spacecraft in spacecrafts)
        {
            if (spacecraft == owner.Spacecraft) continue;

            var dist = Vector2.Distance(owner.transform.position, spacecraft.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = spacecraft;
            }
        }

        if (closest == null || closestDist > 20)
        {
            return null;
        }

        return closest;
    }

    private Vector2 GetAroundRandomPos()
    {
        var randomOffset = (Vector2)Random.onUnitSphere * Random.Range(20, 50);
        return (Vector2)owner.transform.position + randomOffset;
    }
}

public class EnemySeekState : EnemyState 
{
    public EnemySeekState(Enemy enemy) : base(enemy) { }

    public override void Update()
    {
        if (owner.Target == null)
        {
            owner.ChangeState(new EnemyWanderState(owner));
        }

        var targetDist = Vector2.Distance(owner.transform.position, owner.Target.transform.position);
        if (targetDist > 40)
        {
            owner.ChangeState(new EnemyWanderState(owner));
            return;
        }
        
        var move = targetDist > 10;
        owner.Aim(owner.Target.transform.position, move);
    }
}

public class EnemyFleeState : EnemyState
{
    public EnemyFleeState(Enemy enemy) : base(enemy) { }

    public override void Update()
    {
        if (owner.Target == null)
        {
            owner.ChangeState(new EnemyWanderState(owner));
        }

        var targetDist = Vector2.Distance(owner.transform.position, owner.Target.transform.position);
        if (targetDist > 40)
        {
            owner.ChangeState(new EnemyWanderState(owner));
        }
        
        var targetDir = ((Vector2)owner.Target.transform.position - (Vector2)owner.transform.position).normalized;
        owner.Aim((Vector2)owner.transform.position - targetDir * 20, true);
    }
}