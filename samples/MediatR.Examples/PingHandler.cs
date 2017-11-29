using System.IO;
using System.Threading;

namespace MediatR.Examples
{
    using System.Threading.Tasks;

    public class PingHandler : IRequestHandler<Ping, Pong>
    {
        private readonly TextWriter _writer;

        public PingHandler(TextWriter writer)
        {
            _writer = writer;
        }

        public async Task<Pong> Handle(Ping message, CancellationToken token)
        {
            await _writer.WriteLineAsync($"--- Handled Ping: {message.Message}");
            return new Pong { Message = message.Message + " Pong" };
        }
    }
}