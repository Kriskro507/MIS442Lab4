using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class CustomerTests
    {
        
        MMABooksContext dbContext;
        Customer? c;
        List<Customer>? customers;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            var customers = dbContext.Customers.OrderBy(c => c.Name).ToList();

            Console.WriteLine("Customers:");
            foreach (var customer in customers)
            {
                Console.WriteLine($"Customer ID: {customer.CustomerId}, Name: {customer.Name}, City: {customer.City}, State: {customer.StateCode}");
            }
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            c = dbContext.Customers.Find(1);
            Assert.IsNotNull(c);
            Assert.AreEqual("Molunguri, A", c.Name);
            Console.WriteLine(c);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the customers who live in OR
            var customersInOregon = dbContext.Customers.Where(c => c.StateCode == "OR").OrderBy(c => c.Name).ToList();
             
            Console.WriteLine("Oregon Customers:");
            foreach (var customer in customersInOregon)
            {
                Console.WriteLine($"Customer ID: {customer.CustomerId}, Name: {customer.Name}, City: {customer.City}");
            }
        }

        [Test]
        public void GetWithInvoicesTest()
        {
            // get the customer whose id is 20 and all of the invoices for that customer
            var customer = dbContext.Customers
                                    .Include(c => c.Invoices)
                                    .SingleOrDefault(c => c.CustomerId == 20);

            Assert.IsNotNull(customer);
            Assert.AreEqual(20, customer.CustomerId);
            Assert.IsTrue(customer.Invoices.Any());

            Console.WriteLine($"Customer: {customer.Name}");
            foreach (var invoice in customer.Invoices)
            {
                Console.WriteLine($"Invoice ID: {invoice.InvoiceId}, Amount: {invoice.InvoiceTotal}");
            }
        }

        [Test]
        public void GetWithJoinTest()
        {
            // get a list of objects that include the customer id, name, statecode and statename
            var customers = dbContext.Customers.Join(
               dbContext.States,
               c => c.StateCode,
               s => s.StateCode,
               (c, s) => new { c.CustomerId, c.Name, c.StateCode, s.StateName }).OrderBy(r => r.StateName).ToList();
            Assert.AreEqual(696, customers.Count);
            // I wouldn't normally print here but this lets you see what each object looks like
            foreach (var c in customers)
            {
                Console.WriteLine(c);
            }
        }

        [Test]
        public void DeleteTest()
        {
            var dC = dbContext.Customers.FirstOrDefault(cust => cust.Name == "Mickey House");
            Assert.IsNull(dC);
        }

        [Test]
        public void CreateTest()
        {
            c = new Customer();
            c.Name = "Mickey House";
            c.Address = "12345 place";
            c.City = "Orlando";
            c.StateCode = "FL";
            c.ZipCode = "10001";
            dbContext.Customers.Add(c);
            dbContext.SaveChanges();
        }

        [Test]
        public void UpdateTest()
        {
            var customer = dbContext.Customers.FirstOrDefault(c => c.Name == "Mickey House");

            if (customer == null)
            {
                customer = new Customer
                {
                    Name = "Mickey House",
                    Address = "12345 place",
                    City = "Orlando",
                    StateCode = "FL",
                    ZipCode = "10001"
                };
                dbContext.Customers.Add(customer);
                dbContext.SaveChanges();
            }

            customer.Name = "Mickey Mouse";
            dbContext.SaveChanges();

            var uC = dbContext.Customers.FirstOrDefault(c => c.Name == "Mickey Mouse");
            Assert.IsNotNull(uC);
            Assert.AreEqual("Mickey Mouse", uC.Name);
            Assert.AreEqual("12345 place", uC.Address); 
        }

        public void PrintAll(List<Customer> customers)
        {
            foreach (Customer c in customers)
            {
                Console.WriteLine(c);
            }
        } 
        
    }
}