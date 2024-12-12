using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Godot;
using Steamworks;

/// <summary>
/// Abstract the steamapi to a more manageable state for Godot
/// </summary>
[GlobalClass]
public abstract partial class SteamNetwork : Node {	
	/// Callback
	private Callback<GameLobbyJoinRequested_t> lobbyJoinRequested;
	private Callback<LobbyEnter_t> lobbyEnter;
	private Callback<LobbyChatUpdate_t> lobbyChatUpdate;
	private Callback<LobbyDataUpdate_t> lobbyDataUpdate;
    private Callback<LobbyChatMsg_t> lobbyChatMsg;

    // Steam Variables
	protected CSteamID lobbyID;
    protected CSteamID clientID;
    protected Dictionary<CSteamID, Node3D> networkPlayers = new Dictionary<CSteamID, Node3D>();

	public override void _EnterTree() {
		NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "steam_api64.dll"));

		try {
			if (SteamAPI.Init()) { Setup(); }
		}
		catch (Exception e) {
			GD.PrintErr(e);
		}
	}

	private void Setup() {
		clientID = SteamUser.GetSteamID();

		lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
		lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        lobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsg);
	}

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t param){
        SteamMatchmaking.JoinLobby(param.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t param){
        CSteamID lobbyID = new CSteamID(){ m_SteamID = param.m_ulSteamIDLobby };
        OnLobbyConnected(lobbyID);

        foreach (CSteamID member in GetLobbyMembers(lobbyID)) {
			if (member == clientID) { continue; }
			OnLobbyMemberConnected(lobbyID, member);
		}
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t param){
        CSteamID lobbyID = new CSteamID(){ m_SteamID = param.m_ulSteamIDLobby };
        CSteamID userID = new CSteamID(){ m_SteamID = param.m_ulSteamIDUserChanged };
        EChatMemberStateChange stateChange = (EChatMemberStateChange)param.m_rgfChatMemberStateChange;

        switch (stateChange){
            case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
                OnLobbyMemberConnected(lobbyID, userID);
                break;
            case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
                DisconnectUser(lobbyID, userID);
                break;
            case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
                DisconnectUser(lobbyID, userID);
                break;
            case EChatMemberStateChange.k_EChatMemberStateChangeKicked:
                DisconnectUser(lobbyID, userID);
                break;
            case EChatMemberStateChange.k_EChatMemberStateChangeBanned:
                DisconnectUser(lobbyID, userID);
                break;
        }
    }

    private void OnLobbyChatMsg(LobbyChatMsg_t param) {
        CSteamID lobbyID = new CSteamID(){ m_SteamID = param.m_ulSteamIDLobby };
        CSteamID userID = new CSteamID(){ m_SteamID = param.m_ulSteamIDUser };
        int chatID = (int)param.m_iChatID;
        byte[] data = new byte[4096];
        SteamMatchmaking.GetLobbyChatEntry(lobbyID, chatID, out _, data, data.Length, out EChatEntryType type);

        switch (type){
            case EChatEntryType.k_EChatEntryTypeChatMsg:
                OnLobbyUserMsg(lobbyID, userID, data);
                break;
        }

    }

    private void OnLobbyUserMsg(CSteamID lobbyID, CSteamID userID, byte[] data){
        NetworkPacket networkPacket = FromBytes<NetworkPacket>(data);
        
        switch (networkPacket.GetPacketType()){
            case PacketType.PlayerInput:
                OnLobbyPlayerInput(lobbyID, userID, new PlayerInputPacket(networkPacket.header, networkPacket.body));
                break;
            case PacketType.PlayerData:
                OnLobbyPlayerData(lobbyID, userID, new PlayerDataPacket(networkPacket.header, networkPacket.body));
                break;
        }
    }

    protected T FromBytes<T>(byte[] packet) where T : NetworkPacket {
        int headerLength = packet[0];
        int bodyLength = packet[1];

        byte[] header = new byte[headerLength];
        byte[] body = new byte[bodyLength];

        Array.Copy(packet, 2, header, 0, headerLength);
        Array.Copy(packet, 2 + headerLength, body, 0, bodyLength);

        NetworkPacket networkPacket = new NetworkPacket(header, body);
        return (T)networkPacket;
    }
    

    private void DisconnectUser(CSteamID lobbyID, CSteamID userID){
        if (userID == clientID) OnLobbyDisconnected(lobbyID);
        else OnLobbyMemberDisconnected(lobbyID, userID);
    }

    private void OnLobbyDataUpdate(LobbyDataUpdate_t param){

    }

    protected void BroadcastLobbyData(CSteamID lobbyID, NetworkPacket networkPacket){
        byte[] packet = networkPacket.ToBytes();
        SteamMatchmaking.SendLobbyChatMsg(lobbyID, packet, packet.Length);

    }

    public override void _Process(double delta) {
        SteamAPI.RunCallbacks();
    }


    public override void _ExitTree() {
		try {
			SteamAPI.Shutdown();
		}
		catch (Exception e) {
			GD.PrintErr(e);
		}
	}

    // Helper Functions
    /// <summary>
    /// Get the members of a lobby
    /// </summary>
    /// <param name="lobbyID"></param>
    /// <returns>A list of steam ids</returns>
    protected List<CSteamID> GetLobbyMembers(CSteamID lobbyID){
        List<CSteamID> members = new List<CSteamID>();
        int count = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        for (int i = 0; i < count; i++){
            members.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i));
        }
        return members;   
    }

    // Triggered Events
    protected virtual void OnLobbyConnected(CSteamID lobbyID){}
    protected virtual void OnLobbyDisconnected(CSteamID lobbyID){}
    protected virtual void OnLobbyMemberConnected(CSteamID lobbyID, CSteamID memberID){}
    protected virtual void OnLobbyMemberDisconnected(CSteamID lobbyID, CSteamID memberID){}
    protected virtual void OnLobbyUserInput(CSteamID lobbyID, CSteamID userID, PlayerInputPacket playerInputPacket){}
    protected virtual void OnLobbyPlayerData(CSteamID lobbyID, CSteamID userID, PlayerDataPacket playerDataPacket){}
    protected virtual void OnLobbyPlayerInput(CSteamID lobbyID, CSteamID userID, PlayerInputPacket playerInputPacket){}
}
