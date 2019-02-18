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
    CultistSoundModule _sound;
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
        _sound = GetComponent<CultistSoundModule>();
        _loS = GetComponent<LineOfSight>();
        player = FindObjectOfType<PlayerController>().transform;
        LineOfSightModule.SetTarget(player);
    }

    void Start()
    {
        _agent.speed = movementSpeed;
        InitFsm();
    }

    bool _frontalHit = false;
    bool _firstChase = true;


    public float navMeshUpdateTime;
    float _navMeshUpdateCurrent;

    public float idleTime;
    float _currentIdleTime;

    public float playerLostCountdown;
    float _actualPlayerLostCountdown;

    public float berserkDuration;
    float _currentBerserkDuration;
    public float initialRagePerc;
    float _currentRagePerc;

    //Chase
    public float chaseCooldown;

    //Attack
    public float sightRange;
    public float attackRange;

    EventFSM<Inputs> _stateMachine;
    public enum Inputs { EnemyFound, EnemyLost, IdleEnd, EnemyInAttackRange, Rage, RageEnd, Pain, StateEnd, Die };

    public string GetCurrentState()
    {
        return _stateMachine.Current.Name;
    }

    public void InitFsm()
    {
        //-----------------------------------------STATE CREATE-------------------------------------------//
        var idle = new State<Inputs>("Idle");
        var search = new State<Inputs>("Search");
        var chase = new State<Inputs>("Chase");
        var berserk = new State<Inputs>("Berserk");
        var attack = new State<Inputs>("Attack");
        var death = new State<Inputs>("Death");

        StateConfigurer.Create(idle)
            .SetTransition(Inputs.Pain, chase)
            .SetTransition(Inputs.EnemyFound, chase)
            .SetTransition(Inputs.IdleEnd, search)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(search)
            .SetTransition(Inputs.Pain, chase)
            .SetTransition(Inputs.EnemyFound, chase)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(chase)
             .SetTransition(Inputs.EnemyLost, search)
             .SetTransition(Inputs.EnemyInAttackRange, attack)
             .SetTransition(Inputs.Rage, berserk)
             .SetTransition(Inputs.Die, death)
             .Done();

        StateConfigurer.Create(berserk)
            .SetTransition(Inputs.RageEnd, idle)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(attack)
           .SetTransition(Inputs.StateEnd, chase)
           .SetTransition(Inputs.Rage, berserk)
           .SetTransition(Inputs.Die, death)
           .Done();

        StateConfigurer.Create(death).Done();

        //-----------------------------------------STATE SET-------------------------------------------//

        idle.OnEnter += x =>
        {
            _anim.SetIdle();
            _agent.isStopped = true;
            _currentIdleTime = 0;
        };

        idle.OnUpdate += () =>
        {
            if (!_firstChase)
            {
                _currentIdleTime += Time.deltaTime;
                if (_currentIdleTime >= idleTime)
                {
                    ProcessInput(Inputs.IdleEnd);
                    _currentIdleTime = 0;
                }
            }
        };

        search.OnEnter += x =>
        {
            _navMeshUpdateCurrent = 0;
            _agent.isStopped = false;
            _anim.SetWalk();
            _agent.SetDestination(player.transform.position);
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
            _agent.speed = movementSpeed;
            
        };

        search.OnUpdate += () =>
        {
            if (_navMeshUpdateCurrent <= navMeshUpdateTime)
            {
                _navMeshUpdateCurrent += Time.deltaTime;
            }
            else
            {
                _agent.SetDestination(player.transform.position);
                var dir = player.position - transform.position;
                transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
                _navMeshUpdateCurrent = 0;
            }
        };

        chase.OnEnter += x =>
        {
            _navMeshUpdateCurrent = 0;
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
            if (_navMeshUpdateCurrent <= navMeshUpdateTime)
            {
                _navMeshUpdateCurrent += Time.deltaTime;
            }
            else
            {
                _agent.SetDestination(player.transform.position);
                var dir = player.position - transform.position;
                transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
                _navMeshUpdateCurrent = 0;
            }

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

        attack.OnEnter += x =>
        {
            _agent.isStopped = true;
            _anim.SetAttack();
            _sound.OnAttack();
            _model.RangedAttackStart();
        };

        attack.OnExit += x =>
        {

        };

        berserk.OnEnter += x =>
        {
            _sound.OnRage();
            _currentBerserkDuration = 0;

            _agent.isStopped = false;

            _agent.SetDestination(player.transform.position);
            var dir = player.position - transform.position;
            transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
            _agent.speed = movementSpeed * berserkSpeedMultiplier;

            _anim.SetBerserk();
            _model.AttackStart();
        };

        berserk.OnUpdate += () =>
        {
            if (_navMeshUpdateCurrent <= navMeshUpdateTime)
            {
                _navMeshUpdateCurrent += Time.deltaTime;
            }
            else
            {
                _agent.SetDestination(player.transform.position);
                var dir = player.position - transform.position;
                transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
                _navMeshUpdateCurrent = 0;
            }

            if (_currentBerserkDuration < berserkDuration)
            {
                _currentBerserkDuration += Time.deltaTime;
            }
            else ProcessInput(Inputs.RageEnd);
        };

        berserk.OnExit += x =>
        {
            _currentBerserkDuration = 0;
        };

        death.OnEnter += x =>
        {
            _sound.OnDeath();
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
    }

    public void Die(bool frontalHit)
    {
        this._frontalHit = frontalHit;
        ProcessInput(Inputs.Die);
    }

    public void AttackEnd()
    {
        ProcessInput(Inputs.StateEnd);
    }

    //Sensor checking
    void CheckSensors()
    {
        if (_loS.TargetInSight)
        {
            ProcessInput(Inputs.EnemyFound);
            if (PlayerInRange()) ProcessInput(Inputs.EnemyInAttackRange);
        }
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
}
