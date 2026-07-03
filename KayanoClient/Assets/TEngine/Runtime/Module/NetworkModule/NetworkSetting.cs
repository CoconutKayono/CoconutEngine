using Fantasy.Network;
using UnityEngine;

namespace TEngine
{
    [CreateAssetMenu(menuName = "TEngine/NetworkSetting", fileName = "NetworkSetting")]
    public class NetworkSetting : ScriptableObject
    {
        [Header("Gate Server")]
        [SerializeField]
        private string gateHost = "127.0.0.1";

        [SerializeField]
        private int gatePort = 20000;

        [SerializeField]
        private NetworkProtocolType protocol = NetworkProtocolType.KCP;

        [SerializeField]
        private int connectTimeoutMs = 5000;

        [Header("Heartbeat")]
        [SerializeField]
        private bool enableHeartbeat = true;

        [SerializeField]
        private int heartbeatIntervalMs = 2000;

        [SerializeField]
        private int heartbeatTimeOutMs = 30000;

        [SerializeField]
        private int heartbeatTimeOutIntervalMs = 5000;

        [SerializeField]
        private int maxPingSamples = 4;

        public string GateHost => gateHost;

        public int GatePort => gatePort;

        public string GateAddress => $"{gateHost}:{gatePort}";

        public NetworkProtocolType Protocol => protocol;

        public int ConnectTimeoutMs => connectTimeoutMs;

        public bool EnableHeartbeat => enableHeartbeat;

        public int HeartbeatIntervalMs => heartbeatIntervalMs;

        public int HeartbeatTimeOutMs => heartbeatTimeOutMs;

        public int HeartbeatTimeOutIntervalMs => heartbeatTimeOutIntervalMs;

        public int MaxPingSamples => maxPingSamples;
    }
}
