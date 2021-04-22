using Microsoft.EntityFrameworkCore;
using SessionMaster.DAL;

namespace SessionMaster.UnitTests.Domains.Core
{
    public class SessionMasterContextTest
    {
        protected SessionMasterContext _context { get; }

        public SessionMasterContextTest()
        {
            var options = new DbContextOptionsBuilder<SessionMasterContext>()
                .UseInMemoryDatabase(databaseName: "SessionMasteDB")
                .Options;

            _context = new SessionMasterContext(options);
        }

        protected void Seed()
        {
            //setup test data seeds
        }
    }
}
