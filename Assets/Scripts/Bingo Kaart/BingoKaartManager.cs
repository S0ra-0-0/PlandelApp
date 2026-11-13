using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoKaartManager : MonoBehaviour
{
    public static BingoKaartManager instance;
    private int bingoSeed = 2;
    [SerializeField] private List<GameObject> bingoKaartCards;
    [SerializeField] private List<Sprite> backUpBingoKaartTextures;

    [SerializeField] private List<Sprite> bingoKaartTextures;


    private void Awake()
    {
        if(instance == null && instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private int randomTexture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Random.InitState(bingoSeed);

        for (int i = 0; i < bingoKaartCards.Count; i++)
        {
            randomTexture = Random.Range(0, bingoKaartTextures.Count);
            bingoKaartCards[i].GetComponent<Image>().sprite = bingoKaartTextures[randomTexture];
            bingoKaartTextures.Remove(bingoKaartTextures[randomTexture]);
        }
        
        Debug.Log(bingoSeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSeed(int seed)
    {
        bingoSeed = seed;
        Random.InitState(bingoSeed);

        for(int i = 0; i < backUpBingoKaartTextures.Count; i++)
        {
            bingoKaartTextures.Add(backUpBingoKaartTextures[i]);
            Debug.Log("Work");
        }

        for (int i = 0; i < bingoKaartCards.Count; i++)
        {
            Debug.Log("work");
            randomTexture = Random.Range(0, bingoKaartTextures.Count);
            bingoKaartCards[i].GetComponent<Image>().sprite = bingoKaartTextures[randomTexture];
            bingoKaartTextures.Remove(bingoKaartTextures[randomTexture]);
        }

        Debug.Log(bingoSeed);
    }

}
