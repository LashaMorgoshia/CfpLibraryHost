using Microsoft.Azure.Cosmos;

class Program
{
    private static CosmosClient Client { get; set; }
    private static readonly string ConnectionString = "AccountEndpoint=https://cosmosdbapp1.documents.azure.com:443/;AccountKey=ZpZngo2Z1rSEpkvtVCdDIulDWHrLyHKDMDWqpBFDC2o0BIrXdIVSicwIIooOKkxmF76uOEKwtskLh5kRZDXdaQ==;";

    public static void Main(String[] args)
    {
        Task.Run(async () =>
        {
            Client = new CosmosClient(ConnectionString);

            var database = Client.GetDatabase("acme-webstore");
            var cartContainer = database.GetContainer("cart");
            var leaseContainer = database.GetContainer("lease");

            var cfp = cartContainer.GetChangeFeedProcessorBuilder<dynamic>("CfpLibraryDemo", ProcessChanged)
            .WithLeaseContainer(leaseContainer)
            .WithInstanceName("Change Feed processor library demo")
            .WithStartTime(DateTime.MinValue.ToUniversalTime())
            .Build();

            await cfp.StartAsync();

            Console.WriteLine("Started change feed processor - press any key to stop");
            Console.ReadKey(true);
            await cfp.StopAsync();

        }).Wait();
    }

    static async Task ProcessChanged(IReadOnlyCollection<dynamic> docs, CancellationToken cancellationToken)
    {
        foreach(var doc in docs)
        {
            Console.WriteLine($"Document {doc.id} has changed!");
            Console.WriteLine(doc.ToString());
        }
    }
}
