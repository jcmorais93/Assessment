using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Storage.Services;

namespace Storage.Tests;

public class RepositoryTests
{
    private const string LogFilePath = "/tmp/test.log";

    [Fact]
    public void LogData_ShouldAppendToLogFile_WhenTrackerIsValid()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var repository = new Repository(configuration);
        var tracker = new Tracker { Referer = "example.com", UserAgent = "TestUserAgent", IpAddress = "127.0.0.1" };

        // Act
        repository.LogData(tracker);

        // Assert
        File.Exists(LogFilePath).Should().BeTrue();
        File.ReadLines(LogFilePath).Last().Should().EndWith("|example.com|TestUserAgent|127.0.0.1");
    }

    private static IConfigurationRoot BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { {"LogFilePath", LogFilePath} })
            .Build();
}