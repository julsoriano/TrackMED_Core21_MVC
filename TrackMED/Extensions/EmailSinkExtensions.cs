using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using System;

namespace CITT.Sinks.Email
{
    public static class EmailSinkExtensions
    {
        public static SinkSubscription<EmailSink> LogToEmail(
          this IObservable<EventEntry> eventStream, string host, int port,
          string recipients, string subject, string credentials,
          IEventTextFormatter formatter = null)
        {
            var sink = new EmailSink(host, port, recipients, subject, credentials, formatter);

            var subscription = eventStream.Subscribe(sink);

            return new SinkSubscription<EmailSink>(subscription, sink);
        }
    }

}