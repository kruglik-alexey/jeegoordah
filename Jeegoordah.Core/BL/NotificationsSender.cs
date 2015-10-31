using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Jeegoordah.Core.DL;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Core.Logging;
using SendGrid;

namespace Jeegoordah.Core.BL
{
	public static class NotificationsSender
	{
		private static readonly Logger _logger = Logger.For(typeof(NotificationsSender));
		private static readonly MailAddress _from = new MailAddress("noreply@jeegoodrah.azurewebsites.net", "Jeegoordah");
		private static readonly Web _transport = new Web("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");

		public static async Task Send(Db db)
		{
			var host = HttpContext.Current?.Request.ServerVariables["HTTP_HOST"];
            if (host == null || !host.Equals("http://jeegoordah.azurewebsites.net/", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new Exception($"Sending notifications is disabled for {host}");
			}

			var sendTasks = db.Query<Bro>()
				.Where(x => x.Email != null)
				.Where(x => x.Notifications.Count > 0)
				.ToArray()
				.Select(async bro =>
				{
					var message = new SendGridMessage
					{
						From = _from,
						To = new[] {new MailAddress(bro.Email)},
						Subject = "Jeegoordah notifications",
						Text = GetContent(bro.Notifications),
						Html = null,
					};

					_logger.I($"Sending {bro.Notifications.Count} notifications for {bro.Name}");
					await _transport.DeliverAsync(message);
					_logger.I($"Notifications for {bro.Name} sent");
				});

			await Task.WhenAll(sendTasks);

			db.Query<Notification>().ForEach(db.Session.Delete);
			_logger.I("Notifications deleted");
		}

		private static string GetContent(IEnumerable<Notification> notifications)
		{
			return notifications
				.OrderBy(x => x.Date)
				.Aggregate(new StringBuilder(), (s, notification) => s.AppendLine(notification.Content).AppendLine().AppendLine("----------").AppendLine())
				.ToString();
		}
	}
}
