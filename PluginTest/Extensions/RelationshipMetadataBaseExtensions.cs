using System;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk.Metadata;

namespace D365.TestExtension.Extensions
{
    public static class RelationshipMetadataBaseExtensions
    {
        /// <summary>
        /// Creates a XrmFakedRelationship out of a RelationshipMetadataBase.
        /// </summary>
        /// <param name="relationshipMetadata"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static XrmFakedRelationship ToXrmFakedRelationship(this RelationshipMetadataBase relationshipMetadata)
        {
            switch (relationshipMetadata)
            {
                case OneToManyRelationshipMetadata oneToManyRelationshipMetadata:
                    return new XrmFakedRelationship
                    {
                        // TODO: FakeXrmEasy sets the relationship name through the IntersectEntity property in the RetrieveRelationshipRequestExecutor therefore setting the property to schema name here.
                        // see https://github.com/jordimontana82/fake-xrm-easy/blob/master/FakeXrmEasy.Shared/FakeMessageExecutors/RetrieveRelationshipRequestExecutor.cs
                        IntersectEntity = relationshipMetadata.SchemaName,
                        RelationshipType = XrmFakedRelationship.enmFakeRelationshipType.OneToMany,
                        Entity1LogicalName = oneToManyRelationshipMetadata.ReferencingEntity,
                        Entity1Attribute = oneToManyRelationshipMetadata.ReferencingAttribute,
                        Entity2LogicalName = oneToManyRelationshipMetadata.ReferencedEntity,
                        Entity2Attribute = oneToManyRelationshipMetadata.ReferencedAttribute
                    };
                case ManyToManyRelationshipMetadata manyToManyRelationshipMetadata:
                    return new XrmFakedRelationship
                    {
                        RelationshipType = XrmFakedRelationship.enmFakeRelationshipType.ManyToMany,
                        IntersectEntity = manyToManyRelationshipMetadata.IntersectEntityName,
                        Entity1LogicalName = manyToManyRelationshipMetadata.Entity1LogicalName,
                        Entity1Attribute = manyToManyRelationshipMetadata.Entity1IntersectAttribute,
                        Entity2LogicalName = manyToManyRelationshipMetadata.Entity2LogicalName,
                        Entity2Attribute = manyToManyRelationshipMetadata.Entity2IntersectAttribute
                    };
                default:
                    throw new ArgumentException($"Unknown relationship type '{nameof(relationshipMetadata)}'",
                        nameof(relationshipMetadata));
            }
        }
    }
}