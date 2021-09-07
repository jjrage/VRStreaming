using FM.LiveSwitch;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingLibrary
{
    public class RenderTextureVideoSource //: FM.LiveSwitch.VideoSource
    {

        //private VideoCaptureObject _Capture;

        //public override string Label => throw new NotImplementedException();

        //protected override FM.LiveSwitch.Future<object> DoStart()
        //{
        //    var promise = new FM.LiveSwitch.Promise<object>();

        //    _Capture = new VideoCaptureObject();
        //    _Capture.VideoDataAvailable += (int width, int height, byte[] data) =>
        //    {
        //        var dataBuffer = FM.LiveSwitch.DataBuffer.Wrap(data);
        //        var videoBuffer = new FM.LiveSwitch.VideoBuffer(width, height, dataBuffer, this.OutputFormat);
        //        var videoFrame = new FM.LiveSwitch.VideoFrame(videoBuffer);

        //        this.RaiseFrame(videoFrame);
        //    });

        //    promise.Resolve(null);
        //    return promise;
        //}

        //protected override Future<object> DoStop()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
