An example fix for https://bugs.mojang.com/browse/MC-121772 and https://bugs.mojang.com/browse/MC-255800 written in C# (it shows what input methods to use, and what to do with them)

Pre-build binary:
Included on Releases tab.

Building:
Build with VS For Mac or some Mono compiler.

Running:
The correct way to run it is to right click the output .app package and select view package contents, navigate to `.Contents/MacOS/InputTester` and launch it.

What it fixes:
- Correctly detects left click while control is pressed
- (unsure if this is was an issue) Can detect Mouse Buttons other than left, right, and middle to an effectively arbitrary degree (up to 2^63-1 to be fair)
- Make trackpad scrolling not scroll a rediculous number of items at once
- It also fixes momentum scrolling (which changes the number of scroll events based on how quickly you did it, even by like x5-10, meaning you can't easily scroll to the correct item)
- On the trackpad it also only considers scrolling while fingers are on the trackpad (and the same for any fancy mice that support the relevant api e.g. probably apple's fancy mice/trackpad thing)
- It also fixes (almost perfectly) scrolling being broken when shift is down, this issue only affects mice that use older input APIs and doesn't change anything on the trackpad. It converts scrolling with shift down which shows as horizontal scrolling to the correct vertical scroll, the only issue when you actually scroll horizontally and hold shift, this will show as vertical scrolling (which is imo acceptable since very few people would be scrolling Minecraft items with horizantal scrolling on a non-apple input device compared to people scrolling vertical on any mice; and they could, if they need, use vertical scrolling instead which would be completely consistent - and this also isn't an issue if the Minecraft item scroll direction for + vertical scrolling is treated the same as + horizontal scrolling). TL;DR - use the code in the project, add vertical and horizontal sroll in the MC scroll event, and then scroll that many items - it will work properly for both vertical and horizontal scrolling.

License:
See `LICENSE` for the license, if you are Mojang, I'm happy for these changes to be integrated into the Minecraft game itself if I'm added to the credits ;) - name hamarb123.
