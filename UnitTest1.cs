using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Threading;

namespace EvotixTest
{
    public class Tests
    {
        IWebDriver driver;
        private string url => 
            "https://stirling.she-development.net/automation";
        [SetUp]
        public void Setup()
        {
            var option = new ChromeOptions();
            option.AddArguments("start-maximized", "incognito");
            driver = new ChromeDriver(option);
            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void Test1()
        {
            //Login
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            Login();

            //Click Enviroment
            By Environment = By.XPath("//a[@data-areaname='Environment']");
            wait.Until(ExpectedConditions.ElementToBeClickable(Environment));
            driver.FindElement(Environment).Click();

            //Step1 
            CheckIfDisplayed();
            //Click new record
            By newRecord = By.XPath("//a[contains(@class, 'create_record')]");
            wait.Until(ExpectedConditions.ElementToBeClickable(newRecord));
            driver.FindElement(newRecord).Click();

            //Enter description and date
            EnterDescOrDate("SheEnvironmental.Description", "autotest1???");
            EnterDescOrDate("SheEnvironmental.AssessmentDate", "07/03/2022");
            
            //Save and Close
            By SaveAndClosebtn = By.XPath("//button[@name='submitButton'][.='Save & Close']");
            driver.FindElement(SaveAndClosebtn).Click();

            //validate added records
            By addedRecord(string desc) =>
                By.XPath($"//div[@class='item-box ui-selectable']//div//a[contains(@title, '{desc}')]");
            wait.Until(ExpectedConditions.ElementIsVisible(addedRecord("autotest1???")));
            Assert.IsTrue(driver.FindElement(addedRecord("autotest1???")).Displayed);


            //Step2 
            //Click new record
            driver.FindElement(newRecord).Click();

            //Enter description and date
            EnterDescOrDate("SheEnvironmental.Description", "autotest2???");
            EnterDescOrDate("SheEnvironmental.AssessmentDate", "28/03/2022");

            //Save and Close
            //By SaveAndClosebtn = By.XPath("//button[@name='submitButton'][.='Save & Close']");
            driver.FindElement(SaveAndClosebtn).Click();

            //validate added records
            wait.Until(ExpectedConditions.ElementIsVisible(addedRecord("autotest2???")));
            Assert.IsTrue(driver.FindElement(addedRecord("autotest1???")).Displayed);


            //Identify first added record and click cog
            By deletebtnForAddedRecord(string desc) => 
                By.XPath($"//div[@class='item-box ui-selectable']//div//a[contains(@title, '{desc}')]//following::div[@class='manage']/div[@class='btn-group']/button");
            driver.FindElement(deletebtnForAddedRecord("autotest1???")).Click();

            //click on delete from cog dropdown
            By isrecordDisplayed = By.XPath("(//a[@class='deleteDialog'][normalize-space(.)='Delete'])[1]");
            //wait.Until(ExpectedConditions.ElementToBeClickable(isrecordDisplayed));
            var deletebtn = driver.FindElement(isrecordDisplayed);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click()", deletebtn);


            //If confirm delete pop up displayed click confirm
            Thread.Sleep(2000);
            By Options(string option) => By.XPath($"//button[text()='{option}']");
            var confirmbtn = driver.FindElement(Options("Confirm"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click()", confirmbtn);
            Thread.Sleep(3000);

            //Validate record is not displayed
            CheckIfDisplayed();
            var trueOrFalse = Exists(By.XPath($"//div[@class='item-box ui-selectable']//div//a[contains(@title, 'autotest1???')]"));
            Assert.False(trueOrFalse);
        }

        public void Login()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@ng-model='model.username']")));
            driver.FindElement(By.XPath("//input[@ng-model='model.username']")).SendKeys("AnecheP");
            driver.FindElement(By.XPath("//input[@ng-model='model.password']")).SendKeys("Evo@88");
            driver.FindElement(By.XPath("//button[@id='login']")).Click();
        }

        public void EnterDescOrDate(string element, string value)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript($"return document.getElementsByName('{element}')[0].value = '{value}'");
        }

        public void CheckIfDisplayed()
        {
            //Initial Check
            //Identify first added record and click cog
            try
            {
                By deletebtnForAddedRecord(string desc) =>
                By.XPath($"//div[@class='item-box ui-selectable']//div//a[contains(@title, '{desc}')]//following::div[@class='manage']/div[@class='btn-group']/button");
                var descriptions = driver.FindElement(deletebtnForAddedRecord("autotest1???"));
                if (descriptions.Displayed)
                {
                    descriptions.Click();
                    By isrecordDisplayed = By.XPath("(//a[@class='deleteDialog'][normalize-space(.)='Delete'])[1]");
                    //wait.Until(ExpectedConditions.ElementToBeClickable(isrecordDisplayed));
                    var deletebtn = driver.FindElement(isrecordDisplayed);
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click()", deletebtn);
                    //If confirm delete pop up displayed click confirm
                    Thread.Sleep(2000);
                    By Options(string option) => By.XPath($"//button[text()='{option}']");
                    var confirmbtn = driver.FindElement(Options("Confirm"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click()", confirmbtn);
                    Thread.Sleep(2000);
                }

                var descriptions2 = driver.FindElement(deletebtnForAddedRecord("autotest2???"));
                if (descriptions2.Displayed)
                {
                    descriptions2.Click();
                    By isrecordDisplayed = By.XPath("(//a[@class='deleteDialog'][normalize-space(.)='Delete'])[1]");
                    //wait.Until(ExpectedConditions.ElementToBeClickable(isrecordDisplayed));
                    var deletebtn = driver.FindElement(isrecordDisplayed);
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click()", deletebtn);
                    //If confirm delete pop up displayed click confirm
                    Thread.Sleep(2000);
                    By Options(string option) => By.XPath($"//button[text()='{option}']");
                    var confirmbtn = driver.FindElement(Options("Confirm"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click()", confirmbtn);
                    Thread.Sleep(2000);
                }
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine(e);
            }
        }

        public bool Exists(By by)
        {
            if (driver.FindElements(by).Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [TearDown]
        public void Quitbrowser()
        {
            driver.Quit();
        }
    }
}