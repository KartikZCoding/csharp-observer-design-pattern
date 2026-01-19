using Microsoft.VisualBasic;
using System.Text;

namespace BuildingSurveillanceSystemApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            SecuritySurveillanceHub securitySurveillanceHub = new SecuritySurveillanceHub();

            EmployeeNotify employeeNotify = new EmployeeNotify(new Employee
            { Id = 1, FirstName = "Bob", LastName = "Jones", JobTitle = "Development Manager" });
            EmployeeNotify employeeNotify2 = new EmployeeNotify(new Employee
            { Id = 2, FirstName = "Dave", LastName = "Kendal", JobTitle = "Chief Information Officer" });

            SecurityNotify securityNotify = new SecurityNotify();

            // securitySurveillanceHub.Subscribe(employeeNotify);
            employeeNotify.Subscribe(securitySurveillanceHub);
            employeeNotify2.Subscribe(securitySurveillanceHub);
            securityNotify.Subscribe(securitySurveillanceHub);

            securitySurveillanceHub.ConfirmExternalVisitorEnterBuilding(1, "Kartik", "Ahir", "Tridhyatech", "DotNet Developer", DateTime.Parse("15 Jan 2026 10:00"), 1);
            securitySurveillanceHub.ConfirmExternalVisitorEnterBuilding(2, "Aryan", "Nai", "Tridhyatech", "MERN Stack Developer", DateTime.Parse("15 Jan 2026 11:00"), 2);

            employeeNotify.UnSubscribe();

            securitySurveillanceHub.ConfirmExternalVisitorExitBuilding(1, DateTime.Parse("15 Jan 2026 12:00"));
            securitySurveillanceHub.ConfirmExternalVisitorExitBuilding(2, DateTime.Parse("15 Jan 2026 13:00"));

            securitySurveillanceHub.BuildingEntryCutOffTimeReached();

            Console.ReadKey();

        }
    }
    public static class OutputFormatter
    {
        public enum TextOutputTheme
        {
            Security,
            Employee,
            Normal
        }

        public static void ChangeOutputTheme(TextOutputTheme textOutputTheme)
        {
            if (textOutputTheme == TextOutputTheme.Employee)
            {
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (textOutputTheme == TextOutputTheme.Security)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ResetColor();
            }
        }
    }
    public class EmployeeNotify : Observer
    {
        IEmployee _employee;
        public EmployeeNotify(IEmployee employee)
        {
            _employee = employee;
        }

        public override void OnNext(ExternalVisitor value)
        {
            var externalVisitor = value;

            if (externalVisitor.EmployeeContactId == _employee.Id)
            {
                var externalVisitorListItem = _externalVisitors.FirstOrDefault(e => e.Id == externalVisitor.Id);

                if (externalVisitorListItem == null)
                {
                    _externalVisitors.Add(externalVisitor);
                    OutputFormatter.ChangeOutputTheme(OutputFormatter.TextOutputTheme.Employee);
                    Console.WriteLine($"{_employee.FirstName} {_employee.LastName}, your visitor has arrived. Visitor Id({externalVisitor.Id}), FirstName({externalVisitor.FirstName}), LastName({externalVisitor.LastName}), entered the building,  DateTime({externalVisitor.EntryDateTime.ToString("dd MM yyyy hh:mm:ss tt")})");
                    OutputFormatter.ChangeOutputTheme(OutputFormatter.TextOutputTheme.Normal);
                    Console.WriteLine();
                }
                else
                {
                    if (externalVisitor.InBuilding == false)
                    {
                        externalVisitorListItem.InBuilding = false;
                        externalVisitorListItem.ExitDateTime = externalVisitor.ExitDateTime;
                    }
                }
            }
        }
        public override void OnError(Exception error)
        {

        }
        public override void OnCompleted()
        {
            string heading = $"{_employee.FirstName} {_employee.LastName} Daily Visitor's Report";
            Console.WriteLine();
            Console.WriteLine(heading);
            Console.WriteLine(new string('-', heading.Length));
            Console.WriteLine();

            foreach (var externaVisitor in _externalVisitors)
            {
                externaVisitor.InBuilding = false;
                Console.WriteLine($"{externaVisitor.Id,-6}{externaVisitor.FirstName,-15}{externaVisitor.LastName,-15}{externaVisitor.EntryDateTime.ToString("dd MM yyyy hh:mm:ss"),-25}{externaVisitor.ExitDateTime.ToString("dd MM yyyy hh:mm:ss tt"),-25}");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

    }
    public class SecurityNotify : Observer
    {
        public override void OnNext(ExternalVisitor value)
        {
            var externalVisitor = value;
            var externalVisitorListItem = _externalVisitors.FirstOrDefault(e => e.Id == externalVisitor.Id);

            if (externalVisitorListItem == null)
            {
                _externalVisitors.Add(externalVisitor);
                OutputFormatter.ChangeOutputTheme(OutputFormatter.TextOutputTheme.Security);
                Console.WriteLine($"Security notification: Visitor Id({externalVisitor.Id}), FirstName({externalVisitor.FirstName}), LastName({externalVisitor.LastName}), entered the building,  DateTime({externalVisitor.EntryDateTime.ToString("dd MM yyyy hh:mm:ss tt")})");
                OutputFormatter.ChangeOutputTheme(OutputFormatter.TextOutputTheme.Normal);
                Console.WriteLine();
            }
            else
            {
                if (externalVisitorListItem.InBuilding == false)
                {
                    externalVisitorListItem.InBuilding = false;
                    externalVisitorListItem.ExitDateTime = externalVisitor.ExitDateTime;

                    Console.WriteLine($"Security notification: Visitor Id({externalVisitor.Id}), FirstName({externalVisitor.FirstName}), LastName({externalVisitor.LastName}), exited the building,  DateTime({externalVisitor.ExitDateTime.ToString("dd MM yyyy hh:mm:ss tt")})");
                    Console.WriteLine();
                }
            }
        }
        public override void OnError(Exception error)
        {

        }
        public override void OnCompleted()
        {
            string heading = $"Security Daily Visitor's Report";
            Console.WriteLine();
            Console.WriteLine(heading);
            Console.WriteLine(new string('-', heading.Length));
            Console.WriteLine();

            foreach (var externaVisitor in _externalVisitors)
            {
                externaVisitor.InBuilding = false;
                Console.WriteLine($"{externaVisitor.Id,-6}{externaVisitor.FirstName,-15}{externaVisitor.LastName,-15}{externaVisitor.EntryDateTime.ToString("dd MM yyyy hh:mm:ss"),-25}{externaVisitor.ExitDateTime.ToString("dd MM yyyy hh:mm:ss tt"),-25}");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

    }
    public abstract class Observer : IObserver<ExternalVisitor>
    {
        IDisposable _cancellation;
        protected List<ExternalVisitor> _externalVisitors = new List<ExternalVisitor>();

        // react to the notifications
        public abstract void OnNext(ExternalVisitor value);

        // handle error
        public abstract void OnError(Exception error);

        // stream ended
        public abstract void OnCompleted();

        public void Subscribe(IObservable<ExternalVisitor> provider)
        {
            _cancellation = provider.Subscribe(this);
        }

        public void UnSubscribe()
        {
            _cancellation.Dispose();
            _externalVisitors.Clear();
        }
    }
    public class Employee : IEmployee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
    }
    public interface IEmployee
    {
        int Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string JobTitle { get; set; }
    }
    public class UnSubscriber<ExternalVisitor> : IDisposable
    {
        private List<IObserver<ExternalVisitor>> _observers;
        private IObserver<ExternalVisitor> _observer;
        public UnSubscriber(List<IObserver<ExternalVisitor>> observers, IObserver<ExternalVisitor> observer)
        {
            _observer = observer;
            _observers = observers;
        }
        public void Dispose()
        {
            if (_observers.Contains(_observer))
            {
                _observers.Remove(_observer);
            }
        }
    }
    public class SecuritySurveillanceHub : IObservable<ExternalVisitor>
    {
        private List<ExternalVisitor> _externalVisitors;

        // list of (_observers)
        private List<IObserver<ExternalVisitor>> _observers;

        public SecuritySurveillanceHub()
        {
            _externalVisitors = new List<ExternalVisitor>();
            _observers = new List<IObserver<ExternalVisitor>>();
        }

        // add observer to list(_observers)
        public IDisposable Subscribe(IObserver<ExternalVisitor> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new UnSubscriber<ExternalVisitor>(_observers, observer);
        }

        public void ConfirmExternalVisitorEnterBuilding(int id, string FirstName, string LastName, string companyName, string jobTitle, DateTime entryDateTime, int employeeContactId)
        {
            ExternalVisitor externalVisitor = new ExternalVisitor
            {
                Id = id,
                FirstName = FirstName,
                LastName = LastName,
                CompanyName = companyName,
                JobTitle = jobTitle,
                EntryDateTime = entryDateTime,
                InBuilding = true,
                EmployeeContactId = employeeContactId
            };

            _externalVisitors.Add(externalVisitor);

            // notify all (_observers)
            foreach (var observer in _observers)
            {
                observer.OnNext(externalVisitor);
            }

        }

        public void ConfirmExternalVisitorExitBuilding(int externalVisitorId, DateTime exitDateTime)
        {
            var externalVisitor = _externalVisitors.FirstOrDefault(e => e.Id == externalVisitorId);
            if (externalVisitor != null)
            {
                externalVisitor.ExitDateTime = exitDateTime;
                externalVisitor.InBuilding = false;

                // notify all (_observers)
                foreach (var observer in _observers)
                {
                    observer.OnNext(externalVisitor);
                }
            }
        }

        public void BuildingEntryCutOffTimeReached()
        {
            if (_externalVisitors.Where(e => e.InBuilding == true).ToList().Count() == 0)
            {
                // notify all (_observers)
                foreach (var observer in _observers)
                {
                    observer.OnCompleted();
                }
            }
        }
    }
    public class ExternalVisitor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public DateTime EntryDateTime { get; set; }
        public DateTime ExitDateTime { get; set; }
        public bool InBuilding { get; set; }
        public int EmployeeContactId { get; set; }

    }

}
