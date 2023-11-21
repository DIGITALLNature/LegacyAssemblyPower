using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PluginDelegate
    {
        private readonly IServiceProvider _serviceProvider;

        internal PluginDelegate(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new InvalidPluginExecutionException("serviceProvider is null");
        }

        public IPluginExecutionContext PluginExecutionContext => (IPluginExecutionContext)_serviceProvider.GetService(typeof(IPluginExecutionContext));

        /// <summary>
        /// 
        /// </summary>
        public IExecutionContext ExecutionContext => PluginExecutionContext;

        /// <summary>
        /// 
        /// </summary>
        public ITracingService TracingService => (ITracingService)_serviceProvider.GetService(typeof(ITracingService));

        /// <summary>
        /// 
        /// </summary>
        public IOrganizationServiceFactory OrganizationServiceFactory => (IOrganizationServiceFactory)_serviceProvider.GetService(typeof(IOrganizationServiceFactory));

        /// <summary>
        /// 
        /// </summary>
        public IServiceEndpointNotificationService NotificationService => (IServiceEndpointNotificationService)_serviceProvider.GetService(typeof(IServiceEndpointNotificationService));

        /// <summary>
        /// Uses Application Insights configured under "Activate Data Export" in Power Platform admin center
        /// </summary>
        public ILogger PluginTelemetry => (ILogger)_serviceProvider.GetService(typeof(ILogger));
    }
}
