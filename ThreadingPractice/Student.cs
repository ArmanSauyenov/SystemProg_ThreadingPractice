using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingPractice
{
    public class DatabaseConnection
    {
        private static Semaphore semaphore = new Semaphore(5, 5);

        private static List<Student> students = new List<Student>()
        {
            new Student() {Name = "1", Password = "1"},
            new Student() {Name = "2", Password = "2"},
            new Student() {Name = "3", Password = "3"}
        };

        private bool IsAuthentificated(string name, string password)
        {
            return (students
                .FirstOrDefault(
                    p => p.Name == name && 
                    p.Password == password) != null);
        }
        public void ConnectToDb(string name, string password)
        {
            if(IsAuthentificated(name, password))
            {         
                bool result = semaphore.WaitOne();
                if(result == false)
                {
                    Console.WriteLine("Please wait!");
                }
                Console.WriteLine($"Welcome {name}");
                Console.WriteLine($"{name} is reading...");
                Thread.Sleep(20000);
                Console.WriteLine($"{name} is out");
                semaphore.Release();
            }
            else
            {
                return;
            }
        }
    }
    public class Student
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
