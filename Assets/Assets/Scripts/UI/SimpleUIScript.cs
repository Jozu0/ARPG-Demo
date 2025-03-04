using UnityEngine;
using UnityEngine.UI;

public class SimpleUIScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public GameObject book;
    public Sprite overedBookImage;
    public Sprite normalBookImage;

    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BookOver(){
        book.GetComponent<Image>().sprite = overedBookImage;
    }
    
    public void BookNormal(){
        book.GetComponent<Image>().sprite = normalBookImage;
    }

}
