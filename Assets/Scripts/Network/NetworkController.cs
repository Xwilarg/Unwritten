using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using Unwritten.Player;

namespace Unwritten.Network
{
    public class NetworkController : NetworkBehaviour
    {
        private void Start()
        {
            if (!IsLocalPlayer)
            {
                // If we are not local player, we can disable FPS controller and player camera
                GetComponent<FirstPersonController>().enabled = false;
                GetComponentInChildren<Camera>().enabled = false;
                GetComponentInChildren<AudioListener>().enabled = false;
            }
        }

        private NetworkVariableVector3 _position = new(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public void Move(Vector3 pos)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _position.Value = pos;
            }
            else
            {
                SubmitPositionRequestServerRpc(pos);
            }
        }

        [ServerRpc]
        private void SubmitPositionRequestServerRpc(Vector3 pos)
        {
            _position.Value = pos;
        }

        private void Update()
        {
            transform.position = _position.Value;
        }
    }
}
