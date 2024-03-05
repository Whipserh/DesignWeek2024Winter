using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
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
       //     ReportGameCompletedEarly();
        }
    }

    //private void OnColliderEnter(Collider collision)
    //{
    //    items--;
    //}
}
