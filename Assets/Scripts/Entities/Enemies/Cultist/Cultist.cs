using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using FSM;

public class Cultist : MonoBehaviour
{
    public float movementSpeed;
    public float runSpeedMultiplier;
    public float berserkSpeedMultiplier;

    CultistAnimModule _anim;
    CultistModel _model;
    LineOfSight _loS;
    public LineOfSight LineOfSightModule { get => _loS; private set => _loS = value; }
    NavMeshAgent _agent;
    Rigidbody _rb;
    public Transform player;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<CultistAnimModule>();
        _model = GetComponent<CultistModel>();
        _loS = GetComponent<LineOfSight>();
        player = FindObjectOfType<PlayerController>().transform;
    }

    void Start()
    {
        _agent.speed = movementSpeed;
        InitFsm();
    }

    bool _canChase = true;
    bool _frontalHit = false;

    public float playerLostCountdown;
    float _actualPlayerLostCountdown;

    public float idleDuration;
    float _currentIdleTime;

    //Flinch
    public float flinchCooldown;
    bool _canFlinch = true;
    public int initialFlinchPerc;
    int _actualFlinchPerc;

    public float berserkDuration;
    float _currentBerserkDuration;
    public float initialRagePerc;
    float _currentRagePerc;

    //Chase
    public float chaseCooldown;

    //Attack
    public float sightRange;
    public float attackRange;

    bool frontalHit = false;

    private EventFSM<Inputs> _stateMachine;
    public enum Inputs { EnemyFound, EnemyLost, EnemyInAttackRange, Rage, RageEnd, Pain, StateEnd, Die };

    public void InitFsm()
    {
        //-----------------------------------------STATE CREATE-------------------------------------------//
        var idle = new State<Inputs>("Idle");
        var flinch = new State<Inputs>("Flinch");
        var patrol = new State<Inputs>("Patrol");
        var chase = new State<Inputs>("Chase");
        var berserk = new State<Inputs>("Berserk");
        var attack = new State<Inputs>("Attack");
        var death = new State<Inputs>("Death");

        StateConfigurer.Create(idle)
            .SetTransition(Inputs.Pain, flinch)
            .SetTransition(Inputs.StateEnd, patrol)
            .SetTransition(Inputs.EnemyFound, chase)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(flinch)
            .SetTransition(Inputs.StateEnd, chase)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(patrol)
            .SetTransition(Inputs.Pain, flinch)
            .SetTransition(Inputs.EnemyFound, chase)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(chase)
             .SetTransition(Inputs.Pain, flinch)
             .SetTransition(Inputs.EnemyLost, patrol)
             .SetTransition(Inputs.EnemyInAttackRange, attack)
             .SetTransition(Inputs.Rage, berserk)
             .SetTransition(Inputs.Die, death)
             .Done();

        StateConfigurer.Create(berserk)
            //.SetTransition(Inputs.Pain, flinch)
            .SetTransition(Inputs.RageEnd, idle)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(attack)
           .SetTransition(Inputs.StateEnd, chase)
           .SetTransition(Inputs.Pain, flinch)
           .SetTransition(Inputs.Rage, berserk)
           .SetTransition(Inputs.Die, death)
           .Done();

        StateConfigurer.Create(death).Done();

        //-----------------------------------------STATE SET-------------------------------------------//

        idle.OnEnter += x =>
        {
            _currentIdleTime = 0;
            _anim.SetIdle();
            _agent.isStopped = true;
        };

        idle.OnUpdate += () =>
        {
            if (_currentIdleTime < idleDuration)
            {
                _currentIdleTime += Time.deltaTime;
            }
            else ProcessInput(Inputs.StateEnd);
        };

        idle.OnExit += x =>
        {
            _currentIdleTime = 0;
        };


        flinch.OnEnter += x =>
        {
            _agent.isStopped = true;
            _anim.SetFlinch();
        };

        flinch.OnExit += x =>
        {
            StartCoroutine(FlinchCooldown());
        };


        patrol.OnEnter += x =>
        {
            //set waypoints

            _agent.isStopped = false;
            _anim.SetWalk();
            /*_agent.SetDestination(player.transform.position);
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;*/
            _agent.speed = movementSpeed;
        };

        patrol.OnFixedUpdate += () =>
        {
            //cycle through waypoints
        };


        chase.OnEnter += x =>
        {
            _agent.isStopped = false;
            _anim.SetRun();
            _agent.SetDestination(player.transform.position);
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
            _agent.speed = movementSpeed * runSpeedMultiplier;
        };

        chase.OnUpdate += () =>
        {
            if (!_loS.TargetInSight)
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

        attack.OnEnter += x =>
        {
            _agent.isStopped = true;
            _anim.SetAttack();
            _model.AttackStart();
        };

        attack.OnExit += x =>
        {

        };

        berserk.OnEnter += x =>
        {
            _currentBerserkDuration = 0;
            initialFlinchPerc /= 3;

            _agent.isStopped = false;

            _agent.SetDestination(player.transform.position);
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
            _agent.speed = movementSpeed * berserkSpeedMultiplier;

            _anim.SetBerserk();
        };

        berserk.OnUpdate += () =>
        {
            if (_currentBerserkDuration < berserkDuration)
            {
                _currentBerserkDuration += Time.deltaTime;
            }
            else ProcessInput(Inputs.RageEnd);
        };

        berserk.OnFixedUpdate += () =>
        {
            _agent.SetDestination(player.transform.position);
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
        };

        berserk.OnExit += x =>
        {
            _currentBerserkDuration = 0;
            initialFlinchPerc *= 3;
        };

        death.OnEnter += x =>
        {
            _anim.SetDeath(_frontalHit);
            _rb.useGravity = false;
            GetComponent<Collider>().enabled = false;
            _agent.isStopped = true;
        };

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
        UpdateBerserkPerc();
        var rnd = Random.Range(0, 100 + 1);
        var input = rnd <= _currentRagePerc ? Inputs.Rage : Inputs.EnemyFound;

        ProcessInput(input);

       /* UpdateFlinchPerc();

        var rnd = Random.Range(0, 100 + 1);

        if (rnd <= _actualFlinchPerc && _canFlinch)
        {
            ProcessInput(Inputs.Pain);
        }
        else
        {
            UpdateBerserkPerc();

            var input = rnd <= _currentRagePerc ? Inputs.Rage : Inputs.EnemyFound;

            ProcessInput(input);
        }*/
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
        if (_loS.TargetInSight) ProcessInput(Inputs.EnemyFound);

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

    void UpdateBerserkPerc()
    {
        // hp value > initial + hp based
        // 1% hp > 50% + 25%
        // 100%hp > 50% + 0%

        var percHp = _model.HP / _model.maxHp;
        var percBasedOnHp = Mathf.Lerp(25, 0, percHp);
        _currentRagePerc = Mathf.FloorToInt(initialRagePerc + percBasedOnHp);
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
}
