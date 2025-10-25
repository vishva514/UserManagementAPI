using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

public class TestBase : IDisposable
{
    protected IWebDriver Driver { get; private set; }
    protected WebDriverWait Wait { get; private set; }

    // Your React app URL
    protected const string BaseUrl = "http://localhost:3000";

    public TestBase()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-notifications");

        Driver = new ChromeDriver(options);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
    }

    public void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }

    // Helper: Wait for Material-UI element
    protected IWebElement WaitForMuiElement(By locator, int timeoutSeconds = 15)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
        return wait.Until(driver => driver.FindElement(locator));
    }

    // Helper: Fill Material-UI TextField
    protected void FillMuiTextField(int inputIndex, string text)
    {
        var inputs = Driver.FindElements(By.CssSelector(".MuiInputBase-input"));
        if (inputs.Count > inputIndex)
        {
            inputs[inputIndex].Clear();
            inputs[inputIndex].SendKeys(text);
            Thread.Sleep(300); // Small delay for MUI
        }
    }

    // Helper: Click Material-UI Button by text
    protected void ClickMuiButton(string buttonText)
    {
        var button = Driver.FindElement(By.XPath($"//button[contains(text(), '{buttonText}')]"));
        button.Click();
        Thread.Sleep(500);
    }
}
