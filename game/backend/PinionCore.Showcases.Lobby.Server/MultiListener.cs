using System;

namespace PinionCore.Showcases.Lobby.Server
{
    internal sealed class MultiListener : PinionCore.Remote.Soul.IListenable, IDisposable
    {
        private event System.Action<PinionCore.Network.IStreamable> _Enter;
        private event System.Action<PinionCore.Network.IStreamable> _Leave;

        event System.Action<PinionCore.Network.IStreamable> PinionCore.Remote.Soul.IListenable.StreamableEnterEvent
        {
            add { _Enter += value; }
            remove { _Enter -= value; }
        }

        event System.Action<PinionCore.Network.IStreamable> PinionCore.Remote.Soul.IListenable.StreamableLeaveEvent
        {
            add { _Leave += value; }
            remove { _Leave -= value; }
        }

        private readonly System.Collections.Generic.List<PinionCore.Remote.Soul.IListenable> _inners = new();

        public void Add(PinionCore.Remote.Soul.IListenable listenable)
        {
            listenable.StreamableEnterEvent += _OnEnter;
            listenable.StreamableLeaveEvent += _OnLeave;
            _inners.Add(listenable);
        }

        private void _OnEnter(PinionCore.Network.IStreamable s) => _Enter?.Invoke(s);
        private void _OnLeave(PinionCore.Network.IStreamable s) => _Leave?.Invoke(s);

        public void Dispose()
        {
            foreach (var l in _inners)
            {
                try
                {
                    l.StreamableEnterEvent -= _OnEnter;
                    l.StreamableLeaveEvent -= _OnLeave;
                }
                catch { }
            }
            _inners.Clear();
        }
    }
}

