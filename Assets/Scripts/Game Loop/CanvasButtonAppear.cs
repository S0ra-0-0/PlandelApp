using UnityEngine;

public class CanvasButtonAppear : MonoBehaviour
{
    [SerializeField] private GameObject Button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateButton()
    {
        Button.SetActive(true);
    }
}
