using System;
using System.Linq;
using Steamworks;

public class NetworkPacket {
    public enum PacketType {
        Connection,
        Data
    }
    public CSteamID reciever;
    public PacketType packetType;
    public byte[] body;

    public NetworkPacket(CSteamID reciever, PacketType packetType, byte[] body = null) {
        this.reciever = reciever;
        this.body = body;
    }

    public byte[] ToBytes() {
        byte[] header = BitConverter.GetBytes(reciever.m_SteamID);
        header = header.Concat(BitConverter.GetBytes((int)packetType)).ToArray();

        byte[] packet = new byte[header.Length + body.Length + 2];

        packet[0] = (byte)header.Length;
        packet[1] = (byte)body.Length;

        Array.Copy(header, 0, packet, 2, header.Length);
        Array.Copy(body, 0, packet, 2 + header.Length, body.Length);

        return packet;
    }

    public static NetworkPacket FromBytes(byte[] packet) {
        byte headerLength = packet[0];
        byte bodyLength = packet[1];

        byte[] header = new byte[headerLength];
        byte[] body = new byte[bodyLength];

        Array.Copy(packet, 2, header, 0, headerLength);
        Array.Copy(packet, 2 + headerLength, body, 0, bodyLength);

        CSteamID reciever = new CSteamID(BitConverter.ToUInt64(header, 0));
        PacketType packetType = (PacketType)BitConverter.ToInt32(header, 8);

        return new NetworkPacket(reciever, packetType, body);
    }

    public static NetworkPacket ConnectionPacket(CSteamID reciever) {
        NetworkPacket packet = new NetworkPacket(reciever, PacketType.Connection);
        return packet;
    }
}
