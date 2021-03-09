using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace GetCloser.Network
{

    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        string gameVersion = "1";

        [SerializeField]
        private Transform LoginPanel;
        [SerializeField]
        private TMP_Text SuccessMessage;
        [SerializeField]
        private TMP_Text roomIDText;

        [SerializeField]
        private Transform LobbyPanel;

        [SerializeField]
        private TMP_InputField roomIDLogin;

        [SerializeField]
        private Dictionary<string, RoomInfo> cachedRoomList;



        public static void ConnectToPhoton()
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = 1.ToString();
        }


        public override void OnConnectedToMaster()
        {
            Debug.Log("We have authed with photon and CONNECTED!");
            OnSuccessfulConnection();

        }

        private void OnSuccessfulConnection()
        {
            LoginPanel.gameObject.SetActive(false);
            SuccessMessage.gameObject.SetActive(true);
            SuccessMessage.text = "Logged in with the username: " + PhotonNetwork.LocalPlayer.NickName;
            StartCoroutine(GoToLobby());
        }

        public void CreateRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                RoomOptions roomoptions = new RoomOptions();
                roomoptions.MaxPlayers = 2;
                roomoptions.IsVisible = false;
                
                string roomdID = Random.Range(0, 1000).ToString() + Random.Range(0, 1000).ToString();
                PhotonNetwork.CreateRoom(roomdID, roomoptions, TypedLobby.Default);
                roomIDText.gameObject.SetActive(true);
                roomIDText.text = "RoomID is: " + roomdID;
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }

        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {

            UpdateCachedRoomList(roomList);
        }


        public void JoinRoom()
        {
            string roomName = roomIDLogin.text;
            if (!RoomExists(roomName))
            {
                Debug.Log("room created");
                PhotonNetwork.JoinRoom(roomName);
            }
            else
            {
                Debug.Log("room failed to initialize");
            }
        }

        private bool RoomExists(string roomName)
        {
            if (cachedRoomList.ContainsKey(roomName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }


        public override void OnJoinedRoom()
        {
            Debug.Log("You have joined a room");
            GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0, 2, 0), Quaternion.identity, 0, null);
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected because: " + cause.ToString());
        }

        IEnumerator GoToLobby()
        {
            yield return new WaitForSeconds(1f);
            LobbyPanel.gameObject.SetActive(true);

        }

    }
}
