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
    protected CSteamID clientID;



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

    private void OnLobbyEnter(LobbyEnter_t param){
        OnLobbyConnected(new CSteamID(){ m_SteamID = param.m_ulSteamIDLobby });
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t param){
        SteamMatchmaking.JoinLobby(param.m_steamIDLobby);
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

    private void DisconnectUser(CSteamID lobbyID, CSteamID userID){
        if (userID == clientID)
            OnLobbyDisconnected(lobbyID);
        else
            OnLobbyMemberDisconnected(lobbyID, userID);
    }

    private void OnLobbyDataUpdate(LobbyDataUpdate_t param){
        
    }

    protected void BroadcastLobbyData(CSteamID lobbyID, byte[] data){
        SteamMatchmaking.SendLobbyChatMsg(lobbyID, data, data.Length);
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

    protected List<CSteamID> GetLobbyMembers(CSteamID lobbyID){
        List<CSteamID> members = new List<CSteamID>();
        int count = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        for (int i = 0; i < count; i++){
            members.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i));
        }
        return members;   
    }


    /// <summary>
    /// Called when the lobby is connected
    /// </summary>
    /// <param name="lobbyID"></param>
    protected virtual void OnLobbyConnected(CSteamID lobbyID){

    }

    /// <summary>
    /// Called when the lobby is disconnected
    /// </summary>
    /// <param name="lobbyID"></param>
    protected virtual void OnLobbyDisconnected(CSteamID lobbyID){

    }

    /// <summary>
    /// Called when a member is connected to the lobby
    /// </summary>
    /// <param name="memberID"></param>
    protected virtual void OnLobbyMemberConnected(CSteamID lobbyID, CSteamID memberID){

    }

    /// <summary>
    /// Called when a member is disconnected from the lobby
    /// </summary>
    /// <param name="memberID"></param>
    protected virtual void OnLobbyMemberDisconnected(CSteamID lobbyID, CSteamID memberID){

    }

    /// <summary>
    /// Called when a user sends a message to the lobby
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="data"></param>
    protected virtual void OnLobbyUserMsg(CSteamID lobbyID, CSteamID userID, byte[] data){

    }
}
