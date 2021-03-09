using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

namespace GetCloser
{
    public class PlayerController : MonoBehaviourPun, IPunObservable
    {
        bool checker = false;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine)
            {
                Check();
            }
        }



        void Check()
        {
            if (checker)
            {
                checker = false;
                Debug.Log("Player Instantiated");
            }

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsReading)
            {

            }
            else if (stream.IsWriting)
            {

            }
            
        }
    }


    
}
