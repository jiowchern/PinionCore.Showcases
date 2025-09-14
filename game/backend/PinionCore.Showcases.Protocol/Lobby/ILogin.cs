using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinionCore.Showcases.Protocol.Lobby
{
    public interface ILogin
    {
        PinionCore.Remote.Value Login(string username);
    }
}
