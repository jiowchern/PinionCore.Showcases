using PinionCore.Remote;
using PinionCore.Showcases.Protocol.Lobby;
using PinionCore.Utility;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace PinionCore.Showcases.Lobby
{
    class User : PinionCore.Utility.IUpdatable , IUser
    {
        readonly Validator _Validator;
        public readonly IBinder Binder;

        string _Account;
        readonly PinionCore.Utility.StatusMachine _Machine;

        readonly NotifiableCollection<ILogin> _Logins;
        readonly Remote.Notifier<ILogin> _LoginNotifier;
        Remote.Notifier<ILogin> IUser.Notifier => _LoginNotifier;

        public User(IBinder binder , Validator  validator)
        {
            Binder = binder;
            _Machine = new StatusMachine();
            _Validator = validator;            
            _Account = "";
            _Logins = new NotifiableCollection<ILogin>();
            _LoginNotifier = new Remote.Notifier<ILogin>(_Logins);
        }

        event Action _AlreadyLoggedInElsewhereEvent;
        event Action IUser.AlreadyLoggedInElsewhereEvent
        {
            add
            {
                _AlreadyLoggedInElsewhereEvent += value;
            }

            remove
            {
                _AlreadyLoggedInElsewhereEvent -= value;
            }
        }

        event Action _InLobbyEvent;
        event Action IUser.InLobbyEvent
        {
            add
            {
                _InLobbyEvent += value;
            }

            remove
            {
                _InLobbyEvent -= value;
            }
        }

        void IBootable.Launch()
        {
            _Validator.DuplicateEvent += _CheckDuplicate;
            _ToLogin();
        }

        void IBootable.Shutdown()
        {
            _Validator.DuplicateEvent -= _CheckDuplicate;
        }

        bool IUpdatable.Update()
        {
            return true;
        }
        

        void _ToLogin()
        {
            _Validator.Logout(_Account);
            _Account = "";
            
            var state = new LoginState(_Logins.Items);            
            state.DoneEvent += (account)=> {                
                _ToLobby(account); 
            };
            _Machine.Push(state);
        }

        private void _ToLobby(string account)
        {
            _Validator.Login(account);

            _InLobbyEvent();
        }

        private void _CheckDuplicate(string account)
        {
            if (_Account != account)
            {
                return;
            }
            _AlreadyLoggedInElsewhereEvent();
            _ToLogin() ;
        }
    }
}
