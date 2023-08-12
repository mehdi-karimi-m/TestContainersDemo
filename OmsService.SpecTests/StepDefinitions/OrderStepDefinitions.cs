using OmsService.SpecTests.Model;
using System;
using System.Net.Http.Json;
using TechTalk.SpecFlow;

namespace OmsService.SpecTests.StepDefinitions
{
    [Binding]
    public class OrderStepDefinitions
    {
        private Mehdi _mehdi = new();
        private PlaceOrderDto _placeOrderDto;
        private string baseAddress = "https://localhost:7152/";

        [Given(@"Mehdi is a trader with PamCode '(.*)'")]
        public void GivenMehdiIsATraderWithPamCode(string pamCode)
        {
            _mehdi.TraderPamCode = pamCode;
        }

        [When(@"he places a buying order for the Shasta instrument with isin '(.*)' by price (.*) and quantity (.*)")]
        public async Task WhenHePlacesABuyingOrderForTheShastaInstrumentWithIsinByPriceAndQuantity(string instrumentIsin, int price, int quantity)
        {
            _placeOrderDto = new PlaceOrderDto
            {
                TraderPamCode = _mehdi.TraderPamCode,
                InstrumentIsin = instrumentIsin,
                Quantity = quantity,
                Price = price,
                OrderSide = OrderSide.Buy
            };

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            await httpClient.PostAsJsonAsync("Orders", _placeOrderDto);
        }

        [Then(@"he sees his order in his orders")]
        public async Task ThenHeSeesHisOrderInHisOrders()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            var orders = await httpClient.GetFromJsonAsync<List<Order>>("Orders");

            var expectOrder = new Order
            {
                TraderPamCode = _placeOrderDto.TraderPamCode,
                InstrumentIsin = _placeOrderDto.InstrumentIsin,
                Quantity = _placeOrderDto.Quantity,
                Price = _placeOrderDto.Price,
                OrderSide = _placeOrderDto.OrderSide
            };

            orders.Should().ContainEquivalentOf(expectOrder, options => options.Excluding(e => e.Id));          
        }
    }
}
