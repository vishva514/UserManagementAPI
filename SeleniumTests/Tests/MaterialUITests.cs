using FluentAssertions;
using OpenQA.Selenium;
using Xunit;

namespace SeleniumTests.Tests;

public class MaterialUITests : TestBase
{
    [Fact]
    public void Test1_AppLoads()
    {
        // Navigate to home page
        Driver.Navigate().GoToUrl(BaseUrl);
        Thread.Sleep(2000);

        // Verify page loaded
        Driver.Title.Should().NotBeEmpty();
        Driver.Url.Should().Contain("localhost:3000");

        Console.WriteLine($"✅ App loaded successfully!");
        Console.WriteLine($"   Title: {Driver.Title}");
    }

    [Fact]
    public void Test2_LoginPageElements()
    {
        // Go to login page
        Driver.Navigate().GoToUrl($"{BaseUrl}/login");
        Thread.Sleep(3000);

        // Verify on login page
        Driver.Url.Should().Contain("login");

        // Count Material-UI input fields
        var inputs = Driver.FindElements(By.CssSelector(".MuiInputBase-input"));
        Console.WriteLine($"📋 Found {inputs.Count} MUI input fields");

        // Should have at least 2 inputs (email, password)
        inputs.Count.Should().BeGreaterThanOrEqualTo(2, "Login should have email and password fields");

        // Check for buttons
        var buttons = Driver.FindElements(By.CssSelector("button.MuiButton-root"));
        Console.WriteLine($"📋 Found {buttons.Count} MUI buttons");

        buttons.Count.Should().BeGreaterThan(0, "Should have login button");

        Console.WriteLine("✅ Login page elements verified!");
    }

    [Fact]
    public void Test3_FillLoginForm()
    {
        // Go to login page
        Driver.Navigate().GoToUrl($"{BaseUrl}/login");
        Thread.Sleep(3000);

        // Get all MUI input fields
        var inputs = Driver.FindElements(By.CssSelector(".MuiInputBase-input"));

        Console.WriteLine($"Found {inputs.Count} input fields");

        // Fill first field (email/username)
        if (inputs.Count >= 1)
        {
            inputs[0].Click();
            Thread.Sleep(200);
            inputs[0].SendKeys("testuser@example.com");
            Console.WriteLine("✅ Filled email field");
        }

        // Fill second field (password)
        if (inputs.Count >= 2)
        {
            inputs[1].Click();
            Thread.Sleep(200);
            inputs[1].SendKeys("TestPassword123");
            Console.WriteLine("✅ Filled password field");
        }

        Thread.Sleep(1000);

        // Verify fields have values
        inputs = Driver.FindElements(By.CssSelector(".MuiInputBase-input"));
        inputs[0].GetAttribute("value").Should().NotBeEmpty("Email should be filled");
        inputs[1].GetAttribute("value").Should().NotBeEmpty("Password should be filled");

        Console.WriteLine("✅ Form filled successfully!");
    }

    [Fact]
    public void Test4_RegisterPageElements()
    {
        // Go to register page
        Driver.Navigate().GoToUrl($"{BaseUrl}/register");
        Thread.Sleep(3000);

        // Verify on register page
        Driver.Url.Should().Contain("register");

        // Count input fields (register usually has more)
        var inputs = Driver.FindElements(By.CssSelector(".MuiInputBase-input"));
        Console.WriteLine($"📋 Register page has {inputs.Count} input fields");

        // Register page should have at least 4-5 fields
        inputs.Count.Should().BeGreaterThanOrEqualTo(4, "Register should have multiple fields");

        Console.WriteLine("✅ Register page elements verified!");
    }
}
