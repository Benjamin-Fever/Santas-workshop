using System;
using Godot;

public enum PacketType {
    None,
    PlayerInput,
    PlayerData,
    NetworkRequest
}

public class NetworkPacket {
    public byte[] header;
    public byte[] body;

    public NetworkPacket() {}

    public NetworkPacket(byte[] header, byte[] body){
        this.header = header;
        this.body = body;
    }

    public byte[] ToBytes(){
        byte[] packet = new byte[header.Length + body.Length + 2];
        
        packet[0] = (byte)header.Length;
        packet[1] = (byte)body.Length;

        Array.Copy(header, 0, packet, 2, header.Length);
        Array.Copy(body, 0, packet, 2 + header.Length, body.Length);

        return packet;
    }

    public PacketType GetPacketType(){
        return (PacketType)(int)header[0];
    }    


}

public class PlayerInputPacket : NetworkPacket {
    public enum InputType {
        Move,
        Interact,
        Action
    }

    public PlayerInputPacket(byte[] header, byte[] body) : base(header, body) {}

    public PlayerInputPacket(InputType inputType, Vector2? direction = null) {
        Vector2 dir = direction ?? Vector2.Zero;

        header = new byte[] {
            (int)PacketType.PlayerInput
        };

        Vector2I dirI = new Vector2I((int)dir.X, (int)dir.Y);

        body = new byte[] {
            (byte)inputType,
            (byte)dirI.X,
            (byte)dirI.Y
        };
    }

    public InputType GetInputType() {
        return (InputType)body[0];
    }

    public Vector2 GetDirection() {
        return new Vector2((sbyte)body[1], (sbyte)body[2]);
    }
}

public class PlayerDataPacket : NetworkPacket {

    public PlayerDataPacket(byte[] header, byte[] body) : base(header, body) {}

    public PlayerDataPacket(Vector3 position, Vector3 rotation) {

        header = new byte[] { 
            (byte)PacketType.PlayerData
        };
        
        Vector3I pos = new Vector3I((int)position.X, (int)position.Y, (int)position.Z);
        Vector3I rot = new Vector3I((int)rotation.X, (int)rotation.Y, (int)rotation.Z);


        body = new byte[] {
            (byte)pos.X,
            (byte)pos.Y,
            (byte)pos.Z,

            (byte)rot.X,
            (byte)rot.Y,
            (byte)rot.Z
        };
    }

    public Vector3 GetPosition() {
        return new Vector3(body[0], body[1], body[2]);
    }

    public Vector3 GetRotation() {
        return new Vector3(body[3], body[4], body[5]);
    }

}

public class NetworkRequestPacket : NetworkPacket {
    public enum RequestType {
        PlayerData
    }

    public NetworkRequestPacket(RequestType requestType) {
        header = new byte[] { 
            (byte)PacketType.NetworkRequest
        };

        body = new byte[] {
            (byte)requestType
        };
    }

    public RequestType GetRequestType() {
        return (RequestType)body[0];
    }
}