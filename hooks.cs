using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using urllist;

namespace ApiAutomation.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private const string ApiKey = "9b164609-c50d-4a1c-954c-d996f70373c9";

        private static ExtentReports extent;
        private static ExtentTest feature;
        private ExtentTest scenario;

        
        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }
        [BeforeTestRun]
        public static void InitializeReport()
        {
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
            if (!Directory.Exists(reportPath)) Directory.CreateDirectory(reportPath);

            var sparkReporter = new ExtentSparkReporter(Path.Combine(reportPath, "AutomationReport.html"));
            sparkReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Dark;
            sparkReporter.Config.ReportName = "API Automation Test Report";
            sparkReporter.Config.DocumentTitle = "ExtentReports";

            extent = new ExtentReports();
            extent.AttachReporter(sparkReporter);
        }
        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            feature = extent.CreateTest<Feature>(featureContext.FeatureInfo.Title);
        }
        [BeforeScenario]
        public async Task SetupAndCheckApiKeyHeader()
        {
            var options = new RestClientOptions(ApiUrls.BaseUrl);
            var client = new RestClient(options);
            client.AddDefaultHeader("x-api-key", ApiKey);
            _scenarioContext["RestClient"] = client;

            scenario = feature.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);
        }
        [AfterStep]
        public void InsertReportingStep()
        {
            var stepType = _scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            var stepText = _scenarioContext.StepContext.StepInfo.Text;

            if (_scenarioContext.TestError == null)
            {
                if (stepType == "Given") scenario.CreateNode<Given>(stepText);
                else if (stepType == "When") scenario.CreateNode<When>(stepText);
                else if (stepType == "Then") scenario.CreateNode<Then>(stepText);
            }
            else
            {
                string errorMessage = _scenarioContext.TestError.Message;

                if (stepType == "Given") scenario.CreateNode<Given>(stepText).Fail(errorMessage);
                else if (stepType == "When") scenario.CreateNode<When>(stepText).Fail(errorMessage);
                else if (stepType == "Then") scenario.CreateNode<Then>(stepText).Fail(errorMessage);
            }
        }
        [AfterScenario]
        public async Task CleanupCreatedData()
        {
            if (_scenarioContext.ContainsKey("CreatedDeviceId"))
            {
                var client = (RestClient)_scenarioContext["RestClient"];
                var deviceId = _scenarioContext["CreatedDeviceId"].ToString();
                var request = new RestRequest($"/objects/{deviceId}", Method.Delete);

                Console.WriteLine($"[Teardown] Cleaning up created device ID: {deviceId}");
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    Console.WriteLine($"[Teardown] Successfully deleted device ID: {deviceId}");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"[Teardown Info] Device ID {deviceId} was already discarded or did not persist on JSONPlaceholder.");
                }
                else
                {
                    Console.WriteLine($"[Teardown Warning] Failed to clean up device ID {deviceId}. Status: {response.StatusCode}");
                }
            }
        }
        [AfterTestRun]
        public static void FlushReport()
        {
            extent.Flush();
        }
    }
}
