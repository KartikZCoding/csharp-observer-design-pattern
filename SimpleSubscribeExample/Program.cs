// SIMPLE SUBSCRIBE/UNSUBSCRIBE EXAMPLE USING IObserver & IObservable
// Think of it like: YouTube Channel + Subscribers

using System;
using System.Collections.Generic;

namespace SimpleSubscribeExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== YouTube Channel Example (IObserver/IObservable) ===\n");

            // Step 1: Create YouTube Channel (Observable/Subject)
            YouTubeChannel channel = new YouTubeChannel("Kartik's Coding Channel");

            // Step 2: Create Subscribers (Observers)
            Subscriber rahul = new Subscriber("Rahul");
            Subscriber priya = new Subscriber("Priya");
            Subscriber amit = new Subscriber("Amit");

            // Step 3: SUBSCRIBE - Add subscribers to channel
            Console.WriteLine("--- Subscribing ---");
            IDisposable rahulSubscription = channel.Subscribe(rahul);  // Returns IDisposable for unsubscribe
            IDisposable priyaSubscription = channel.Subscribe(priya);
            IDisposable amitSubscription = channel.Subscribe(amit);

            // Step 4: Upload video - ALL subscribers get notified (OnNext)
            Console.WriteLine("\n--- Uploading First Video ---");
            channel.UploadVideo("C# Tutorial Part 1");

            // Step 5: UNSUBSCRIBE - Rahul leaves using Dispose()
            Console.WriteLine("\n--- Rahul Unsubscribes ---");
            rahulSubscription.Dispose();  // ← This is how you unsubscribe!

            // Step 6: Upload another video - Only Priya and Amit get notified
            Console.WriteLine("\n--- Uploading Second Video ---");
            channel.UploadVideo("C# Tutorial Part 2");

            // Step 7: Channel ends (OnCompleted)
            Console.WriteLine("\n--- Channel Ends (OnCompleted) ---");
            channel.EndChannel();

            Console.WriteLine("\n=== Notice: Rahul didn't get the second video notification! ===");
            Console.ReadKey();
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // OBSERVABLE (Subject/Publisher) - The YouTube Channel
    // Implements: IObservable<T>
    // ═══════════════════════════════════════════════════════════════════════════
    public class YouTubeChannel : IObservable<string>
    {
        private string _channelName;
        private List<IObserver<string>> _subscribers = new List<IObserver<string>>();

        public YouTubeChannel(string name)
        {
            _channelName = name;
        }

        // SUBSCRIBE: Required by IObservable<T>
        // Returns IDisposable so subscriber can unsubscribe later
        public IDisposable Subscribe(IObserver<string> observer)
        {
            if (!_subscribers.Contains(observer))
            {
                _subscribers.Add(observer);
                Console.WriteLine($"✅ Subscribed to {_channelName}");
            }

            // Return an Unsubscriber object
            return new Unsubscriber(_subscribers, observer);
        }

        // Upload video = Notify all subscribers using OnNext()
        public void UploadVideo(string videoTitle)
        {
            Console.WriteLine($"\n📹 New Video Uploaded: \"{videoTitle}\"\n");

            foreach (var subscriber in _subscribers)
            {
                subscriber.OnNext(videoTitle);  // ← Calls OnNext on each observer
            }
        }

        // Channel ends = Notify all using OnCompleted()
        public void EndChannel()
        {
            foreach (var subscriber in _subscribers.ToList())
            {
                subscriber.OnCompleted();  // ← Calls OnCompleted on each observer
            }
            _subscribers.Clear();
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // UNSUBSCRIBER - Helper class to remove observer when Dispose() is called
    // Implements: IDisposable
    // ═══════════════════════════════════════════════════════════════════════════
    public class Unsubscriber : IDisposable
    {
        private List<IObserver<string>> _subscribers;
        private IObserver<string> _observer;

        public Unsubscriber(List<IObserver<string>> subscribers, IObserver<string> observer)
        {
            _subscribers = subscribers;
            _observer = observer;
        }

        // When Dispose() is called, remove the observer from the list
        public void Dispose()
        {
            if (_subscribers.Contains(_observer))
            {
                _subscribers.Remove(_observer);
                Console.WriteLine("❌ Unsubscribed successfully!");
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // OBSERVER (Subscriber) - The People Watching
    // Implements: IObserver<T>
    // ═══════════════════════════════════════════════════════════════════════════
    public class Subscriber : IObserver<string>
    {
        public string Name { get; set; }

        public Subscriber(string name)
        {
            Name = name;
        }

        // Called when new video is uploaded
        public void OnNext(string videoTitle)
        {
            Console.WriteLine($"   🔔 {Name} received notification: New video - {videoTitle}");
        }

        // Called when channel ends
        public void OnCompleted()
        {
            Console.WriteLine($"   ✋ {Name}: Channel has ended. No more videos.");
        }

        // Called when there's an error
        public void OnError(Exception error)
        {
            Console.WriteLine($"   ⚠️ {Name}: Error occurred - {error.Message}");
        }
    }
}
