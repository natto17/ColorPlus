using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;

    float gameLength = 60;
    float turnLength = 2;
    int turn = 0;

    GameObject[,] grid;
    int gridX = 8;
    int gridY = 5;
    Vector3 cubePos;

    GameObject nextCube;
    Vector3 nextCubePos = new Vector3 (8, 10, 0);
    Color[] myColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };

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
                Instantiate(cubePrefab, cubePos, Quaternion.identity);
            }
        }

    }

    void CreateNextCube()
    {if (nextCube == null)
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

    int FindAvailableCube(int x)
    {
        //find random white cube in row, return y value
        //or if there isnt one return -1
        return (Random.Range(0, gridY));

    }

    void PlaceNextCube(int x)
    {
        int y = FindAvailableCube(x);

        if (y == -1)
        {
            EndGame(false);
        }
        else{
            grid[x, y].GetComponent<Renderer>().material.color = nextCube.GetComponent<Renderer>().material.color;
            Destroy(nextCube);
            nextCube = null;
        }
    }


    /*bool CheckForColorPlus(int x, int y)
//checks for plus by scanning through each row and column
//adds to score

    void PlaceBlackCube()
//checks for space to place
// if a place: randomly places cube
//subtracts from score
//if no place, sends to GameEnd

    void GenerateNextCube(int x, int y)
//chooses random color from array
//places cube in set spot
//sets cube as nextCube
*/

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
        if (nextCube != null)
        {
            PlaceNextCube(numKeyPressed - 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessKeyboardInput();
        if (Time.time > turnLength * turn)
        {
            turn++;
            CreateNextCube();

        }
    }
}
