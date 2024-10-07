# LocalStack.NET Bug Reproduction

This application is designed to reproduce a bug reported for LocalStack with .NET 8 integration. The application provides a scenario for the user to `Create an SNS topic and delete it`

Related Issue:
- [https://github.com/localstack/localstack/issues/11652](https://github.com/localstack/localstack/issues/11652)

## Prerequisites

Before you can run this application, you must have .NET 8 installed. If you don't have .NET 8 installed, follow these steps:

1. Download the .NET 8 SDK from [here](https://dotnet.microsoft.com/download/dotnet/8.0).
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

Upon execution, the application will prompt you to press any key and proceed with the scenario, then it will create an SNS topic and delete it.
