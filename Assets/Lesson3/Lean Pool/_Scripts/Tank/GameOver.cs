using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;

        Time.timeScale = 0;

        Debug.LogWarning("GAME OVER");
        Debug.LogWarning("GAME OVER");
        Debug.LogWarning("GAME OVER");
    }
}
