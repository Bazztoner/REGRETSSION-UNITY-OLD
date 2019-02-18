using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FSM;
using UnityEngine.AI;
using PhoenixDevelopment.Utility;

public class Mafia : MonoBehaviour
{
    public float movementSpeed;
    public float runSpeedMultiplier;

    MafiaAnimModule _anim;
    MafiaModel _model;
    MafiaSoundModule _sound;
    LineOfSight _loS;
    public LineOfSight LineOfSightModule { get => _loS; private set => _loS = value; }
    NavMeshAgent _agent;
    Rigidbody _rb;
    public Transform player;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<MafiaAnimModule>();
        _model = GetComponent<MafiaModel>();
        _sound = GetComponent<MafiaSoundModule>();
        _loS = GetComponent<LineOfSight>();
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

    Vector3 _initialPosition;
    Vector3 _initialForward;

    public float playerLostCountdown;
    float _actualPlayerLostCountdown;

    //Evade
    public float evadeCooldown;
    bool _canEvade = true;
    public int initialEvadePerc;
    int _actualEvadePerc;
    Coroutine _evadePercCoroutine;
    EvadeDirection _evadeDir;
    Vector3 _rollPosition;

    enum EvadeDirection { Duck, Left, Right, Forward, Backwards };

    //Chase
    public float chaseCooldown;

    //Attack
    public float attackCooldown;
    public int attackPerc;

    public float attackRange;

    bool _frontalHit = false;
    #endregion


    public float navMeshUpdateTime;
    float _navMeshUpdateCurrent;

    #region Properties
    private EventFSM<Inputs> _stateMachine;
    public enum Inputs { EnemyFound, EnemyLost, EnemyInAttackRange, Evade, StateEnd, Die };

    void InitFsm()
    {
        _initialPosition = transform.position;
        _initialForward = transform.forward;

        //-----------------------------------------STATE CREATE-------------------------------------------//
        var idle = new State<Inputs>("Idle");
        var evade = new State<Inputs>("Evade");
        var chase = new State<Inputs>("Chase");
        var returnToPosition = new State<Inputs>("ReturnToStartPosition");
        var attack = new State<Inputs>("Attack");
        var death = new State<Inputs>("Death");

        StateConfigurer.Create(idle)
            .SetTransition(Inputs.EnemyFound, chase)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(evade)
            .SetTransition(Inputs.StateEnd, chase)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(chase)
            .SetTransition(Inputs.EnemyLost, returnToPosition)
            .SetTransition(Inputs.EnemyInAttackRange, attack)
            .SetTransition(Inputs.Evade, evade)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(returnToPosition)
            .SetTransition(Inputs.StateEnd, idle)
            .SetTransition(Inputs.EnemyFound, chase)
            .SetTransition(Inputs.Die, death)
            .Done();

        StateConfigurer.Create(attack)
           .SetTransition(Inputs.StateEnd, chase)
           .SetTransition(Inputs.Die, death)
           .Done();

        StateConfigurer.Create(death).Done();

        //-----------------------------------------STATE SET-------------------------------------------//

        //idle
        idle.OnEnter += x =>
        {
            _anim.SetIdle();
            _agent.isStopped = true;
        };

        //backToPos
        returnToPosition.OnEnter += x =>
        {
            _navMeshUpdateCurrent = 0;
            _anim.SetWalk();
            _agent.isStopped = false;
            _agent.SetDestination(_initialPosition);
            _agent.speed = movementSpeed;
        };

        returnToPosition.OnFixedUpdate += () =>
        {
            var dir = _agent.velocity.normalized;
            transform.forward = new Vector3(dir.x, 0, dir.z);

            if (Vector3.Distance(transform.position, _initialPosition) <= 2f)
            {
                ArrivedToStart();
                transform.forward = _initialForward;
            }
        };

        //chase
        chase.OnEnter += x =>
        {
            _navMeshUpdateCurrent = 0;
            if (_firstChase) _sound.OnEnemyFound();
            _firstChase = false;
            _agent.isStopped = false;
            _agent.SetDestination(player.transform.position);
            _anim.SetRun();
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

        chase.OnExit += x =>
        {
            StartCoroutine(ChaseCooldown());
            //if (_evadePercCoroutine != null) StopCoroutine(_evadePercCoroutine);
        };

        //evade
        evade.OnEnter += x =>
        {
            if (_evadeDir == EvadeDirection.Duck)
            {
                _anim.SetDuck();
                //_model.SetEvade(true);
                _agent.isStopped = true;
            }
            else
            {
                var dirLeft = _evadeDir == EvadeDirection.Left;
                _anim.SetRoll(dirLeft);
                _agent.isStopped = false;

                var rollDest = dirLeft ? transform.right * 2 : transform.right * -2;

                _rollPosition = transform.position + rollDest;

                _agent.SetDestination(_rollPosition);
            }
        };

        evade.OnFixedUpdate += () =>
        {
            if (_evadeDir != EvadeDirection.Duck)
            {
                var pos = new Vector3(transform.position.x, 0, transform.position.z);

                if (Vector3.Distance(pos, _rollPosition) <= .25f)
                {
                    ProcessInput(Inputs.StateEnd);
                }
            }
        };

        evade.OnExit += x =>
        {
            //_model.SetEvade(false);
            StartCoroutine(EvadeCooldown());
        };

        //attack
        attack.OnEnter += x =>
        {
            _agent.isStopped = true;
            _anim.SetAttack();
            _sound.OnAttack();
            _model.AttackStart();
        };

        attack.OnExit += x =>
        {

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
        ProcessInput(Inputs.EnemyFound);
    }

    public void Die(bool frontalHit)
    {
        this._frontalHit = frontalHit;
        ProcessInput(Inputs.Die);
    }

    public void EvadeEnd()
    {
        ProcessInput(Inputs.StateEnd);
    }

    public void ArrivedToStart()
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
        if (_loS.TargetInSight)
        {
            ProcessInput(Inputs.EnemyFound);

            if (PlayerInRange())
            {
                if (Random.Range(0, 101) >= attackPerc)
                {
                    ProcessInput(Inputs.EnemyInAttackRange);
                }
                else
                {
                    var leftRay = Physics.Raycast(transform.position, -transform.right, 2.01f, HitscanLayers.BlockerLayerMask());
                    var rightRay = Physics.Raycast(transform.position, transform.right, 2.01f, HitscanLayers.BlockerLayerMask());

                    var hits = new Tuple<bool, EvadeDirection>[3] { Tuple.Create(leftRay, EvadeDirection.Left), Tuple.Create(rightRay, EvadeDirection.Right), Tuple.Create(true, EvadeDirection.Duck) };

                    var actionList = hits.Where(x => x.Item1).ToList();
                    actionList.KnuthShuffle();
                    _evadeDir = actionList.First().Item2;

                    ProcessInput(Inputs.Evade);
                }

            }
        }
    }

    void UpdateEvadePerc()
    {
        // 1% hp > 50%
        // 100%hp > 0%

        var percHp = _model.HP / _model.maxHp;
        var percBasedOnHp = Mathf.Lerp(50, 0, percHp);
        _actualEvadePerc = Mathf.FloorToInt(initialEvadePerc + percBasedOnHp);
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

    IEnumerator EvadePercCounter()
    {
        while (_canEvade)
        {
            yield return new WaitForSeconds(.25f);
            UpdateEvadePerc();
            var rnd = Random.Range(0, 100 + 1);

            if (rnd <= _actualEvadePerc && _canEvade)
            {
                var leftRay = Physics.Raycast(transform.position, -transform.right, 2.01f, HitscanLayers.BlockerLayerMask());
                var rightRay = Physics.Raycast(transform.position, transform.right, 2.01f, HitscanLayers.BlockerLayerMask());

                var hits = new Tuple<bool, EvadeDirection>[3] { Tuple.Create(leftRay, EvadeDirection.Left), Tuple.Create(rightRay, EvadeDirection.Right), Tuple.Create(true, EvadeDirection.Duck) };

                var actionList = hits.Where(x => x.Item1).ToList();
                actionList.KnuthShuffle();
                _evadeDir = actionList.First().Item2;

                ProcessInput(Inputs.Evade);
            }
        }
    }

    IEnumerator ChaseCooldown()
    {
        _canChase = false;

        yield return new WaitForSeconds(chaseCooldown);

        _canChase = true;
    }

    IEnumerator EvadeCooldown()
    {
        _canEvade = false;

        yield return new WaitForSeconds(evadeCooldown);

        _canEvade = true;
    }

    #endregion
}
