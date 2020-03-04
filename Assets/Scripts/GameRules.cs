using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRules : MonoBehaviour
{
    public static GameRules instance;
    public float boardUnit = .5f;
    public int boardSize = 15;
    public float pieceRadius = .2f;
    public int goal = 5;

    void Awake()
    {
        // Typical singleton initialization code.
        if(instance != null && instance != this)
        {
            // If there already exists a ToastManager, we need to go away.
            Destroy(gameObject);
            return;
        } else
        {
            // If we are the first ToastManager, we claim the "instance" variable so others go away.
            instance = this;
            DontDestroyOnLoad(gameObject); // Survive scene changes
        }
    }

    private void Start() {
        EventBus.Subscribe<winningEvent>(_onWinning);
    }

    void _onWinning(winningEvent e) {
        ToastManager.Toast(e.getColor() + " wins!");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F5)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
