using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;




public class GameLogic : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public int[,] pole = new int[4, 4];
        public bool[,] freePool = new bool[4, 4];
    }
    Data saveData = new Data();
    Data lastData = new Data();

    [Serializable]
    public class ObjLists
    {
        public List<GameObject> list = new List<GameObject>();
    }
    public ObjLists[] startObjtLists = new ObjLists[16];

    public Text winText;
    public Text scoreText;
    public Text highScoreText;
    int highScore, score;
    public Text clearSkillText, undoSkillText, upSkillText;
    int clearSkillCount = 9;
    int undoSkillCount = 9;
    int upSkillCount = 9;

    public Button restartButton, musicButton, upButton, undoButton, clearButton;
    public AudioSource ambient;

    public Text[] text;
    Text[,] textPole = new Text[4, 4];

    public Transform[] poleTransform;
    public ObjLists[] objList = new ObjLists[16];
    GameObject[,] objectPole = new GameObject[4, 4];


    int[,] pole = new int[4, 4];
    bool[,] freePool = new bool[4, 4];
    List<Vector2> numberFree = new List<Vector2>();
    bool[,] wasMerge = new bool[4, 4];


    void Start()
    {
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < objList[i].list.Count; j++)
            {
                startObjtLists[i].list.Add(objList[i].list[j]);
            }
        }

        if (!File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            int t = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    textPole[x, y] = text[t];
                    t++;
                    textPole[x, y].text = "";

                    pole[x, y] = saveData.pole[x, y];
                    freePool[x, y] = saveData.freePool[x, y];
                    numberFree.Add(new Vector2(x, y));
                }
            }

            for (int i = 0; i < 2; i++)
            {
                Generator();
            }
        }
        else
        {
            LoadGame();
        }

        restartButton.onClick.AddListener(Restart);
        musicButton.onClick.AddListener(Music);
        upButton.onClick.AddListener(UpSkill);
        undoButton.onClick.AddListener(UndoSkill);
        clearButton.onClick.AddListener(ClearSkill);
    }

    bool waitMove = false;
    bool win = false;
    void Update()
    {
        if (Input.anyKeyDown && !waitMove && !win)
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    wasMerge[x, y] = false;
                }
            }

            if (numberFree.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    SaveLastMove();
                    StartCoroutine(MoveUp());
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SaveLastMove();
                    StartCoroutine(MoveDown());
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    SaveLastMove();
                    StartCoroutine(MoveLeft());
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    SaveLastMove();
                    StartCoroutine(MoveRight());
                }
            }
            else Debug.Log("ti proigral");
        }
    }

    IEnumerator MoveUp()
    {
        bool wasChange = true;
        bool canMove = true;
        while (wasChange || canMove)
        {
            wasChange = false;
            canMove = false;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 1; x < 4; x++)
                {
                    if (pole[x, y] != 0)
                    {
                        if (CheckerObj(new Vector2(-1, 0), x, y))
                        {
                            canMove = true;
                        }
                        else
                        if (Checker(new Vector2(-1, 0), x, y))
                        {
                            wasChange = true;
                        }
                    }
                }
            }
            yield return null;
        }
        Generator();
        yield return null;
    }

    IEnumerator MoveDown()
    {
        bool wasChange = true;
        bool canMove = true;
        while (wasChange || canMove)
        {
            wasChange = false;
            canMove = false;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 2; x > -1; x--)
                {
                    if (pole[x, y] != 0)
                    {
                        if (CheckerObj(new Vector2(1, 0), x, y))
                        {
                            canMove = true;
                        }
                        else
                        if (Checker(new Vector2(1, 0), x, y))
                        {
                            wasChange = true;
                        }
                    }
                }
            }
            yield return null;
        }
        Generator();
        yield return null;
    }
    IEnumerator MoveLeft()
    {
        bool wasChange = true;
        bool canMove = true;
        while (wasChange || canMove)
        {
            wasChange = false;
            canMove = false;
            for (int y = 1; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (pole[x, y] != 0)
                    {
                        if (CheckerObj(new Vector2(0, -1), x, y))
                        {
                            canMove = true;
                        }
                        else
                        if (Checker(new Vector2(0, -1), x, y))
                        {
                            wasChange = true;
                        }
                    }
                }
            }
            yield return null;
        }
        Generator();
        yield return null;
    }
    IEnumerator MoveRight()
    {
        bool wasChange = true;
        bool canMove = true;
        while (wasChange || canMove)
        {
            wasChange = false;
            canMove = false;
            for (int y = 2; y > -1; y--)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (pole[x, y] != 0)
                    {
                        if (CheckerObj(new Vector2(0, 1), x, y))
                        {
                            canMove = true;
                        }
                        else
                        if (Checker(new Vector2(0, 1), x, y))
                        {
                            wasChange = true;
                        }
                    }
                }
            }
            yield return null;
        }
        Generator();
        yield return null;
    }

    bool Checker(Vector2 dir, int x, int y)
    {
        int xx = x + (int)dir.x;
        int yy = y + (int)dir.y;

        if ((!wasMerge[xx, yy] && !wasMerge[x, y] && (pole[xx, yy] == pole[x, y])) || pole[xx, yy] == 0)
        {
            if (pole[xx, yy] == 0)
            {
                pole[xx, yy] = pole[x, y];
            } else if (pole[xx,yy] == 16)
            {
                win = true;
                winText.transform.gameObject.SetActive(true);
                Debug.Log("YOU WIN");
            }
            else if (!wasMerge[xx, yy] && !wasMerge[x, y] && (pole[xx, yy] == pole[x, y]))
            {
                score += 10 * (pole[xx, yy] + pole[x, y]);
                pole[xx, yy]++;
                wasMerge[xx, yy] = true;
            }

            pole[x, y] = 0;

            freePool[xx, yy] = true;
            freePool[x, y] = false;

            changeText(dir, x, y);

            return true;
        }
        else return false;
    }

    bool CheckerObj(Vector2 dir, int x, int y)
    {
        waitMove = true;

        int xx = x + (int)dir.x;
        int yy = y + (int)dir.y;

        Vector3 current = objectPole[x, y].transform.position;
        Vector3 target = poleTransform[xx * 4 + yy].position;

        if ((!wasMerge[xx, yy] && !wasMerge[x, y] && (pole[xx, yy] == pole[x, y])) || pole[xx, yy] == 0)
        {
            if (Vector3.Distance(current, target) > 0.1f)
            {
                objectPole[x, y].transform.position = Vector3.MoveTowards(current, target, 10 * Time.deltaTime);
                return true;
            }
            else
            {
                if (pole[xx, yy] == 0)
                {
                    objectPole[x, y].transform.position = target;
                    objectPole[xx, yy] = objectPole[x, y];
                    objectPole[x, y] = null;
                }
                else if (!wasMerge[xx, yy] && !wasMerge[x, y] && (pole[xx, yy] == pole[x, y]))
                {
                    objectPole[x, y].SetActive(false);
                    objectPole[xx, yy].SetActive(false);

                    changeObj(pole[x, y], x, y, xx, yy);

                    objectPole[x, y] = null;
                    objectPole[xx, yy].transform.position = target;
                    objectPole[xx, yy].SetActive(true);
                }
                return false;
            }
        }
        else return false;
    }

    void Generator()
    {
        numberFree.Clear();
        for (int y1 = 0; y1 < 4; y1++)
        {
            for (int x1 = 0; x1 < 4; x1++)
            {
                if (!freePool[x1, y1])
                {
                    numberFree.Add(new Vector2(x1, y1));
                }
            }
        }

        int randNum = UnityEngine.Random.Range(0, (int)numberFree.Count);
        int x = (int)numberFree[randNum].x;
        int y = (int)numberFree[randNum].y;

        int randValue = UnityEngine.Random.Range(1, 11);
        if (randValue < 10)
        {
            pole[x, y] = 1;
            objectPole[x, y] = objList[0].list[0];
            objList[0].list.RemoveAt(0);
        }
        else
        {
            pole[x, y] = 2;
            objectPole[x, y] = objList[1].list[0];
            objList[1].list.RemoveAt(0);
        }

        objectPole[x, y].SetActive(true);
        objectPole[x, y].transform.position = poleTransform[x * 4 + y].position;
        textPole[x, y].text = pole[x, y].ToString();

        freePool[x, y] = true;
        numberFree.RemoveAt(randNum);
        SaveGame();
        waitMove = false;
    }

    void changeText(Vector2 dir, int x, int y)
    {
        int xx = x + (int)dir.x;
        int yy = y + (int)dir.y;

        textPole[xx, yy].text = pole[xx, yy].ToString();
        textPole[x, y].text = "";
        scoreText.text = score.ToString();
    }

    void changeObj(int list, int x, int y, int xx, int yy)
    {
        list--;
        objList[list].list.Add(objectPole[x, y]);
        objList[list].list.Add(objectPole[xx, yy]);

        if (list < 15)
        {
            objectPole[xx, yy] = objList[list + 1].list[0];
            objList[list + 1].list.RemoveAt(0);
        }
    }

    void Restart()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            File.Delete(Application.persistentDataPath + "/SaveData.dat");
            Debug.Log("Data reset complete!");
        }
        
        for (int i = 0; i < 16; i++)
        {
            objList[i].list.Clear();
            objList[i].list.AddRange(startObjtLists[i].list);
            for (int j = 0; j < startObjtLists[i].list.Count; j++)
            {
                objList[i].list[j].SetActive(false);
            }
        }

        int t = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                textPole[x, y] = text[t];
                t++;
                textPole[x, y].text = "";

                pole[x, y] = 0;
                freePool[x, y] = false;
                numberFree.Add(new Vector2(x, y));
            }
        }

        for (int i = 0; i < 2; i++)
        {
            Generator();
        }
    }
    void Music()
    {
        ambient.mute = !ambient.mute;
    }
    void UpSkill()
    {
        int m = 15;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if ((pole[x, y] != 0) && (pole[x, y] < m))
                {
                    m = pole[x, y];
                }
            }
        }

        SaveGame();
        for (int i = 0; i < 16; i++)
        {
            objList[i].list.Clear();
            objList[i].list.AddRange(startObjtLists[i].list);
            for (int j = 0; j < startObjtLists[i].list.Count; j++)
            {
                objList[i].list[j].SetActive(false);
            }
        }

        int t = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                textPole[x, y] = text[t];
                t++;
                textPole[x, y].text = "";

                pole[x, y] = saveData.pole[x, y];
                freePool[x, y] = saveData.freePool[x, y];

                if ((pole[x, y] == m) && (pole[x, y] < 16))
                {
                    pole[x, y]++;
                }

                if (pole[x, y] == 0)
                {
                    numberFree.Add(new Vector2(x, y));
                }
                else
                {
                    objectPole[x, y] = objList[pole[x, y] - 1].list[0];
                    objList[pole[x, y] - 1].list.RemoveAt(0);

                    objectPole[x, y].transform.position = poleTransform[x * 4 + y].position;
                    objectPole[x, y].SetActive(true);
                }
            }
        }

        upSkillCount--;
        upSkillText.text = upSkillCount.ToString();
    }

    bool wasUndo = true;
    void UndoSkill()
    {
        if (!wasUndo)
        {
            for (int i = 0; i < 16; i++)
            {
                objList[i].list.Clear();
                objList[i].list.AddRange(startObjtLists[i].list);
                for (int j = 0; j < startObjtLists[i].list.Count; j++)
                {
                    objList[i].list[j].SetActive(false);
                }
            }

            int t = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    textPole[x, y] = text[t];
                    t++;
                    textPole[x, y].text = "";

                    pole[x, y] = lastData.pole[x, y];
                    freePool[x, y] = lastData.freePool[x, y];

                    if (pole[x, y] == 0)
                    {
                        numberFree.Add(new Vector2(x, y));
                    }
                    else
                    {
                        objectPole[x, y] = objList[pole[x, y] - 1].list[0];
                        objList[pole[x, y] - 1].list.RemoveAt(0);

                        objectPole[x, y].transform.position = poleTransform[x * 4 + y].position;
                        objectPole[x, y].SetActive(true);
                    }
                }
            }
            undoSkillCount--;
            undoSkillText.text = undoSkillCount.ToString();
            wasUndo = true;
        }
    }
    void ClearSkill()
    {
        bool wasClear = false;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (pole[x, y] == 1)
                {
                    objList[pole[x, y] - 1].list.Add(objectPole[x, y]);
                    objectPole[x, y].SetActive(false);
                    pole[x, y] = 0;
                    wasClear = true;
                }
            }
        }
        if (wasClear)
        {
            clearSkillCount--;
            clearSkillText.text = clearSkillCount.ToString();
        }
    }

    void SaveLastMove()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                lastData.pole[x, y] = pole[x, y];
                lastData.freePool[x, y] = freePool[x, y];
            }
        }
        wasUndo = false;
    }
    void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        saveData.pole = pole;
        saveData.freePool = freePool;
        bf.Serialize(file, saveData);
        file.Close();
        Debug.Log("Game data saved!");
    }
    void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
            saveData = (Data)bf.Deserialize(file);
            file.Close();
            Debug.Log("Game data loaded!");
        }

        int t = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                textPole[x, y] = text[t];
                t++;
                textPole[x, y].text = "";

                pole[x, y] = saveData.pole[x, y];
                freePool[x, y] = saveData.freePool[x, y];

                if (pole[x, y] == 0)
                {
                    numberFree.Add(new Vector2(x, y));
                } else
                {
                    objectPole[x, y] = objList[pole[x, y] - 1].list[0];
                    objList[pole[x, y] - 1].list.RemoveAt(0);

                    objectPole[x, y].transform.position = poleTransform[x * 4 + y].position;
                    objectPole[x, y].SetActive(true);
                }
            }
        }
    }
}
