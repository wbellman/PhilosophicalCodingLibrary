// Let's play with Schrodinger's box

using Schrodinger.Variable;

var box = new OpenSchrodingerBox<string>(
    () => "Found absolute truth!",
    () => "Lost in the void...",
    () => ((DateTime.Now.Ticks / 10) & 1 ) == 0
).Lock();

var values = Enumerable.Range(0, 10000)
    .Select(_ => box.Value)
    .ToList();

var trueCount = values.Count(v => v == "Found absolute truth!");
var falseCount = values.Count(v => v == "Lost in the void...");

Console.WriteLine($"True: {trueCount}, False: {falseCount}");
