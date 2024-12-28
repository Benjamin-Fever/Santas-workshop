using Godot;
using Steamworks;
using System;
using System.Collections.Generic;

public partial class Steam : Node {
    /// ========================================
    /// Steam P2P Callback Processing
    /// ========================================
    private void OnP2PSessionRequest(P2PSessionRequest_t param) {
        CSteamID user = param.m_steamIDRemote;
        if (!GetLobbyMembers().Contains(user)) return;

        if (IsHost()) {
            AcceptP2PUser(user);
            return;
        }
        
        if (user.m_SteamID == ulong.Parse(GetLobbyData("host_id"))) {
            AcceptP2PUser(user);
        }
    }

    


    /// ========================================
    /// Steam peer to peer helper functions
    /// ========================================
    public static bool AcceptP2PUser(CSteamID user) {
        return SteamNetworking.AcceptP2PSessionWithUser(user);
    }

    public static void SendP2PPacket(CSteamID reciever, byte[] data) {
        NetworkPacket networkPacket = new NetworkPacket(reciever, NetworkPacket.PacketType.Data, data);
        byte[] packet = networkPacket.ToBytes();
        SteamNetworking.SendP2PPacket(reciever, packet, (uint)packet.Length, EP2PSend.k_EP2PSendUnreliable);
    }

    public static void SendP2PPacket(NetworkPacket networkPacket) {
        byte[] packet = networkPacket.ToBytes();
        SteamNetworking.SendP2PPacket(networkPacket.reciever, packet, (uint)packet.Length, EP2PSend.k_EP2PSendUnreliable);
    }

    // Steam User Functions
    public static List<CSteamID> GetFriends() {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        List<CSteamID> friends = new List<CSteamID>();
        for (int i = 0; i < friendCount; i++) {
            CSteamID friend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            friends.Add(friend);
        }
        return friends;
    }
}