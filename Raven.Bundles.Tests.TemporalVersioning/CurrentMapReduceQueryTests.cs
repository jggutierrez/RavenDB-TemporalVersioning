﻿using System;
using System.Linq;
using Raven.Bundles.Tests.TemporalVersioning.Entities;
using Raven.Bundles.Tests.TemporalVersioning.Indexes;
using Raven.Client.Bundles.TemporalVersioning;
using Raven.Tests.Helpers;
using Xunit;

namespace Raven.Bundles.Tests.TemporalVersioning
{
    public class CurrentMapReduceQueryTests : RavenTestBase
    {
        [Fact]
        public void TemporalVersioning_CurrentMapReduceQuery()
        {
            using (var documentStore = this.GetTemporalDocumentStore())
            {
                documentStore.ExecuteIndex(new Employees_CurrentCount());

                var effectiveDate1 = new DateTimeOffset(new DateTime(2012, 1, 1));
                var effectiveDate2 = new DateTimeOffset(new DateTime(2012, 2, 1));
                var effectiveDate3 = new DateTimeOffset(new DateTime(2012, 3, 1));
                var effectiveDate4 = new DateTimeOffset(new DateTime(2012, 4, 1));

                // Store some documents
                using (var session = documentStore.OpenSession())
                {
                    session.Effective(effectiveDate1).Store(new Employee { Id = "employees/1", Name = "John", PayRate = 10 });
                    session.Effective(effectiveDate1).Store(new Employee { Id = "employees/2", Name = "Mary", PayRate = 20 });
                    session.Effective(effectiveDate2).Store(new Employee { Id = "employees/3", Name = "Sam", PayRate = 30 });

                    session.SaveChanges();
                }

                // Make some changes
                using (var session = documentStore.OpenSession())
                {
                    var employee1 = session.Effective(effectiveDate2).Load<Employee>("employees/1");
                    employee1.PayRate = 40;

                    var employee2 = session.Effective(effectiveDate3).Load<Employee>("employees/2");
                    employee2.PayRate = 50;

                    var employee3 = session.Effective(effectiveDate3).Load<Employee>("employees/3");
                    employee3.PayRate = 60;

                    session.SaveChanges();
                }

                // Delete a document
                using (var session = documentStore.OpenSession())
                {
                    var employee2 = session.Effective(effectiveDate4).Load<Employee>("employees/2");
                    session.Delete(employee2);

                    session.SaveChanges();
                }

                // Query and check the results
                using (var session = documentStore.OpenSession())
                {
                    var result = session.Query<Employees_CurrentCount.Result, Employees_CurrentCount>()
                                        .Customize(x => x.WaitForNonStaleResults())
                                        .First();

                    Assert.Equal(2, result.Count);
                }
            }
        }
    }
}
