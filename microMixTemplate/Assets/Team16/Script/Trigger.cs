using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace team16
{
    public class Trigger : MicrogameEvents
    {
        public GameObject[] environment;
        public List<GameObject> endGame;

        // Start is called before the first frame update
        public void Start()
        {
            environment = GameObject.FindGameObjectsWithTag("Tag0");

            endGame = environment.ToList<GameObject>();

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log(other.gameObject.name);
            endGame.Remove(other.gameObject);

        }


        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);
        }
    }
}