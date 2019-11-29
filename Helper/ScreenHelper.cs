using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 素材合成.Helper
{
    class ScreenHelper
    {
//        //物理显示设备集合
//        var screens = Screen.AllScreens.OrderBy(m => m.DeviceName).ToArray();
//            foreach (var item in screens)
//            {
//                DisplayDevice configDevice = null;
//                try
//                {
//                    var deviceConfig = Configer.Read(item.DeviceName);
//                    if (!string.IsNullOrEmpty(deviceConfig))
//                    {
//                        configDevice = JsonConvert.DeserializeObject<DisplayDevice>(deviceConfig);
//                    }
//}
//                catch (Exception)
//                {
//                    // ignored
//                }
//                var displayDevice = new DisplayDevice
//                {
//                    MaxHeight = item.Bounds.Height,
//                    MaxWidth = item.Bounds.Width,
//                    Opened = configDevice?.Opened ?? false,
//                    VirtualDevice = false,
//                    Opacity = configDevice?.Opacity ?? 100,
//                    LeftMarginX = configDevice?.LeftMarginX ?? item.Bounds.X,
//                    TopMarginY = configDevice?.TopMarginY ?? item.Bounds.Y,
//                    Width = configDevice?.Width ?? item.Bounds.Width,
//                    Height = configDevice?.Height ?? item.Bounds.Height,
//                    ScreenTitle = item.DeviceName

//                };
//ControlParam.Screens.Add(displayDevice);
    }
}
