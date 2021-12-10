internal class JoinInfoBase
{
    internal JoinInfoBase(string name, string channel, bool isBroadcaster)
    {
        Name = name;
        ChannelId = channel;
        IsBroadcaster = isBroadcaster;
    }

    internal string Name { get; set; }
    internal string ChannelId { get; set; }
    internal string AppId
    {
        get
        {
            return "testing";
        }
    }
    internal string SharedKey
    {
        get
        {
            return "ae8164d18e264f2da966a7b74f987f2e0b13315c3c974a148e80a6260027fca3";
        }
    }
    internal string Gateway
    {
        get
        {
            return "http://192.168.0.101:8080/sync";
        }
    }
    internal bool AudioOnly
    {
        get
        {
            return false;
        }
    }


    internal bool ReceiveOnly
    {
        get
        {
            return false;
        }
    }
    internal bool CaptureWithUnityCamera
    {
        get
        {
            return true;
        }
    }
    internal bool IsBroadcaster { get; set; }
    internal Mode Mode = Mode.Sfu;
}
