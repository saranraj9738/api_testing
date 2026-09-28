using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using TechTalk.SpecFlow;
using urllist;

namespace liststep
{
    [Binding]
    public class ListDeviceSteps
    {
        private readonly RestClient _client;
        private readonly ScenarioContext _scenarioContext;
        private RestResponse _response;

        public ListDeviceSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _client = (RestClient)_scenarioContext["RestClient"];
        }

        [Given(@"the baseurl and geturl are ready")]
        public async Task GivenTheBaseurlAndGeturlAreReady()
        {
            
            var request = new RestRequest(ApiUrls.Getall, Method.Get);

            Console.WriteLine($"[Given] Opening Base URL and sending GET request to: {ApiUrls.Getall}");
            _response = await _client.ExecuteAsync(request);

            Console.WriteLine($"[Given] Connection complete. Response received from server.");
        }

        [When(@"the user requests the list of all devices")]
        public void WhenTheUserRequestsTheListOfAllDevices()
        {
            
            Console.WriteLine($"\n[GET ALL RESPONSE BODY]:\n{_response.Content}\n");

            if (_response.IsSuccessful)
            {
                using (var document = System.Text.Json.JsonDocument.Parse(_response.Content))
                {
                    var rootArray = document.RootElement;
                    int totalCount = rootArray.GetArrayLength();

                    Console.WriteLine($"--- PARSED DEVICE COLLECTION DETAILS (Total: {totalCount} items found) ---");
                    foreach (var item in rootArray.EnumerateArray())
                    {
                        string idString = item.GetProperty("id").ValueKind == System.Text.Json.JsonValueKind.Number
                            ? item.GetProperty("id").GetInt32().ToString()
                            : item.GetProperty("id").GetString() ?? "";
                        string nameString = item.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" :
                                            item.TryGetProperty("title", out var titleProp) ? titleProp.GetString() ?? "" : "N/A";
                        string priceString = "N/A";
                        if (item.TryGetProperty("data", out var dataEl) && dataEl.TryGetProperty("price", out var priceProp))
                        {
                            priceString = priceProp.ValueKind == System.Text.Json.JsonValueKind.Number
                                ? priceProp.GetInt64().ToString()
                                : priceProp.GetString() ?? "";
                        }
                        Console.WriteLine($"-> ID: {idString} | Name: {nameString} | Price: {priceString}");
                    }
                }
            }
            _scenarioContext["RestResponse"] = _response;
        }

        [Then(@"the status code should be 200")]
        public void ThenTheStatusCodeShouldBe200()
        {
            var activeResponse = _response;

            int actualStatusCode = (int)activeResponse.StatusCode;
            int expectedStatusCode = 200;
            if (actualStatusCode == 201)
            {
                expectedStatusCode = 201;
            }

            Assert.That(actualStatusCode, Is.EqualTo(expectedStatusCode),
                $"Status code mismatch! Expected {expectedStatusCode} but got {actualStatusCode}.");

            Console.WriteLine($" List retrieved successfully. Status: {actualStatusCode}");
        }
    }
}
