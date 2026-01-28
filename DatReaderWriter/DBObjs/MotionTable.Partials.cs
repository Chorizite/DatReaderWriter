using DatReaderWriter.Enums;
using DatReaderWriter.Lib.Attributes;

namespace DatReaderWriter.DBObjs {
    [DBObjType(typeof(MotionTable), DatFileType.Portal, DBObjType.MotionTable, DBObjHeaderFlags.HasId, 0x09000000, 0x0900FFFF, 0)]
    public partial class MotionTable {
    }
}
