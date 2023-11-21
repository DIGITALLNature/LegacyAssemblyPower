using System;
using D365.Extension.Core;
using D365.Extension.Model;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Plugin.Test
{
    /// <summary>
    /// Wrapper context for usage in unit tests.
    /// </summary>
    /// <typeparam name="TPlugin">The plug-in type to test</typeparam>
    public class PluginTestContext<TPlugin>
        where TPlugin : Executor
    {
        public PluginTestContext(XrmFakedContext xrmContext,
            XrmFakedPluginExecutionContext pluginExecutionContext)
        {
            XrmContext = xrmContext;
            PluginExecutionContext = pluginExecutionContext;
            DataContext = new DataContext(XrmContext.GetOrganizationService());
        }

        /// <summary>
        /// Pre-configured fake xrm context
        /// </summary>
        public XrmFakedContext XrmContext { get; }

        /// <summary>
        /// Pre-configured faked plug-in execution context
        /// </summary>
        public XrmFakedPluginExecutionContext PluginExecutionContext { get; }

        /// <summary>
        /// DataContext which can be used to query after plug-in execution
        /// </summary>
        public DataContext DataContext { get; }

        public bool GetOutputParameter<T>(string key, out T value)
        {
            if (PluginExecutionContext.OutputParameters.Contains(key) &&
                PluginExecutionContext.OutputParameters[key] is T)
            {
                value = (T)PluginExecutionContext.OutputParameters[key];
                return true;
            }
            value = default(T);
            return false;
        }

        public bool GetInputParameter<T>(string key, out T value)
        {
            if (PluginExecutionContext.InputParameters.Contains(key) &&
                PluginExecutionContext.InputParameters[key] is T)
            {
                value = (T)PluginExecutionContext.InputParameters[key];
                return true;
            }
            value = default(T);
            return false;
        }

        public Entity Entity
        {
            get
            {
                GetInputParameter("Target", out Entity value);
                return value;
            }
        }

        public EntityReference EntityReference
        {
            get
            {
                GetInputParameter("Target", out EntityReference value);
                return value;
            }
        }

        /// <summary>
        /// Executes the specified plug-in with the configured test context.
        /// Creates a new plug-in instance internally.
        /// </summary>
        /// <returns>The execution result of the plug-in</returns>
        public ExecutionResult ExecutePlugin()
        {
            var instance = Activator.CreateInstance<TPlugin>();
            return ExecutePlugin(instance);
        }

        /// <summary>
        /// Executes the specified plug-in with the configured test context.
        /// Uses the plug-in instance given as parameter.
        /// This can be useful, for setting instance properties like <see cref="Executor.UnsecureConfig"/>.
        /// </summary>
        /// <param name="instance">The plugin instance to execute</param>
        /// <returns>The execution result of the plug-in</returns>
        public ExecutionResult ExecutePlugin(TPlugin instance)
        {
            XrmContext.ExecutePluginWith(PluginExecutionContext, instance);
            return instance.Result;
        }
    }
}