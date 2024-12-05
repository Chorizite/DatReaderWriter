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
        private DBObjCollection<GfxObj>? _GfxObjs;
        private DBObjCollection<Setup>? _Setups;
        private DBObjCollection<Animation>? _Animations;
        private DBObjCollection<Palette>? _Palettes;
        private DBObjCollection<SurfaceTexture>? _SurfaceTextures;
        private DBObjCollection<RenderSurface>? _RenderSurfaces;
        private DBObjCollection<Surface>? _Surfaces;
        private DBObjCollection<MotionTable>? _MotionTables;
        private DBObjCollection<Wave>? _Waves;
        private DBObjCollection<Environment>? _Environments;
        private DBObjCollection<PaletteSet>? _PaletteSets;
        private DBObjCollection<Clothing>? _Clothings;
        private DBObjCollection<Scene>? _Scenes;
        private DBObjCollection<Region>? _Regions;
        private DBObjCollection<Keymap>? _Keymaps;
        private DBObjCollection<MaterialModifier>? _MaterialModifiers;
        private DBObjCollection<MaterialInstance>? _MaterialInstances;
        private DBObjCollection<EnumMapper>? _EnumMappers;
        private DBObjCollection<DataIdMapper>? _DataIdMappers;
        private DBObjCollection<DualDataIdMapper>? _DualDataIdMappers;
        private DBObjCollection<LanguageString>? _LanguageStrings;
        private DBObjCollection<ParticleEmitter>? _ParticleEmitters;
        private DBObjCollection<PhysicsScript>? _PhysicsScripts;
        
        /// <summary>
        /// All GfxObjs in the database.
        /// </summary>
        public DBObjCollection<GfxObj> GfxObjs => _GfxObjs ??= new DBObjCollection<GfxObj>(this);
        
        /// <summary>
        /// All Setups in the database.
        /// </summary>
        public DBObjCollection<Setup> Setups => _Setups ??= new DBObjCollection<Setup>(this);
        
        /// <summary>
        /// All Animations in the database.
        /// </summary>
        public DBObjCollection<Animation> Animations => _Animations ??= new DBObjCollection<Animation>(this);
        
        /// <summary>
        /// All Palettes in the database.
        /// </summary>
        public DBObjCollection<Palette> Palettes => _Palettes ??= new DBObjCollection<Palette>(this);
        
        /// <summary>
        /// All SurfaceTextures in the database.
        /// </summary>
        public DBObjCollection<SurfaceTexture> SurfaceTextures => _SurfaceTextures ??= new DBObjCollection<SurfaceTexture>(this);
        
        /// <summary>
        /// All RenderSurfaces in the database.
        /// </summary>
        public DBObjCollection<RenderSurface> RenderSurfaces => _RenderSurfaces ??= new DBObjCollection<RenderSurface>(this);
        
        /// <summary>
        /// All Surfaces in the database.
        /// </summary>
        public DBObjCollection<Surface> Surfaces => _Surfaces ??= new DBObjCollection<Surface>(this);
        
        /// <summary>
        /// All MotionTables in the database.
        /// </summary>
        public DBObjCollection<MotionTable> MotionTables => _MotionTables ??= new DBObjCollection<MotionTable>(this);
        
        /// <summary>
        /// All Waves in the database.
        /// </summary>
        public DBObjCollection<Wave> Waves => _Waves ??= new DBObjCollection<Wave>(this);
        
        /// <summary>
        /// All Environments in the database.
        /// </summary>
        public DBObjCollection<Environment> Environments => _Environments ??= new DBObjCollection<Environment>(this);
        
        /// <summary>
        /// The ChatPoseTable DBObj in the database.
        /// </summary>
        public ChatPoseTable? ChatPoseTable {
            get {
                TryReadFile<ChatPoseTable>(0x0E000007u, out var dbObj);
                return dbObj;
            }
        }
        /// <summary>
        /// The BadDataTable DBObj in the database.
        /// </summary>
        public BadDataTable? BadDataTable {
            get {
                TryReadFile<BadDataTable>(0x0E00001Au, out var dbObj);
                return dbObj;
            }
        }
        /// <summary>
        /// All PaletteSets in the database.
        /// </summary>
        public DBObjCollection<PaletteSet> PaletteSets => _PaletteSets ??= new DBObjCollection<PaletteSet>(this);
        
        /// <summary>
        /// All Clothings in the database.
        /// </summary>
        public DBObjCollection<Clothing> Clothings => _Clothings ??= new DBObjCollection<Clothing>(this);
        
        /// <summary>
        /// All Scenes in the database.
        /// </summary>
        public DBObjCollection<Scene> Scenes => _Scenes ??= new DBObjCollection<Scene>(this);
        
        /// <summary>
        /// All Regions in the database.
        /// </summary>
        public DBObjCollection<Region> Regions => _Regions ??= new DBObjCollection<Region>(this);
        
        /// <summary>
        /// All Keymaps in the database.
        /// </summary>
        public DBObjCollection<Keymap> Keymaps => _Keymaps ??= new DBObjCollection<Keymap>(this);
        
        /// <summary>
        /// All MaterialModifiers in the database.
        /// </summary>
        public DBObjCollection<MaterialModifier> MaterialModifiers => _MaterialModifiers ??= new DBObjCollection<MaterialModifier>(this);
        
        /// <summary>
        /// All MaterialInstances in the database.
        /// </summary>
        public DBObjCollection<MaterialInstance> MaterialInstances => _MaterialInstances ??= new DBObjCollection<MaterialInstance>(this);
        
        /// <summary>
        /// All EnumMappers in the database.
        /// </summary>
        public DBObjCollection<EnumMapper> EnumMappers => _EnumMappers ??= new DBObjCollection<EnumMapper>(this);
        
        /// <summary>
        /// All DataIdMappers in the database.
        /// </summary>
        public DBObjCollection<DataIdMapper> DataIdMappers => _DataIdMappers ??= new DBObjCollection<DataIdMapper>(this);
        
        /// <summary>
        /// All DualDataIdMappers in the database.
        /// </summary>
        public DBObjCollection<DualDataIdMapper> DualDataIdMappers => _DualDataIdMappers ??= new DBObjCollection<DualDataIdMapper>(this);
        
        /// <summary>
        /// All LanguageStrings in the database.
        /// </summary>
        public DBObjCollection<LanguageString> LanguageStrings => _LanguageStrings ??= new DBObjCollection<LanguageString>(this);
        
        /// <summary>
        /// All ParticleEmitters in the database.
        /// </summary>
        public DBObjCollection<ParticleEmitter> ParticleEmitters => _ParticleEmitters ??= new DBObjCollection<ParticleEmitter>(this);
        
        /// <summary>
        /// All PhysicsScripts in the database.
        /// </summary>
        public DBObjCollection<PhysicsScript> PhysicsScripts => _PhysicsScripts ??= new DBObjCollection<PhysicsScript>(this);
        
        /// <summary>
        /// The CharGen DBObj in the database.
        /// </summary>
        public CharGen? CharGen {
            get {
                TryReadFile<CharGen>(0x0E000002u, out var dbObj);
                return dbObj;
            }
        }
        /// <summary>
        /// The VitalTable DBObj in the database.
        /// </summary>
        public VitalTable? VitalTable {
            get {
                TryReadFile<VitalTable>(0x0E000003u, out var dbObj);
                return dbObj;
            }
        }
        /// <summary>
        /// The SkillTable DBObj in the database.
        /// </summary>
        public SkillTable? SkillTable {
            get {
                TryReadFile<SkillTable>(0x0E000004u, out var dbObj);
                return dbObj;
            }
        }
        /// <summary>
        /// The SpellTable DBObj in the database.
        /// </summary>
        public SpellTable? SpellTable {
            get {
                TryReadFile<SpellTable>(0x0E00000Eu, out var dbObj);
                return dbObj;
            }
        }
        /// <summary>
        /// The SpellComponentTable DBObj in the database.
        /// </summary>
        public SpellComponentTable? SpellComponentTable {
            get {
                TryReadFile<SpellComponentTable>(0x0E00000Fu, out var dbObj);
                return dbObj;
            }
        }
        /// <summary>
        /// The ExperienceTable DBObj in the database.
        /// </summary>
        public ExperienceTable? ExperienceTable {
            get {
                TryReadFile<ExperienceTable>(0x0E000018u, out var dbObj);
                return dbObj;
            }
        }
    }
}
