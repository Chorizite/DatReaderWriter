using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatReaderWriter.Enums {
    /// <summary>
    /// ToggleType used in ActionMaps
    /// </summary>
    public enum ToggleType : uint {
        Invalid = 0,
        Momentary = 1,
        Toggle = 2,
        Impulse = 3,
        AutoRepeat = 4,
        Continuous = 5
    }
}
