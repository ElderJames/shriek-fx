using NLog.Common;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confluent.Kafka;
using NLog.Kafka;
using System.Globalization;
using System.Threading;

namespace NLog.Targets
{
	[Target("Kafka")]
	public class Kafka : TargetWithLayout
	{
		private Producer _producer = null;

		public Kafka()
		{
			brokers = new List<KafkaBroker>();
		}

		protected override void Write(LogEventInfo logEvent)
		{
			var obj = new LogstashEvent
			{
				version = 1,
				timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
				app = topic,
				source_host = Environment.MachineName,
				thread_name = Thread.CurrentThread.Name,
				@class = logEvent.CallerClassName,
				method = logEvent.CallerMemberName,
				line_number = logEvent.CallerLineNumber.ToString(),
				level = logEvent.Level.Name,
				logger_name = logEvent.LoggerName,
				message = logEvent.FormattedMessage
			};

			if (logEvent.Exception != null)
			{
				obj.exception = new LogstashException
				{
					exception_class = logEvent.Exception.GetType().ToString(),
					exception_message = logEvent.Exception.Message,
					stacktrace = logEvent.Exception.StackTrace
				};
			}

			var message = obj.ToJson();
			SendMessageToQueue(message);
			base.Write(logEvent);
		}

		public Producer GetProducer()
		{
			if (this.brokers == null || this.brokers.Count == 0) throw new Exception("Broker is not found");

			if (_producer == null)
			{
				var config = new Dictionary<string, object>
				{
					{ "bootstrap.servers", string.Join(",",this.brokers.Select(x => x.address)) },
					{ "queue.buffering.max.messages", 2000000 },
					{ "message.send.max.retries", 3 },
					{ "retry.backoff.ms", 500 }
				};
				_producer = new Producer(config);

#if DEBUG
				_producer.OnError += _producer_OnError;
				_producer.OnLog += _producer_OnLog;
				_producer.OnStatistics += _producer_OnStatistics;
#endif
			}
			return _producer;
		}

		public void CloseProducer()
		{
			if (_producer != null)
			{
				_producer?.Flush(TimeSpan.FromSeconds(60));
				_producer?.Dispose();
			}
			_producer = null;
		}

		private void SendMessageToQueue(string message)
		{
			try
			{
				if (string.IsNullOrEmpty(message))
					return;

				var key = Encoding.UTF8.GetBytes("Multiple." + DateTime.Now.Ticks);
				var msg = Encoding.UTF8.GetBytes(message);
				this.GetProducer().ProduceAsync(topic, key, msg, null);
			}
			catch (Exception ex)
			{
				InternalLogger.Error("Unable to send message to kafka queue", ex);
			}
		}

		private void _producer_OnStatistics(object sender, string e)
		{
			InternalLogger.Warn($"nlog.kafka statistics: {e}");
		}

		private void _producer_OnLog(object sender, LogMessage e)
		{
			InternalLogger.Debug($"nlog.kafka on log: [ Level: {e.Level} Facility:{e.Facility} Name:{e.Name} Message:{e.Message} ]");
		}

		private void _producer_OnError(object sender, Error e)
		{
			InternalLogger.Error($"nlog.kafka error: [ Code:{e.Code} HasError:{e.HasError} IsBrokerError:{e.IsBrokerError} IsLocalError:{e.IsLocalError} Reason:{e.Reason} ]");
		}

		protected override void CloseTarget()
		{
			CloseProducer();
			base.CloseTarget();
		}

		[RequiredParameter]
		public string App { get; set; }

		[RequiredParameter]
		public string topic { get; set; }

		[RequiredParameter]
		[ArrayParameter(typeof(KafkaBroker), "broker")]
		public IList<KafkaBroker> brokers { get; set; }
	}
}