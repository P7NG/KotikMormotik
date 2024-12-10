using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class GameBehaviour : MonoBehaviour
{
    public List<Sprite> Sprites = new List<Sprite>();
    public List<int> Indexes = new List<int>();
    public List<Card> OriginalCards = new List<Card>();
    public List<Card> AllReadyCards = new List<Card>();
    public List<Card> AllStartCards = new List<Card>();
    public GameObject[] CardObjects;
    public bool IsActive = true;
    public YandexGame yandexGame;
    [SerializeField] private int _cardCount;
    [SerializeField] private int _totalAttemps;
    [SerializeField] private float _waitTime;
    [SerializeField] private GameObject _lastCard;
    [SerializeField] private Text _attempsText;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private Text _winText;
    [SerializeField] private Text _loseText;
    [SerializeField] private string _levelName;
    [SerializeField] private Text _adText;
    [SerializeField] private AudioSource _audio;
    private int _lastIndex;
    private int _attemps = 0;
    [SerializeField] private ImagePack _imagePack;

    private void Awake()
    {
        for(int i = 0; i < _cardCount/2; i++)
        {
            int num = Random.Range(0, _imagePack.Sprites.Count);
            Sprite sprite = _imagePack.Sprites[num];
            Sprites.Add(_imagePack.Sprites[num]);
            _imagePack.Sprites.Remove(sprite);
        }

        for(int i = 0; i < CardObjects.Length; i++)
        {
            CardObjects[i].SetActive(true);
        }
        
        for (int i = 1; i <= _cardCount / 2; i++)
        {
            Indexes.Add(i);
        }
        _audio.volume = PlayerPrefs.GetFloat("Volume");
    }


    public void Versus(GameObject currentCard)
    {
        int index = currentCard.GetComponent<Card>().CardIndex;

        if (_lastIndex == 0)
        {
            _lastIndex = index;
            _lastCard = currentCard;
        }
        else
        {
            if (index == _lastIndex & _lastCard.name != currentCard.name)
            {
                currentCard.GetComponent<Card>().Found();
                _lastCard.GetComponent<Card>().Found();
                _lastCard = null;
                _lastIndex = 0;
                if (AllReadyCards.Count == 0)
                {
                    Win();
                }
            }
            else
            {
                StartCoroutine(ReturnTimer(currentCard));
            }
            _attemps++;
            _attempsText.text = (_totalAttemps - _attemps).ToString();
            if (_totalAttemps - _attemps < 0)
            {
                Lose();
            }
        }
    }

    IEnumerator ReturnTimer(GameObject currentCard)
    {
        IsActive = false;
        yield return new WaitForSeconds(_waitTime);
        currentCard.GetComponent<Card>().Return();
        _lastCard.GetComponent<Card>().Return();
        _lastCard = null;
        _lastIndex = 0;
        IsActive = true;
    }

    public void FlipAll()
    {
        foreach (Card i in AllStartCards)
        {
            i.End();
        }
        Exit();
    }

    IEnumerator ExitTimer()
    {
        yield return new WaitForSeconds(3);
        Exit();
    }

    public void Win()
    {
        _winPanel.SetActive(true);
        _loseText.gameObject.SetActive(false);
        _winText.gameObject.SetActive(true);

        //if (_levelName == "Easy")
        //{
        //    if (_attemps < YandexGame.savesData.RecordEasy)
        //    {
        //        YandexGame.NewLeaderboardScores(_levelName, _attemps);
        //        YandexGame.savesData.RecordEasy = _attemps;
        //        YandexGame.SaveProgress();
        //    }
        //}
        //if (_levelName == "Normal")
        //{
        //    if (_attemps < YandexGame.savesData.RecordNormal)
        //    {
        //        YandexGame.NewLeaderboardScores(_levelName, _attemps);
        //        YandexGame.savesData.RecordNormal = _attemps;
        //        YandexGame.SaveProgress();
        //    }
        //}
        //if (_levelName == "Hard")
        //{
        //    if (_attemps < YandexGame.savesData.RecordHard)
        //    {
        //        YandexGame.NewLeaderboardScores(_levelName, _attemps);
        //        YandexGame.savesData.RecordHard = _attemps;
        //        YandexGame.SaveProgress();
        //    }
        //}

    }

    public void Lose()
    {
        _winPanel.SetActive(true);
        _loseText.gameObject.SetActive(true);
        _winText.gameObject.SetActive(false);
    }

    

    public void Exit()
    {
        if (yandexGame.timer >= yandexGame.infoYG.fullscreenAdInterval)
        {
            Debug.Log("Open");
            YandexGame.FullscreenShow();
            
        }
        else
        {
            Debug.Log("Not open");
            SceneManager.LoadScene(0);
        }
    }
}
