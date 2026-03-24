namespace SmartCarry.Runtime;

internal static class TimedProfileCache
{
    internal readonly struct Entry
    {
        public Entry(int baselineCapacity, double expiresAt)
        {
            BaselineCapacity = baselineCapacity;
            ExpiresAt = expiresAt;
        }

        public int BaselineCapacity { get; }

        public double ExpiresAt { get; }
    }

    public static Entry Create(int baselineCapacity, double now, double lifetimeSeconds)
    {
        return new Entry(baselineCapacity, now + lifetimeSeconds);
    }

    public static bool CanReuse(Entry entry, int baselineCapacity, double now)
    {
        return entry.BaselineCapacity == baselineCapacity && entry.ExpiresAt > now;
    }
}
