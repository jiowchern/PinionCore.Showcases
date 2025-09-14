
namespace PinionCore.Showcases.Lobby
{
    class Validator
    {
        public System.Action<string> DuplicateEvent;

        readonly System.Collections.Generic.HashSet<string> _Accounts;
        public Validator()
        {
            _Accounts = new HashSet<string>();
        }
        internal void Login(string account)
        {

            if(_Accounts.TryGetValue(account , out _))
            {
                DuplicateEvent(account);                
            }

            _Accounts.Add(account);
        }

        public void Logout(string account)
        {
            _Accounts.Remove(account);
        }
    }
}
