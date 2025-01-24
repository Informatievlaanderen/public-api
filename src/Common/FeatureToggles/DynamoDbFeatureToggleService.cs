namespace Common.FeatureToggles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;

    public interface IDynamicFeatureToggleService
    {
        public bool IsFeatureEnabled(string featureName);
    }


    /// <summary>
    /// Possible registration:
    /// <code>
    /// .AddSingleton&lt;IDynamicFeatureToggleService&gt;(c =>
    /// {
    ///     var featureToggleService = new DynamoDbFeatureToggleService(c.GetRequiredService&lt;IAmazonDynamoDB&gt;(),
    ///         _configuration["FeatureToggleTableName"]);
    ///     featureToggleService.Initialize().GetAwaiter().GetResult();
    ///     return featureToggleService;
    /// })
    /// </code>
    /// </summary>
    public sealed class DynamoDbFeatureToggleService : IDynamicFeatureToggleService
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly string _tableName;

        private readonly Dictionary<string, bool> _featureToggles =
            new Dictionary<string, bool>(StringComparer.CurrentCultureIgnoreCase);

        public DynamoDbFeatureToggleService(
            IAmazonDynamoDB amazonDynamoDb,
            string tableName)
        {
            ArgumentException.ThrowIfNullOrEmpty(tableName);

            _amazonDynamoDb = amazonDynamoDb;
            _tableName = tableName;
        }

        public async Task Initialize()
        {
            await EnsureTableExists();
            await GetAllFeaturesFromDynamoDb();
        }

        public bool IsFeatureEnabled(string featureName)
            => _featureToggles.ContainsKey(featureName) && _featureToggles[featureName];

        public async Task Migrate(IEnumerable<IKeyedFeatureToggle> keyedFeatureToggles)
        {
            foreach (var featureToggle in keyedFeatureToggles.Select(x => x.Key))
            {
                if (_featureToggles.TryAdd(featureToggle, false))
                {
                    try
                    {
                        await _amazonDynamoDb.PutItemAsync(new PutItemRequest
                        {
                            TableName = _tableName,
                            Item =
                            {
                                ["FeatureName"] = new AttributeValue(featureToggle),
                                ["Enabled"] = new AttributeValue { BOOL = false }
                            },
                            ConditionExpression = "attribute_not_exists(FeatureName)"
                        });
                    }
                    catch (ConditionalCheckFailedException)
                    { }
                }
            }
        }

        private async Task GetAllFeaturesFromDynamoDb()
        {
            var scanRequest = new ScanRequest
            {
                TableName = _tableName
            };
            do
            {
                var scanResponse = await _amazonDynamoDb.ScanAsync(scanRequest);
                foreach (var item in scanResponse.Items)
                {
                    var featureName = item["FeatureName"].S;
                    var enabled = item["Enabled"].BOOL;
                    _featureToggles[featureName] = enabled;
                }

                scanRequest.ExclusiveStartKey = scanResponse.LastEvaluatedKey;
            } while (scanRequest.ExclusiveStartKey is { Count: > 0 });
        }

        private async Task EnsureTableExists()
        {
            try
            {
                await _amazonDynamoDb.DescribeTableAsync(_tableName);
                await Task.Delay(500); //give a bit of room for table to be created
            }
            catch (ResourceNotFoundException)
            {
                var response = await _amazonDynamoDb.CreateTableAsync(new CreateTableRequest
                {
                    TableName = _tableName,
                    AttributeDefinitions =
                    [
                        new AttributeDefinition("FeatureName", ScalarAttributeType.S)
                    ],
                    KeySchema =
                    [
                        new KeySchemaElement("FeatureName", KeyType.HASH)
                    ],
                    BillingMode = BillingMode.PAY_PER_REQUEST
                });
                if((int)response.HttpStatusCode >= 400)
                {
                    throw new InvalidOperationException("Failed to create table");
                }
            }
        }
    }
}
