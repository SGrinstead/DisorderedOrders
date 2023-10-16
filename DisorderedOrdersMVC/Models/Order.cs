using DisorderedOrdersMVC.Services;

namespace DisorderedOrdersMVC.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public int CalculateTotalPrice()
        {
			int total = 0;
			foreach (var orderItem in Items)
			{
				var itemPrice = orderItem.Item.Price * orderItem.Quantity;
				total += itemPrice;
			}
			return total;
		}

		public void VerifyStockAvailabile()
		{
			foreach (var orderItem in Items)
			{
				if (!orderItem.Item.InStock(orderItem.Quantity))
				{
					orderItem.Quantity = orderItem.Item.StockQuantity;
				}

				orderItem.Item.DecreaseStock(orderItem.Quantity);
			}
		}

		public void Checkout(string paymentType)
		{
			// verify stock available
			VerifyStockAvailabile();

			// calculate total price
			var total = CalculateTotalPrice();

			// process payment
			IPaymentProcessor processor = GetPaymentProcessor(paymentType);

			processor.ProcessPayment(total);
		}

		private IPaymentProcessor GetPaymentProcessor(string paymentType)
		{
			if (paymentType == "bitcoin")
			{
				return new BitcoinProcessor();
			}
			else if (paymentType == "paypal")
			{
				return new PayPalProcessor();
			}
			else
			{
				return new CreditCardProcessor();
			}
		}
	}
}
