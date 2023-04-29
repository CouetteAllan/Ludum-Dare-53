using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Spot spot = collision.gameObject.GetComponent<Spot>();
        if(spot != null)
        {
            spot.parentBuilding.ChangeState(State.ColoredP1);
        }
    }
}
