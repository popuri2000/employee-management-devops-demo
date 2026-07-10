using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EmployeeManagement.Tests.Services;

public class AuthServiceTests
{
    private static AuthService CreateSut()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DemoLogin:Username"] = "admin",
                ["DemoLogin:Password"] = "Admin@123"
            })
            .Build();

        return new AuthService(configuration);
    }

    [Fact]
    public void Login_ReturnsSuccess_WhenCredentialsAreCorrect()
    {
        var sut = CreateSut();

        var result = sut.Login(new LoginRequestDto { Username = "admin", Password = "Admin@123" });

        Assert.True(result.Success);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public void Login_ReturnsFailure_WhenPasswordIsIncorrect()
    {
        var sut = CreateSut();

        var result = sut.Login(new LoginRequestDto { Username = "admin", Password = "WrongPassword" });

        Assert.False(result.Success);
        Assert.Null(result.Token);
    }

    [Fact]
    public void Login_ReturnsFailure_WhenUsernameIsUnknown()
    {
        var sut = CreateSut();

        var result = sut.Login(new LoginRequestDto { Username = "nobody", Password = "Admin@123" });

        Assert.False(result.Success);
    }

    [Fact]
    public void Login_IsCaseInsensitive_ForUsername()
    {
        var sut = CreateSut();

        var result = sut.Login(new LoginRequestDto { Username = "ADMIN", Password = "Admin@123" });

        Assert.True(result.Success);
    }
}
