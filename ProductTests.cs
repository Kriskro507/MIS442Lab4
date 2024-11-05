using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {
        
        MMABooksContext dbContext;
        Product? p;
        List<Product> products;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            products = dbContext.Products.OrderBy(p => p.ProductCode).ToList();
            Assert.AreEqual(16, products.Count);
            Assert.AreEqual("A4CS", products[0].ProductCode);
           
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            p = dbContext.Products.Find("A4CS");
            Assert.IsNotNull(p);
            Assert.AreEqual("A4CS", p.ProductCode);
            Console.WriteLine(p);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the products that have a unit price of 56.50
            var products = dbContext.Products
                .Where(p => p.UnitPrice == 56.50m) 
                .Select(p => new
                {
                    p.ProductCode,
                    p.UnitPrice,
                    p.OnHandQuantity,
                    Value = p.UnitPrice * p.OnHandQuantity
                })
                .OrderBy(p => p.ProductCode)
                .ToList();

            Assert.IsTrue(products.Count > 0); 
            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void GetWithCalculatedFieldTest()
        {
            // get a list of objects that include the productcode, unitprice, quantity and inventoryvalue
            var products = dbContext.Products.Select(
            p => new { p.ProductCode, p.UnitPrice, p.OnHandQuantity, Value = p.UnitPrice * p.OnHandQuantity }).
            OrderBy(p => p.ProductCode).ToList();
            Assert.AreEqual(16, products.Count);
            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test, Order(3)]
        public void DeleteTest()
        {
            var dP = dbContext.Products.FirstOrDefault(Prod => Prod.ProductCode == "abc123");
            Assert.IsNull(dP);
        }

        [Test, Order(1)]
        public void CreateTest()
        {
            p = new Product();
            p.ProductCode = "abc123";
            p.Description = "It's okay";
            p.UnitPrice = 1.5m;
            p.OnHandQuantity = 11;
            dbContext.Products.Add(p);
            dbContext.SaveChanges();
        }

        [Test, Order(2)]
        public void UpdateTest()
        {
            p = dbContext.Products.Find("A4CS");
            Assert.IsNotNull(p);
            if (p != null) 
            {
                p.Description = "Updated Description";
                dbContext.Products.Update(p);
                dbContext.SaveChanges();

                p = dbContext.Products.Find("A4CS");
                Assert.AreEqual("Updated Description", p.Description);
            }
        }
        public void PrintAll(List<Product> products)
        {
            foreach (Product p in products)
            {
                Console.WriteLine(p);
            }
        }

    }
}
