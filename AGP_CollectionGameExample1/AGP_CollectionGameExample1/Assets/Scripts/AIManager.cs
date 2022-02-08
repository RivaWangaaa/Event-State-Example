using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager
{
    private List<GameAgent> _gameAgents;
	private List<Gold> _golds;
	private int amount_GoldShower = 20;
	private float timer_GoldShower = 10;
	private float timerTotal_GoldShower = 10;

	#region Lifecycle Management
	public void Initialize()
    {
        _gameAgents = new List<GameAgent>();
		_golds = new List<Gold>();
		_CreateAIPlayers();

		Services.EventManager.Register<Event_GoalScored>(OnGoalScored);
	}

	public Gold GetClosestGold(GameAgent gameAgent)
	{
		if (_golds.Count == 0) return null;

		var closest = _golds[0];
		var distance = float.MaxValue;

		foreach (var gold in _golds)
		{
			var distanceToPlayer = Vector3.Distance(gold.transform.position, gameAgent.position);
			if (distanceToPlayer < distance)
			{
				closest = gold;
				distance = distanceToPlayer;
			}
		}

		return closest;
	}

	public void DestroyGold(Gold gold)
	{
		if (!_golds.Contains(gold)) return;
		_golds.Remove(gold);
		Object.Destroy(gold.gameObject);
	}

	public void Update()
    {
        foreach (var agent in _gameAgents)
        {
            agent.Update();
		}
		foreach (var player in Services.Players)
		{
			player.Update();
		}

		if (timer_GoldShower > timerTotal_GoldShower)
		{
			timer_GoldShower = 0;
			_CreateGoldShower();
		}
		else
		{
			timer_GoldShower += Time.deltaTime;
		}
    }

    public void Destroy()
    {
        Services.EventManager.Unregister<Event_GoalScored>(OnGoalScored);

		foreach (var agent in _gameAgents)
		{
            agent.Destroy();
		}
	}

    #endregion

    private void _CreateAIPlayers()
    {
		for(int teamID = 0; teamID < 2; teamID++)
		{
			// Make AI players
			for (var i = Services.Players.Count(player => player.teamID == teamID);
				i < GameManager.GameSetting_PlayersPerTeam;
				i++)
			{
				//var playerGameObject = Instantiate(Resources.Load<GameObject>("Player"));
				var createdGameObject = Object.Instantiate(Services.GameManager.prefab_GameAgent);
				createdGameObject.name = "Player_" + teamID + "_" + i;
				createdGameObject.tag = "GameAgent_" + teamID;
				_gameAgents.Add(new AIPlayer(createdGameObject).SetTeam(teamID).SetPosition(Random.Range(-7.0f, 7.0f), 1, Random.Range(-3.0f, 3.0f), true));
			}
		}
	}
	private void _CreateGoldShower()
	{
		// Make gold
		for (var i = 0; i < GameManager.GameSetting_GoldShowerAmount; i++)
		{
			var createdGameObject = Object.Instantiate(Services.GameManager.prefab_Gold);
			createdGameObject.name = "Gold_" + i + "_" + Time.deltaTime;
			createdGameObject.transform.position = new Vector3(Random.Range(-7.0f, 7.0f), 1, Random.Range(-3.5f, 3.5f));
			_golds.Add(createdGameObject);
		}
	}

	private void OnGoalScored(AGPEvent e)
    {
		//Reset all agents to the start point, which is no need for this demo
		//foreach (var agent in _gameAgents)
		//    agent.SetToStartingPosition();
	}
}
