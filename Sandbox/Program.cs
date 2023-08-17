using System.Net;
using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Console;

var configuration = new ConfigurationBuilder()
    //.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection()
    .AddLocalStack(configuration)
    .AddAWSServiceLocalStack<IAmazonS3>()
    .AddAWSServiceLocalStack<IAmazonSQS>()
    .BuildServiceProvider();

var s3Client = services.GetRequiredService<IAmazonS3>();
var sqsClient = services.GetRequiredService<IAmazonSQS>();

Write("Please enter which scenario you want to run, sqs or s3 (default: s3):");
string scenario = ReadLine() ?? "s3";
if (scenario != "s3" && scenario != "sqs")
{
    WriteLine("Invalid scenario, defaulting to s3");
    scenario = "s3";
}

WriteLine("Running scenario: " + scenario);


if (scenario == "s3")
{
    var bucketName = Guid.NewGuid().ToString();
    WriteLine("Creating bucket: " + bucketName);
    
    PutBucketResponse putBucketResponse = await CreateS3Bucket(bucketName);
    
    if (putBucketResponse.HttpStatusCode != HttpStatusCode.OK)
    {
        WriteLine("Failed to create bucket");
    }
    else
    {
        WriteLine("Successfully created bucket");
        WriteLine("Press any key to delete bucket");
        ReadKey();
        WriteLine("Deleting bucket: " + bucketName);
        
        DeleteBucketResponse deleteBucketResponse = await DeleteS3Bucket(bucketName);

        WriteLine(deleteBucketResponse.HttpStatusCode != HttpStatusCode.NoContent
            ? "Failed to delete bucket"
            : "Successfully deleted bucket");
    }
}
else
{
    var guid = Guid.NewGuid();
    var queueName = $"{guid}.fifo";
    var dlQueueName = $"{guid}-DLQ.fifo";
    WriteLine("Creating queue: " + queueName);
    
    CreateQueueResponse createQueueResponse = await CreateFifoQueueWithRedrive(queueName, dlQueueName);

    if (createQueueResponse.HttpStatusCode != HttpStatusCode.OK)
    {
        WriteLine("Failed to create queue");
    }
    else
    {
        WriteLine("Successfully created queue");
        WriteLine("Press any key to delete queue");
        ReadKey();
        WriteLine("Deleting queue: " + queueName);
        
        DeleteQueueResponse deleteQueueResponse = await DeleteQueue(createQueueResponse.QueueUrl);

        WriteLine(deleteQueueResponse.HttpStatusCode != HttpStatusCode.OK
            ? "Failed to delete queue"
            : "Successfully deleted queue");
    }
}

WriteLine("Scenario completed, press any key to exit");
ReadKey();

async Task<PutBucketResponse> CreateS3Bucket(string bucketName)
{
    var putBucketRequest = new PutBucketRequest { BucketName = bucketName, UseClientRegion = true };

    return await s3Client.PutBucketAsync(putBucketRequest);
}

async Task<DeleteBucketResponse> DeleteS3Bucket(string bucketName)
{
    var deleteBucketRequest = new DeleteBucketRequest { BucketName = bucketName, UseClientRegion = true };

    return await s3Client.DeleteBucketAsync(deleteBucketRequest);
}

async Task<CreateQueueResponse> CreateFifoQueueWithRedrive(string queueName, string dlQueueName)
{
    var createDlqRequest = new CreateQueueRequest
        { QueueName = dlQueueName, Attributes = new Dictionary<string, string> { { "FifoQueue", "true" }, } };

    CreateQueueResponse createDlqResult = await sqsClient.CreateQueueAsync(createDlqRequest);

    GetQueueAttributesResponse attributes = await sqsClient.GetQueueAttributesAsync(new GetQueueAttributesRequest
    {
        QueueUrl = createDlqResult.QueueUrl,
        AttributeNames = new List<string> { "QueueArn" }
    });

    var redrivePolicy = new { maxReceiveCount = "1", deadLetterTargetArn = attributes.Attributes["QueueArn"] };

    var createQueueRequest = new CreateQueueRequest
    {
        QueueName = queueName,
        Attributes = new Dictionary<string, string>
            { { "FifoQueue", "true" }, { "RedrivePolicy", JsonSerializer.Serialize(redrivePolicy) }, }
    };

    return await sqsClient.CreateQueueAsync(createQueueRequest);
}

async Task<DeleteQueueResponse> DeleteQueue(string queueUrl)
{
    var deleteQueueRequest = new DeleteQueueRequest(queueUrl);

    return await sqsClient.DeleteQueueAsync(deleteQueueRequest);
}