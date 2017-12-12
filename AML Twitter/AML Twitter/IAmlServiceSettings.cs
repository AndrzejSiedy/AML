namespace AML.Twitter
{
    public interface IAmlServiceSettings
    {
        string AmlBaseUrl { get; set; }
        string AmlHarvesterUrl { get; set; }
        string AmlListVersionUrl { get; set; }
        int ServiceCallInterval { get; set; }
    }
}