//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


using System.Numerics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DatReaderWriter.Enums;
using DatReaderWriter.Types;
using DatReaderWriter.Lib;
using DatReaderWriter.Lib.Attributes;
using DatReaderWriter.Lib.IO;
using DatReaderWriter.DBObjs;

namespace DatReaderWriter {
    public partial class PortalDatabase {
        /// <summary>
        /// The ChatPoseTable DBObj in the database. This will always use the cache.
        /// </summary>
        public ChatPoseTable? ChatPoseTable => GetCached<ChatPoseTable>(0x0E000007u);
        
        /// <summary>
        /// The ObjectHierarchy DBObj in the database. This will always use the cache.
        /// </summary>
        public ObjectHierarchy? ObjectHierarchy => GetCached<ObjectHierarchy>(0x0E00000Du);
        
        /// <summary>
        /// The BadDataTable DBObj in the database. This will always use the cache.
        /// </summary>
        public BadDataTable? BadDataTable => GetCached<BadDataTable>(0x0E00001Au);
        
        /// <summary>
        /// The TabooTable DBObj in the database. This will always use the cache.
        /// </summary>
        public TabooTable? TabooTable => GetCached<TabooTable>(0x0E00001Eu);
        
        /// <summary>
        /// The NameFilterTable DBObj in the database. This will always use the cache.
        /// </summary>
        public NameFilterTable? NameFilterTable => GetCached<NameFilterTable>(0x0E000020u);
        
        /// <summary>
        /// The CharGen DBObj in the database. This will always use the cache.
        /// </summary>
        public CharGen? CharGen => GetCached<CharGen>(0x0E000002u);
        
        /// <summary>
        /// The VitalTable DBObj in the database. This will always use the cache.
        /// </summary>
        public VitalTable? VitalTable => GetCached<VitalTable>(0x0E000003u);
        
        /// <summary>
        /// The SkillTable DBObj in the database. This will always use the cache.
        /// </summary>
        public SkillTable? SkillTable => GetCached<SkillTable>(0x0E000004u);
        
        /// <summary>
        /// The SpellTable DBObj in the database. This will always use the cache.
        /// </summary>
        public SpellTable? SpellTable => GetCached<SpellTable>(0x0E00000Eu);
        
        /// <summary>
        /// The SpellComponentTable DBObj in the database. This will always use the cache.
        /// </summary>
        public SpellComponentTable? SpellComponentTable => GetCached<SpellComponentTable>(0x0E00000Fu);
        
        /// <summary>
        /// The ExperienceTable DBObj in the database. This will always use the cache.
        /// </summary>
        public ExperienceTable? ExperienceTable => GetCached<ExperienceTable>(0x0E000018u);
        
        /// <summary>
        /// The ContractTable DBObj in the database. This will always use the cache.
        /// </summary>
        public ContractTable? ContractTable => GetCached<ContractTable>(0x0E00001Du);
        
    }
}
