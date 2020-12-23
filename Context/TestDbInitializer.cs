using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceMoveFiles
{
    public class TestDbInitializer : SqliteDropCreateDatabaseWhenModelChanges<TestContext>
    {
        public TestDbInitializer(DbModelBuilder modelBuilder)
            : base(modelBuilder)
        { }

        protected override void Seed(TestContext context)
        {
            // Here you can seed your core data if you have any.
        }
    }
}
