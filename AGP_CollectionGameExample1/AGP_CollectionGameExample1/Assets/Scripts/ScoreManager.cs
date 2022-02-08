using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager
{

    private int redScore, blueScore;

    public float timer = 0;
    float timeForGame = 10;

    public ScoreManager()
    {
        Services.EventManager.Register<Event_GoalScored>(IncrementTeamScore);
        Services.EventManager.Register<Event_GameStarted>(OnGameStart);
    }

    public void Destroy()
    {
        Services.EventManager.Unregister<Event_GoalScored>(IncrementTeamScore);
        Services.EventManager.Unregister<Event_GameStarted>(OnGameStart);
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeForGame)
        {
            Services.EventManager.Fire(new Event_TimedOut(blueScore, redScore));
        }

        Services.GameManager.page_InGame.redScore.text = "" + redScore;
        Services.GameManager.page_InGame.blueScore.text = "" + blueScore;
        Services.GameManager.page_InGame.timer.text = ((int)(timeForGame - timer)).ToString();
    }

    private void OnGameStart(AGPEvent e)
    {
        timer = 0;
        blueScore = 0;
        redScore = 0;
        timeForGame = GameManager.GameSetting_GameTimerTotal;
    }

    private void IncrementTeamScore(AGPEvent e)
    {
        var goalScoredEvent = (Event_GoalScored)e;

        if (goalScoredEvent.teamIDScored == 0)
        {
            blueScore++;
        }
        else
        {
            redScore++;
        }

    }
}
