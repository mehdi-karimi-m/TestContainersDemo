namespace OmsService.SpecTests.Model
{
    public class PlaceOrderDto
    {
        public string TraderPamCode { get; set; }
        public string InstrumentIsin { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public OrderSide OrderSide { get; set; }
    }
}
