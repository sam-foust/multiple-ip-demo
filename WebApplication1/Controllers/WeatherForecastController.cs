using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers
{
    /// <summary>    
    /// docker network create network1
    /// docker network create network2
    /// docker run --network network1 --hostname=5baf37b9ecae --env=ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS=true --env=ASPNETCORE_ENVIRONMENT=Development --env=DOTNET_USE_POLLING_FILE_WATCHER=1 --env=NUGET_PACKAGES=/root/.nuget/fallbackpackages --env=NUGET_FALLBACK_PACKAGES=/root/.nuget/fallbackpackages --env=PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin --env=ASPNETCORE_URLS=http://+:80 --env=DOTNET_RUNNING_IN_CONTAINER=true --env=DOTNET_VERSION=7.0.0 --env=ASPNET_VERSION=7.0.0 --volume=C:\Users\sfoust\vsdbg\vs2017u5:/remote_debugger:rw --volume=C:\Users\sfoust\source\repos\WebApplication1\WebApplication1:/app --volume=C:\Users\sfoust\source\repos\WebApplication1:/src/ --volume=C:\Users\sfoust\.nuget\packages\:/root/.nuget/fallbackpackages --workdir=/app --restart=no --label='com.microsoft.created-by=visual-studio' --label='com.microsoft.visual-studio.project-name=WebApplication1' --runtime=runc -t -d webapplication1:dev
    /// docker network connect network2 WebApplication1
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
      
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets IP Addresses of system
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return GetAddresses().Select(s=>s.ToString());
        }

        /// <summary>
        /// Post to url on all IP Addresses
        /// </summary>
        /// <param name="hookUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] string hookUrl)
        {          
            foreach (var item in GetAddresses())
            {
                var resp = await GetHttpClient(item).PostAsync(hookUrl, null);

            }
            return new OkResult();
        }
        private IEnumerable<IPAddress>  GetAddresses()
        {
             if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
        private HttpClient GetHttpClient(IPAddress address)
        {
            if (IPAddress.Any.Equals(address))
                return new HttpClient();

            SocketsHttpHandler handler = new SocketsHttpHandler();

            handler.ConnectCallback = async (context, cancellationToken) =>
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                socket.Bind(new IPEndPoint(address, 0));

                socket.NoDelay = true;

                try
                {
                    await socket.ConnectAsync(context.DnsEndPoint, cancellationToken).ConfigureAwait(false);

                    return new NetworkStream(socket, true);
                }
                catch
                {
                    socket.Dispose();

                    throw;
                }
            };

            return new HttpClient(handler);
        }

    }
}