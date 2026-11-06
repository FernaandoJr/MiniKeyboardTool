using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace HIDTester
{
	/// <summary>
	/// Checks for newer releases on GitHub and lets the user run the setup
	/// directly from the app. Fully fire-and-forget: every failure path is
	/// swallowed (offline, rate limited, parse hiccup) and results in
	/// "nothing to do" — the check must never disturb normal usage,
	/// and no telemetry of any kind is collected.
	/// </summary>
	internal class Updater
	{
		private const string GitHubApiUrl = "https://api.github.com/repos/FernaandoJr/MiniKeyboardTool/releases/latest";

		public string LatestTag;

		public string LatestVersionText;

		public string SetupDownloadUrl;

		private static string CurrentVersion()
		{
			return Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		/// <summary>
		/// Runs the check on a background thread. onNewVersion runs on that
		/// thread too — the caller must marshal back to the UI thread.
		/// </summary>
		public void BeginCheck(Action<Updater> onNewVersion)
		{
			Thread thread = new Thread(new ThreadStart(delegate()
			{
				Updater info = TryFetch();
				if (info != null && onNewVersion != null)
				{
					onNewVersion(info);
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		private Updater TryFetch()
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GitHubApiUrl);
				request.UserAgent = "MiniKeyboardTool/" + CurrentVersion();
				request.Timeout = 15000;
				request.ReadWriteTimeout = 15000;
				string json;
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					using (StreamReader reader = new StreamReader(response.GetResponseStream()))
					{
						json = reader.ReadToEnd();
					}
				}
				Match tagMatch = Regex.Match(json, "\"tag_name\"\\s*:\\s*\"v?([0-9]+\\.[0-9]+(?:\\.[0-9]+)*)\"");
				if (!tagMatch.Success)
				{
					return null;
				}
				string latest = tagMatch.Groups[1].Value;
				Version current;
				Version newer;
				if (!Version.TryParse(latest, out newer))
				{
					return null;
				}
				if (!Version.TryParse(CurrentVersion(), out current))
				{
					return null;
				}
				if (newer <= current)
				{
					return null;
				}
				Updater info = new Updater();
				info.LatestTag = "v" + latest;
				info.LatestVersionText = latest;
				Match assetMatch = Regex.Match(json, "\"browser_download_url\"\\s*:\\s*\"(https:\\/\\/[^/\"]+\\/[^\"]+\\/releases\\/download\\/[^\"]+" + Regex.Escape(info.LatestTag) + "[^\"]*setup\\.exe)\"");
				if (assetMatch.Success)
				{
					info.SetupDownloadUrl = assetMatch.Groups[1].Value.Replace("\\/", "/");
				}
				return info;
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Downloads the setup for the latest release to a temp file and runs
		/// it. The installer then takes over — including closing this running
		/// instance via the AppMutex handshake in [Setup].
		/// </summary>
		public void RunSetup()
		{
			if (string.IsNullOrEmpty(SetupDownloadUrl))
			{
				string releasePage = "https://github.com/FernaandoJr/MiniKeyboardTool/releases/latest";
				Process.Start(releasePage);
				return;
			}
			string targetFile = Path.Combine(Path.GetTempPath(), "MiniKeyboardTool-" + LatestVersionText + "-setup.exe");
			try
			{
				using (WebClient client = new WebClient())
				{
					client.Headers[HttpRequestHeader.UserAgent] = "MiniKeyboardTool/" + CurrentVersion();
					client.DownloadFile(SetupDownloadUrl, targetFile);
				}
				Process.Start(targetFile);
			}
			catch (Exception)
			{
				Process.Start(SetupDownloadUrl);
			}
		}
	}
}
