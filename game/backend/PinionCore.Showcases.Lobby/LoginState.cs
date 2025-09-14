using PinionCore.Remote;
using PinionCore.Showcases.Protocol.Lobby;
using PinionCore.Utility;

namespace PinionCore.Showcases.Lobby
{
    internal class LoginState : IStatus , PinionCore.Showcases.Protocol.Lobby.ILogin
    {
        
        

        public event System.Action<string> DoneEvent;

        readonly ICollection<ILogin> _Logins;
        public LoginState(ICollection<ILogin> logins)
        {
            _Logins = logins;
        }

        void IStatus.Enter()
        {
            _Logins.Add(this);
        }

        void IStatus.Leave()
        {
            _Logins.Remove(this);
        }

        Value ILogin.Login(string username)
        {
            DoneEvent(username);
            return new Value();
        }

        void IStatus.Update()
        {
            
        }
    }
}