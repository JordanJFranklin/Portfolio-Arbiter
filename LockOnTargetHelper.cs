using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnTargetHelper : MonoBehaviour
{
    public GameObject lockOnEmpty;
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(lockOnEmpty != null && Player != null)
        {
            lockOnEmpty.transform.position = transform.position;
            lockOnEmpty.transform.SetParent(null);
        }
    }

    public void SetLockOn(GameObject newPlayer)
    {
        Player = newPlayer;
    }
        

}
