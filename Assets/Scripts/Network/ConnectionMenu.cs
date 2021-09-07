using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine;

namespace Unwritten.Network
{
    public class ConnectionMenu : MonoBehaviour
    {
        [SerializeField]
        private UNetTransport _transport;

        private string _ip = "";

        private void Start()
        {
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
            {
                Debug.Log("Headless mode detected, starting as server mode");
                NetworkManager.Singleton.StartServer();
            }
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            _ip = GUILayout.TextField(_ip, 15);
            if (GUILayout.Button("Client"))
            {
                _transport.ConnectAddress = _ip;
                NetworkManager.Singleton.StartClient();
            }
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Mode: " + mode);
        }
    }
}
