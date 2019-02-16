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
    DrugAddictSoundModule _sound;
    DrugAddictModel _model;
    LineOfSight _loS;
    NavMeshAgent _agent;
    Rigidbody _rb;
    public Transform player;

    public LineOfSight LineOfSightModule { get => _loS; private set => _loS = value; }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<DrugAddictAnimModule>();
        _sound = GetComponent<DrugAddictSoundModule>();
        _model = GetComponent<DrugAddictModel>();
        LineOfSightModule = GetComponent<LineOfSight>();
        player = FindObjectOfType<PlayerController>().transform;
        LineOfSightModule.SetTarget(player);
    }

    void Start()
    {
        _agent.speed = movementSpeed;
        InitFsm();
    }

    #region FSM

    #region Variables
    bool _canChase = true;
    bool _firstChase = true;

    public float playerLostCountdown;
    float _actualPlayerLostCountdown;

    //Flinch
    public float flinchCooldown;
    bool _canFlinch = true;
    public int initialFlinchPerc;
    int _actualFlinchPerc;

    //Chase
    public float chaseCooldown;

    //Attack

    public float sightRange;
    public float attackRange;

    bool _frontalHit = false;
    #endregion

    #region Properties
    private EventFSM<Inputs> _stateMachine;

    public enum Inputs { EnemyFound, EnemyLost, EnemyInAttackRange, Pain, StateEnd, Die };

    void InitFsm()
    {
        //-----------------------------------------STATE CREATE-------------------------------------------//
        var idle = new State<Inputs>("Idle");
        var flinch = new State<Inputs>("Flinch");
        var chase = new State<Inputs>("Chase");
        var attack = new State<Inputs>("Attack");
        var death = new State<Inputs>("Death");

        /*
        idle - No se mueve. Puede ser un descanso
	        EnemyFound -> follow
	        Anger => chase
	        Rest > patrol
         * */

        StateConfigurer.Create(idle)
            .SetTransition(Inputs.EnemyFound, chase)
            .SetTransition(Inputs.Pain, flinch)
            .SetTransition(Inputs.Die, death)
            .Done();

        /*
         * Flinch
         */

        StateConfigurer.Create(flinch)
            .SetTransition(Inputs.StateEnd, chase)
            .SetTransition(Inputs.Die, death)
            .Done();

        /*chase(run hacia jugador)
        EnemyLost->idle
        Rest => follow
        enemyInAttackRange > attackLong*/

        StateConfigurer.Create(chase)
            .SetTransition(Inputs.Pain, flinch)
            .SetTransition(Inputs.EnemyLost, idle)
            .SetTransition(Inputs.EnemyInAttackRange, attack)
            .SetTransition(Inputs.Die, death)
            .Done();

        /*
         attack
	        rest => idle
         * */
        StateConfigurer.Create(attack)
           .SetTransition(Inputs.StateEnd, chase)
           .SetTransition(Inputs.Pain, flinch)
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

        //chase
        chase.OnEnter += x =>
        {
            if (_firstChase) _sound.OnEnemyFound();
            _firstChase = false;
            _agent.isStopped = false;
            _anim.SetRun();
            _agent.SetDestination(player.transform.position);
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
            _agent.speed = movementSpeed * runSpeedMultiplier;
        };

        chase.OnUpdate += () =>
        {
            if (!LineOfSightModule.TargetInSight)
            {
                if (_actualPlayerLostCountdown >= playerLostCountdown)
                {
                    ProcessInput(Inputs.EnemyLost);
                    _actualPlayerLostCountdown = 0;
                }
                else _actualPlayerLostCountdown += Time.deltaTime;
            }

        };

        chase.OnFixedUpdate += () =>
        {
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
            _model.AttackStart();
            _sound.OnAttack();
        };

        attack.OnExit += x =>
        {

        };

        //Flinch
        flinch.OnEnter += x =>
        {
            _agent.isStopped = true;
            _anim.SetFlinch();
            _sound.OnFlinch();
        };

        //Flinch
        flinch.OnExit += x =>
        {
            StartCoroutine(FlinchCooldown());
        };

        //death
        death.OnEnter += x =>
        {
            _sound.OnDeath();
            _anim.SetDeath(_frontalHit);
            _rb.useGravity = false;
            GetComponent<Collider>().enabled = false;
            _agent.isStopped = true;
        };


        //-----------------------------------------FSM INIT-------------------------------------------//
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
        _stateMachine.FixedUpdate();
    }

    public void OnTakeDamage()
    {
        UpdateFlinchPerc();

        var rnd = Random.Range(0, 100 + 1);

        if (rnd <= _actualFlinchPerc && _canFlinch)
        {
            ProcessInput(Inputs.Pain);
        }
        else
        {
            ProcessInput(Inputs.EnemyFound);
        }
    }

    public void Die(bool frontalHit)
    {
        this._frontalHit = frontalHit;
        ProcessInput(Inputs.Die);
    }

    public void FlinchEnd()
    {
        ProcessInput(Inputs.StateEnd);
    }

    public void AttackEnd()
    {
        ProcessInput(Inputs.StateEnd);
    }

    //Sensor checking
    void CheckSensors()
    {
        if (LineOfSightModule.TargetInSight) ProcessInput(Inputs.EnemyFound);

        if (PlayerInRange()) ProcessInput(Inputs.EnemyInAttackRange);
    }

    void UpdateFlinchPerc()
    {
        // 1% hp > 50%
        // 100%hp > 0%

        var percHp = _model.HP / _model.maxHp;
        var percBasedOnHp = Mathf.Lerp(50, 0, percHp);
        _actualFlinchPerc = Mathf.FloorToInt(initialFlinchPerc + percBasedOnHp);
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

    IEnumerator FlinchCooldown()
    {
        _canFlinch = false;

        yield return new WaitForSeconds(flinchCooldown);

        _canFlinch = true;
    }

    #endregion
}
