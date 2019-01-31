using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FSM;
using UnityEngine.AI;

public class DrugAddict : MonoBehaviour
{
    public float movementSpeed;
    public float runSpeedMultiplier;

    DrugAddictAnimModule _anim;
    DrugAddictModelModule _model;
    LineOfSight _loS;
    NavMeshAgent _agent;
    Rigidbody _rb;
    public Transform player;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<DrugAddictAnimModule>();
        _model = GetComponent<DrugAddictModelModule>();
        _loS = GetComponent<LineOfSight>();
        player = FindObjectOfType<PlayerController>().transform;
    }

    void Start()
    {
        _agent.speed = movementSpeed;
        InitFsm();
    }

    #region FSM

    #region Variables
    #region State Conditions
    bool _canAttack = true;
    bool _canChase = true;
    #endregion

    //Follow
    public float maxFollowDistance;
    float _actualFollowDistance;

    //Chase
    public float maxChaseDistance;
    float _actualChaseDistance;
    public float maxChaseTime;
    float _actualChaseTime;
    public float chaseCooldown;
    public int initialChasePerc;
    int _chasePerc;

    //Attack
    public float attackCooldown;

    public float sightRange;
    public float attackRange;

    bool frontalHit = false;
    #endregion

    #region Properties
    private EventFSM<Inputs> _stateMachine;
    public enum Inputs { EnemyFound, EnemyLost, EnemyInAttackRange, Anger, Rest, Die };

    void InitFsm()
    {
        //-----------------------------------------STATE CREATE-------------------------------------------//
        var idle = new State<Inputs>("Idle");
        var follow = new State<Inputs>("Follow");
        var chase = new State<Inputs>("Chase");
        var attack = new State<Inputs>("AttackShort");
        var death = new State<Inputs>("Death");

        /*
        idle - No se mueve. Puede ser un descanso
	        EnemyFound -> follow
	        Anger => chase
	        Rest > patrol
         * */

        StateConfigurer.Create(idle)
            //.SetTransition(Inputs.EnemyFound, follow)
            .SetTransition(Inputs.EnemyFound, chase)
            //.SetTransition(Inputs.Anger, chase)
            .SetTransition(Inputs.Rest, idle)
            .SetTransition(Inputs.Die, death)
            .Done();

        /*
        follow(walk hacia jugador)
	    EnemyLost -> idle/patrol
	    Anger => chase
	    EnemyInAttackRange > attackShort*/

        StateConfigurer.Create(follow)
            .SetTransition(Inputs.EnemyLost, idle)
            .SetTransition(Inputs.Anger, chase)
            .SetTransition(Inputs.EnemyInAttackRange, attack)
            .SetTransition(Inputs.Die, death)
            .Done();

        /*chase(run hacia jugador)
        EnemyLost->idle
        Rest => follow
        enemyInAttackRange > attackLong*/

        StateConfigurer.Create(chase)
            .SetTransition(Inputs.EnemyLost, idle)
            .SetTransition(Inputs.Rest, chase)
            .SetTransition(Inputs.EnemyInAttackRange, attack)
            .SetTransition(Inputs.Die, death)
            .Done();

        /*
         attack(short de run, long de walk/idle)
	        rest => idle
         * */
        StateConfigurer.Create(attack)
           .SetTransition(Inputs.Rest, idle)
           .SetTransition(Inputs.Die, death)
           .Done();

        StateConfigurer.Create(death).Done();


        //-----------------------------------------STATE SET-------------------------------------------//
        // idle  > follow > chase > attackLong > Death //

        //idle
        idle.OnEnter += x =>
        {
            _anim.SetIdle();
            _agent.isStopped = true;
        };

        //follow
        follow.OnEnter += x =>
        {
            _agent.isStopped = false;
            _agent.SetDestination(player.transform.position);
            _anim.SetWalk();
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
            _agent.speed = movementSpeed;
        };

        follow.OnFixedUpdate += () =>
        {
            if (_canChase)
            {
                var rnd = UnityEngine.Random.Range(0, 100);
                if (rnd < _chasePerc)
                {
                    ProcessInput(Inputs.Anger);
                }
            }
            _agent.SetDestination(player.transform.position);
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
        };


        //chase
        chase.OnEnter += x =>
        {
            _agent.isStopped = false;
            _agent.SetDestination(player.transform.position);
            _anim.SetRun();
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
            _agent.speed = movementSpeed * runSpeedMultiplier;
        };

        chase.OnFixedUpdate += () =>
        {
            if (_actualChaseTime >= maxChaseTime)
            {
                _actualChaseDistance = 0;
                _actualChaseTime = 0;
                ProcessInput(Inputs.Rest);
            }
            _actualChaseTime += Time.fixedDeltaTime;
            _agent.SetDestination(player.transform.position);
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
        };

        chase.OnExit += x =>
        {
            StartCoroutine(ChaseCooldown());
        };

        //attack
        attack.OnEnter += x =>
        {
            _agent.isStopped = true;
            _anim.SetAttack();
        };

        attack.OnExit += x =>
        {
            StartCoroutine(AttackCooldown());
        };


        //death
        death.OnEnter += x =>
        {
            _anim.SetDeath(frontalHit);
            _rb.useGravity = false;
            GetComponent<Collider>().enabled = false;
            _agent.isStopped = true;
            _agent.enabled = false;
        };


        //-----------------------------------------FSM INIT-------------------------------------------//
        AddEvents();
        _stateMachine = new EventFSM<Inputs>(idle);
    }

    private void ProcessInput(Inputs inp)
    {
        _stateMachine.SendInput(inp);
    }

    private void Update()
    {
        CheckSensors();
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        CheckSensors();
        _stateMachine.FixedUpdate();
    }

    void AddEvents()
    {

    }

    void RemoveEvents()
    {

    }

    public void Die(bool frontalHit)
    {
        this.frontalHit = frontalHit;
        ProcessInput(Inputs.Die);
        RemoveEvents();
    }

    public void AttackEnd()
    {
        ProcessInput(Inputs.Rest);
    }

    //Sensor checking
    void CheckSensors()
    {
        if (_loS.TargetInSight) ProcessInput(Inputs.EnemyFound);
        else ProcessInput(Inputs.EnemyLost);

        if (PlayerInRange() && _canAttack) ProcessInput(Inputs.EnemyInAttackRange);

        UpdateChasePerc();
    }

    void UpdateChasePerc()
    {
        // 1% hp ?> 50%
        // 100%hp > 0%

        var percHp = _model.HP / _model.maxHp;
        var percBasedOnHp = Mathf.Lerp(50, 0, percHp);
        _chasePerc = Mathf.FloorToInt(initialChasePerc + percBasedOnHp);
    }

    bool PlayerInRange()
    {
        var xDistance = Vector3.Distance(new Vector3(player.position.x, 0), new Vector3(transform.position.x, 0));
        var yDistance = Vector3.Distance(new Vector3(0, player.position.y, 0), new Vector3(0, transform.position.y, 0));
        var zDistance = Vector3.Distance(new Vector3(0, 0, player.position.z), new Vector3(0, 0, transform.position.z));

        var xCondition = xDistance <= attackRange;
        var yCondition = yDistance <= 1;
        var zCondition = zDistance <= attackRange;

        return xCondition && yCondition && zCondition;
    }

    #endregion

    #endregion

    #region Corroutines

    IEnumerator ChaseCooldown()
    {
        _canChase = false;

        yield return new WaitForSeconds(chaseCooldown);

        _canChase = true;
    }

    IEnumerator AttackCooldown()
    {
        _canAttack = false;

        yield return new WaitForSeconds(chaseCooldown);

        _canAttack = true;
    }

    #endregion
}
