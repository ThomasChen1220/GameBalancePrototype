using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    public int playerFoodPoints = 10;                      //Starting value for Player food points.
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
    public GameObject player;
    public bool gameOver = false;

    private Text levelText, distText;
    private Transform p, exit;
    private GameObject levelImage;
    private int level = 0;                                  //Current level number, expressed in game as "Day 1".
    private bool sceneUpdate;                             //Boolean to check if enemies are moving.
    private bool doingSetup;
    private GameObject interactable;

    public int GetLevel() {
        return level;
    }
        
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
                
            //if not, set instance to this
            instance = this;
            
        //If instance already exists and it's not this:
        else if (instance != this)
                
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);    
            
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
            
        //Call the InitGame function to initialize the first level 
        InitGame();
    }
        
    //This is called each time a scene is loaded.
    void OnLevelWasLoaded(int index)
    {
        //Add one to our level number.
        level++;
        //Call InitGame to initialize our level.
        InitGame();
    }
    int GetDist()
    {
        int res = 0;
        res += (int)Mathf.Abs(exit.position.x - p.position.x);
        res += (int)Mathf.Abs(exit.position.y - p.position.y);
        return res;
    }
    //Initializes the game for each level.
    void InitGame()
    {
        gameOver = false;
        GameObject pobj = Instantiate(player) as GameObject;
        p = pobj.transform;
        p.position = new Vector3(0.5f, 0.5f, 0);
        doingSetup = true;
        levelImage = GameObject.Find("levelImage");
        if (GameObject.Find("levelText") != null)
        {
            levelText = GameObject.Find("levelText").GetComponent<Text>();
            levelText.text = "Day " + level;
        }
        distText = GameObject.Find("distText").GetComponent<Text>();
        GameObject exitObj = GameObject.Find("Exit");
        exit = exitObj.transform;

        distText.text = "distance to exit: " + GetDist();
        levelImage.SetActive(true);
        GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().Follow = p.transform;
        Invoke("HideLevelImage", 2);
            
        
    }

    private void HideLevelImage() {
        levelImage.SetActive(false);
        doingSetup = false;
    }
        
        
    //Update is called every frame.
    void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if(playersTurn || sceneUpdate || doingSetup)
                
            //If any of these are true, return and do not start MoveEnemies.
            return;

        StartCoroutine(DoSceneUpdate());
    }
    IEnumerator DoSceneUpdate()
    {
        distText.text = "distance to exit: " + GetDist();
        sceneUpdate = true;
        yield return new WaitForSeconds(0.5f);
        playersTurn = true;
        sceneUpdate = false;
    }
    void Reload() {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        gameOver = true;
        //Enable black background image gameObject.
        if (levelText != null)
        {
            levelText.text = "After " + level + " days, you starved.";
        }
        levelImage.SetActive(true);
        Destroy(p.gameObject);
        
        Invoke("Reload", 3f);
    }
        
}