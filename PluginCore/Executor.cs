using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;
using D365.Extension.Model;
using D365.Extension.Plugin;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract partial class Executor : IPlugin
    {
        public readonly string UnsecureConfig;

        public readonly string SecureConfig;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unsecure"></param>
        /// <param name="secure"></param>
        protected Executor(string unsecure = null, string secure = null)
        {
            UnsecureConfig = unsecure;
            SecureConfig = secure;
        }

        public PluginDelegate Delegate { get; private set; }

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
        /// This creates a new IOrganizationService behind the scenes, prefer the usage of DataContext(IOrganizationService) 
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
            using (var serviceContext = DataContext(ElevatedOrganizationService))
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
        public Guid CorrelationId => Delegate.ExecutionContext.CorrelationId;

        /// <summary>
        /// The execution context initiating user id (OrganizationServiceProxy.CallerId).
        /// </summary>
        public Guid CallerId => Delegate.ExecutionContext.InitiatingUserId;

        /// <summary>
        /// The business unit that the user making the request, also known as the calling user.
        /// </summary>
        public Guid BusinessUnitId => Delegate.ExecutionContext.BusinessUnitId;

        /// <summary>
        /// String representation of the currenty executed process.
        /// </summary>
        public string ProcessName => $"CRM.{GetType().Name}.{Delegate.PluginExecutionContext.MessageName}.{Mode}.{Stage}.{Depth}";
        #endregion

        /// <summary>
        /// The target entity of the context.
        /// </summary>
        public Entity Entity
        {
            get
            {
                GetInputParameter("Target", out Entity value);
                return value;
            }
        }

        /// <summary>
        /// The target entity reference of the context.
        /// </summary>
        public EntityReference EntityReference
        {
            get
            {
                GetInputParameter("Target", out EntityReference value);
                return value;
            }
        }

        /// <summary>
        /// The relationship of the context.
        /// </summary>
        public Relationship Relationship
        {
            get
            {
                GetInputParameter("Relationship", out Relationship value);
                return value;
            }
        }

        /// <summary>
        /// The related entities of the context.
        /// </summary>
        public EntityReferenceCollection RelatedEntities
        {
            get
            {
                GetInputParameter("RelatedEntities", out EntityReferenceCollection value);
                return value;
            }
        }

        /// <summary>
        /// Get the execution context depth.
        /// </summary>
        public int Depth => Delegate.ExecutionContext.Depth;

        /// <summary>
        /// Get the execution context stage as string representation
        /// </summary>
        public string Stage
        {
            get
            {
                switch (Delegate.PluginExecutionContext.Stage)
                {
                    case 10:
                        return "PreValidation";
                    case 20:
                        return "PreOperation";
                    case 30:
                        return "MainOperation";
                    case 40:
                        return "PostOperation";
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Get the execution context mode as string representation
        /// </summary>
        public string Mode
        {
            get
            {
                switch (Delegate.PluginExecutionContext.Mode)
                {
                    case 0:
                        return "Synchronous";
                    case 1:
                        return "Asynchronous";
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
        /// Generic getter for input parameters in execution context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dafaultValue"></param>
        /// <returns></returns>
        public bool GetInputParameter<T>(string key, out T value, T dafaultValue = default(T))
        {
            if (Delegate.PluginExecutionContext.InputParameters.Contains(key) &&
                Delegate.PluginExecutionContext.InputParameters[key] is T)
            {
                value = (T)Delegate.PluginExecutionContext.InputParameters[key];
                return true;
            }
            value = dafaultValue;
            return false;
        }

        /// <summary>
        /// Generic getter for output parameters in execution context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dafaultValue"></param>
        /// <returns></returns>
        public bool GetOutputParameter<T>(string key, out T value, T dafaultValue = default(T))
        {
            if (Delegate.PluginExecutionContext.OutputParameters.Contains(key) &&
                Delegate.PluginExecutionContext.OutputParameters[key] is T)
            {
                value = (T)Delegate.PluginExecutionContext.OutputParameters[key];
                return true;
            }
            value = dafaultValue;
            return false;
        }

        /// <summary>
        /// Generic setter for output parameters in execution context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetOutputParameter<T>(string key, T value)
        {
            Delegate.PluginExecutionContext.OutputParameters[key] = value;
        }

        /// <summary>
        /// The "PreImage" pre-entity image; see Plugin Registration
        /// </summary>
        public Entity PreEntityImage => Delegate.PluginExecutionContext.PreEntityImages.Contains("PreImage") ? Delegate.PluginExecutionContext.PreEntityImages["PreImage"] : null;

        /// <summary>
        /// The "PostImage" post-entity image; see Plugin Registration
        /// </summary>
        public Entity PostEntityImage => Delegate.PluginExecutionContext.PostEntityImages.Contains("PostImage") ? Delegate.PluginExecutionContext.PostEntityImages["PostImage"] : null;

        /// <summary>
        /// Get column set from execution context.
        /// </summary>
        public ColumnSet ColumnSet
        {
            get
            {
                GetInputParameter("ColumnSet", out ColumnSet value);
                return value;
            }
        }

        /// <summary>
        /// Get query from execution context.
        /// </summary>
        public bool Query(out QueryExpression query, out ColumnSet columnSet)
        {
            columnSet = ColumnSet;
            // ReSharper disable once InvertIf
            if (GetInputParameter("Query", out query))
            {
                if (ColumnSet == default(ColumnSet))
                {
                    columnSet = query.ColumnSet;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get query from execution context.
        /// </summary>
        public bool Query(out QueryByAttribute query, out ColumnSet columnSet)
        {
            columnSet = ColumnSet;
            // ReSharper disable once InvertIf
            if (GetInputParameter("Query", out query))
            {
                if (ColumnSet == default(ColumnSet))
                {
                    columnSet = query.ColumnSet;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get query from execution context.
        /// </summary>
        public bool Query(out FetchExpression query, out ColumnSet columnSet)
        {
            columnSet = ColumnSet;
            // ReSharper disable once InvertIf
            if (GetInputParameter("Query", out query))
            {
                if (ColumnSet == default(ColumnSet))
                {
                    columnSet = new ColumnSet(XDocument.Load(XmlReader.Create(new StringReader(query.Query)))
                        .Descendants("attribute").Select(d => d.Attribute("name")).ToList()
                        .Select(e => e.Value.ToString()).ToArray());
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the business entity from output parameters in execution context.
        /// </summary>
        public Entity RetrieveEntity
        {
            get
            {
                GetOutputParameter("BusinessEntity", out Entity value);
                return value;
            }
        }

        /// <summary>
        /// Get the business entity collection from output parameters in execution context.
        /// </summary>
        public EntityCollection RetrieveMultipleEntities
        {
            get
            {
                GetOutputParameter("BusinessEntityCollection", out EntityCollection value);
                return value;
            }
        }

        /// <summary>
        /// IPlugin execute impl.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {
            var timer = Stopwatch.StartNew();
			//follow the "stateless" recommendation of Microsoft
            var inner = (Executor)MemberwiseClone();
            inner.Delegate = new PluginDelegate(serviceProvider);
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
        /// Abstract implementation for the Plugin. The custom code goes here!
        /// </summary>
        /// <returns>ExecutionResult of the process.</returns>
        protected abstract ExecutionResult Execute();
    }
}
