using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua.MahuaEvents;
using UnoSharp;

namespace Origind.Card.Uno.MahuaEvents
{
    public class Cleaner
    : IPluginDisabledMahuaEvent, IPluginEnabledMahuaEvent
    {
        public void Disable(PluginDisabledContext context)
        {
            Cleanup();
        }

        public void Enabled(PluginEnabledContext context)
        {
            Cleanup();
        }

        private void Cleanup()
        {
            foreach (var file in Directory.EnumerateFiles(ImageExtensions.ImagePath))
                File.Delete(file);
        }
    }
}
