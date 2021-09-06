using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Unwritten.Network
{
    public class NetworkController : NetworkBehaviour
    {
        private NetworkVariableVector3 _position = new(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public void Move(Vector3 pos)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                transform.position = pos;
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
