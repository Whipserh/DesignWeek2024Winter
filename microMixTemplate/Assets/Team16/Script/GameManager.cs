using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MicrogameInputEvents

{

    public static int items;
    // Start is called before the first frame update
    void Start()
    {
        items = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if(items == 0)
        {
                 ReportGameCompletedEarly();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       // if (collision.tag == "Tag1")
        {
            items--;
        }
    }
}
