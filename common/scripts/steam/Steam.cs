using System;
using System.IO;
using System.Runtime.InteropServices;
using Godot;
using Steamworks;

[GlobalClass]
public partial class Steam : Node {
    public static readonly string _appId = "1746520";
    public static Steam Instance { get; private set; }

    protected CSteamID _userId;
    protected CSteamID _lobbyId;


    // Lobby Callbacks
    private Callback<LobbyCreated_t> _lobbyCreated;
    private Callback<LobbyEnter_t> _lobbyEnter;
    private Callback<LobbyDataUpdate_t> _lobbyDataUpdate;
    private Callback<LobbyChatMsg_t> _lobbyChatMsg;
    private Callback<LobbyChatUpdate_t> _lobbyChatUpdate;
    private Callback<LobbyGameCreated_t> _lobbyGameCreated;
    private Callback<LobbyInvite_t> _lobbyInvite;
    private Callback<LobbyKicked_t> _lobbyKicked;
    private Callback<LobbyMatchList_t> _lobbyMatchList;
    private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;


    // P2P Callbacks
    private Callback<P2PSessionConnectFail_t> _p2PSessionConnectFail;
    private Callback<P2PSessionRequest_t> _p2PSessionRequest;
    private Callback<P2PSessionState_t> _p2PSessionState;

    public Steam() {
        Instance = this;
        Init();
    }

    private void Init() {
        NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "steam_api64.dll"));
        SteamAPI.Init();
        _userId = SteamUser.GetSteamID();
        TreeExited += Shutdown;

        
        // Lobby Callbacks
        _lobbyCreated = Callback<LobbyCreated_t>.Create(OnCreateLobby);
        _lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        _lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        _gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(_OnGameLobbyJoinRequested);
        _lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnDataUpdate);

        // P2P Callbacks
        _p2PSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
        
    }

    private void Shutdown() {
        SteamAPI.Shutdown();
    }

    public sealed override void _Process(double delta) {
        SteamAPI.RunCallbacks();

        // P2P Packet Handling
        if (SteamNetworking.IsP2PPacketAvailable(out uint packetSize)) {
            byte[] packet = new byte[packetSize];
            if (SteamNetworking.ReadP2PPacket(packet, packetSize, out uint bytesRead, out CSteamID user)) {
                NetworkPacket networkPacket = NetworkPacket.FromBytes(packet);
                _OnPeer2PeerPacketReceived(user, networkPacket);
            }
        }  

        _Process((float)delta);
    }


    /// ========================================
    /// Virtual Functions for Callbacks
    /// ========================================


    /// <summary>
    /// Callback for when the lobby receives data updates from steam
    /// </summary>
    /// <param name="param"></param>
    protected virtual void _OnDataUpdate(LobbyDataUpdate_t param) {
        
    }

    /// <summary>
    /// Callback for the user requests to join a lobby.
    /// This is called when the user Joins game, or accepts a friends game invite
    /// </summary>
    /// <param name="param"></param>
    /// <returns>True if the game should be joined, false if the game should be ignored</returns>
    protected virtual void _OnGameLobbyJoinRequested(GameLobbyJoinRequested_t param) {
        
    }

    /// <summary>
    /// Callback for when the player enters a lobby. Runs for both the host and the client
    /// </summary>
    protected virtual void _OnLobbyEnter() {
        
    }

    /// <summary>
    /// Callback for when a lobby is created
    /// </summary>
    protected virtual void _OnLobbyCreated() {
        
    }

    /// <summary>
    /// Callback for when another user joins the lobby
    /// </summary>
    /// <param name="user"></param>
    protected virtual void _OnLobbyMemberJoined(CSteamID user) {
        
    }

    /// <summary>
    /// Callback for when another user leaves the lobby
    /// </summary>
    /// <param name="user"></param>
    protected virtual void _OnLobbyMemberLeft(CSteamID user) {
        
    }

    /// <summary>
    /// Callback for when the player exits the lobby
    /// </summary>
    protected virtual void _OnLobbyExited() {

    }

    /// <summary>
    /// Callback for when a packet is received from another user in the p2p network
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="packet"></param>
    protected virtual void _OnPeer2PeerPacketReceived(CSteamID sender, NetworkPacket packet) {

    }

    protected virtual void _Process(float delta) {

    }

}
