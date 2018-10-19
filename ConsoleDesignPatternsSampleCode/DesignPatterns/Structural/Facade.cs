using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleDesignPatternsSampleCode.DesignPatterns.Structural
{

    public interface IInventory
    {
        void Update(int productId);
    }

    public interface IOrderVerify
    {
        bool VerifyShippingAddress(int pincode);
    }

    public interface ICosting
    {
        float ApplyDiscounts(float price, float discountPercent);
    }

    public interface IPaymentGateway
    {
        bool VerifyCardDetails(string cardNo);
        bool ProcessPayment(string cardNo, float cost);
    }

    public interface ILogistics
    {
        void ShipProduct(string productName, string shippingAddress);
    }

    public class InventoryManager : IInventory
    {
        public void Update(int productId)
        {
            WriteLine($"Product# {productId} is subtracted from the store's inventory.");
        }
    }

    public class OrderVerificationManager : IOrderVerify
    {
        public bool VerifyShippingAddress(int pincode)
        {
            WriteLine($"The product can be shipped to the pincode {pincode}.");
            return true;
        }
    }

    public class CostManager : ICosting
    {
        public float ApplyDiscounts(float price, float discountPercent)
        {
            WriteLine($"A discount of {discountPercent}% has been applied on the product's price of {price}");
            return price - ((discountPercent / 100) * price);
        }
    }

    public class PaymentGatewayManager : IPaymentGateway
    {
        public bool VerifyCardDetails(string cardNo)
        {
            WriteLine($"Card# {cardNo} has been verified and is accepted.");
            return true;
        }

        public bool ProcessPayment(string cardNo, float cost)
        {
            WriteLine($"Card# {cardNo} is used to make a payment of {cost}.");
            return true;
        }
    }

    public class LogisticsManager : ILogistics
    {
        public void ShipProduct(string productName, string shippingAddress)
        {
            WriteLine($"Congratulations your product {productName} has been shipped at the following address: {shippingAddress}");
        }
    }

    public class OrderDetails
    {
        public int ProductNo { get; private set; }

        public string ProductName { get; private set; }
        public string ProductDescription { get; private set; }
        public float Price { get; set; }
        public float DiscountPercent { get; private set; }
        public string AddressLine1 { get; private set; }
        public string AddressLine2 { get; private set; }
        public int PinCode { get; private set; }
        public string CardNo { get; private set; }

        public OrderDetails(string productName, string prodDescription, float price,
                            float discount, string addressLine1, string addressLine2,
                            int pinCode, string cardNo)
        {
            this.ProductNo = new Random(1).Next(1, 100);
            this.ProductName = productName;
            this.ProductDescription = prodDescription;
            this.Price = price;
            this.DiscountPercent = discount;
            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.PinCode = pinCode;
            this.CardNo = cardNo;
        }
    }

    /// <summary>
    /// Use of Façade design pattern which can expose several components through a single interface
    /// </summary>
    public class OnlineShoppingFacade
    {
        IInventory inventory = new InventoryManager();
        IOrderVerify orderVerify = new OrderVerificationManager();
        ICosting costManger = new CostManager();
        IPaymentGateway paymentGateWay = new PaymentGatewayManager();
        ILogistics logistics = new LogisticsManager();

        public void FinalizeOrder(OrderDetails orderDetails)
        {
            inventory.Update(orderDetails.ProductNo);
            orderVerify.VerifyShippingAddress(orderDetails.PinCode);
            orderDetails.Price = costManger.ApplyDiscounts(orderDetails.Price,
                                                           orderDetails.DiscountPercent);
            paymentGateWay.VerifyCardDetails(orderDetails.CardNo);
            paymentGateWay.ProcessPayment(orderDetails.CardNo, orderDetails.Price);
            logistics.ShipProduct(orderDetails.ProductName, string.Format("{0}, {1} - {2}.",
                                  orderDetails.AddressLine1, orderDetails.AddressLine2,
                                  orderDetails.PinCode));
        }
    }

    public class Facade
    {
        public static void CreateFacade()
        {
            // Creating the Order/Product details
            OrderDetails orderDetails = new OrderDetails("C# Design Pattern Book",
                                                         "Simplified book on design patterns in C#",
                                                         500,
                                                         10,
                                                         "Street No 1",
                                                         "Educational Area",
                                                         1212,
                                                         "4156213754"
                                                         );

            // Using Facade
            OnlineShoppingFacade facade = new OnlineShoppingFacade();
            facade.FinalizeOrder(orderDetails);

            //ReadLine();
        }
    }
}
