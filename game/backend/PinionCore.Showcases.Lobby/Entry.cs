using PinionCore.Remote;
using System.ComponentModel.DataAnnotations;

namespace PinionCore.Showcases.Lobby
{
    public class Entry : PinionCore.Remote.IEntry
    {
        readonly Validator _Validator;
        readonly Dictionary<IBinder, User> _Users;
        readonly Dictionary<IBinder, ISoul> _Souls;
        readonly PinionCore.Utility.Updater _UsersUpdater;
        public Entry()
        {
            _Validator = new Validator();
            _Users = new Dictionary<IBinder, User>();
            _Souls = new Dictionary<IBinder, ISoul>();
            _UsersUpdater = new PinionCore.Utility.Updater();            
        }
        void IBinderProvider.RegisterClientBinder(IBinder binder)
        {
            var user = new User(binder, _Validator);
            _Users.Add(binder, user);
            var soul = binder.Bind<PinionCore.Showcases.Protocol.Lobby.IUser>(user);
            _Souls[binder] = soul;
            _UsersUpdater.Add(user);
        }

        void IBinderProvider.UnregisterClientBinder(IBinder binder)
        {
            var user = _Users[binder];
            _Users.Remove(binder);
            if (_Souls.TryGetValue(binder, out var soul))
            {
                binder.Unbind(soul);
                _Souls.Remove(binder);
            }
            _UsersUpdater.Remove(user);
        }

        void IEntry.Update()
        {
            _UsersUpdater.Working();
        }
    }
}
