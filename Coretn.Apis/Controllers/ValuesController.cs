using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coretn.Apis.Models;
using Coretn.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace Coretn.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private RabbitMqService _rabbitMqService;
        public ValuesController() {
            var connectionfactory = new ConnectionFactory
            {
                //Endpoint = new AmqpTcpEndpoint(new Uri("amqp://47.52.230.90:15672/")),
                HostName = "47.52.230.90",
                Port = 5672,
                VirtualHost = "coretn_mq",
                UserName = "song",
                Password = "232256",
                AutomaticRecoveryEnabled = true
            };
            _rabbitMqService = new RabbitMqService(connectionfactory);
           
        }
      
        // GET api/values
        [HttpGet("basicpublish")]
        public string BasicPublish(string message = "BasicPublish")
        {
            Task.Run(() =>
            {
                _rabbitMqService.BasicPublish(new MqRequestModel { Message = message });
            });
            return message;
        }

        [HttpGet("received")]
        public string Received(string message = "Received")
        {
           Task.Run(() =>
            {
                _rabbitMqService.Received();
            });
            return message;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<bool> Get(int id)
        {
            return "II" == "IlIl";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
