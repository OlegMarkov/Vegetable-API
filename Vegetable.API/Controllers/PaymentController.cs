using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Vegetable.API.Attributes;
using Vegetable.Core.Database;
using Vegetable.API.Services;
using Vegetable.API.ViewModels.Payment;
using Vegetable.Entities;

namespace Vegetable.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly IOwnerRepo _ownerRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly ISettingsRepo _settingsRepo;

        public PaymentController(IPaymentService checkPaymentNotificationSignService, IMapper mapper, IOwnerRepo ownerRepo, IOrderRepo orderRepo, ISettingsRepo settingsRepo)
        {
            _paymentService = checkPaymentNotificationSignService;
            _mapper = mapper;
            _ownerRepo = ownerRepo;
            _orderRepo = orderRepo;
            _settingsRepo = settingsRepo;
        }

        [HttpPost("notification")]
        public async Task<ActionResult<string>> Notification([FromBody] PaymentNotificationMessage message)
        {
            if (!_paymentService.CheckSign(message)) return BadRequest();
            var paymentNotification = _mapper.Map<PaymentNotification>(message);
            var paymentNotificationSaved = await _orderRepo.AddPaymentNotification(paymentNotification);
            if (paymentNotification.Status == "CONFIRMED")
                await _ownerRepo.UpdateOwnerSubscription(paymentNotification.OwnerId.Value, paymentNotification.OrderId);
            if (!paymentNotificationSaved) return BadRequest();
            return Ok("OK");
        }

        [AuthorizeOwner]
        //[HttpGet("initPayment/{subscriptionTypeId}")]
        public async Task<ActionResult<string>> InitPaymentLive(int subscriptionTypeId, int quantity)
        {
            if (quantity < 1) return BadRequest("Incorrect quantity");
            var subscriptionType = await _settingsRepo.GetSubscriptionTypesById(subscriptionTypeId);
            if (subscriptionType == null) return BadRequest("Subscription unavalible");

            var ownerId = Guid.Parse((string)HttpContext.Items["OwnerId"]);
            var owner = await _ownerRepo.GetOwnerInformation(ownerId);

            var pendingOrder = await _orderRepo.GetPendingOrder(ownerId, subscriptionTypeId, quantity);
            if (pendingOrder != null) return Ok(pendingOrder.PaymentURL);
            
            Order order = new Order() { 
                OwnerId = ownerId, 
                Quantity = quantity, 
                SubscriptionTypeId = subscriptionTypeId };

            var discount = await _settingsRepo.GetDiscountByQuantity(quantity);
            var totalQuantity = owner.SubscriptionStartDate != null ? quantity : quantity - discount.TrialQuantity;
            order.Amount = discount == null ? subscriptionType.Price * quantity : 
                subscriptionType.Price * totalQuantity * (100 - discount.Percentage) / 100;
            
            order = await _orderRepo.CreateOrder(order);

            if(order.Amount == 0)
            {
                await _ownerRepo.UpdateOwnerSubscription(order.OwnerId.Value, order.Id);
                return Ok();
            }

            InitResponse paymentResponse;
            try
            {
                paymentResponse = await _paymentService.InitPaymentRequest(order, owner, subscriptionType);
            }
            catch (Exception)
            {
                await _orderRepo.DeleteOrder(order);
                return StatusCode(503);
            }
           
            _mapper.Map(paymentResponse, order);

            var saved = await _orderRepo.UpdateOrder(order);

            if (!saved) return BadRequest();
            return Ok(order.PaymentURL);
        }

        [AuthorizeOwner]
        [HttpGet("initPayment/{subscriptionTypeId}")]
        public async Task<ActionResult<string>> InitPayment(int subscriptionTypeId, int quantity)
        {
            if (quantity < 1) return BadRequest("Incorrect quantity");
            var subscriptionType = await _settingsRepo.GetSubscriptionTypesById(subscriptionTypeId);
            if (subscriptionType == null) return BadRequest("Subscription unavalible");

            var ownerId = Guid.Parse((string)HttpContext.Items["OwnerId"]);
            var owner = await _ownerRepo.GetOwnerInformation(ownerId);

            var pendingOrder = await _orderRepo.GetPendingOrder(ownerId, subscriptionTypeId, quantity);
            if (pendingOrder != null) return Ok(pendingOrder.PaymentURL);

            Order order = new Order()
            {
                OwnerId = ownerId,
                Quantity = quantity,
                SubscriptionTypeId = subscriptionTypeId
            };

            var discount = await _settingsRepo.GetDiscountByQuantity(quantity);
            var totalQuantity = owner.SubscriptionStartDate != null ? quantity : quantity - discount.TrialQuantity;
            order.Amount = discount == null ? subscriptionType.Price * quantity :
                subscriptionType.Price * totalQuantity * (100 - discount.Percentage) / 100;

            order = await _orderRepo.CreateOrder(order);

            if (order.Amount == 0)
            {
                await _ownerRepo.UpdateOwnerSubscription(order.OwnerId.Value, order.Id);
                return Ok();
            }

            await _ownerRepo.UpdateOwnerSubscription(order.OwnerId.Value, order.Id);
            return Ok();
        }
    }
}
