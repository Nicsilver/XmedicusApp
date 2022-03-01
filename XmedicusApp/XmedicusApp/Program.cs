using System.Net.Http.Json;

HttpClient client = new();

//Get the json object of regents
HttpResponseMessage result =
    await client.GetAsync("https://gist.githubusercontent.com/adbir/e8b768cc854f0499034cd40fcf34a720/raw");

List<Regent>? regents;

//Convert the json string into a list of Regents
regents = await result.Content.ReadFromJsonAsync<List<Regent>>();

Regent longestRegent = new();
int longestReignTime = 0;

string currentRegentGenus = "";
string longestReigningGenus = "";
int currentRegentGenusLength = 0;
int longestReigningGenusLength = 0;

foreach (Regent regent in regents!)
{
    regent.Yrs = regent.Yrs.Replace(" ", "");
    string[] years = regent.Yrs.Split("-");

    if (years.Any(s => s.Equals("")) == false)
    {
        if (years[1].Equals("present"))
            years[1] = DateTime.Now.ToString("yyyy");

        int reignTime = int.Parse(years[1]) - int.Parse(years[0]);


        if (reignTime > longestReignTime)
        {
            longestRegent = regent;
            longestReignTime = reignTime;
        }

        if (!regent.Hse.Equals(""))
        {
            if (currentRegentGenus.Equals(regent.Hse))
            {
                currentRegentGenusLength += reignTime;
            }
            else
            {
                if (currentRegentGenusLength > longestReigningGenusLength)
                {
                    longestReigningGenusLength = currentRegentGenusLength;
                    longestReigningGenus = regent.Hse;
                }

                currentRegentGenus = regent.Hse;
                currentRegentGenusLength = 0;
            }
        }
    }
}

List<string> regentNames = new();

regents.ForEach(regent => regentNames.Add(regent.Nm.Split(" ").First()));

string mostPoplarName = regentNames.GroupBy(i => i).OrderByDescending(grp => grp.Count())
    .Select(grp => grp.Key).First(_ => true);


int yearsWithoutRegent = 0;

IEnumerable<Regent> noRegentList = regents.Where(regent => regent.Nm.Equals("Interregnum"));

foreach (Regent regent in noRegentList)
{
    string[] years = regent.Yrs.Split("-");
    int reignTime = int.Parse(years[1]) - int.Parse(years[0]);
    yearsWithoutRegent += reignTime;
}


Console.WriteLine($"Denmark has had {regents.Count} regents");
Console.WriteLine($"{longestRegent.Nm} has reigned the longest, and reigned for {longestReignTime} years");
Console.WriteLine(
    $"Denmark's longest reigning genus is {longestReigningGenus}, which lasted for {longestReigningGenusLength} years");

Console.WriteLine($"The most popular name for a regent is {mostPoplarName}");
Console.WriteLine($"Denmark has been without a regent for a total of {yearsWithoutRegent} years");


internal class Regent
{
    public int Id { get; set; }
    public string Nm { get; set; }
    public string Yrs { get; set; }
    public string Hse { get; set; }
}