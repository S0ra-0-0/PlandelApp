using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoKaartManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> bingoKaartCards;
    [SerializeField] private List<Sprite> bingoKaartTextures;

    private int randomTexture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < bingoKaartCards.Count; i++)
        {
            randomTexture = Random.Range(0, bingoKaartTextures.Count);
            bingoKaartCards[i].GetComponent<Image>().sprite = bingoKaartTextures[randomTexture];
            bingoKaartTextures.Remove(bingoKaartTextures[randomTexture]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPress()
    {
    }
}
