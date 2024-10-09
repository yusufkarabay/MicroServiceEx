using Bus.Shared;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace MSOne.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IPublishEndpoint _publishEndpoint;

        public UserController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint=publishEndpoint;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser()
        {
            var userCreatedEvent = new UserCreatedEvent
            {
                UserId = Guid.NewGuid(),
                Name = "Yusuf",
                Email = "yusufkarabay21@gmail.com",
                Phone ="555 555 55 55"
            };
            // rabbitmq ayakta değilse deneme süresi
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            cancellationToken.CancelAfter(TimeSpan.FromSeconds(40));

            await _publishEndpoint.Publish(userCreatedEvent,
                pupline =>
                {
                    pupline.SetAwaitAck(true);
                    //mesaj kalıcı olarak rabbitmq de saklansın
                    pupline.Durable = true;
                },
                cancellationToken.Token
                );

            return Ok();
        }
    }
}
