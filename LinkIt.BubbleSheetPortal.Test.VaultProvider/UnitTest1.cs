using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIt.BubbleSheetPortal.Test.VaultProvider
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            string tableName = "Music";

            CreateTableRequest createTableRequest = new CreateTableRequest()
            {
                TableName = tableName
            };

            //ProvisionedThroughput
            createTableRequest.ProvisionedThroughput = new ProvisionedThroughput()
            {
                ReadCapacityUnits = (long)5,
                WriteCapacityUnits = (long)5
            };

            //AttributeDefinitions
            List<AttributeDefinition> attributeDefinitions = new List<AttributeDefinition>();

            attributeDefinitions.Add(new AttributeDefinition()
            {
                AttributeName = "Artist",
                AttributeType = "S"
            });

            attributeDefinitions.Add(new AttributeDefinition()
            {
                AttributeName = "SongTitle",
                AttributeType = "S"
            });

            attributeDefinitions.Add(new AttributeDefinition()
            {
                AttributeName = "AlbumTitle",
                AttributeType = "S"
            });

            createTableRequest.AttributeDefinitions = attributeDefinitions;

            //KeySchema
            List<KeySchemaElement> tableKeySchema = new List<KeySchemaElement>();

            tableKeySchema.Add(new KeySchemaElement() { AttributeName = "Artist", KeyType = "HASH" });
            tableKeySchema.Add(new KeySchemaElement() { AttributeName = "SongTitle", KeyType = "RANGE" });

            createTableRequest.KeySchema = tableKeySchema;

            List<KeySchemaElement> indexKeySchema = new List<KeySchemaElement>();
            indexKeySchema.Add(new KeySchemaElement() { AttributeName = "Artist", KeyType = "HASH" });
            indexKeySchema.Add(new KeySchemaElement() { AttributeName = "AlbumTitle", KeyType = "RANGE" });

            Projection projection = new Projection() { ProjectionType = "INCLUDE" };

            List<string> nonKeyAttributes = new List<string>();
            nonKeyAttributes.Add("Genre");
            nonKeyAttributes.Add("Year");
            projection.NonKeyAttributes = nonKeyAttributes;

            LocalSecondaryIndex localSecondaryIndex = new LocalSecondaryIndex()
            {
                IndexName = "AlbumTitleIndex",
                KeySchema = indexKeySchema,
                Projection = projection
            };

            List<LocalSecondaryIndex> localSecondaryIndexes = new List<LocalSecondaryIndex>();
            localSecondaryIndexes.Add(localSecondaryIndex);
            createTableRequest.LocalSecondaryIndexes = localSecondaryIndexes;

            CreateTableResponse result = client.CreateTable(createTableRequest);
            Console.WriteLine(result.CreateTableResult.TableDescription.TableName);
            Console.WriteLine(result.CreateTableResult.TableDescription.TableStatus);    
        }
    }
}
