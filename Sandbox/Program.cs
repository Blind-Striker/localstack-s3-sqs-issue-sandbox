using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Console;

var configuration = new ConfigurationBuilder()
    //.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

// Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL_SNS", "http://localhost:8080");

var services = new ServiceCollection()
    .AddLocalStack(configuration)
    //.AddAWSService<IAmazonSimpleNotificationService>()
    .AddAWSServiceLocalStack<IAmazonSimpleNotificationService>()
    .BuildServiceProvider();

var snsClient = services.GetRequiredService<IAmazonSimpleNotificationService>();

WriteLine("Press any key to start the scenario...");
ReadLine();

WriteLine("Creating a SNS topic...");
CreateTopicResponse createTopicResponse = await CreateSnsTopicAsync("MyTopic");

if (createTopicResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
{
    WriteLine("Failed to create the SNS topic");
    return;
}

ListTopicsResponse listTopicsResponse = await snsClient.ListTopicsAsync();
Topic? snsTopic = listTopicsResponse.Topics.SingleOrDefault(topic => topic.TopicArn == createTopicResponse.TopicArn);

WriteLine("Deleting the SNS topic...");
await DeleteSnsTopicAsync(snsTopic!.TopicArn);

WriteLine("Scenario completed");
return;

async Task<CreateTopicResponse> CreateSnsTopicAsync(string topic)
{
    var request = new CreateTopicRequest(topic);

    CreateTopicResponse response = await snsClient.CreateTopicAsync(request);

    return response;
}

async Task<DeleteTopicResponse> DeleteSnsTopicAsync(string topic)
{
    var request = new DeleteTopicRequest(topic);

    DeleteTopicResponse response = await snsClient.DeleteTopicAsync(request);

    return response;
}