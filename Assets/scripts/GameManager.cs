using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    int hearts;
    LevelManager lvlManager;
    Player megaman;

    [SerializeField] TextMeshProUGUI hearts_txt;
    [SerializeField] GameObject gameover;
    [SerializeField] TextMeshProUGUI gameover_txt;

    // Start is called before the first frame update
    void Start()
    {
        hearts = GameObject.FindGameObjectsWithTag("Item").Length;
        hearts_txt.text = hearts.ToString();
        gameover.SetActive(false);
        lvlManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        megaman = GameObject.Find("player").GetComponent<Player>();
    }

    public void ReduceHearts()
    {
        hearts--;
        hearts_txt.text = hearts.ToString();

        if (hearts < 1)
            WinGame();
    }

    public void WinGame()
    {
        gameover_txt.text = "YOU WIN!";
        gameover.SetActive(true);
        Time.timeScale = 0;
        megaman.pause = true;
    }

    public void GameOver()
    {
        gameover_txt.text = "GAME OVER!";
        gameover.SetActive(true);
    }




    public void ResetGame()
    {
        lvlManager.StartGame();
    }
}
