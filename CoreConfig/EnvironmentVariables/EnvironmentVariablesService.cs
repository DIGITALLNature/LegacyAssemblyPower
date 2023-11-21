using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public sealed class EnvironmentVariablesService : IConfigService
    {
        private readonly Executor _executor;
        private readonly Func<string, int, string> _translator;

        public EnvironmentVariablesService(Executor executor)
        {
            _executor = executor;
            _translator = null;
        }

        public EnvironmentVariablesService(Executor executor, Func<string, int, string> translator)
        {
            _executor = executor;
            _translator = translator;
        }

        public string GetConfig(string key, int lcid = 1033, string defaultValue = null)
        {
            key = _translator == null ? key : _translator(key, lcid);

            var cacheKey = $"EnvironmentVariables-{key.ToLowerInvariant()}.{lcid}";

            if (_executor.CacheService.TryGet(cacheKey, out object value))
            {
                return (string)value;
            }
            var configValue = defaultValue;

            using (var serviceContext = _executor.DataContext(_executor.ElevatedOrganizationService))
            {
                var environmentVariableDefinition = (from def in serviceContext.EnvironmentVariableDefinitionSet
                                                     where def.SchemaName == key
                                                     select new {def.SchemaName, def.DefaultValue, def.Id}).SingleOrDefault();

                if (environmentVariableDefinition != null)
                {
                    var environmentVariableValue = (from cfg in serviceContext.EnvironmentVariableValueSet
                                                    where cfg.EnvironmentVariableDefinitionId.Id == environmentVariableDefinition.Id
                                                    select new {cfg.Value}).SingleOrDefault();
                    if (environmentVariableValue != null)
                    {
                        configValue = environmentVariableValue.Value;
                    }
                    else if (environmentVariableDefinition.DefaultValue != null)
                    {
                        configValue = environmentVariableDefinition.DefaultValue;
                    }
                }
            }

            _executor.CacheService.SetSliding(cacheKey, configValue ?? string.Empty, 121);
            return configValue;
        }
    }
}
