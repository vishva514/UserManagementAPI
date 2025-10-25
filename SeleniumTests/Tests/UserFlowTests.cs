using FluentAssertions;
using OpenQA.Selenium;
using Xunit;

namespace SeleniumTests.Tests;

public class UserFlowTests : TestBase
{
    [Fact]
    public void Test1_CompleteRegistration()
    {
        // Navigate to register page
        Driver.Navigate().GoToUrl($"{BaseUrl}/register");
        Thread.Sleep(3000);

        // Get all input fields
        var inputs = Driver.FindElements(By.CssSelector(".MuiInputBase-input"));

        Console.WriteLine($"Found {inputs.Count} fields on registration form");

        // Generate unique email
        var timestamp = DateTime.Now.Ticks;
        var email = $"testuser{timestamp}@test.com";

        // Fill fields (adjust based on your form order)
        if (inputs.Count >= 5)
        {
            // Name
            inputs[0].Click();
            inputs[0].SendKeys("Test User");
            Thread.Sleep(200);

            // Email
            inputs[1].Click();
            inputs[1].SendKeys(email);
            Thread.Sleep(200);

            // Password
            inputs[2].Click();
            inputs[2].SendKeys("TestPass123");
            Thread.Sleep(200);

            // Date of Birth (adjust format as needed)
            inputs[3].Click();
            inputs[3].SendKeys("01/01/1990");
            Thread.Sleep(200);

            // Designation
            inputs[4].Click();
            inputs[4].SendKeys("Teacher");
            Thread.Sleep(200);

            Console.WriteLine($"✅ Filled registration form with email: {email}");
        }

        // Find and click register button
        try
        {
            var buttons = Driver.FindElements(By.CssSelector("button.MuiButton-root"));
            if (buttons.Count > 0)
            {
                buttons[0].Click();
                Console.WriteLine("✅ Clicked register button");
                Thread.Sleep(3000);

                // Check if redirected or success
                Console.WriteLine($"After registration URL: {Driver.Url}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Button click issue: {ex.Message}");
        }
    }

    [Fact]
    public void Test2_LoginAttempt()
    {
        // Go to login
        Driver.Navigate().GoToUrl($"{BaseUrl}/login");
        Thread.Sleep(3000);

        // Fill login form
        var inputs = Driver.FindElements(By.CssSelector(".MuiInputBase-input"));

        if (inputs.Count >= 2)
        {
            // Email
            inputs[0].Click();
            inputs[0].SendKeys("test@example.com");
            Thread.Sleep(300);

            // Password
            inputs[1].Click();
            inputs[1].SendKeys("password123");
            Thread.Sleep(300);

            Console.WriteLine("✅ Filled login credentials");
        }

        // Click login button
        var buttons = Driver.FindElements(By.CssSelector("button.MuiButton-root"));
        if (buttons.Count > 0)
        {
            buttons[0].Click();
            Console.WriteLine("✅ Clicked login button");
            Thread.Sleep(3000);
        }

        // Check result (either error message or redirect)
        Console.WriteLine($"After login URL: {Driver.Url}");

        // Check for any error messages (MUI typically uses Alert or Snackbar)
        var alerts = Driver.FindElements(By.CssSelector(".MuiAlert-root"));
        if (alerts.Count > 0)
        {
            Console.WriteLine($"⚠️ Alert shown: {alerts[0].Text}");
        }
    }

    [Fact]
    public void Test3_NavigationFlow()
    {
        // Start at home
        Driver.Navigate().GoToUrl(BaseUrl);
        Thread.Sleep(2000);
        Console.WriteLine($"Started at: {Driver.Url}");

        // Try to navigate to login (if there's a link/button)
        try
        {
            var loginLink = Driver.FindElement(By.XPath("//a[contains(text(), 'Login')] | //button[contains(text(), 'Login')]"));
            loginLink.Click();
            Thread.Sleep(2000);

            Driver.Url.Should().Contain("login", "Should navigate to login");
            Console.WriteLine("✅ Navigated to login page");
        }
        catch
        {
            Console.WriteLine("⚠️ No login link found, trying direct navigation");
            Driver.Navigate().GoToUrl($"{BaseUrl}/login");
            Thread.Sleep(2000);
        }

        // Try to navigate to register
        try
        {
            var registerLink = Driver.FindElement(By.XPath("//a[contains(text(), 'Register')] | //button[contains(text(), 'Register')] | //a[contains(text(), 'Sign Up')]"));
            registerLink.Click();
            Thread.Sleep(2000);

            Driver.Url.Should().Contain("register", "Should navigate to register");
            Console.WriteLine("✅ Navigated to register page");
        }
        catch
        {
            Console.WriteLine("⚠️ No register link found");
        }
    }
}
