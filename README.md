# Augment

.NET Object Extension Library (formerly NOX) - renamed from NOX since
there were several overlapping projects of various areas using NOX as
a codename.

*What is Augment?* By definition _augment_ means to make (something)
greater by adding to it.  While .NET has an abundance of libraries
that make coding fantastic, I found myself reusing and copying several
extension files from project  to project. So I decided to place them
into a common library and share with anyone who is interested.  This
is a small collection of common code I've collected over the years
from various sources (some known, others unfortunately forgotten).

### Samples

[DateTime](https://github.com/ellerbus/Augment/blob/master/Augment/AugmentTests/Extensions/DateTimeExtensionTests.cs), [TimeSpan](https://github.com/ellerbus/Augment/blob/master/Augment/AugmentTests/Extensions/TimeSpanExtensionTests.cs) Extensions

``` csharp
DateTime.Now.BeginningOfDay()		// Week, Month, Year, Quarter
DateTime.Now.EndOfDay()				// Week, Month, Year, Quarter

DateTime.Now.IsBusinessDay()
DateTime.Now.IsHoliday()
DateTime.Now.IsWeekend()

DateTime.Now.ToRelativeDateString()	//	3 seconds ago or 3 seconds from now

3.Seconds().Ago()

6.Months().FromNow()
```

[Int](https://github.com/ellerbus/Augment/blob/master/Augment/AugmentTests/Extensions/IntExtensionTests.cs), [Double](https://github.com/ellerbus/Augment/blob/master/Augment/AugmentTests/Extensions/DoubleExtensionTests.cs) Extensions

``` csharp
3.25.PercentOf(125.548)

25.PercentOf(20000)
```

[String](https://github.com/ellerbus/Augment/blob/master/Augment/AugmentTests/Extensions/StringExtensionTests.cs), [StringBuilder](https://github.com/ellerbus/Augment/blob/master/Augment/AugmentTests/Extensions/StringBuilderExtensionTests.cs) Extensions

``` csharp
string text = "123abc";

text = text.GetLeftOf("abc");

StringBuilder sb = new StringBuilder(text);

sb.AppendIf(text.Contains("abc"), "def" /*true*/, "456" /*false*/);
// or
sb.AppendIf(text.Contains("abc"), "def");

string so = null;

if (so.IsNullOrEmtpy()) so = "what!";

string d = so.AssertNotNull("abc");

```

### Supported .NET Framework Versions

-	The [NuGet Package](http://www.nuget.org/packages/Augment) is compiled against 4.5
