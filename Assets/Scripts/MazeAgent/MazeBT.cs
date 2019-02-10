using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//using UnitySteer.Behaviors;

using FluentBehaviourTree;

public class MazeBT : MonoBehaviour
{
    private bool verbose = false;
    private IBehaviourTreeNode tree;
    private float enemyKillPlayerDistance = 1f;
    private float helpTimeInterval = 10f;
    private float playerLostTime = 10f;
    private float playerAdvancedTime = 0.5f;
    private float timepassed = 0f;
    private Transform EndGate;  // Save EndGate Position
    private Transform player1; // Save Player Info
    private Transform enemy1; // Save Enemy Info
    private float MoveWallDistance = 5f; //wall will be moved up. Distance of the displacment
    private float currentWallLerptime = 0;  // time to control wall movement
    private float wallLerptime = 5; // time to control wall movement
    private bool isWallCoroutineStarted = false;
    private float previousposition;
    RemoveFirstWall removeforplayer;
    RemoveFirstWallEnemy removeforenemy;
    GameObject canvas;
    Animator gameoveranim;
    restartButton rbutton;
    bool gameisover = false;
    string name = "EndMenu";
    
        


    // Use this for initialization
    void Start()
    {
        if (this.verbose) Debug.Log("MazeBT start");
        //Debug.Break();
        GameObject eg = GameObject.Find("End");
        GameObject pl = GameObject.Find("Player");
        GameObject en = GameObject.Find("Enemy");
        this.EndGate = eg.transform;
        this.player1 = pl.transform;
        this.enemy1 = en.transform;
        this.canvas = GameObject.Find("Canvas");
        GameObject restart_b = GameObject.Find("RestartButton");
        this.rbutton = restart_b.GetComponent<restartButton>();

        this.gameoveranim = this.canvas.GetComponent<Animator>();

        this.removeforplayer = GetComponent<RemoveFirstWall>();
        this.removeforenemy = GetComponent<RemoveFirstWallEnemy>();
        this.removeforplayer.enabled = false;
        this.removeforenemy.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        this.previousposition = GoalPlayerDistance();
        // Building the tree
        var builder = new BehaviourTreeBuilder();

        this.tree = builder
         .Sequence("Game")
            .Condition("IsGameON", t => IsGameON())
            .Selector("GameON")
                .Sequence("KillPlayerEndGame")
                    .Condition("EnemyReachedPlayer", t => EnemyIsWithPlayer())
                    .Do("KillThePlayer", t => KillPlayer())
                    .Do("EndOfGame", t => EndGame())
                .End()
                .Sequence("AdaptDifficulty")
                    .Condition("HelpTime", t => IsTimeToHelp())
                    .Selector("WhoToHelp")
                        .Sequence("HelpPlayer")
                            .Condition("PlayerProgressSlow", t => PlayerIsLost())
                            .Condition("EnemyCloserToPlayer", t => EnemyIsCloserToPlayer())
                            .Do("RemoveWallForPlayer", t => RemoveWallForPlayer())
                        .End()
                        .Sequence("HelpEnemy")
                            .Condition("PlayerProgressFast", t => PlayerReachingGoalFast())
                            .Do("RemoveWallForEnemy", t => RemoveWallForEnemy())
                        .End()
                    .End()
                .End()
            .End()
          .End()
          .Build();
    }
    // Update is called once per frame
    void Update()
    {
         if (this.verbose) Debug.Log("MazeBT update");
        //Debug.Break();
        var delta = new TimeData(Time.deltaTime);
        this.timepassed += delta.deltaTime;
        this.tree.Tick(new TimeData(Time.deltaTime));

    }



    /////////    CONDITIONS  ////////
    bool IsGameON(){
        //Debug.Log("Is the game on " + !this.gameisover); 
        return !this.gameisover;
    }
    bool EnemyIsWithPlayer()
    {
        if (EnemyPlayerDistance() <= this.enemyKillPlayerDistance)
        {
             if (this.verbose) Debug.Log("------------------- MazeBT Enemy is with player " + EnemyPlayerDistance()) ;
            //Debug.Break();
            return true;
        }
         if (this.verbose) Debug.Log("------------------- MazeBT Enemy is not with player " + EnemyPlayerDistance());
        //Debug.Break();
        return false;
    }

    bool IsTimeToHelp()
    {
        if (this.timepassed >= this.helpTimeInterval)
        {
            this.timepassed = 0;
             if (this.verbose) Debug.Log("------------------- MazeBT it is time to help");
            // Debug.Break();
            return true;
        }
         if (this.verbose) Debug.Log("-------------------  MazeBT it is NOT time to help");
        // Debug.Break();
        return false;
    }

    bool PlayerIsLost()
    {
        if (this.verbose) Debug.Log("------------------ MazeBT  distance to interval time ratio " + ( this.previousposition - GoalPlayerDistance()) / helpTimeInterval );
        if (this.verbose) Debug.Log("------------------ MazeBT Time  " + Time.time );
        if (this.verbose) Debug.Log("------------------ MazeBT Distance  " + GoalPlayerDistance()  );
        if (this.verbose) Debug.Log("------------------ MazeBT Advanced Distance  " + ( this.previousposition - GoalPlayerDistance()));
        //Debug.Break();
        if (Time.time > 19 && ( (this.previousposition - GoalPlayerDistance()) < -10 || (Mathf.Abs(this.previousposition-GoalPlayerDistance()) / helpTimeInterval  < 0.15) )) {
            if (this.verbose) Debug.Log("------------------ MazeBT Player is lost ");
            this.previousposition = GoalPlayerDistance();
            return true;
        }
        if (this.verbose) Debug.Log("------------------ MazeBT Player is NOT lost ");
        //Debug.Break();
        return false;
    }
    bool PlayerReachingGoalFast()
    {
        if (this.verbose) Debug.Log("------------------ MazeBT  distance to interval time ratio " + ( this.previousposition - GoalPlayerDistance()) / helpTimeInterval );
        if (this.verbose) Debug.Log("------------------ MazeBT Time  " + Time.time );
        if (this.verbose) Debug.Log("------------------ MazeBT Distance  " + GoalPlayerDistance()  );
        if (this.verbose) Debug.Log("------------------ MazeBT Advanced Distance  " + ( this.previousposition - GoalPlayerDistance() ) );
        if ((this.previousposition - GoalPlayerDistance()) / helpTimeInterval  > 1){
            if (this.verbose) Debug.Log("------------------ MazeBT Player is moving fast ");
            this.previousposition = GoalPlayerDistance();
            //Debug.Break();
            return true;
        }
        if (this.verbose) Debug.Log("------------------ MazeBT Player is NOT moving fast ");
        this.previousposition = GoalPlayerDistance();
        //Debug.Break();
        return false;
    }

    bool EnemyIsCloserToPlayer()
    {
        float ed = EnemyPlayerDistance();
        float eg = GoalPlayerDistance();
        if (this.verbose) Debug.Log(ed);
        if (this.verbose) Debug.Log(eg);
        if (EnemyPlayerDistance() <= GoalPlayerDistance())
        {
            if (this.verbose) Debug.Log("------------------- MazeBT Enemy is closer Enemy   Goal");
            // Debug.Break();
            return true;
        }
        if (this.verbose) Debug.Log("------------------- MazeBT Enemy is NOT closer Enemy   Goal");
        //     Debug.Break();
        return false;

    }


    /////////    ACTIONS  ////////

    BehaviourTreeStatus KillPlayer()
    {
        // Kill player Code 
        //if (this.verbose) 
        Debug.Log("------------------- MazeBT Kill the player");
        //Destroy(GameObject.Find("Player"));
        //Destroy(GameObject.Find("Enemy"));

        return BehaviourTreeStatus.Success;
    }
    BehaviourTreeStatus EndGame()
    {
        // End Game Code 
        //if (this.verbose) 
        Debug.Log("--------------------- Maze BT End Game");
        //gameoveranim.enabled = true;

        SceneManager.LoadScene(name);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        this.gameisover = true;
        //this.rbutton.restartGame("NewMaze");

        return BehaviourTreeStatus.Success;
    }
    BehaviourTreeStatus RemoveWallForPlayer()
    {
        if (this.verbose) Debug.Log("------------------- Maze BT Removing Wall For player");
        //Debug.Break();
        if (!this.isWallCoroutineStarted)
            {
                StartCoroutine(RemoveWallBTPlayer());
            }
        
        return BehaviourTreeStatus.Success;
    }
    

    BehaviourTreeStatus RemoveWallForEnemy()
    {
        if (this.verbose) Debug.Log("------------------- Maze BT Removing Wall For enemy");
        //Debug.Break();
        if (!this.isWallCoroutineStarted)
            {
                StartCoroutine(RemoveWallBTEnemy());
            }
        
        return BehaviourTreeStatus.Success;
        

    }

    float EnemyPlayerDistance()
    {
        float distance_enemy_palyer = Vector3.Distance(this.player1.position, this.enemy1.position);
        return distance_enemy_palyer;

    }
    float GoalPlayerDistance()
    {
        Vector3 distance_endgate11 = this.EndGate.position;
        Vector3 distance_endgate12 = this.player1.position;
        float distance_endgate = Vector3.Distance(this.player1.position, this.EndGate.position);
        return distance_endgate;
    }

    IEnumerator RemoveWallBTPlayer()
    {

        this.isWallCoroutineStarted = true;
        removeforplayer.enabled = true;
        if (this.verbose) Debug.Log("-------------------------    MazeBT RemoveWallBTPlayer ENABLED");
        yield return new WaitForSeconds(10);
        //Debug.Break();
        this.isWallCoroutineStarted = false;
        if (this.verbose) Debug.Log("-------  DO I REACH THIS POINT??????  MazeBT RemoveWallBTPlayer: wall destroyed");

        }

    IEnumerator RemoveWallBTEnemy()
        {

        this.isWallCoroutineStarted = true;
        removeforenemy.enabled = true;
        if (this.verbose) Debug.Log("-------------------------    MazeBT RemoveWallBTEnemy ENABLED");
        //Debug.Break();
        yield return new WaitForSeconds(10);
        //Debug.Break();
        this.isWallCoroutineStarted = false;
        if (this.verbose) Debug.Log("-------  DO I REACH THIS POINT??????  MazeBT RemoveWallBTEnemy: wall destroyed");

    }
}