using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const int GameSetting_PlayersPerTeam = 5;
    public const int GameSetting_GoldShowerAmount = 20;
    public const float GameSetting_GameTimerTotal = 60;


    public Material[] teamMaterials;
    public Material playerMaterial;
    public GameObject prefab_GameAgent;
    public Gold prefab_Gold;

    public GameObject page_TitleScreen;
    public Page_InGame page_InGame;
    public Page_GameOver page_GameOver;

    public FiniteStateMachine<GameManager> _fsm;

    

    private void Awake()
    {
        _InitializeServices();
    }

    private void _InitializeServices()
    {
        Services.GameManager = this;
        _fsm = new FiniteStateMachine<GameManager>(this);

        _fsm.TransitionTo<State_TitleScreen>();

        Services.EventManager = new EventManager();
    }

	private void Update()
    {
        _fsm.Update();
    }
    private abstract class BaseState : FiniteStateMachine<GameManager>.State
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Context.page_TitleScreen.SetActive(false);
            Context.page_InGame.gameObject.SetActive(false);
            Context.page_GameOver.gameObject.SetActive(false);
        }
    }

    private class State_TitleScreen : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            Context.page_TitleScreen.SetActive(true);
        }

        public override void Update()
        {
            base.Update();
            if (Input.anyKeyDown)
                TransitionTo<State_InGame>();
        }
    }

    private class State_InGame : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            Context.page_InGame.gameObject.SetActive(true);

            var playerGameObject = Instantiate(Services.GameManager.prefab_GameAgent);
            Services.Players = new[] { new Player(playerGameObject) };

            Services.AIManager = new AIManager();
            Services.AIManager.Initialize();

            Services.Score = new ScoreManager();
            Services.EventManager.Fire(new Event_GameStarted());

            Services.EventManager.Register<Event_TimedOut>(OnGameTimedOut);
            Services.EventManager.Register<Event_GoalScored>(OnGoalScored);
        }

        public override void Update()
        {
            base.Update();

            //foreach (var player in Services.Players) player.Update();

            Services.AIManager.Update();
            Services.Score.Update();
        }

        public override void OnExit()
        {
            base.OnExit();

            Services.EventManager.Unregister<Event_TimedOut>(OnGameTimedOut);
            Services.EventManager.Unregister<Event_GoalScored>(OnGoalScored); 

            Services.Score.Destroy();
        }

        private void OnGameTimedOut(AGPEvent e)
        {
            var timedOut = (Event_TimedOut)e;
            Context.page_GameOver.SetWinnerMessage(timedOut.blueScore > timedOut.redScore ? "Blue won!" : "Red won!");

            TransitionTo<State_GameOver>();
        }

        public void OnGoalScored(AGPEvent e)
        {

        }
    }

    private class State_GameOver : BaseState
    {
        private const float timeBeforeAllowReturnToTitle = 1.0f;
        private float timeInGameOver = 0;

        public override void OnEnter()
        {
            base.OnEnter();

            Context.page_GameOver.gameObject.SetActive(true);
            timeInGameOver = 0;
            Context.page_GameOver.returnToTitleMessage.SetActive(false);

            Services.AIManager.Destroy();
        }

        public override void Update()
        {
            base.Update();

            timeInGameOver += Time.deltaTime;

            if (timeInGameOver < timeBeforeAllowReturnToTitle) return;

            Context.page_GameOver.returnToTitleMessage.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Space))
                TransitionTo<State_TitleScreen>();
        }
    }
}
