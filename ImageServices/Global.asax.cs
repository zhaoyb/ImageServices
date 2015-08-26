using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using FastDFS.Client;
using FastDFS.Client.Config;

namespace ImageServices
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            if (ConfigHelper.GetConfigString("SaveMode") == "Distributed")
            {
                FastDfsConfig globalConfig = FastDfsManager.GetConfigSection();
                ConnectionManager.InitializeForConfigSection(globalConfig);
                FastDfsGlobalConfig.Config = globalConfig;
            }
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
