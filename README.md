# 🔔 Observer Design Pattern - C# Examples

This repository contains **three projects** demonstrating the **Observer Design Pattern** in C#. Each project shows a different way to implement this pattern, from simple to advanced.

---

## 📚 What is Observer Pattern?

The Observer Pattern is like a **YouTube subscription**:
- **Subject (Publisher)** → The YouTube Channel
- **Observers (Subscribers)** → People who subscribed
- **Notification** → "New video uploaded!"

When the Subject's state changes, all Observers are automatically notified.

```
┌──────────────┐         Notifies         ┌───────────┐
│              │  ─────────────────────▶  │ Observer1 │
│   SUBJECT    │                          └───────────┘
│  (Publisher) │         Notifies         ┌───────────┐
│              │  ─────────────────────▶  │ Observer2 │
└──────────────┘                          └───────────┘
```

---

## 📁 Projects Overview

| # | Project | Difficulty | Approach |
|---|---------|-----------|----------|
| 1 | [SimpleSubscribeExample](https://github.com/KartikZCoding/csharp-observer-design-pattern/blob/3a85a6b74baeda236d7cb73d4f1a93f2d3764685/SimpleSubscribeExample/Program.cs) | ⭐ Easy | `IObserver<T>` / `IObservable<T>` |
| 2 | [StockMarketObserverSystem](https://github.com/KartikZCoding/csharp-observer-design-pattern/blob/3a85a6b74baeda236d7cb73d4f1a93f2d3764685/StockMarketObserverSystem/Program.cs) | ⭐ Easy | `Action<T>` delegates |
| 3 | [BuildingSurveillanceSystemApplication](https://github.com/KartikZCoding/csharp-observer-design-pattern/blob/3a85a6b74baeda236d7cb73d4f1a93f2d3764685/BuildingSurveillanceSystemApplication/Program.cs) | ⭐⭐⭐ Advanced | Full `IObserver<T>` / `IObservable<T>` |

---

## 1. SimpleSubscribeExample

**📌 Purpose:** Learn Subscribe/Unsubscribe basics using YouTube-like example

### Concept
A YouTube channel notifies subscribers when a new video is uploaded. When someone unsubscribes, they stop getting notifications.

### Key Classes
| Class | Role | Interface |
|-------|------|-----------|
| `YouTubeChannel` | Publisher/Subject | `IObservable<string>` |
| `Subscriber` | Observer | `IObserver<string>` |
| `Unsubscriber` | Removes observer | `IDisposable` |

### How It Works
```csharp
// Create channel and subscribers
YouTubeChannel channel = new YouTubeChannel("Kartik's Channel");
Subscriber rahul = new Subscriber("Rahul");

// Subscribe
IDisposable subscription = channel.Subscribe(rahul);

// Upload video → All subscribers notified via OnNext()
channel.UploadVideo("C# Tutorial");

// Unsubscribe
subscription.Dispose();  // Rahul stops getting notifications
```

### Run
```bash
cd SimpleSubscribeExample
dotnet run
```

---

## 2. StockMarketObserverSystem

**📌 Purpose:** Simple Observer using `Action<T>` delegates (shorter code)

### Concept
Stock market price changes notify multiple displays (Trader Display, Logger Display).

### Key Classes
| Class | Role |
|-------|------|
| `StockMarket` | Publisher - tracks stock prices |
| `TradeDisplay` | Observer - shows price to traders |
| `LogDisplay` | Observer - logs price changes |
| `StockPrice` | Data - contains Name, Price, Time |

### How It Works
```csharp
var market = new StockMarket();
var traderDisplay = new TradeDisplay();
var logDisplay = new LogDisplay();

// Subscribe using Action delegate
market.Subscribe(traderDisplay.TraderDisplay);
market.Subscribe(logDisplay.LoggerDisplay);

// Update price → All observers notified
market.UpdatePrice("AAPL", 2000);
```

### Run
```bash
cd StockMarketObserverSystem
dotnet run
```

---

## 3. BuildingSurveillanceSystemApplication

**📌 Purpose:** Complete real-world example with full `IObserver<T>` / `IObservable<T>` implementation

### Concept
A building security system tracks visitors. When a visitor enters/exits:
- **Employee** gets notified if it's THEIR visitor
- **Security** gets notified about ALL visitors

### Key Classes
| Class | Role | Interface |
|-------|------|-----------|
| `SecuritySurveillanceHub` | Publisher - tracks all visitors | `IObservable<ExternalVisitor>` |
| `EmployeeNotify` | Observer - notifies specific employee | `IObserver<ExternalVisitor>` |
| `SecurityNotify` | Observer - notifies security team | `IObserver<ExternalVisitor>` |
| `ExternalVisitor` | Data - visitor information | - |
| `UnSubscriber<T>` | Removes observer from list | `IDisposable` |

### How It Works
```csharp
// Create the hub (Subject)
SecuritySurveillanceHub hub = new SecuritySurveillanceHub();

// Create observers
EmployeeNotify bobNotify = new EmployeeNotify(new Employee { Id = 1, FirstName = "Bob" });
SecurityNotify securityNotify = new SecurityNotify();

// Subscribe
hub.Subscribe(bobNotify);
hub.Subscribe(securityNotify);

// Visitor enters → OnNext() called on all observers
hub.ConfirmExternalVisitorEnterBuilding(1, "Kartik", "Ahir", "Company", "Developer", DateTime.Now, 1);

// Visitor exits → OnNext() called again
hub.ConfirmExternalVisitorExitBuilding(1, DateTime.Now);

// End of day → OnCompleted() generates reports
hub.BuildingEntryCutOffTimeReached();
```

### Flow Diagram
```
Visitor Enters                    Visitor Exits                   End of Day
     │                                 │                              │
     ▼                                 ▼                              ▼
hub.ConfirmEnter()              hub.ConfirmExit()              hub.CutOffTime()
     │                                 │                              │
     ▼                                 ▼                              ▼
observer.OnNext(visitor)        observer.OnNext(visitor)       observer.OnCompleted()
     │                                 │                              │
     ▼                                 ▼                              ▼
"Visitor arrived!"              "Visitor left!"                "Daily Report"
```

### Run
```bash
cd BuildingSurveillanceSystemApplication
dotnet run
```

---

## 🎯 When to Use Observer Pattern?

| ✅ USE When | ❌ DON'T USE When |
|------------|------------------|
| One object change affects many others | Only one observer exists |
| You don't know how many observers exist | Updates are rare |
| Objects need loose coupling | Synchronous guaranteed delivery needed |

---

## 🔑 Key Interfaces

### IObservable<T> (Publisher)
```csharp
public interface IObservable<T>
{
    IDisposable Subscribe(IObserver<T> observer);
}
```

### IObserver<T> (Subscriber)
```csharp
public interface IObserver<T>
{
    void OnNext(T value);       // New data received
    void OnCompleted();         // Stream ended
    void OnError(Exception e);  // Error occurred
}
```

---

## 🛠️ How to Run

```bash
# Clone repository
git clone https://github.com/KartikZCoding/csharp-observer-design-pattern.git
cd ObserverDesignPattern

# Run any project
dotnet run --project SimpleSubscribeExample
dotnet run --project StockMarketObserverSystem
dotnet run --project BuildingSurveillanceSystemApplication
```

---

## 📖 Learning Path

1. **Start with SimpleSubscribeExample** → Understand basic Subscribe/Unsubscribe
2. **Then StockMarketObserverSystem** → See simpler delegate approach
3. **Finally BuildingSurveillanceSystemApplication** → Master complete implementation

---

## 👨‍💻 Author

Kartik Ahir

---

## 📄 License

This project is for learning purposes.
