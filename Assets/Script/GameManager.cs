using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //EventSystem.current.currentSelectedGameObject;
    public CanvasGroup right;
    public CanvasGroup left;

    public GameObject BlueArrow;
    public GameObject RedArrow;

    private bool SecondPick = false;
    private int TotalPickCount = 0;
    public Image LaneSprite;
    private List<TileManager> _BlueTeam_1 = new List<TileManager>();
    private List<TileManager> _BlueTeam_2 = new List<TileManager>();
    private List<TileManager> _BlueTeam_3 = new List<TileManager>();
    private List<TileManager> _RedTeam_1 = new List<TileManager>();
    private List<TileManager> _RedTeam_2 = new List<TileManager>();
    private List<TileManager> _RedTeam_3 = new List<TileManager>();
    public TMP_Text TimeDisplay;
    public TMP_Text StatusDisplay;
    public GameObject SoundManager;
    private float TimeCounter = 29.5f;
    private GameObject ChampionSelected = null;
    private GameObject Tile = null;
    public Phase phase = Phase.Prepare;
    public Lane lane = Lane.Top;
    public GameObject[] Pick;

    public Champion[] _Champion;
    public Dictionary<string, Champion> ChampionList = new Dictionary<string, Champion>();
    private List<TileManager> _BlueTurn = new List<TileManager>();
    private List<TileManager> _RedTurn = new List<TileManager>();
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(_BlueTeam_1.Count);
        for (int i = 0; i < _Champion.Length; i++)
        {
            ChampionList.Add(_Champion[i].name, _Champion[i]);
        }
        StatusDisplay.text = "Prepare phase";
        TimeDisplay.text = $"{Mathf.Round(TimeCounter)} S";
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeCounter > 0f)
        {
            TimeCounter -= Time.deltaTime;
        }
        else if (TimeCounter < 0f)
        {
            TimeCounter = 0f;
            TimeDisplay.gameObject.SetActive(false);
            SoundManager.SetActive(true);
            phase = Phase.Pick;
            StatusDisplay.text = "Ban & Pick phase";
            LaneSprite.gameObject.GetComponent<Image>().enabled = true;
            Pick[0].SetActive(true);
            Pick[1].SetActive(true);
            BlueArrow.SetActive(true);
            left.gameObject.GetComponent<Image>().enabled = true;
            left.interactable = true;
        }
        TimeDisplay.text = $"{Mathf.Round(TimeCounter)} s";
        if (phase == Phase.End)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void SelectChampion()
    {
        ChampionSelected = EventSystem.current.currentSelectedGameObject;
    }

    public void SelectPosition()
    {
        Tile = EventSystem.current.currentSelectedGameObject;
        if (Tile.GetComponent<TileManager>()._ChampionHolder != null || ChampionSelected == null)
        {
            Debug.Log("?");
            return;
        }
        var champion = Instantiate(ChampionList[ChampionSelected.name], Tile.transform);
        champion.transform.position = Tile.transform.position;
        Tile.GetComponent<TileManager>()._ChampionHolder = champion;
        champion.GetComponent<Champion>().SetCurrentTile(Tile.GetComponent<TileManager>());
        TotalPickCount++;
        if (TotalPickCount == 1 || TotalPickCount == 4 || TotalPickCount == 5 || TotalPickCount == 8 || TotalPickCount == 9)
            _BlueTurn.Add(Tile.GetComponent<TileManager>());
        else
            _RedTurn.Add(Tile.GetComponent<TileManager>());
        Debug.Log(_BlueTurn.Count);
        Debug.Log(_RedTurn.Count);
        Tile = null;
        ChampionSelected.gameObject.SetActive(false);
        ChampionSelected = null;
        if (TotalPickCount >= 10)
        {
            left.interactable = false;
            right.interactable = false;
            left.gameObject.GetComponent<Image>().enabled = false;
            right.gameObject.GetComponent<Image>().enabled = false;
            RedArrow.SetActive(false);
            LaneSprite.gameObject.SetActive(false);
            foreach (TileManager tile in _BlueTurn)
            {
                tile._ChampionHolder.SetHP();
                int line = tile.Y;

                List<TileManager> temp = new List<TileManager>();
                if (line == 1)
                {
                    temp = _BlueTeam_1;
                }
                else if (line == 2)
                {
                    temp = _BlueTeam_2;
                }
                else
                    temp = _BlueTeam_3;

                if (temp.Count == 0)
                {
                    temp.Add(tile);
                }
                else if (temp.Count == 1)
                {
                    if (temp[0].X < tile.X)
                    {
                        temp.Add(tile);
                    }
                    else
                    {
                        temp.Insert(0, tile);
                    }
                }
                else
                {
                    temp.Insert(tile.X - 1, tile);
                }
            }
            foreach (TileManager tile in _RedTurn)
            {
                tile._ChampionHolder.SetHP();
                int line = tile.Y;

                List<TileManager> temp = new List<TileManager>();
                if (line == 1)
                {
                    temp = _RedTeam_1;
                }
                else if (line == 2)
                {
                    temp = _RedTeam_2;
                }
                else
                    temp = _RedTeam_3;

                if (temp.Count == 0)
                {
                    temp.Add(tile);
                }
                else if (temp.Count == 1)
                {
                    if (temp[0].X < tile.X)
                    {
                        temp.Add(tile);
                    }
                    else
                    {
                        temp.Insert(0, tile);
                    }
                }
                else
                {
                    temp.Insert(tile.X - 1, tile);
                }
            }
            phase = Phase.Play;
            StatusDisplay.text = "Play phase";

            StartCoroutine(Play(true));
            return;
        }
        LaneSprite.sprite = SpriteManager.Instance._LaneIcons[(int)(TotalPickCount / 2)];
        if (!SecondPick)
        {
            bool temp = BlueArrow.activeInHierarchy;
            BlueArrow.SetActive(RedArrow.activeInHierarchy);
            RedArrow.SetActive(temp);
            left.interactable = BlueArrow.activeInHierarchy;
            left.gameObject.GetComponent<Image>().enabled = BlueArrow.activeInHierarchy;
            right.interactable = RedArrow.activeInHierarchy;
            right.gameObject.GetComponent<Image>().enabled = RedArrow.activeInHierarchy;
            SecondPick = true;
        }
        else
        {
            SecondPick = false;
            Pick[TotalPickCount].SetActive(true);
            Pick[TotalPickCount + 1].SetActive(true);
        }
    }
    private TileManager LastBlue;
    private TileManager LastRed;
    IEnumerator Play(bool isBlueTurn)
    {
        yield return new WaitForSeconds(1f);
        List<TileManager> turn = new List<TileManager>();
        turn = isBlueTurn ? _BlueTurn : _RedTurn;
        if (turn.Count == 0)
        {
            StopAllCoroutines();
            StatusDisplay.text = isBlueTurn ? "Red win" : "Blue win";
            phase = Phase.End;
        }
        else
        {


            Debug.Log(turn.Count);
            int index = 0;
            TileManager tempTile = isBlueTurn ? LastBlue : LastRed;
            if (tempTile != null)
            {
                if (turn.IndexOf(tempTile) != turn.Count - 1)
                {
                    Debug.Log("turn" + turn.IndexOf(tempTile));
                    index = turn.IndexOf(tempTile) + 1;
                }
            }
            int line = turn[index].Y;
            if (isBlueTurn)
            {
                LastBlue = turn[index];
            }
            else
            {
                LastRed = turn[index];
            }
            Debug.Log(Time.deltaTime + " Atk: " + turn[index]._ChampionHolder.name);
            for (int i = 0; i < 3; i++)
            {
                List<TileManager> temp = new List<TileManager>();
                if (line == 1)
                {
                    temp = !isBlueTurn ? _BlueTeam_1 : _RedTeam_1;
                }
                else if (line == 2)
                {
                    temp = !isBlueTurn ? _BlueTeam_2 : _RedTeam_2;
                }
                else
                    temp = !isBlueTurn ? _BlueTeam_3 : _RedTeam_3;
                int count = temp.Count;
                if (count == 0)
                {
                    if (line != 3)
                    {
                        line++;
                    }
                    else
                    {
                        line = 1;
                    }
                    continue;
                }
                else if (count == 1)
                {
                    turn[index]._Atk.SetActive(true);
                    temp[0]._Def.SetActive(true);
                    turn[index]._ChampionHolder.Attack_Normal(temp[0]._ChampionHolder);
                    Debug.Log(Time.deltaTime + " Def: " + temp[0]._ChampionHolder.name);
                    yield return new WaitForSeconds(0.5f);
                    temp[0]._Def.SetActive(false);
                    turn[index]._Atk.SetActive(false);
                }
                else
                {
                    if (turn[index]._ChampionHolder.GetTarget() == 1)
                    {
                        if (turn[index]._ChampionHolder.GetRange() == 1)
                        {
                            temp[0]._Def.SetActive(true);
                            turn[index]._Atk.SetActive(true);
                            turn[index]._ChampionHolder.Attack_Normal(temp[0]._ChampionHolder);
                            Debug.Log(Time.deltaTime + " Def: " + temp[0]._ChampionHolder.name);
                            yield return new WaitForSeconds(0.5f);
                            temp[0]._Def.SetActive(false);
                            turn[index]._Atk.SetActive(false);
                        }
                        else
                        {
                            turn[index]._Atk.SetActive(true);
                            temp[0]._Def.SetActive(true);
                            turn[index]._ChampionHolder.Attack_Normal(temp[0]._ChampionHolder);
                            Debug.Log(Time.deltaTime + " Def: " + temp[0]._ChampionHolder.name);
                            temp[1]._Def.SetActive(true);
                            Debug.Log(Time.deltaTime + " Def: " + temp[1]._ChampionHolder.name);
                            turn[index]._ChampionHolder.Attack_Normal(temp[1]._ChampionHolder);
                            yield return new WaitForSeconds(0.5f);
                            temp[0]._Def.SetActive(false);
                            temp[1]._Def.SetActive(false);
                            turn[index]._Atk.SetActive(false);
                        }

                    }
                    else
                    {
                        if (turn[index]._ChampionHolder.GetRange() == 1)
                        {
                            turn[index]._Atk.SetActive(true);
                            temp[1]._Def.SetActive(true);
                            turn[index]._ChampionHolder.Attack_Normal(temp[1]._ChampionHolder);
                            Debug.Log(Time.deltaTime + " Def: " + temp[1]._ChampionHolder.name);
                            yield return new WaitForSeconds(0.5f);
                            temp[1]._Def.SetActive(false);
                            turn[index]._Atk.SetActive(false);
                        }
                        else if (turn[index]._ChampionHolder.GetRange() == 2 && temp.Count > 2)
                        {
                            turn[index]._Atk.SetActive(true);
                            temp[1]._Def.SetActive(true);
                            turn[index]._ChampionHolder.Attack_Normal(temp[1]._ChampionHolder);
                            Debug.Log(Time.deltaTime + " Def: " + temp[1]._ChampionHolder.name);
                            temp[2]._Def.SetActive(true);
                            turn[index]._ChampionHolder.Attack_Normal(temp[2]._ChampionHolder);
                            Debug.Log(Time.deltaTime + " Def: " + temp[2]._ChampionHolder.name);
                            yield return new WaitForSeconds(0.5f);
                            temp[1]._Def.SetActive(false);
                            temp[2]._Def.SetActive(false);
                            turn[index]._Atk.SetActive(false);
                        }
                    }
                }
                for (int j = 0; j < temp.Count; j++)
                {
                    if (temp[j]._ChampionHolder != null && temp[j]._ChampionHolder.GetHP() <= 0)
                    {
                        Destroy(temp[j]._ChampionHolder.gameObject);
                        if (isBlueTurn)
                        {
                            _RedTurn.Remove(temp[j]);
                        }
                        else
                        {
                            _BlueTurn.Remove(temp[j]);
                        }
                        temp[j]._HP.text = "Die";
                        temp.Remove(temp[j]);
                        j--;
                    }
                }
                break;
            }
            //Play(int index, bool isBlueTurn)
            StartCoroutine(Play(!isBlueTurn));
        }
    }
}

public enum Phase
{
    Prepare,
    Pick,
    Play,
    End
}

public enum Lane
{
    Top,
    Jungle,
    Mid,
    Adc,
    Sp
}

