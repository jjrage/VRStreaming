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
            return "trial2911";
        }
    }
    internal string SharedKey
    {
        get
        {
            return "4a41c764ad374139b08a21c5002185928b4c20396d5644f49211ac1204086a8d";
        }
    }
    internal string Gateway
    {
        get
        {
            return "http://10.10.150.68:8080/sync";
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
