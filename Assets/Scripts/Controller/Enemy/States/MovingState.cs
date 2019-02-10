using UnityEngine;
using System.Collections;

public class MovingState : FSMState<EnemyController>
{
    private static MovingState instance = null;
    public static MovingState Instance()
    {
        {
            if (instance == null)
                instance = new MovingState();

            return instance;
        }
    }

    private MovingState() { }
    AnimatorStateInfo info;
    bool clipInfo;

    public override void Enter(EnemyController entity)
    {
        entity.SetSpeed(1);
        entity.ChangeAnimation("Walk", 1);
        entity.ChangeAnimation("Run", 0);
    }

    public override void Execute(EnemyController entity)
    {
        if (entity.wallIsRemoved)
        {
            entity.SetPoint(entity.wallRemovedId.transform.position);
            entity.wallIsRemoved = false;
        }
        else
        {

            info = entity.GetAnimatorStateInfo(0);
            if (!entity.agent.pathPending && entity.agent.remainingDistance < 0.5f)
                entity.GotoNextPoint();

            // Detect player's scent
            GameObject nearestSmell = FindSmellPoints.FindSmell(
            entity.GetPosition(), "playerScent", 30);
            if (nearestSmell != null)
            {
                entity.FiniteStateMachine.ChangeState(
                    ChasingState.Instance(nearestSmell));
            }
        }
    }

    public override void Exit(EnemyController entity)
    {
       
    }

}
