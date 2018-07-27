using System;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using HtmlAgilityPack;

namespace TKN {
	class MainClass {
		public static void Main(string[] args) {
			someBoolClass sbc = new someBoolClass();
			if (sbc.isConnectedToInternetFast()) {
				string dataFolder = System.AppDomain.CurrentDomain.BaseDirectory + "/TKN_Data";
				WebClient x = new WebClient ();
				string source = x.DownloadString ("http://twokinds.keenspot.com/comic");
				string title = Regex.Match (source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups ["Title"].Value;
				if (!File.Exists (dataFolder + "/lastSketch.txt")) {
					if (!Directory.Exists (dataFolder)) {
						Directory.CreateDirectory (dataFolder);
					}
					File.CreateText (dataFolder + "/lastSketch.txt");
					File.WriteAllText (dataFolder + "/lastSketch.txt", String.Empty);
				} else {
					string sketchLink = "";
					string html;
					using (var client = new WebClient ()) {
						html = client.DownloadString ("http://twokinds.keenspot.com/comic");
					}
					HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument ();
					doc.LoadHtml (html);
					foreach (HtmlNode item in doc.DocumentNode.SelectNodes("//body")) {
						if (item.Attributes ["class"].Value == "preload") {
							foreach (HtmlNode item2 in item.SelectNodes("//a[@href]")) {
								if (item2.Attributes.Count == 2) {
									if (item2.Attributes ["href"].Value.StartsWith ("https://www.patreon.com/posts")) {
										sketchLink = item2.Attributes ["href"].Value;
									}
								}
							}
						}
					}
					if (File.ReadAllText (dataFolder + "/lastSketch.txt") != sketchLink) {
						Console.WriteLine("New sketch found: " + sketchLink);
						DialogResult clickedOk = MessageBox.Show ("A new sketch was found: " + sketchLink, "New TwoKinds Sketch!", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
						File.WriteAllText (dataFolder + "/lastSketch.txt", sketchLink);
						if (clickedOk == DialogResult.OK) {
							Application.Run (new Form2 ());
						}
					}
				}
				if (!File.Exists (dataFolder + "/lastPage.txt")) {
					File.CreateText (dataFolder + "/lastPage.txt");
					File.WriteAllText (dataFolder + "/lastPage.txt", String.Empty);
				} else {
					if (File.ReadAllText (dataFolder + "/lastPage.txt") != title) {
						string pageName = "";
						string html;
						using (var client = new WebClient ()) {
							html = client.DownloadString ("http://twokinds.keenspot.com/comic");
						}
						HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument ();
						doc.LoadHtml (html);
						foreach (HtmlNode item in doc.DocumentNode.SelectNodes("//article")) {
							if (item.Attributes ["class"].Value == "comic") {
								foreach (HtmlNode item2 in item.SelectNodes("//img")) {
									if (item2.Attributes.Count == 3) {
										if (item2.Attributes ["alt"].Value == "Comic Page") {
											pageName = item2.Attributes ["title"].Value;
										}
									}
								}
							}
						}
						Console.WriteLine("New page found: " + pageName);
						DialogResult clickedOk = MessageBox.Show ("A new page was found: " + pageName, "New TwoKinds Page!", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
						File.WriteAllText (dataFolder + "/lastPage.txt", title);
						if (clickedOk == DialogResult.OK) {
							Application.Run (new Form1 ());
						}
					}
				}
			}
			System.Threading.Thread.Sleep(5000);
			string[] testString = new string[0];
			Main(testString);
		}

		class someBoolClass {
		    public bool isConnectedToInternet() {
			    try {
				    using (var client = new WebClient())
				    using (client.OpenRead("http://clients3.google.com/generate_204")) {
					    return true;
				    }
			    } catch {
				    return false;
			    }
		    }

			public bool isConnectedToInternetFast() {
				try {
					Ping newPing = new Ping();
					byte[] buffer = new byte[32];
					PingOptions pO = new PingOptions();
					PingReply reply = newPing.Send("8.8.8.8", 1000, buffer, pO);
					return (reply.Status == IPStatus.Success);
				} catch (Exception) {
					return false;
				}
			}
		}

		class Form1 : Form {
			PictureBox pb1 = new PictureBox();
			public Form1() {
				FormBorderStyle = FormBorderStyle.Sizable;
				WindowState = FormWindowState.Maximized;
				Controls.Add(pb1);
				this.Load += Form1_Load;
				this.KeyDown += new KeyEventHandler(frmLogon_KeyDown);
			}

			void Form1_Load(object sender, EventArgs e) {
				string html;
				using (var client = new WebClient()) {
					html = client.DownloadString("http://twokinds.keenspot.com/comic");
				}
				HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
				doc.LoadHtml(html);
				foreach (HtmlNode item in doc.DocumentNode.SelectNodes("//article")) {
					if (item.Attributes ["class"].Value == "comic") {
						foreach (HtmlNode item2 in item.SelectNodes("//img")) {
							if (item2.Attributes.Count == 3) {
								if (item2.Attributes ["alt"].Value == "Comic Page") {
									pb1.ImageLocation = item2.Attributes ["src"].Value;
								}
							}
						}
					}
				}
				pb1.SizeMode = PictureBoxSizeMode.AutoSize;
				pb1.MouseClick += new MouseEventHandler(pb1_Click);
			}

			void pb1_Click(object sender, EventArgs e) {
				System.Diagnostics.Process.Start("http://twokinds.keenspot.com/comic/");
				this.Close();
			}

			void frmLogon_KeyDown(object sender, KeyEventArgs e) {
				if (e.KeyCode == System.Windows.Forms.Keys.Escape) {
					this.Close();
				}
			}
		}

		class Form2 : Form {
			PictureBox pb1 = new PictureBox();
			public Form2() {
				FormBorderStyle = FormBorderStyle.Sizable;
				WindowState = FormWindowState.Normal;
				Controls.Add(pb1);
				this.Load += Form2_Load;
				this.KeyDown += new KeyEventHandler(frmLogon_KeyDown);
			}

			void Form2_Load(object sender, EventArgs e) {
				string html;
				using (var client = new WebClient()) {
					html = client.DownloadString("http://twokinds.keenspot.com/comic");
				}
				HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
				doc.LoadHtml(html);
				foreach (HtmlNode item in doc.DocumentNode.SelectNodes("//body")) {
					if (item.Attributes ["class"].Value == "preload") {
						foreach (HtmlNode item2 in item.SelectNodes("//img")) {
							if (item2.Attributes.Count == 3) {
								if (item2.Attributes ["src"].Value.EndsWith("dailysketch.png")) {
									pb1.ImageLocation = item2.Attributes ["src"].Value;
								}
							}
						}
					}
				}
				pb1.SizeMode = PictureBoxSizeMode.AutoSize;
				pb1.MouseClick += new MouseEventHandler(pb1_Click);
			}

			void pb1_Click(object sender, EventArgs e) {
				string sketchLink = "";
				string html;
				using (var client = new WebClient()) {
					html = client.DownloadString("http://twokinds.keenspot.com/comic");
				}
				HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
				doc.LoadHtml(html);
				foreach (HtmlNode item in doc.DocumentNode.SelectNodes("//body")) {
					if (item.Attributes ["class"].Value == "preload") {
						foreach (HtmlNode item2 in item.SelectNodes("//a[@href]")) {
							if (item2.Attributes.Count == 2) {
								if (item2.Attributes ["href"].Value.StartsWith("https://www.patreon.com/posts")) {
									sketchLink = item2.Attributes ["href"].Value;
								}
							}
						}
					}
				}
				System.Diagnostics.Process.Start(sketchLink);
				this.Close();
			}

			void frmLogon_KeyDown(object sender, KeyEventArgs e) {
				if (e.KeyCode == System.Windows.Forms.Keys.Escape) {
					this.Close();
				}
			}
		}
	}
}
