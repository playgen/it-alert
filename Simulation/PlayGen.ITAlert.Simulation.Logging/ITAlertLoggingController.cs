using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Logging.Database;
using Event = Engine.Logging.Database.Model.Event;

namespace PlayGen.ITAlert.Simulation.Logging
{
    // ReSharper disable once InconsistentNaming
    public class ITAlertLoggingController : IDisposable
    {
        private readonly ITAlertLoggingContext _context = ITAlertLoggingContextFactory.Create();

        ~ITAlertLoggingController()
        {
            Dispose();
        }

        public List<Event> GetGameEvents(Guid gameId)
        {
            return _context.GetEventsByGame(gameId).ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
