using System;
using System.Runtime.Caching;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using D365.Extension.Core;
using D365.Extension.Registration;
using D365.TestExtension.Extensions;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Plugin.Test
{
    /// <summary>
    /// Builder class to configure the test execution context
    /// </summary>
    /// <typeparam name="TPlugin">The plug-in type to test</typeparam>
    /// <typeparam name="TRequest">The SDK message to test</typeparam>
    /// <typeparam name="TEntity">The primary entity to test</typeparam>
    public class PluginTestContextBuilder<TPlugin, TRequest, TEntity>
        where TPlugin : Executor
        where TRequest : OrganizationRequest, new()
        where TEntity : Entity, new()
    {
        private const string PreImageName = "PreImage";
        private const string PostImageName = "PostImage";

        private Target _target;
        private IEnumerable<Entity> _data;
        private IEnumerable<EntityMetadata> _metadata;
        private Entity _preImage;
        private Entity _postImage;
        private ParameterCollection _inputParameters = new ParameterCollection();
        private ParameterCollection _outputParameters = new ParameterCollection();
        private ParameterCollection _sharedVariables = new ParameterCollection();
        private Guid _userId = Guid.NewGuid();
        private Guid _correlationId = Guid.NewGuid();
        private string _organizationName = "Dev";
        private Guid _organizationId = Guid.NewGuid();
        private bool _keepCache = false;
        private string _sdkMessageName;

        private IDictionary<string, XrmFakedRelationship> _relationships =
            new Dictionary<string, XrmFakedRelationship>();

        public PluginTestContextBuilder()
        {
        }

        /// <summary>
        /// Adds the given entity as target to the plug-in execution context
        /// </summary>
        /// <param name="target">The entity to add as target</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithTarget(Entity target)
        {
            _target = new Target(target);
            return this;
        }
        
        /// <summary>
        /// Adds the given entity reference as target to the plug-in execution context
        /// </summary>
        /// <param name="target">The entity reference to add as target</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithTarget(EntityReference target)
        {
            _target = new Target(target);
            return this;
        }

        /// <summary>
        /// Adds the given entity as pre-image to the plug-in execution context
        /// </summary>
        /// <param name="preImage">The entity to add as pre-image</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithPreImage(Entity preImage)
        {
            _preImage = preImage;
            return this;
        }

        /// <summary>
        /// Adds the given entity as post-image to the plug-in execution context
        /// </summary>
        /// <param name="postImage">The entity to add as post-image</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithPostImage(Entity postImage)
        {
            _postImage = postImage;
            return this;
        }

        /// <summary>
        /// Adds the given entities as data to the xrm context
        /// </summary>
        /// <param name="data">The entities to add</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithData(IEnumerable<Entity> data)
        {
            _data = _data != null ? _data.Concat(data) : data;
            return this;
        }

        /// <summary>
        /// Adds the given entity as data to the xrm context
        /// </summary>
        /// <param name="data">The entity to add</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithData(Entity data) => WithData(new[] { data });

        /// <summary>
        /// Adds the given entity metadata to the xrm context
        /// </summary>
        /// <param name="metadata">The entity metadata to add</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithMetaData(IEnumerable<EntityMetadata> metadata)
        {
            _metadata = metadata;
            return this;
        }

        /// <summary>
        /// Adds a input parameter to the parameter collection
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithInputParameter(string name, object value)
        {
            _inputParameters[name] = value;
            return this;
        }

        /// <summary>
        /// Sets the given parameters as parameter collection
        /// </summary>
        /// <param name="parameters">The parameter collection</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithInputParameters(ParameterCollection parameters)
        {
            _inputParameters = parameters;
            return this;
        }

        /// <summary>
        /// Adds a output parameter to the parameter collection
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithOutputParameter(string name, object value)
        {
            _outputParameters[name] = value;
            return this;
        }

        /// <summary>
        /// Sets the given parameters as parameter collection
        /// </summary>
        /// <param name="parameters">The parameter collection</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithOutputParameters(ParameterCollection parameters)
        {
            _outputParameters = parameters;
            return this;
        }

        /// <summary>
        /// Adds a shared variable to the variable collection
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="value">The value of the variable</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithSharedVariable(string name, object value)
        {
            _sharedVariables[name] = value;
            return this;
        }

        /// <summary>
        /// Sets the given variables as variable collection
        /// </summary>
        /// <param name="variables">The variable collection</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithSharedVariables(ParameterCollection variables)
        {
            _sharedVariables = variables;
            return this;
        }

        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithInitiatingUserId(Guid userId)
        {
            _userId = userId;
            return this;
        }

        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithOrganizationName(string name)
        {
            _organizationName = name;
            return this;
        }
        
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithOrganizationId(Guid organizationId)
        {
          _organizationId = organizationId;
          return this;
        }

        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> KeepCache()
        {
            _keepCache = true;
            return this;
        }

        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithRelationship(string schemaName,
            XrmFakedRelationship relationship)
        {
            _relationships[schemaName] = relationship;
            return this;
        }
        
        /// <summary>
        /// Allows override of SDK message name. This is required when using e.g. WinOpportunityRequest because request name and SDK message name differ
        /// </summary>
        /// <param name="sdkMessageName">The SDK message name to use</param>
        /// <returns>self</returns>
        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> OverrideSdkMessageName(string sdkMessageName)
        {
          _sdkMessageName = sdkMessageName;
          return this;
        }

        /// <summary>
        /// Builds the plug-in test context based on the provided values
        /// </summary>
        /// <returns>The test context containing pre-configured xrm and plug-in execution context</returns>
        public PluginTestContext<TPlugin> Build()
        {
            // reset cache
            if(!_keepCache) MemoryCache.Default.Dispose();

            _data = _data ?? new List<Entity>();
            _metadata = _metadata ?? new List<EntityMetadata>();

            var fakeXrmContext = new XrmFakedContextOnSteroids
            {
                ProxyTypesAssembly = Assembly.GetAssembly(typeof(TPlugin))
            };
            fakeXrmContext.Initialize(_data, _metadata);

            foreach (var relationship in _relationships)
            {
                fakeXrmContext.AddRelationship(relationship.Key, relationship.Value);
            }

            var fakePluginExecutionContext = fakeXrmContext.GetDefaultPluginContext();
            fakePluginExecutionContext.CorrelationId = Guid.NewGuid();

            if (_target != null)
            {
                fakePluginExecutionContext.PrimaryEntityId = _target.Id;
                fakePluginExecutionContext.PrimaryEntityName = _target.LogicalName;
                fakePluginExecutionContext.InputParameters["Target"] = _target.Value;
            }

            var execution = GetPluginExecutionConfig(_sdkMessageName);
            
            fakePluginExecutionContext.MessageName = execution.SdkMessageName;
            fakePluginExecutionContext.Mode = (int)execution.ExecutionMode;
            fakePluginExecutionContext.OrganizationId = _organizationId;
            fakePluginExecutionContext.OrganizationName = _organizationName;
            fakePluginExecutionContext.Stage = (int)execution.ExecutionStage;
            fakePluginExecutionContext.InitiatingUserId = _userId;
            fakePluginExecutionContext.CorrelationId = _correlationId;

            if (_preImage != null)
                fakePluginExecutionContext.PreEntityImages.Add(PreImageName, _preImage);

            if (_postImage != null)
                fakePluginExecutionContext.PostEntityImages.Add(PostImageName, _postImage);

            fakePluginExecutionContext.SharedVariables.AddRange(_sharedVariables);
            fakePluginExecutionContext.InputParameters.AddRange(_inputParameters);
            fakePluginExecutionContext.OutputParameters.AddRange(_outputParameters);

            ConfigureXrmContext(fakeXrmContext);
            ConfigurePluginExecutionContext(fakePluginExecutionContext);

            return new PluginTestContext<TPlugin>(fakeXrmContext, fakePluginExecutionContext);
        }

        /// <summary>
        /// Gets called after xrm context creation and can be overriden to configure the newly created context
        /// </summary>
        /// <param name="xrmContext">The newly created xrm context</param>
        protected virtual void ConfigureXrmContext(XrmFakedContext xrmContext)
        {
        }

        /// <summary>
        /// Gets called after plug-in execution context creation and can be override to configure the newly created context (e.g. add execution mocks).
        /// </summary>
        /// <param name="executionContext">The newly created plug-in execution context</param>
        protected virtual void ConfigurePluginExecutionContext(XrmFakedPluginExecutionContext executionContext)
        {
        }

        private static PluginExecutionConfig GetPluginExecutionConfig(string sdkMessageName = null)
        {
            var attributes = GetPluginExecutionConfigForPlugin(sdkMessageName).Concat(GetPluginExecutionConfigForCustomApi());

            return attributes.Single();
        }

        private static IEnumerable<PluginExecutionConfig> GetPluginExecutionConfigForPlugin(string sdkMessageName = null)
        {
            var request = new TRequest();
            var messageName = sdkMessageName ?? request.RequestName;
            var primaryEntity = new TEntity();


            var attributes = Attribute.GetCustomAttributes(typeof(TPlugin), typeof(PluginRegistrationAttribute))
                .Cast<PluginRegistrationAttribute>();

            if (!string.IsNullOrWhiteSpace(messageName))
            {
                attributes = attributes.Where(x => x.MessageName == messageName);
            }

            if (!string.IsNullOrWhiteSpace(primaryEntity.LogicalName))
            {
                attributes = attributes.Where(x => x.PrimaryEntityName == primaryEntity.LogicalName);
            }

            return attributes.Select(x => new PluginExecutionConfig
            {
                SdkMessageName = x.MessageName,
                ExecutionMode = (PluginExecutionMode)x.Mode,
                ExecutionStage = (PluginExecutionStage)x.Stage
            });
        }

        private static IEnumerable<PluginExecutionConfig> GetPluginExecutionConfigForCustomApi()
        {
            var attributes = Attribute.GetCustomAttributes(typeof(TPlugin), typeof(CustomApiRegistrationAttribute))
                .Cast<CustomApiRegistrationAttribute>();

            return attributes.Select(x => new PluginExecutionConfig
            {
                SdkMessageName = x.MessageName,
                ExecutionMode = PluginExecutionMode.Synchronous,
                ExecutionStage = PluginExecutionStage.MainOperation
            });
        }
        
        private class Target
        {
            public Target(Entity entity)
            {
                Id = entity.Id;
                LogicalName = entity.LogicalName;
                Value = entity;
            }

            public Target(EntityReference entityReference)
            {
                Id = entityReference.Id;
                LogicalName = entityReference.LogicalName;
                Value = entityReference;
            }

            public Guid Id { get; set; }
            public string LogicalName { get; set; }
            public object Value { get; set; }
        }

        private class PluginExecutionConfig
        {
            public string SdkMessageName { get; set; }
            public PluginExecutionMode ExecutionMode { get; set; }
            public PluginExecutionStage ExecutionStage { get; set; }
        }

        private class XrmFakedContextOnSteroids : XrmFakedContext
        {
            public void Initialize(IEnumerable<Entity> entities, IEnumerable<EntityMetadata> metadata)
            {
                var metadataList = metadata.ToList();
                if (metadataList.Any())
                {
                    InitializeMetadata(metadataList);
                    InitializeGlobalOptionsetMetadata(metadataList);
                    InitializeRelationshipMetadata(metadataList);
                }
                else
                {
                    InitializeMetadata(Assembly.GetAssembly(typeof(TPlugin)));
                }

                base.Initialize(entities);
            }

            private void InitializeRelationshipMetadata(ICollection<EntityMetadata> metadata)
            {
                metadata.SelectMany(x => x.ManyToManyRelationships)
                    .Select(x => (RelationshipMetadataBase)x)
                    .Concat(metadata
                        .SelectMany(x => x.OneToManyRelationships)
                        .Select(x => x as RelationshipMetadataBase)
                    ).Concat(metadata
                        .SelectMany(x => x.ManyToOneRelationships)
                        .Select(x => x as RelationshipMetadataBase)
                    ).ToList()
                    .ForEach(AddRelationshipMetadataToContext);
            }

            private void AddRelationshipMetadataToContext(RelationshipMetadataBase x)
            {
                if (GetRelationship(x.SchemaName) == null)
                {
                    AddRelationship(x.SchemaName, x.ToXrmFakedRelationship());
                }
            }

            private void InitializeGlobalOptionsetMetadata(IEnumerable<EntityMetadata> metadata) => metadata
                .SelectMany(x => x.Attributes)
                .Where(x => x.AttributeType == AttributeTypeCode.Picklist)
                .Select(x => (PicklistAttributeMetadata)x)
                .Where(x => x.OptionSet.IsGlobal == true)
                .Select(x => x.OptionSet)
                .ToList()
                .ForEach(AddOptionsetMetadataToContext);

            private void AddOptionsetMetadataToContext(OptionSetMetadata x)
            {
                if (!OptionSetValuesMetadata.ContainsKey(x.Name))
                {
                    OptionSetValuesMetadata.Add(x.Name, x);
                }
            }
        }

        public PluginTestContextBuilder<TPlugin, TRequest, TEntity> WithCorrelationId(Guid correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }

    /// <summary>
    /// Builder class to configure the test execution context
    /// </summary>
    /// <typeparam name="TPlugin">The plug-in type to test</typeparam>
    /// <typeparam name="TRequest">The SDK message to test</typeparam>
    public class PluginTestContextBuilder<TPlugin, TRequest> : PluginTestContextBuilder<TPlugin, TRequest, Entity>
        where TPlugin : Executor
        where TRequest : OrganizationRequest, new()
    {
    }

    /// <summary>
    /// Builder class to configure the test execution context
    /// </summary>
    /// <typeparam name="TPlugin">The plug-in type to test</typeparam>
    public class PluginTestContextBuilder<TPlugin> : PluginTestContextBuilder<TPlugin, OrganizationRequest, Entity>
        where TPlugin : Executor
    {
    }
}