using System;

namespace Game.CubeGame.GameMessage
{
    public interface IGameMessageSystem
    {
        event Action<string> OnMessageReceived;

        void SendMessage(string message);
    }
}
