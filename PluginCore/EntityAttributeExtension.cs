using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    /// <summary>
    /// EntityAttribute extensions
    /// </summary>
    public static class EntityAttributeExtension
    {
        /// <summary>
        /// Get value T from entity. Lookup order 1st Entity, 2nd PreEntityImage, 3rd default!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executor">self</param>
        /// <param name="attribute">lookup attribute</param>
        /// <returns></returns>
        public static T GetEntityAttributeValue<T>(this Executor executor, string attribute)
        {
            if (executor.Entity != null && executor.Entity.Attributes.Contains(attribute))
            {
                return (T)executor.Entity.Attributes[attribute];
            }
            if (executor.PreEntityImage != null && executor.PreEntityImage.Attributes.Contains(attribute))
            {
                return (T)executor.PreEntityImage.Attributes[attribute];
            }
            return default;
        }

        /// <summary>
        /// Evaluates if Entity contains attribute and PreEntityImage does not
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executor">self</param>
        /// <param name="attribute">lookup attribute</param>
        /// <returns></returns>
        public static bool IsEntityAttributeValueNew<T>(this Executor executor, string attribute)
        {
            return executor.Entity != null && executor.Entity.Contains(attribute) && (executor.PreEntityImage == null || !executor.PreEntityImage.Contains(attribute));
        }

        /// <summary>
        /// Evaluates if attribute in Entity is set and is different from PreEntityImage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executor">self</param>
        /// <param name="attribute">lookup attribute</param>
        /// <returns></returns>
        public static bool IsEntityAttributeValueChanged<T>(this Executor executor, string attribute)
        {
            //not in target
            if (executor.Entity != null && !executor.Entity.Contains(attribute)) return false;
            //no pre-image
            if (executor.PreEntityImage == null) return true;

            if (typeof(T) != typeof(string))
            {
                //was empty, stays empty
                if (!executor.PreEntityImage.Contains(attribute) && executor.Entity[attribute] == null) return false;
            }
            else
            {
                //was empty, stays empty
                if (!executor.PreEntityImage.Contains(attribute) && string.IsNullOrEmpty((string)executor.Entity[attribute])) return false;
                //treat null == "" as true
                if (executor.PreEntityImage.Contains(attribute) && string.IsNullOrEmpty((string)executor.PreEntityImage[attribute]) && string.IsNullOrEmpty((string)executor.Entity[attribute])) return false;
            }

            var cacheKey = $"EntityAttributeExtension-{typeof(T).FullName}";
            IEqualityComparer<T> typeEqualizer;
            if (executor.CacheService.TryGet(cacheKey, out object value))
            {
                typeEqualizer = value as EqualityComparer<T>;
            }
            else
            {
                typeEqualizer = EqualityComparer<T>.Default;
                executor.CacheService.SetSliding(cacheKey, typeEqualizer, 1800);
            }
            return !(executor.PreEntityImage.Contains(attribute) && typeEqualizer.Equals((T)executor.PreEntityImage[attribute], (T)executor.Entity[attribute]));
        }

        /// <summary>
        /// Evaluates if attribute contained in Entity or PreEntityImage and not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executor">self</param>
        /// <param name="attribute">lookup attribute</param>
        /// <returns></returns>
        public static bool IsEntityAttributeValueNullOrEmpty<T>(this Executor executor, string attribute)
        {
            return (executor.Entity != null && (!executor.Entity.Contains(attribute) || executor.Entity[attribute] == null)) &&
                   (executor.PreEntityImage == null || !executor.PreEntityImage.Contains(attribute) || executor.PreEntityImage[attribute] == null);
        }

        /// <summary>
        /// Merge Entity and PreEntityImage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executor">self</param>
        /// <returns></returns>
        public static T MergeEntity<T>(this Executor executor) where T : Entity
        {
            if (executor.PreEntityImage == null) return executor.Entity.ToEntity<T>();

            var mergedEntity = new Entity
            {
                Id = executor.PreEntityImage.Id,
                LogicalName = executor.PreEntityImage.LogicalName
            };

            // return all AttributeLogicalNameAttribute from the given type
            var attributes = from property in typeof(T).GetProperties()
                             from attribute in
                                 property.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), false).OfType<AttributeLogicalNameAttribute>()
                             select attribute;

            foreach (var attribute in attributes)
            {
                if (executor.Entity.Contains(attribute.LogicalName))
                {
                    mergedEntity[attribute.LogicalName] = executor.Entity[attribute.LogicalName];
                    if (executor.Entity.FormattedValues.ContainsKey(attribute.LogicalName)) mergedEntity.FormattedValues.Add(attribute.LogicalName, executor.Entity.FormattedValues[attribute.LogicalName]);
                }
                else if (executor.PreEntityImage.Contains(attribute.LogicalName))
                {
                    mergedEntity[attribute.LogicalName] = executor.PreEntityImage[attribute.LogicalName];
                    if (executor.PreEntityImage.FormattedValues.ContainsKey(attribute.LogicalName)) mergedEntity.FormattedValues.Add(attribute.LogicalName, executor.PreEntityImage.FormattedValues[attribute.LogicalName]);
                }
            }

            return mergedEntity.ToEntity<T>();
        }
    }
}