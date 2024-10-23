using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // TODO: handle data that is processed during a session
    // like score, time, remainingMoves, maxMoves ...
}
