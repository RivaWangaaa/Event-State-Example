using UnityEngine;

public static class Services
{
    public static GameManager GameManager;
    public static AIManager AIManager { get; set; }


    private static ScoreManager _score;
    public static ScoreManager Score
    {
        get
        {
            Debug.Assert(_score != null);
            return _score;
        }
        set => _score = value;
    }


    //In case it's multiplayer
    private static Player[] _players;
	public static Player[] Players
	{
		get
		{
			Debug.Assert(_players != null);
			return _players;
		}
		set => _players = value;
	}

	private static EventManager _eventManager;
    public static EventManager EventManager
    {
        get
        {
            Debug.Assert(_eventManager != null);
            return _eventManager;
        }
        set => _eventManager = value;
    }
}