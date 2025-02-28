using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera cam;
    public GameObject player;
    
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newCameraLocation = new Vector3(player.transform.position.x,player.transform.position.y+2.5f, -1.6f);
        cam.transform.position = newCameraLocation;
    }
}
