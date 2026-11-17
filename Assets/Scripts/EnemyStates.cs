using UnityEngine;

public interface IEnemyState
{
    void Enter(Enemy e);
    void Tick(Enemy e);
}

public class IdleState : IEnemyState
{
    public void Enter(Enemy e) { e.agent.ResetPath(); }

    public void Tick(Enemy e)
    {
        e.idleTimeCounter -= Time.deltaTime;
        if (e.idleTimeCounter < 0)
        {
            e.idleTimeCounter = e.idleTime;
            e.ChangeState(e.patrollingState);
        }
    }
}

public class PatrollingState : IEnemyState
{
    public void Enter(Enemy e) { }

    public void Tick(Enemy e)
    {
        if (Vector3.Distance(e.currentTarget, e.transform.position) < e.positionThreshhold)
        {
            if (Random.Range(0, 100) < 10)
            {
                e.ChangeState(e.idleState);
                return;
            }
            e.currentPointIndex++;
            e.currentTarget = e.patrolPoints[e.currentPointIndex % e.patrolPoints.Length].position;
        }
        else
        {
            e.agent.SetDestination(e.currentTarget);
        }
    }
}

public class ChasingState : IEnemyState
{
    public void Enter(Enemy e) { e.idleTimeCounter = e.idleTime; }

    public void Tick(Enemy e)
    {
        e.agent.SetDestination(e.lastKnownPlayerPosition);
        float dist = Vector3.Distance(e.transform.position, e.playerTransform.position);

        if (e.health < e.minChasingHealth)
            e.ChangeState(e.patrollingState);
        else if (dist <= e.attackDistance && e.canSeePlayer)
            e.ChangeState(e.attackingState);
        else if (dist > e.maxVisionDistance)
            e.ChangeState(e.patrollingState);
        else if (dist < e.positionThreshhold && !e.canSeePlayer)
            e.ChangeState(e.patrollingState);
    }
}

public class AttackingState : IEnemyState
{
    public void Enter(Enemy e)
    {
        e.idleTimeCounter = e.idleTime;
        e.agent.ResetPath();
    }

    public void Tick(Enemy e)
    {
        e.Shoot();
        float dist = Vector3.Distance(e.transform.position, e.playerTransform.position);

        if (dist > e.attackDistance || !e.canSeePlayer)
        {
            if (e.health > e.minChasingHealth) e.ChangeState(e.patrollingState);
            else e.ChangeState(e.chasingState);
        }
    }
}