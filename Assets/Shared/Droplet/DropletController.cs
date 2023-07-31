using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Vase"))
            {
                Debug.Log("true");
                var controller = collision.gameObject.GetComponent<Growth>();
                controller.SetGrouwth(controller.growth + 0.1f);
                Destroy(gameObject);
            }
        }
    }
}
