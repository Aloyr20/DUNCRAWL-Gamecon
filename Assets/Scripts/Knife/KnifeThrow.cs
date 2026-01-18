using UnityEngine;

public class KnifeThrow : MonoBehaviour
{
    public GameObject Knife;
    public Transform player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            Instantiate(Knife, transform.position, Quaternion.Euler(0, player.eulerAngles.y + 90, 90));
        }
    }
}
