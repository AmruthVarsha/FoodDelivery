namespace OrderService.Domain.Enums
{
    public enum PaymentMethod
    {
        COD,    // Cash on Delivery — confirmed on delivery
        Online  // Online gateway (Razorpay-ready) — pre-paid
    }
}

