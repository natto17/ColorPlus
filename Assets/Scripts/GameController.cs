using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;
    public Text scoreText;

    float gameLength = 60;
    float turnLength = 2;
    int turn = 0;
    int score = 0;

    int sameColorScore = 10;
    int rainbowScore = 5;
    int loseScore = -1;

    GameObject[,] grid;
    int gridX = 8;
    int gridY = 5;
    Vector3 cubePos;

    bool gameOver = false;

    GameObject nextCube;
    Vector3 nextCubePos = new Vector3 (7, 10, 0);
    Color[] myColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };
    int[] colorBinValue = { 0, 1, 2, 4, 8 };

    GameObject activeCube = null;

    // Start is called before the first frame update
    void Start()
    {
        //set up grid
        grid = new GameObject[gridX, gridY];
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                cubePos = new Vector3(x*2, y*2, 0);
                grid[x,y]= Instantiate(cubePrefab, cubePos, Quaternion.identity);
                grid[x, y].GetComponent<CubeController>().myX = x;
                grid[x, y].GetComponent<CubeController>().myY = y;
            }
        }

    }

    void CreateNextCube()
    {
        if (nextCube == null)
        {
            nextCube = Instantiate(cubePrefab, nextCubePos, Quaternion.identity);
        }
        int randomColor = Random.Range(0, myColors.Length);
        nextCube.GetComponent<Renderer>().material.color = myColors[randomColor];
        nextCube.GetComponent<CubeController>().colorValue = colorBinValue[randomColor];
    }

    void EndGame(bool win)
    {
        //change scene
        //end the game
        if (win)
        {
            SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
            print("You Win!");
        }
        else
        {
            SceneManager.LoadScene("LoseScene", LoadSceneMode.Single);
            print("You Lose. Play Again?");
        }
        gameOver = true;
        enabled = false;
    }

    GameObject FindAvailableCube(int y)
    {//find random white cube in row, return y value
        List<GameObject> whiteCubes = new List<GameObject> ();
        for (int x = 0; x<gridX; x++)
        {
            if (grid[x, y].GetComponent<Renderer>().material.color== Color.white)
            {
                whiteCubes.Add(grid[x, y]);
            }
        }
        //if no white cubes available
        if (whiteCubes.Count == 0)
        {
            return null;
        }

        GameObject randomWhiteCube = whiteCubes[Random.Range(0, whiteCubes.Count)];

        return randomWhiteCube;

    }
    GameObject FindAvailableCubeForBlack()
    {//find random white cube in row, return y value
        List<GameObject> whiteCubes = new List<GameObject>();
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                if (grid[x, y].GetComponent<Renderer>().material.color == Color.white)
                {
                    whiteCubes.Add(grid[x, y]);
                }
            }
        }
        //if no white cubes available
        if (whiteCubes.Count == 0)
        {
            return null;
        }

        GameObject randomWhiteCube = whiteCubes[Random.Range(0, whiteCubes.Count)];

        return randomWhiteCube;

    }

    void PlaceNextCube(int y)
    {
        GameObject whiteCube = FindAvailableCube(y);

        if (whiteCube == null)
        {
            EndGame(false);
        }
        else{
            whiteCube.GetComponent<Renderer>().material.color = nextCube.GetComponent<Renderer>().material.color;
            Destroy(nextCube);
            nextCube = null;
        }
    }

    void PlaceBlackCube()
    {
        GameObject whiteCube = FindAvailableCubeForBlack();

        //make random white cube black
        //if impossible, end game

     if (whiteCube == null)
        {
            EndGame(false);
        }
        else
        {
            whiteCube.GetComponent<Renderer>().material.color = Color.black;
        }
        print("Placed a black cube!");
    }
    
    void MakeBlackPlus(int x, int y)
    {if (x== 0 || y==0 || x==gridX-1 || y == gridY - 1) //error check
        {
            return;
        }
        grid[x, y].GetComponent<Renderer>().material.color = Color.black;
        grid[x+1, y].GetComponent<Renderer>().material.color = Color.black;
        grid[x-1, y].GetComponent<Renderer>().material.color = Color.black;
        grid[x, y+1].GetComponent<Renderer>().material.color = Color.black;
        grid[x, y-1].GetComponent<Renderer>().material.color = Color.black;

        grid[x, y].GetComponent<CubeController>().colorValue = 99;
        grid[x + 1, y].GetComponent<CubeController>().colorValue = 99;
        grid[x - 1, y].GetComponent<CubeController>().colorValue = 99;
        grid[x, y + 1].GetComponent<CubeController>().colorValue = 99;
        grid[x, y - 1].GetComponent<CubeController>().colorValue = 99;

        if (activeCube != null && activeCube.GetComponent<Renderer>().material.color == Color.black)
        {
            activeCube.transform.localScale /= 1.5f;
            activeCube.GetComponent<CubeController>().active = false;
        }
    }

    void CheckScore()
    {
        
        for (int x=1; x<gridX-1; x++)
        {
            for (int y=1; y<gridY-1; y++)
            {
                if (CheckForRainbowColorPlus(x, y))
                {
                    score += rainbowScore;
                    MakeBlackPlus(x, y);
                }
                if (CheckForOneColorPlus(x, y))
                {
                    score += sameColorScore;
                    MakeBlackPlus(x, y);
                }
            }

        }
        
    }


    bool CheckForOneColorPlus(int x, int y)
    {
        //checks for plus by scanning through each row and column
        Color tempColor = grid[x, y].GetComponent<Renderer>().material.color;
        if (tempColor != Color.white && tempColor != Color.black)
        {
            if (grid[x + 1, y].GetComponent<Renderer>().material.color == tempColor &&
                grid[x - 1, y].GetComponent<Renderer>().material.color == tempColor &&
                grid[x, y + 1].GetComponent<Renderer>().material.color == tempColor &&
                grid[x, y - 1].GetComponent<Renderer>().material.color == tempColor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    bool CheckForRainbowColorPlus(int x, int y)
    {
        //checks for rainbow

        int sum = 0;
        sum += grid[x + 1, y].GetComponent<CubeController>().colorValue;
        sum += grid[x - 1, y].GetComponent<CubeController>().colorValue;
        sum += grid[x, y + 1].GetComponent<CubeController>().colorValue;
        sum += grid[x, y - 1].GetComponent<CubeController>().colorValue;

        if (sum == 15)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    void ProcessKeyboardInput()
    {
        //if player presses 1-5, and there is still a nextcube
        int numKeyPressed = 0;

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            numKeyPressed = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            numKeyPressed = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            numKeyPressed = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            numKeyPressed = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            numKeyPressed = 5;
        }

        //places nextcube into row or endgame if row is full
        if (nextCube != null && numKeyPressed != 0)
        {
            PlaceNextCube(numKeyPressed - 1);
        }
    }
    public void ProcessClick(GameObject clickedCube, int x, int y, Color cubeColor, bool active, int colorValue)
    {
        //if colored cube, highlight cube
        if (cubeColor != Color.white && cubeColor != Color.black && clickedCube!=nextCube)
        {   //deactivate
            if (active)
            {
                clickedCube.transform.localScale /= 1.5f;
                clickedCube.GetComponent<CubeController>().active = false;
                activeCube = null;
            }
            else
            {   //activate
                if (activeCube != null)
                {
                    activeCube.transform.localScale /= 1.5f;
                    activeCube.GetComponent<CubeController>().active = false;
                    clickedCube.transform.localScale *= 1.5f;
                    clickedCube.GetComponent<CubeController>().active = true;
                    activeCube = clickedCube;

                }
                else
                {
                    clickedCube.transform.localScale *= 1.5f;
                    clickedCube.GetComponent<CubeController>().active = true;
                    activeCube = clickedCube;
                }
            }
        }
        else if(cubeColor == Color.white)
        {
            int xDistance = clickedCube.GetComponent<CubeController>().myX - activeCube.GetComponent<CubeController>().myX;
            int yDistance = clickedCube.GetComponent<CubeController>().myY - activeCube.GetComponent<CubeController>().myY;

            if (Mathf.Abs(yDistance)<= 1 && Mathf.Abs(xDistance) <= 1)
            {
                clickedCube.GetComponent<Renderer>().material.color = activeCube.GetComponent<Renderer>().material.color;
                clickedCube.transform.localScale *= 1.5f;
                clickedCube.GetComponent<CubeController>().active = true;
                clickedCube.GetComponent<CubeController>().colorValue = activeCube.GetComponent<CubeController>().colorValue;

                //turn old clicked cube into white cube
                activeCube.GetComponent<Renderer>().material.color = Color.white;
                activeCube.transform.localScale /= 1.5f;
                activeCube.GetComponent<CubeController>().active = false;
                activeCube.GetComponent<CubeController>().colorValue = 99;

                activeCube = clickedCube;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time < gameLength)
        {
            ProcessKeyboardInput();
            CheckScore();
            if (Time.time > turnLength * turn)
            {
                turn++;

                if (nextCube != null)
                {
                    score -= loseScore;
                    if (score < 0)
                    {
                        score = 0; //make sure never negative
                    }
                    Destroy(nextCube);
                    PlaceBlackCube();
                }
                CreateNextCube();
            }
            scoreText.text = "Score: " + score;
        }
        else if (!gameOver) //once time runs out, will end game
        {
            if (score > 0)
            {
                EndGame(true);
            }
            else
            {
                EndGame(false);
            }
        }
    }
}
