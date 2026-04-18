using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MOBWEB_TEST.Location
{
    public class DeviceLocationMessage : ValueChangedMessage<DeviceLocation>
    {
        public DeviceLocationMessage(DeviceLocation value) : base(value)
        {

        }
    }
}
