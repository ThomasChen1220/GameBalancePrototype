using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
using UnityEngine.UI;
    
public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    public int playerFoodPoints = 100;                      //Starting value for Player food points.
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.


    private Text levelText;
    private GameObject levelImage;
    private int level = 1;                                  //Current level number, expressed in game as "Day 1".
    private bool sceneUpdate;                             //Boolean to check if enemies are moving.
    private bool doingSetup;

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
        
    //Initializes the game for each level.
    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("levelImage");
        levelText = GameObject.Find("levelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
            
            
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
        sceneUpdate = true;
        yield return new WaitForSeconds(0.5f);
        playersTurn = true;
        sceneUpdate = false;
    }
        
    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {

        //Enable black background image gameObject.
        levelText.text = "After " + level + " days, you starved.";  
        levelImage.SetActive(true);
            
        //Disable this GameManager.
        enabled = false;
    }
        
}