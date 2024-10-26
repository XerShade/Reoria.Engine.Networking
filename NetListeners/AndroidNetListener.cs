using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.NetListeners.Interfaces;

namespace Reoria.Engine.Networking.NetListeners;

public class AndroidNetListener : ClientNetListener, IAndroidNetListener
{
    public AndroidNetListener(ILogger<NetListener> logger, IConfigurationRoot configuration) : base(logger, configuration)
    {
    }
}
