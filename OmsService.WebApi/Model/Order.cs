namespace OmsService.WebApi.Model
{
    public class Order
    {
        public Guid Id { get; set; }
        public string TraderPamCode { get; set; }
        public string InstrumentIsin { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public OrderSide OrderSide { get; set; }
    }
}
