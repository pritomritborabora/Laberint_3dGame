using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingState : FSMState<EnemyController>
{
    AnimatorStateInfo info;
    bool clipInfo;
    GameObject currentScent;

    private static ChasingState instance = null;
    public static ChasingState Instance(GameObject scent)
    {
        {
            if (instance == null)
                instance = new ChasingState();
            instance.currentScent = scent;
            return instance;
        }
    }

    private ChasingState(){ }

    public override void Enter(EnemyController entity)
    {
        entity.SetSpeed(3);
        entity.ChangeAnimation("Run", 1);
        entity.SetPoint(currentScent.transform.position);
    }

    public override void Execute(EnemyController entity)
    {
        if (entity.DistanceToPlayer() < 1)
        {
            entity.FiniteStateMachine.ChangeState(AttackingState.Instance());
        }

        if (entity.agent.remainingDistance < 0.1f)
        {
            entity.DestroyScent(currentScent);

        }
        GameObject nearestSmell = FindSmellPoints.FindSmell(entity.GetPosition(), "playerScent",
                30);
        if (nearestSmell == null)
        {
            entity.FiniteStateMachine.ChangeState(MovingState.Instance());
        }
        else
        {
            currentScent = nearestSmell;
            entity.SetPoint(currentScent.transform.position);
        }
    }

    public override void Exit(EnemyController entity)
    {

    }

}
