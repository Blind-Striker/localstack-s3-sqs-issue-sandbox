# LocalStack.NET Bug Reproduction

This application is designed to reproduce a bug reported for LocalStack with .NET 7 integration. The application provides two scenarios for the user:
1. Create an S3 bucket and delete it.
2. Create an SQS queue and delete it.

## Prerequisites

Before you can run this application, you must have .NET 7 installed. If you don't have .NET 7 installed, follow these steps:

1. Download the .NET 7 SDK from [here](https://dotnet.microsoft.com/download/dotnet/7.0).
2. Install the SDK by following the instructions specific to your OS.

## Building the Application

Navigate to the root of the project directory in your terminal or command prompt and run the following command:

```bash
dotnet build
```

## Configuration

Before running the application, you might need to modify the default LocalStack host and port settings. This can be done by editing the `appsettings.json` file.

Here's a sample configuration:

```json
"LocalStack": {
    "UseLocalStack": true,
    ...
    "Config": {
      "LocalStackHost": "localhost.localstack.cloud",
      ...
      "EdgePort": 4566
    }
}
```

- LocalStackHost: The hostname of your LocalStack instance. Modify this if you're running LocalStack on a different host.

- EdgePort: The port on which LocalStack services are exposed. Adjust this if your LocalStack instance uses a different port.

## Running the Application

Navigate to the root of the project directory in your terminal or command prompt and run the following command:

```bash
dotnet run --project .\Sandbox\Sandbox.csproj
```

Upon execution, the application will prompt you to choose a scenario (s3 or sqs). Based on your choice, it will either create an S3 bucket or an SQS queue and then delete it