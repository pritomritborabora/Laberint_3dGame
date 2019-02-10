using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : FSMState<EnemyController>
{
    private static AttackingState instance = null;
    public static AttackingState Instance()
    {
        {
            if (instance == null)
                instance = new AttackingState();

            return instance;
        }
    }
    public override void Enter(EnemyController entity)
    {

    }

    public override void Execute(EnemyController entity)
    {
        if (entity.DistanceToPlayer() > 1)
        {
            GameObject nearestSmell = FindSmellPoints.FindSmell(
                entity.GetPosition(), "playerScent", 30);
            entity.FiniteStateMachine.ChangeState(
                ChasingState.Instance(nearestSmell));
        }
        else
        {
            entity.SetSpeed(5);
            entity.SetPoint(
                GameObject.FindGameObjectsWithTag("Player")[0].transform.position);
        }
    }

    public override void Exit(EnemyController entity)
    {
        entity.ChangeAnimation("Run", 1);

    }

}

