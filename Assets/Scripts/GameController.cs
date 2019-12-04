using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;

    float gameLength = 60;
    float turnLength = 2;
    int turn = 0;
    int score = 0;

    GameObject[,] grid;
    int gridX = 8;
    int gridY = 5;
    Vector3 cubePos;

    GameObject nextCube;
    Vector3 nextCubePos = new Vector3 (8, 10, 0);
    Color[] myColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };

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
            }
        }

    }

    void CreateNextCube()
    {
        if (nextCube == null)
        {
            nextCube = Instantiate(cubePrefab, nextCubePos, Quaternion.identity);
        }
        nextCube.GetComponent<Renderer>().material.color = myColors[Random.Range(0, myColors.Length)];
    }

    void EndGame(bool win)
    {
        //end the game
        if (win)
        {
            print("You Win!");
        }
        else
        {
            print("You Lose. Play Again?");
        }
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


    void CheckForOneColorPlus()
    {
        //checks for plus by scanning through each row and column
        //adds to score
        for (int x= 0; x<gridX; x++)
        {
            for (int y = 0; y<gridY; y++)
            {
                Color tempColor = grid[x, y].GetComponent<Renderer>().material.color;
                if (grid[x + 1, y].GetComponent<Renderer>().material.color == tempColor && grid[x - 1, y].GetComponent<Renderer>().material.color == tempColor && grid[x, y + 1].GetComponent<Renderer>().material.color == tempColor && grid[x, y - 1].GetComponent<Renderer>().material.color == tempColor)
                {
                    score += 10;
                }
            }
        }       
    }

    bool CheckForRainbowColorPlus(int x, int y)
    {
        //checks for rainbow
        Color tempColor = grid[x, y].GetComponent<Renderer>().material.color;
        score += 5;
        return true;
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
    public void ProcessClick(GameObject clickedCube, int x, int y, Color cubeColor, bool active)
    {
        //if colored cube, highlight cube
        if (cubeColor != Color.white && cubeColor != Color.black && nextCube != clickedCube)
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
                }
                else
                {
                    clickedCube.transform.localScale *= 1.5f;
                    clickedCube.GetComponent<CubeController>().active = false;
                    activeCube = clickedCube;
                }
            }
        }
        else if(cubeColor == Color.white)
        {

        }
    }
    // Update is called once per frame
    void Update()
    {
        ProcessKeyboardInput();
        if (Time.time > turnLength * turn)
        {
            turn++;

            if(nextCube != null)
            {
                score -= 1;
                Destroy(nextCube);
                PlaceBlackCube();
            }
            CreateNextCube();

        }
    }
}
