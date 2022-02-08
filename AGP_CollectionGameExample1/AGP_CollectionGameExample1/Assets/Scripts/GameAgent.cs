using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameAgent
{
    #region Variables
    
    private const float MovementSpeed = 20.0f;

    protected GameObject _gameObject;
    private Renderer _renderer;
    protected Rigidbody _rigidbody;
    private Vector3 _startingPosition;
    public Vector3 position => _gameObject.transform.position;

    [HideInInspector]
    public int teamID;

    #endregion

    #region Lifecycle Management

    protected GameAgent(GameObject gameObject)
    {
        _gameObject = gameObject;
        _gameObject.transform.parent = Services.GameManager.page_InGame.transform;
        _renderer = _gameObject.GetComponent<Renderer>();
        _rigidbody = _gameObject.GetComponent<Rigidbody>();
    }

    public abstract void Update();

    public void Destroy()
    {
        UnityEngine.Object.Destroy(_gameObject);
    }
    
    #endregion
    
    #region Core Functionality

    public GameAgent SetTeam(int teamID, bool isPlayer = false)
    {
        this.teamID = teamID;
        _renderer.material = isPlayer ? Services.GameManager.playerMaterial : Services.GameManager.teamMaterials[teamID];

        return this;
    }

    public GameAgent SetPosition(float x, float y, float z, bool isStartingPosition = false)
    {
        _gameObject.transform.position = new Vector3(x, y, z);

        if (isStartingPosition)
        {
            _startingPosition = new Vector3(x, y, z);
        }
        
        return this;
    }

    public void SetToStartingPosition()
    {
        _gameObject.transform.position = _startingPosition;
    }

    protected void MoveInDirection(Vector3 direction)
    {
        var newPosition = _gameObject.transform.position;

        newPosition += Time.deltaTime * MovementSpeed * direction;

        _rigidbody.MovePosition(newPosition);
    }
    
    #endregion
}

public class AIPlayer : GameAgent
{
    private FiniteStateMachine<AIPlayer> _fsm;

    private Transform currentTarget;
    
    #region Lifecycle Management
    public AIPlayer(GameObject gameObject) : base(gameObject)
    {
        _fsm = new FiniteStateMachine<AIPlayer>(this);
        //AI state by default
        _fsm.TransitionTo<Offense>();
    }

    public override void Update()
    {
        _fsm.Update();
        if(currentTarget != null)
        {
            MoveInDirection((currentTarget.position - _gameObject.transform.position).normalized);
        }
        else
		{
            currentTarget = Services.AIManager.GetClosestGold(this)?.transform;
        }
    }

    #endregion

    #region Core Functionality
    

    #endregion

    #region States

    private abstract class AIPlayerState : FiniteStateMachine<AIPlayer>.State { }

    private class Offense : AIPlayerState
    {
        public override void OnEnter()
        {
            // Be Angry
            // pick a defender
        }

        public override void Update()
        {
            base.Update();

            // Become defender if close enough?

            //if (Services.GameManager.gold.transform.position.x < 0 && Context.playerTeam ||
            //    Services.GameManager.gold.transform.position.x > 0 && !Context.playerTeam)
            //{
            //    TransitionTo<Defense>();
            //}
        }

        public override void OnExit()
        {
            //To another state
        }
    }

    private class Defense : AIPlayerState
    {
        
    }

    private class NearGold : AIPlayerState
    {
        
    }

    #endregion
}

public class Player : GameAgent
{
    #region Variables
    
    #endregion

    #region Lifecycle Management

    
    public Player(GameObject gameObject) : base(gameObject)
    {
    }

	public override void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        MoveInDirection(new Vector3(horizontal, 0, vertical));
        //_rigidbody.AddForce(moveDirection * Time.deltaTime * 1000);
    }

	#endregion

	#region Core Functionality

    
    #endregion
}