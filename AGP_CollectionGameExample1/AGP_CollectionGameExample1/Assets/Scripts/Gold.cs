using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //"GameAgent_0" or "GameAgent_1" means blue & red team
        if (!other.tag.Contains("GameAgent")) return;

        var teamID = int.Parse(other.tag.Split('_')[1]);
        Services.EventManager.Fire(new Event_GoalScored(teamID));
        Services.AIManager.DestroyGold(this);
    }
}
