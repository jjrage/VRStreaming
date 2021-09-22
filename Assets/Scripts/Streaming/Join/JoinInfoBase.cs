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
            return "605f7e8e097442488e0b38adb2e218c28039be4906ed4ec8ad455968ecaaa900";
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
