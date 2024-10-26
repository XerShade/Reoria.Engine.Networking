using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Reoria.Engine.Networking.NetListeners;

public class AndroidNetListener : ClientNetListener
{
    public AndroidNetListener(ILogger<NetListener> logger, IConfigurationRoot configuration) : base(logger, configuration)
    {
    }
}
