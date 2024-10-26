using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Reoria.Engine.Networking.NetListeners;

public class AndroidNetEventListener : ClientNetEventListener
{
    public AndroidNetEventListener(ILogger<NetEventListener> logger, IConfigurationRoot configuration) : base(logger, configuration)
    {
    }
}
