namespace PinionCore.Showcases.Protocol.Lobby
{
    public interface IUser
    {
        event System.Action AlreadyLoggedInElsewhereEvent;
        event System.Action InLobbyEvent; // test
        PinionCore.Remote.Notifier<ILogin> Notifier { get; }
    }
}
