using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HIDTester
{
	/// <summary>
	/// LED configuration screen ("LED Lab").
	/// The vendor UI exposed only LED modes 0/1/2; the firmware additionally
	/// accepts a colour in one combined byte (mode | colour), proven live and
	/// confirmed against community protocol knowledge
	/// (https://github.com/rOzzy1987/MacroPad, GPL-3.0, protocol corroboration).
	///
	/// Supported frame (identical to the factory code path):
	///   write   [0xB0, 0x08, (colour|mode)]   (type bits = LED, same |= 8 flag)
	///   commit  [0xAA, 0xA1, 0x00 ...]        (Send_WriteFlashLED_Cmd)
	///   route   WriteMode 1 via HidLib (report id 3), WriteMode 0 via HID.Hid
	///
	/// The original three buttons stay untouched; everything below is added
	/// programmatically so the embedded resources stay byte-identical.
	/// </summary>
	public class LEDkey : UserControl
	{
		private IContainer components;

		private Button KEY_LEDMode0;

		private Button KEY_LEDMode1;

		private Button KEY_LEDMode2;

		private Button KEY_LEDMode3;

		private Button KEY_LEDMode4;

		private Button KEY_LEDMode5;

		private readonly List<Button> colourButtons = new List<Button>();

		private byte[] colourNibbles = new byte[8];

		private Button selectedColourButton;

		private static readonly byte[] ColdourNibbleTable = new byte[8]
		{
			0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70
		};

		private static readonly string[] ColourNames = new string[8]
		{
			"Random", "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple"
		};

		private static readonly Color[] ColourSamples = new Color[8]
		{
			Color.FromArgb(60, 60, 60),
			Color.FromArgb(220, 40, 40),
			Color.FromArgb(240, 130, 30),
			Color.FromArgb(230, 210, 40),
			Color.FromArgb(60, 200, 60),
			Color.FromArgb(40, 200, 200),
			Color.FromArgb(50, 90, 230),
			Color.FromArgb(160, 60, 220)
		};

		private byte selectedMode = 1;

		private int selectedColourIndex;

		private FormMain formOwner;

		private bool initSent;

		public LEDkey(FormMain owner)
		{
			InitializeComponent();
			KEY_Colour_Init();
			formOwner = owner;
			BuildLedLabControls();
			SelectColour(0); // Random as default highlight, no write issued
		}

		private void KEY_Colour_Init()
		{
			int red = 152;
			int green = 251;
			int blue = 152;
			KEY_LEDMode0.BackColor = Color.FromArgb(red, green, blue);
			KEY_LEDMode1.BackColor = Color.FromArgb(red, green, blue);
			KEY_LEDMode2.BackColor = Color.FromArgb(red, green, blue);
		}

		private void LEDGeneral_Char_Set()
		{
			FormMain.KeyParam.Data_Send_Buff[FormMain.KeyParam.KeyType_Num] |= 8;
		}

		private void KEY_LEDMode0_Click(object sender, EventArgs e)
		{
			SelectMode(0);
		}

		private void KEY_LEDMode1_Click(object sender, EventArgs e)
		{
			SelectMode(1);
		}

		private void KEY_LEDMode2_Click(object sender, EventArgs e)
		{
			SelectMode(2);
		}

		/// <summary>
		/// Central apply path for every LED change on this screen (old buttons
		/// included). Sends the validated legacy-protocol frame and the commit.
		/// </summary>
		private void ApplySelected()
		{
			if (formOwner == null)
			{
				return;
			}
			if (!initSent)
			{
				formOwner.SendLedInit();
				initSent = true;
			}
			byte combo = (byte)(ColdourNibbleTable[selectedColourIndex] | selectedMode);
			formOwner.ExecuteLedWrite(combo);
			HighlightSelection();
		}

		private void SelectColour(int index)
		{
			selectedColourIndex = index;
		}

		private void SelectMode(int mode)
		{
			selectedMode = (byte)mode;
			ApplySelected();
		}

		private void HighlightSelection()
		{
			int red = 152;
			int green = 251;
			int blue = 152;
			KEY_LEDMode0.BackColor = Color.FromArgb(red, green, blue);
			KEY_LEDMode1.BackColor = Color.FromArgb(red, green, blue);
			KEY_LEDMode2.BackColor = Color.FromArgb(red, green, blue);
			KEY_LEDMode3.BackColor = Color.FromArgb(red, green, blue);
			KEY_LEDMode4.BackColor = Color.FromArgb(red, green, blue);
			KEY_LEDMode5.BackColor = Color.FromArgb(red, green, blue);
			switch (selectedMode)
			{
			case 0:
				KEY_LEDMode0.BackColor = Color.FromArgb(255, 48, 48);
				break;
			case 1:
				KEY_LEDMode1.BackColor = Color.FromArgb(255, 48, 48);
				break;
			case 2:
				KEY_LEDMode2.BackColor = Color.FromArgb(255, 48, 48);
				break;
			case 3:
				KEY_LEDMode3.BackColor = Color.FromArgb(255, 48, 48);
				break;
			case 4:
				KEY_LEDMode4.BackColor = Color.FromArgb(255, 48, 48);
				break;
			case 5:
				KEY_LEDMode5.BackColor = Color.FromArgb(255, 48, 48);
				break;
			}
			foreach (Button b in colourButtons)
			{
				int idx = (int)b.Tag;
				if (idx == selectedColourIndex)
				{
					b.FlatAppearance.BorderSize = 2;
				}
				else
				{
					b.FlatAppearance.BorderSize = 1;
				}
			}
		}

		private void ColourButton_Click(object sender, EventArgs e)
		{
			Button b = (Button)sender;
			SelectColour((int)b.Tag);
			ApplySelected();
		}

		private void BuildLedLabControls()
		{
			FlowLayoutPanel panel = new FlowLayoutPanel();
			panel.Dock = DockStyle.Bottom;
			panel.Height = 150;
			panel.FlowDirection = FlowDirection.LeftToRight;
			panel.WrapContents = true;
			panel.Padding = new Padding(6);

			Label lbl = new Label();
			lbl.Text = "LED Lab";
			lbl.AutoSize = true;
			lbl.Font = new Font(Font.FontFamily, 9f, FontStyle.Bold);
			lbl.Margin = new Padding(6, 18, 12, 0);
			panel.Controls.Add(lbl);

			for (int i = 0; i < ColourNames.Length; i++)
			{
				Button b = new Button();
				b.Text = ColourNames[i];
				b.Tag = i;
				b.Width = 78;
				b.Height = 34;
				b.FlatStyle = FlatStyle.Flat;
				b.BackColor = ColourSamples[i];
				b.ForeColor = (i == 0) ? SystemColors.ControlText : Color.White;
				b.Margin = new Padding(3, 10, 3, 3);
				b.Click += new EventHandler(ColourButton_Click);
				colourButtons.Add(b);
				panel.Controls.Add(b);
			}

			Label lblMode = new Label();
			lblMode.Text = "Mode";
			lblMode.AutoSize = true;
			lblMode.Font = new Font(Font.FontFamily, 9f, FontStyle.Bold);
			lblMode.Margin = new Padding(18, 18, 10, 0);
			panel.Controls.Add(lblMode);

			for (int mode = 3; mode <= 5; mode++)
			{
				Button b = new Button();
				b.Width = 86;
				b.Height = 34;
				b.Margin = new Padding(3, 10, 3, 3);
				b.FlatStyle = FlatStyle.Flat;
				if (mode == 3)
				{
					KEY_LEDMode3 = b;
					b.Text = "LED_Mode3";
					b.Tag = 3;
				}
				else if (mode == 4)
				{
					KEY_LEDMode4 = b;
					b.Text = "LED_Mode4";
					b.Tag = 4;
				}
				else
				{
					KEY_LEDMode5 = b;
					b.Text = "LED_Mode5";
					b.Tag = 5;
				}
				b.Click += delegate(object s, EventArgs e)
				{
					SelectMode((int)((Button)s).Tag);
				};
				panel.Controls.Add(b);
			}

			this.Controls.Add(panel);
			this.Height += 160;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HIDTester.LEDkey));
			this.KEY_LEDMode0 = new System.Windows.Forms.Button();
			this.KEY_LEDMode1 = new System.Windows.Forms.Button();
			this.KEY_LEDMode2 = new System.Windows.Forms.Button();
			base.SuspendLayout();
			resources.ApplyResources(this.KEY_LEDMode0, "KEY_LEDMode0");
			this.KEY_LEDMode0.Name = "KEY_LEDMode0";
			this.KEY_LEDMode0.UseVisualStyleBackColor = true;
			this.KEY_LEDMode0.Click += new System.EventHandler(this.KEY_LEDMode0_Click);
			resources.ApplyResources(this.KEY_LEDMode1, "KEY_LEDMode1");
			this.KEY_LEDMode1.Name = "KEY_LEDMode1";
			this.KEY_LEDMode1.UseVisualStyleBackColor = true;
			this.KEY_LEDMode1.Click += new System.EventHandler(this.KEY_LEDMode1_Click);
			resources.ApplyResources(this.KEY_LEDMode2, "KEY_LEDMode2");
			this.KEY_LEDMode2.Name = "KEY_LEDMode2";
			this.KEY_LEDMode2.UseVisualStyleBackColor = true;
			this.KEY_LEDMode2.Click += new System.EventHandler(this.KEY_LEDMode2_Click);
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.KEY_LEDMode2);
			base.Controls.Add(this.KEY_LEDMode1);
			base.Controls.Add(this.KEY_LEDMode0);
			base.Name = "LEDkey";
			base.ResumeLayout(false);
		}
	}
}
