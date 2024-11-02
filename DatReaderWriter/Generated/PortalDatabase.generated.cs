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
using ACClientLib.DatReaderWriter.Enums;
using ACClientLib.DatReaderWriter.IO;
using ACClientLib.DatReaderWriter.Types;
using ACClientLib.DatReaderWriter.Attributes;
using ACClientLib.DatReaderWriter.Lib;
using ACClientLib.DatReaderWriter.DBObjs;

namespace ACClientLib.DatReaderWriter {
    public partial class PortalDatabase {
        private DBObjCollection<GfxObj>? _GfxObjs;
        private DBObjCollection<Setup>? _Setups;
        private DBObjCollection<Animation>? _Animations;
        private DBObjCollection<SurfaceTexture>? _SurfaceTextures;
        private DBObjCollection<RenderSurface>? _RenderSurfaces;
        private DBObjCollection<Surface>? _Surfaces;
        private DBObjCollection<MotionTable>? _MotionTables;
        private DBObjCollection<Environment>? _Environments;
        private DBObjCollection<Region>? _Regions;
        private DBObjCollection<MaterialModifier>? _MaterialModifiers;
        private DBObjCollection<MaterialInstance>? _MaterialInstances;
        private DBObjCollection<DataIdMapper>? _DataIdMappers;
        private DBObjCollection<ExperienceTable>? _ExperienceTables;

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
        /// All Environments in the database.
        /// </summary>
        public DBObjCollection<Environment> Environments => _Environments ??= new DBObjCollection<Environment>(this);

        /// <summary>
        /// All Regions in the database.
        /// </summary>
        public DBObjCollection<Region> Regions => _Regions ??= new DBObjCollection<Region>(this);

        /// <summary>
        /// All MaterialModifiers in the database.
        /// </summary>
        public DBObjCollection<MaterialModifier> MaterialModifiers => _MaterialModifiers ??= new DBObjCollection<MaterialModifier>(this);

        /// <summary>
        /// All MaterialInstances in the database.
        /// </summary>
        public DBObjCollection<MaterialInstance> MaterialInstances => _MaterialInstances ??= new DBObjCollection<MaterialInstance>(this);

        /// <summary>
        /// All DataIdMappers in the database.
        /// </summary>
        public DBObjCollection<DataIdMapper> DataIdMappers => _DataIdMappers ??= new DBObjCollection<DataIdMapper>(this);

        /// <summary>
        /// All ExperienceTables in the database.
        /// </summary>
        public DBObjCollection<ExperienceTable> ExperienceTables => _ExperienceTables ??= new DBObjCollection<ExperienceTable>(this);

    }
}
