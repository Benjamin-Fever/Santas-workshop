using Godot;
using Steamworks;
using System;
using System.Collections.Generic;

public partial class Steam : Node {

    [Signal] public delegate void LobbyCreatedEventHandler();
    [Signal] public delegate void LobbyEnteredEventHandler();
    [Signal] public delegate void LobbyDataUpdateEventHandler(ulong userId, ulong lobbyId, bool success);
    [Signal] public delegate void LobbyMemberJoinedEventHandler(ulong user);
    [Signal] public delegate void LobbyMemberLeftEventHandler(ulong user);
    

    /// ========================================
    /// Steam Lobby Callback Processing
    /// ========================================
    private void OnLobbyEntered(LobbyEnter_t param) {
        _lobbyId = new CSteamID(param.m_ulSteamIDLobby);

        SteamFriends.SetRichPresence("lobby_id", _lobbyId.ToString());
        SteamFriends.SetRichPresence("game_id", _appId);

        _OnLobbyEnter();
        EmitSignal(SignalName.LobbyEntered);
    }

    private void OnCreateLobby(LobbyCreated_t param) {
        _lobbyId = new CSteamID(param.m_ulSteamIDLobby);

        SetLobbyData("host_id", _userId.ToString());
        SetLobbyData("game_id", _appId);

        SteamFriends.SetRichPresence("lobby_id", _lobbyId.ToString());
        SteamFriends.SetRichPresence("game_id", _appId);

        _OnLobbyCreated();
        EmitSignal(SignalName.LobbyCreated);
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t param) {
        switch ((EChatMemberStateChange)param.m_rgfChatMemberStateChange) {
            case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
                CSteamID joinedUser = new CSteamID(param.m_ulSteamIDUserChanged);
                if (joinedUser == _userId) break;
                EmitSignal(SignalName.LobbyMemberJoined, joinedUser.m_SteamID);
                _OnLobbyMemberJoined(joinedUser);
                break;
            case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
                CSteamID userLeft = new CSteamID(param.m_ulSteamIDUserChanged);
                if (userLeft == _userId) {
                    _OnLobbyExited();
                } else {
                    EmitSignal(SignalName.LobbyMemberLeft, userLeft.m_SteamID);
                    _OnLobbyMemberLeft(userLeft);
                }
                break;
        }
    }

    private void OnDataUpdate(LobbyDataUpdate_t param) {
        _OnDataUpdate(param);
        EmitSignal(SignalName.LobbyDataUpdate, param.m_ulSteamIDMember, param.m_ulSteamIDLobby, param.m_bSuccess);
    }




    /// ========================================
    /// Steam lobby helper functions
    /// ========================================
    public static void CreateLobby(ELobbyType type = ELobbyType.k_ELobbyTypeFriendsOnly, int maxMembers = 4) {
        SteamMatchmaking.CreateLobby(type, maxMembers);
    }

    public static void JoinLobby(CSteamID lobbyId) {
        SteamMatchmaking.JoinLobby(lobbyId);
    }

    public static void LeaveLobby() {
        CSteamID hostId = GetLobbyHost();
        if (hostId == Instance._userId && GetLobbyMemberCount() > 1) {
            foreach (CSteamID member in GetLobbyMembers()) {
                if (member == Instance._userId) continue;
                SetLobbyData("host_id", member.ToString());
                break;
            }
        }

        SteamFriends.ClearRichPresence();
        SteamMatchmaking.LeaveLobby(Instance._lobbyId);
    }



    public static void SetLobbyType(ELobbyType type, CSteamID lobbyId = default) {
        if (lobbyId == default) {
            lobbyId = Instance._lobbyId;
        }
        SteamMatchmaking.SetLobbyType(lobbyId, type);
    }

    public static void SetLobbyMaxMembers(int maxMembers, CSteamID lobbyId = default) {
        if (lobbyId == default) {
            lobbyId = Instance._lobbyId;
        }

        if (SteamMatchmaking.SetLobbyMemberLimit(lobbyId, maxMembers)) {
            GD.Print("Set lobby member limit to " + maxMembers);
        }
        else {
            GD.Print("Failed to set lobby member limit to " + maxMembers);
        }
    }


    public static void SetLobbyData(string key, string value) {
        SteamMatchmaking.SetLobbyData(Instance._lobbyId, key, value);
    }

    public static string GetLobbyData(string key, CSteamID lobbyId = default) {
        if (lobbyId == default) {
            lobbyId = Instance._lobbyId;
        }
        return SteamMatchmaking.GetLobbyData(lobbyId, key);
    }

    public static void SetLobbyMemberData(string key, string value) {
        SteamMatchmaking.SetLobbyMemberData(Instance._lobbyId, key, value);
    }

    public static string GetLobbyMemberData(string key, CSteamID user = default, CSteamID lobbyId = default) {
        if (lobbyId == default) {
            lobbyId = Instance._lobbyId;
        }
        if (user == default) {
            user = Instance._userId;
        }
        return SteamMatchmaking.GetLobbyMemberData(lobbyId, user, key);
    }

    public static int GetLobbyMaxMembers(CSteamID lobbyId = default) {
        if (lobbyId == default) {
            lobbyId = Instance._lobbyId;
        }
        return SteamMatchmaking.GetLobbyMemberLimit(lobbyId);
    }

    public static int GetLobbyMemberCount(CSteamID lobbyId = default) { 
        if (lobbyId == default) {
            lobbyId = Instance._lobbyId;
        }
        return SteamMatchmaking.GetNumLobbyMembers(lobbyId);
    }

    public static List<CSteamID> GetLobbyMembers(CSteamID lobbyId = default) {
        if (lobbyId == default) {
            lobbyId = Instance._lobbyId;
        }
        int memberCount = SteamMatchmaking.GetNumLobbyMembers(Instance._lobbyId);
        List<CSteamID> members = new List<CSteamID>();
        for (int i = 0; i < memberCount; i++) {
            CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(Instance._lobbyId, i);
            members.Add(member);
        }
        return members;
    }

    public static CSteamID GetLobbyHost() {
        return new CSteamID(ulong.Parse(GetLobbyData("host_id")));
    }

    public static bool IsHost() {
        return GetLobbyHost() == Instance._userId; 
    }

    public static HashSet<CSteamID> GetFriendsLobbys() {
        HashSet<CSteamID> lobbys = new HashSet<CSteamID>();
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < friendCount; i++) {
            CSteamID friend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            string lobbyId = SteamFriends.GetFriendRichPresence(friend, "lobby_id");
            string gameId = SteamFriends.GetFriendRichPresence(friend, "game_id");
            if (SteamFriends.GetFriendPersonaState(friend) == EPersonaState.k_EPersonaStateInvisible) continue;

            if (lobbyId != "" && gameId == _appId) {
                SteamMatchmaking.RequestLobbyData(new CSteamID(ulong.Parse(lobbyId)));
                lobbys.Add(new CSteamID(ulong.Parse(lobbyId)));
            }
        }
        return lobbys;
    }

    public static void InviteFriendsOverlay(){
        SteamFriends.ActivateGameOverlayInviteDialog(Instance._lobbyId);
    }
}