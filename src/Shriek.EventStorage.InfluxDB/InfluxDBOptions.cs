namespace Shriek.EventStorage.InfluxDB
{
    /// <summary>
    /// InfluxDB设置
    /// </summary>
    public class InfluxDBOptions
    {
        public string Host { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string DatabaseName { get; set; }
    }
}