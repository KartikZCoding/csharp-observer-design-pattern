/*
    1. what changes? :- Stock Price, (Publisher)
    2. who cares? :- Trader, Mobile App, Alert System, Logger, Dashboard, (Subscribers)
    3. what info do they need? :- Stock Name, Current Price, Time, (Data)
 */

namespace StockMarketObserverSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var market = new StockMarket();
            var tradDisplay = new TradeDisplay();

            market.Subscribe(tradDisplay.TraderDisplay);

            market.UpdatePrice("Hello", 2000);
        }
    }

    public class TradeDisplay
    {
        public void TraderDisplay(StockPrice stock)
        {
            Console.WriteLine($"Trader sees {stock.Name}: {stock.Price}");
        }
    }

    public class StockMarket
    {
        List<Action<StockPrice>> observers = new();

        public void Subscribe(Action<StockPrice> observer)
        {
            observers.Add(observer);
        }

        public void UpdatePrice(string name, decimal price)
        {
            var data = new StockPrice
            {
                Name = name,
                Price = price,
                Time = DateTime.Now,
            };

            foreach (var observer in observers)
            {
                observer(data);
            }
        }
    }

    public class StockPrice
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime Time { get; set; }
    }
}
