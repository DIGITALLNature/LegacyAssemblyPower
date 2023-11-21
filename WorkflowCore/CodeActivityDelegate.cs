using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

// ReSharper disable once CheckNamespace
namespace D365.Extension.CodeActivity
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CodeActivityDelegate
    {
        private readonly CodeActivityContext _context;

        internal CodeActivityDelegate(CodeActivityContext context)
        {
            _context = context ?? throw new InvalidPluginExecutionException("context is null");
        }

        public IWorkflowContext WorkflowContext => _context.GetExtension<IWorkflowContext>();

        public IExecutionContext ExecutionContext => WorkflowContext;

        public ITracingService TracingService => _context.GetExtension<ITracingService>();

        public IOrganizationServiceFactory OrganizationServiceFactory => _context.GetExtension<IOrganizationServiceFactory>();

        public IServiceEndpointNotificationService NotificationService => _context.GetExtension<IServiceEndpointNotificationService>();

        public static implicit operator CodeActivityContext(CodeActivityDelegate target)
        {
            return target._context;
        }
    }
}
