namespace PinionCore.Showcases.Texas.Protocol
{
    public static partial class ProtocolCreater
    {
        public static PinionCore.Remote.IProtocol Create()
        {
            PinionCore.Remote.IProtocol protocol = null;
            _Create(ref protocol);
            return protocol;
        }

        /*
			Create a partial method as follows.
        */
        [PinionCore.Remote.Protocol.Creater]
        static partial void _Create(ref PinionCore.Remote.IProtocol protocol);
    }
}
