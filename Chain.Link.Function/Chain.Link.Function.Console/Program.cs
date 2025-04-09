using Chain.Link.Function;

Console.WriteLine("Starting function chain...");

string[] words =
[
    "silly",
    "elegant",
    "pointless",
    "nonsense",
    "happy",
    "ninja"
];

var random = new Random();

Func<string> getWord = () => words[random.Next(words.Length)]; 

string SomethingSilly(string x) => x + $"|{getWord()}";
var silly = SomethingSilly;

var instance = new Instanced();
Func<string,string> otherSilly = instance.SomethingElseSilly;


var generator = ChainLinkFunction
    .Start<string>()
    .Transform(x => x.Trim())
    .Transform(x => otherSilly(x))
    .Transform(x => string.Join(string.Empty, x.Reverse()))
    .Transform(x => x.ToUpper())
    .Transform(x => x.Replace(x[0], 'X'))
    .Transform(x => silly(x))
    .Infinite()
    .Lock()
    ;

var list = generator("Hello, World!").Take(50).ToList();

foreach(var item in list)
{
    Console.WriteLine(item);
}

public class Instanced
{
    public string SomethingElseSilly(string x) => "a-b-c|" + x;
}
