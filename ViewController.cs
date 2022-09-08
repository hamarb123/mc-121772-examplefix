using System;

using AppKit;
using CoreGraphics;
using Foundation;

namespace InputTester
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.
		}

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}

	[Register("CustomView")]
	public class CustomView : NSView
	{
		protected internal CustomView(IntPtr handle) : base(handle)
		{
		}

		public CustomView()
		{
		}

		public CustomView(NSCoder coder) : base(coder)
		{
		}

		protected CustomView(NSObjectFlag t) : base(t)
		{
		}

		public CustomView(CGRect frameRect) : base(frameRect)
		{
		}

		public override void MouseDown(NSEvent theEvent)
		{
			MouseEvent(theEvent, false);
			base.MouseDown(theEvent);
		}

		public override void MouseUp(NSEvent theEvent)
		{
			MouseEvent(theEvent, true);
			base.MouseUp(theEvent);
		}

		public override void RightMouseDown(NSEvent theEvent)
		{
			MouseEvent(theEvent, false);
			base.RightMouseDown(theEvent);
		}

		public override void RightMouseUp(NSEvent theEvent)
		{
			MouseEvent(theEvent, true);
			base.RightMouseUp(theEvent);
		}

		public override void OtherMouseDown(NSEvent theEvent)
		{
			MouseEvent(theEvent, false);
			base.OtherMouseDown(theEvent);
		}

		public override void OtherMouseUp(NSEvent theEvent)
		{
			MouseEvent(theEvent, true);
			base.OtherMouseUp(theEvent);
		}

		public void MouseEvent(NSEvent theEvent, bool up)
		{
			if (theEvent.ButtonNumber < 3)
			{
				var button = (long)theEvent.ButtonNumber switch
				{
					0 => "Left",
					1 => "Right",
					2 => "Middle",
					_ => default,
				};
				Console.WriteLine($"{button} Mouse {(up ? "Up" : "Down")}");
			}
			else
			{
				Console.WriteLine($"Mouse Button { (long)theEvent.ButtonNumber + 1 } {(up ? "Up" : "Down")}");
			}
		}

		public override bool AcceptsFirstResponder()
		{
			return true;
		}

		double scrollX, scrollY;

		//scrollWheel: selector
		public override void ScrollWheel(NSEvent theEvent)
		{
			double x = theEvent.ScrollingDeltaX, y = theEvent.ScrollingDeltaY;

			//if shift is down, macos adds vertical axis to horizontal axis and sets vertical to 0 for 'legacy scroll events' (indicated by NSEventPhase.None)
			if ((theEvent.ModifierFlags & NSEventModifierMask.ShiftKeyMask) != 0 && theEvent.Phase == NSEventPhase.None)
			{
				//note: this conversion is slightly incorrect for horizontal scrolling on legacy input devices when shift is down (doesn't affect the trackpad),
				//using (x, y) = (y, x) doesn't fix it, so we do the below instead since it would work if someone is running an app that converts horizontal scrolls to vertical always
				//(which is currently illegal for speedrunning, and no it's not a good solution to the original bug)
				y += x;
				x = 0;
			}

			//enable the following block if you want to ensure scrolling direction is the same regardless of user preferences:
			/*
			if (!theEvent.IsDirectionInvertedFromDevice)
			{
				x = -x;
				y = -y;
			}
			*/

			//if it is a non-legacy scrolling event (meaning it has a beginning and an end), and it is on the trackpad or other high precision scroll,
			//then we don't want to interpret it as a million scroll events for a very small movement. also check that it wasn't caused by momentum
			if (theEvent.Phase != NSEventPhase.None && theEvent.HasPreciseScrollingDeltas && theEvent.MomentumPhase == NSEventPhase.None)
			{
				const int sensitivity = 20;
				if (theEvent.Phase == NSEventPhase.Began)
				{
					//reset scroll counter, and ensure that the first event generates a scroll
					scrollX = Math.Sign(x) * Math.Max(Math.Abs(x) - 1, 0);
					scrollY = Math.Sign(y) * Math.Max(Math.Abs(y) - 1, 0);
					x = Math.Sign(x);
					y = Math.Sign(y);
				}
				else
				{
					//group scrolls together up to a magnitude of the sensitivity
					scrollX += x;
					scrollY += y;
					x = 0;
					y = 0;
					if (Math.Abs(scrollX) >= sensitivity)
					{
						x = Math.Sign(scrollX) * (int)(Math.Abs(scrollX) / sensitivity);
						scrollX = Math.Sign(scrollX) * (Math.Abs(scrollX) % sensitivity);
					}
					if (Math.Abs(scrollY) >= sensitivity)
					{
						y = Math.Sign(scrollY) * (int)(Math.Abs(scrollY) / sensitivity);
						scrollY = Math.Sign(scrollY) * (Math.Abs(scrollY) % sensitivity);
					}
				}
			}

			//if there isn't precise deltas (ie. mouse), macos often causes smooth scrolling anyway, this effectively disables it
			if (!theEvent.HasPreciseScrollingDeltas)
			{
				x = Math.Sign(x);
				y = Math.Sign(y);
			}

			//check that there is an event left and that it wasn't caused by momentum
			if (x != 0 || y != 0)
			{
				if (theEvent.MomentumPhase == NSEventPhase.None)
				{
					Console.WriteLine($"{x + y} ({x} {y})");
				}
			}

			base.ScrollWheel(theEvent);
		}
	}
}
