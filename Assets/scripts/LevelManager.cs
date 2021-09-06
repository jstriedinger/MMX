using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] AudioClip sfx_startgame;
    AudioSource myAudio;

    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    //Singleton
    private void Awake()
    {
        int lvlManagers = FindObjectsOfType<LevelManager>().Length;
        if (lvlManagers > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
            DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        myAudio.PlayOneShot(sfx_startgame);
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
}
