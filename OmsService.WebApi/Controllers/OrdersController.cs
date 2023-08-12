using Microsoft.AspNetCore.Mvc;
using OmsService.WebApi.Dtos;
using OmsService.WebApi.Model;

namespace OmsService.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderDto orderDto)
        {
            if (orderDto is null)
            {
                throw new ArgumentNullException(nameof(orderDto));
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                TraderPamCode = orderDto.TraderPamCode,
                InstrumentIsin = orderDto.InstrumentIsin,
                Quantity = orderDto.Quantity,
                Price = orderDto.Price,
                OrderSide = orderDto.OrderSide,
            };

            await _orderRepository.AddAsync(order);
            
            return Ok(order.Id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderRepository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _orderRepository.GetAsync(id);
            return Ok(order);
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _orderRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
