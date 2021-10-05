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
            return "VRStreaming";
        }
    }
    internal string SharedKey
    {
        get
        {
            return "0cd898db1abd408f86aaf2e55146a9820ee86c3c2c5247d39f15f8afd86df093";
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
