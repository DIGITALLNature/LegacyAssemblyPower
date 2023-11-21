using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using D365.Extension.CodeActivity;
using D365.Extension.Model;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public abstract partial class Executor : System.Activities.CodeActivity
    {
        /// <summary>
        /// 
        /// </summary>
        public CodeActivityDelegate Delegate { get; private set; }

        #region base methods
        /// <summary>
        /// Invokes the OrganizationServiceFactory; prefer to use the SecuredOrganizationService or ElevatedOrganizationService
        /// </summary>
        /// <param name="elevated"></param>
        /// <returns></returns>
        public IOrganizationService OrganizationService(bool elevated = false)
        {
            if (Delegate == null) return null;//prevent NullRef exception
            return elevated ? Delegate.OrganizationServiceFactory.CreateOrganizationService(null) : Delegate.OrganizationServiceFactory.CreateOrganizationService(Delegate.ExecutionContext.UserId);
        }

        /// <summary>
        /// Invokes the OrganizationServiceFactory; prefer to use the SecuredOrganizationService
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IOrganizationService OrganizationService(Guid userId)
        {
            if (Delegate == null) return null;//prevent NullRef exception
            return userId == Guid.Empty ? Delegate.OrganizationServiceFactory.CreateOrganizationService(null) : Delegate.OrganizationServiceFactory.CreateOrganizationService(userId);
        }

        /// <summary>
        /// This creates a new IOrganizationService behind the scenes, prefer the usage of DataContext DataContext(IOrganizationService) 
        /// </summary>
        /// <param name="elevated"></param>
        /// <param name="mergeOption"></param>
        /// <returns></returns>
        public DataContext DataContext(bool elevated = false, MergeOption mergeOption = MergeOption.NoTracking)
        {
            return new DataContext(OrganizationService(elevated)) { MergeOption = mergeOption };
        }

        /// <summary>
        /// Create a new DataContext
        /// </summary>
        /// <param name="service"></param>
        /// <param name="mergeOption"></param>
        /// <returns></returns>
        public DataContext DataContext(IOrganizationService service, MergeOption mergeOption = MergeOption.NoTracking)
        {
            return new DataContext(service) { MergeOption = mergeOption };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceEndpointName">ServiceEndpoint name</param>
        /// <param name="data">ParameterCollection set as ExecutionContext.SharedVariables</param>
        public void PushMessage(string serviceEndpointName, ParameterCollection data = null)
        {
            using (var serviceContext = DataContext(true))
            {
                var serviceEndpoint = (from s in serviceContext.ServiceEndpointSet
                                       where s.Name == serviceEndpointName
                                       select s).SingleOrDefault();
                if (serviceEndpoint == null)
                {
                    throw new InvalidPluginExecutionException($"ServiceEndpoint {serviceEndpointName} unknown!");
                }

                if (data != null)
                {
                    foreach (var entry in data)
                    {
                        Delegate.ExecutionContext.SharedVariables.Add(entry);
                    }
                }
                const int max = 3;
                var response = string.Empty;
                var retry = 0;
                do
                {
                    try
                    {
                        response = Delegate.NotificationService.Execute(serviceEndpoint.ToEntityReference(), Delegate.ExecutionContext);
                        break;//stop loop
                    }
                    catch (FaultException<OrganizationServiceFault> e)
                    {
                        ExecutorService.Notify(this, new NotifyEvent
                        {
                            EventException = e.RootException(),
                            EventOrigin = serviceEndpointName,
                            EventMessage = $"{e.Message} ({retry})"
                        });
                        //Delegate.TracingService.Trace($"Exception: {e.Message}");
                        retry++;
                        if (retry >= max) throw;
                        RandomService.Sleep();//wait a bit
                        //retry loop
                    }
                } while (retry < max);
                if (!string.IsNullOrEmpty(response))
                {
                    throw new InvalidPluginExecutionException(response);
                }
            }
        }
        #endregion

        #region crm executioncontext
        /// <summary>
        /// The execution context correlation id.
        /// </summary>
        public Guid CorrelationId => Delegate?.ExecutionContext.CorrelationId ?? Guid.Empty;

        /// <summary>
        /// The execution context initiating user id (OrganizationServiceProxy.CallerId).
        /// </summary>
        public Guid CallerId => Delegate?.ExecutionContext.InitiatingUserId ?? Guid.Empty;

        /// <summary>
        /// The business unit that the user making the request, also known as the calling user.
        /// </summary>
        public Guid BusinessUnitId => Delegate?.ExecutionContext.BusinessUnitId ?? Guid.Empty;

        /// <summary>
        /// String representation of the currenty executed process.
        /// </summary>
        public string ProcessName => $"CRM.{GetType().Name}.{WorkflowMode}.{WorkflowCategory}.{Delegate?.ExecutionContext.Depth}";
        #endregion

        /// <summary>
        /// Get the WorkflowContext.WorkflowMode as string representation
        /// </summary>
        public string WorkflowMode
        {
            get
            {
                switch (Delegate?.WorkflowContext.WorkflowMode)
                {
                    case 0:
                        return "Background";
                    case 1:
                        return "Realtime";
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Get the WorkflowContext.WorkflowCategory as string representation
        /// </summary>
        public string WorkflowCategory
        {
            get
            {
                switch (Delegate?.WorkflowContext.WorkflowCategory)
                {
                    case 0:
                        return "Workflow";
                    case 1:
                        return "Dialog";
                    case 3:
                        return "Action";
                    default:
                        return null;
                }
            }
        }

        private IOrganizationService _contextRef;
        /// <summary>
        /// Context bounded OrganizationService (secured)
        /// </summary>
        public IOrganizationService SecuredOrganizationService
        {
            get => _contextRef ?? (_contextRef = OrganizationService());
            set => _contextRef = value;
        }

        private IOrganizationService _elevatedRef;
        /// <summary>
        /// Context bounded OrganizationService (elevated)
        /// </summary>
        public IOrganizationService ElevatedOrganizationService
        {
            get => _elevatedRef ?? (_elevatedRef = OrganizationService(true));
            set => _elevatedRef = value;
        }

        /// <summary>
        /// CodeActivity execute impl.
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            var timer = Stopwatch.StartNew();
			//follow the "stateless" recommendation of Microsoft
			var inner = (Executor)MemberwiseClone();
            inner.Delegate = new CodeActivityDelegate(context);
            try
            {
                inner.ExecutorService.BeforeExecute(inner);
                inner.Result = inner.Execute();
                inner.ExecutorService.AfterExecute(inner, Result, timer);
            }
            catch (InvalidPluginExecutionException i)
            {
                //if the InvalidPluginExecutionException is used as message handler,
                //e.g. show validation messages,
                //just an ability to bypass the telemetry/logging as error
                if (OperationStatus.Succeeded == i.Status)
                {
                    inner.Result = ExecutionResult.Ok;
                    inner.ExecutorService.AfterExecute(inner, Result, timer);
                }
                else
                {
                    inner.Result = ExecutionResult.Failure;
                    inner.ExecutorService.ExecuteException(inner, i, timer);
                }
                throw;
            }
            catch (Exception e)
            {
                inner.Result = ExecutionResult.Failure;
                inner.ExecutorService.ExecuteException(inner, e, timer);
                throw;
            }
            //for unit testing only
            Result = inner.Result;
        }

        /// <summary>
        /// Get current execution result
        /// </summary>
        public ExecutionResult Result { get; private set; }

        /// <summary>
        /// Abstract implementation for the CodeActivity. The custom code goes here!
        /// </summary>
        /// <returns>ExecutionResult of the process.</returns>
        protected abstract ExecutionResult Execute();
    }
}
