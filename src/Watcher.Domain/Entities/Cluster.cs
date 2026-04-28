namespace Watcher.Domain.Entities;

public record Cluster(
    string IdCluster,
    string Name,
    decimal BaseLimit,
    decimal Cap
);

public static class ClusterDefinitions
{
    public static readonly Cluster CLUSTER_A = new("CLUSTER_A", "Diamond", 50000, 100000);
    public static readonly Cluster CLUSTER_B = new("CLUSTER_B", "Gold", 20000, 40000);
    public static readonly Cluster CLUSTER_C = new("CLUSTER_C", "Silver", 5000, 10000);
    public static readonly Cluster CLUSTER_D = new("CLUSTER_D", "Bronze", 0, 0);
}