using PinionCore.Remote;
using System.ComponentModel.DataAnnotations;

namespace PinionCore.Showcases.Lobby
{
    public class Entry : PinionCore.Remote.IEntry
    {
        readonly Validator _Validator;
        readonly Dictionary<IBinder, User> _Users;
        readonly PinionCore.Utility.Updater _UsersUpdater;
        public Entry()
        {
            _Validator = new Validator();
            _Users = new Dictionary<IBinder, User>();
            _UsersUpdater = new PinionCore.Utility.Updater();            
        }
        void IBinderProvider.RegisterClientBinder(IBinder binder)
        {
            var user = new User(binder, _Validator);
            _Users.Add(binder, user);            
            _UsersUpdater.Add(user);
        }

        void IBinderProvider.UnregisterClientBinder(IBinder binder)
        {
            var user = _Users[binder];
            _Users.Remove(binder);
            _UsersUpdater.Remove(user);
        }

        void IEntry.Update()
        {
            _UsersUpdater.Working();
        }
    }
}
