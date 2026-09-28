using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using TechTalk.SpecFlow;
using urllist;

namespace ApiAutomation.StepDefinitions
{
    [Binding]
    public class DeviceSteps
    {
        private readonly RestClient _client;
        private readonly ScenarioContext _scenarioContext;
        private RestResponse _response;

        public DeviceSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _client = (RestClient)_scenarioContext["RestClient"];
        }

        [Given(@"the baseurl and posturl are ready")]
        public void GivenTheBaseurlAndPosturlAreReady()
        {
            Console.WriteLine("[Steps] RestClient connection and endpoints are ready to accept outbound payloads.");
        }

        [When(@"the user sends the devicename ""(.*)"" and price (.*)")]
        public async Task WhenTheUserSendsThePhonenameAndPrice(string deviceName, long price)
        {
            
            var request = new RestRequest(ApiUrls.Post, Method.Post);
            request.AddJsonBody(new
            {
                name = deviceName,
                data = new { price = price }
            });
            _response = await _client.ExecuteAsync(request);
            if (_response.IsSuccessful)
            {
                using (var document = System.Text.Json.JsonDocument.Parse(_response.Content))
                {
                    if (document.RootElement.TryGetProperty("id", out var idProperty))
                    {
                        string createdId = idProperty.ValueKind == System.Text.Json.JsonValueKind.Number
                            ? idProperty.GetInt32().ToString()
                            : idProperty.GetString() ?? "";

                        _scenarioContext["CreatedDeviceId"] = createdId;
                    }
                }
            }
        }

        [When(@"the user sends the devicename ""(.*)"" and text price ""(.*)""")]
        public async Task WhenTheUserSendsTheDevicenameAndTextPrice(string deviceName, string Price)
        {
            var request = new RestRequest(ApiUrls.Post, Method.Post);
            request.AddJsonBody(new
            {
                name = deviceName,
                data = new { price = Price }
            });
            _response = await _client.ExecuteAsync(request);
        }

        [Then(@"the status code must shown (.*)")]
        public void ThenTheStatusCodeShouldBe(int expectedStatusCode)
        {
            var activeResponse = _response ?? (RestResponse)_scenarioContext["RestResponse"];
            int actualStatusCode = (int)activeResponse.StatusCode;

            Assert.That(actualStatusCode, Is.EqualTo(expectedStatusCode),
                $"Status code mismatch! Expected {expectedStatusCode} but got {actualStatusCode}. Response: {activeResponse.Content}");

            Console.WriteLine($"[Assertion Passed] Verified status code is {actualStatusCode}");
        }
    }
}
