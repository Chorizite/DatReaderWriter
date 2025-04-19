//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
//                                                            //
//                          WARNING                           //
//                                                            //
//           DO NOT MAKE LOCAL CHANGES TO THIS FILE           //
//               EDIT THE .tt TEMPLATE INSTEAD                //
//                                                            //
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//


using System;
namespace DatReaderWriter.Enums {
    /// <summary>
    /// DBObjTypes
    /// </summary>
    public enum DBObjType : int {
        /// <summary>
        /// Unknown type
        /// </summary>
        Unknown,

        /// <summary>
        /// DBObj Iteration
        /// </summary>
        Iteration,

        /// <summary>
        /// DBObj GfxObj - GfxObj / DB_TYPE_GFXOBJ in the client.
        /// </summary>
        GfxObj,

        /// <summary>
        /// DBObj Setup - DB_TYPE_SETUP in the client.
        /// </summary>
        Setup,

        /// <summary>
        /// DBObj Animation - DB_TYPE_ANIM in the client.
        /// </summary>
        Animation,

        /// <summary>
        /// DBObj Palette - DB_TYPE_PALETTE in the client.
        /// </summary>
        Palette,

        /// <summary>
        /// DBObj SurfaceTexture - DB_TYPE_SURFACETEXTURE in the client.
        /// </summary>
        SurfaceTexture,

        /// <summary>
        /// DBObj RenderSurface - This holds raw texture data. DB_TYPE_RENDERSURFACE in the client.
        /// </summary>
        RenderSurface,

        /// <summary>
        /// DBObj Surface - DB_TYPE_SURFACE in the client.
        /// </summary>
        Surface,

        /// <summary>
        /// DBObj MotionTable - DB_TYPE_MTABLE in the client.
        /// </summary>
        MotionTable,

        /// <summary>
        /// DBObj Wave - DB_TYPE_WAVE in the client.
        /// </summary>
        Wave,

        /// <summary>
        /// DBObj Environment - DB_TYPE_ENVIRONMENT in the client.
        /// </summary>
        Environment,

        /// <summary>
        /// DBObj ChatPoseTable - DB_TYPE_CHAT_POSE_TABLE in the client.
        /// </summary>
        ChatPoseTable,

        /// <summary>
        /// DBObj ObjectHierarchy - DB_TYPE_OBJECT_HIERARCHY in the client.
        /// </summary>
        ObjectHierarchy,

        /// <summary>
        /// DBObj BadDataTable - DB_TYPE_BADDATA in the client.
        /// </summary>
        BadDataTable,

        /// <summary>
        /// DBObj TabooTable - DB_TYPE_TABOO_TABLE in the client.
        /// </summary>
        TabooTable,

        /// <summary>
        /// DBObj NameFilterTable - DB_TYPE_NAME_FILTER_TABLE in the client.
        /// </summary>
        NameFilterTable,

        /// <summary>
        /// DBObj PaletteSet - DB_TYPE_PAL_SET in the client.
        /// </summary>
        PaletteSet,

        /// <summary>
        /// DBObj Clothing - DB_TYPE_CLOTHING in the client.
        /// </summary>
        Clothing,

        /// <summary>
        /// DBObj GfxObjDegradeInfo - DB_TYPE_DEGRADEINFO in the client.
        /// </summary>
        GfxObjDegradeInfo,

        /// <summary>
        /// DBObj Scene - DB_TYPE_SCENE in the client.
        /// </summary>
        Scene,

        /// <summary>
        /// DBObj Region - DB_TYPE_REGION in the client.
        /// </summary>
        Region,

        /// <summary>
        /// DBObj Keymap - DB_TYPE_KEYMAP in the client.
        /// </summary>
        Keymap,

        /// <summary>
        /// DBObj RenderTexture - DB_TYPE_RENDERTEXTURE in the client.
        /// </summary>
        RenderTexture,

        /// <summary>
        /// DBObj RenderMaterial - 
        /// </summary>
        RenderMaterial,

        /// <summary>
        /// DBObj MaterialModifier - DB_TYPE_MATERIALMODIFIER in the client.
        /// </summary>
        MaterialModifier,

        /// <summary>
        /// DBObj MaterialInstance - DB_TYPE_MATERIALINSTANCE in the client.
        /// </summary>
        MaterialInstance,

        /// <summary>
        /// DBObj RenderMesh - DB_TYPE_RENDER_MESH in the client.
        /// </summary>
        RenderMesh,

        /// <summary>
        /// DBObj SoundTable - DB_TYPE_STABLE in the client.
        /// </summary>
        SoundTable,

        /// <summary>
        /// DBObj EnumMapper - DB_TYPE_ENUM_MAPPER in the client.
        /// </summary>
        EnumMapper,

        /// <summary>
        /// DBObj DataIdMapper - DB_TYPE_DID_MAPPER in the client.
        /// </summary>
        DataIdMapper,

        /// <summary>
        /// DBObj ActionMap - DB_TYPE_ACTIONMAP in the client.
        /// </summary>
        ActionMap,

        /// <summary>
        /// DBObj DualDataIdMapper - DB_TYPE_DUAL_DID_MAPPER in the client.
        /// </summary>
        DualDataIdMapper,

        /// <summary>
        /// DBObj LanguageString - DB_TYPE_STRING in the client.
        /// </summary>
        LanguageString,

        /// <summary>
        /// DBObj ParticleEmitter - DB_TYPE_PARTICLE_EMITTER in the client.
        /// </summary>
        ParticleEmitter,

        /// <summary>
        /// DBObj PhysicsScript - DB_TYPE_PHYSICS_SCRIPT in the client.
        /// </summary>
        PhysicsScript,

        /// <summary>
        /// DBObj PhysicsScriptTable - DB_TYPE_PHYSICS_SCRIPT_TABLE in the client.
        /// </summary>
        PhysicsScriptTable,

        /// <summary>
        /// DBObj MasterProperty - DB_TYPE_MASTER_PROPERTY in the client.
        /// </summary>
        MasterProperty,

        /// <summary>
        /// DBObj Font - DB_TYPE_FONT in the client.
        /// </summary>
        Font,

        /// <summary>
        /// DBObj DBProperties - DB_TYPE_DBPROPERTIES in the client.
        /// </summary>
        DBProperties,

        /// <summary>
        /// DBObj CharGen - DB_TYPE_CHAR_GEN_0 in the client.
        /// </summary>
        CharGen,

        /// <summary>
        /// DBObj VitalTable - DB_TYPE_ATTRIBUTE_2ND_TABLE_0 in the client.
        /// </summary>
        VitalTable,

        /// <summary>
        /// DBObj SkillTable - DB_TYPE_SKILL_TABLE_0 in the client.
        /// </summary>
        SkillTable,

        /// <summary>
        /// DBObj SpellTable - DB_TYPE_SPELL_TABLE_0 in the client.
        /// </summary>
        SpellTable,

        /// <summary>
        /// DBObj SpellComponentTable - DB_TYPE_SPELLCOMPONENT_TABLE_0 in the client.
        /// </summary>
        SpellComponentTable,

        /// <summary>
        /// DBObj ExperienceTable - Holds the experience required for different attributes/vitals/skill/levels, as well as the amount of skill credits obtained for each level. DB_TYPE_XP_TABLE_0 in the client.
        /// </summary>
        ExperienceTable,

        /// <summary>
        /// DBObj QualityFilter - DB_TYPE_QUALITY_FILTER_0 in the client.
        /// </summary>
        QualityFilter,

        /// <summary>
        /// DBObj CombatTable - DB_TYPE_COMBAT_TABLE_0 in the client.
        /// </summary>
        CombatTable,

        /// <summary>
        /// DBObj ContractTable - DB_TYPE_CONTRACT_TABLE_0 in the client.
        /// </summary>
        ContractTable,

        /// <summary>
        /// DBObj LandBlock - DB_TYPE_LAND_BLOCK in the client.
        /// </summary>
        LandBlock,

        /// <summary>
        /// DBObj LandBlockInfo - Stores static spawns, buildings. DB_TYPE_LBI in the client.
        /// </summary>
        LandBlockInfo,

        /// <summary>
        /// DBObj EnvCell - DB_TYPE_CELL in the client.
        /// </summary>
        EnvCell,

        /// <summary>
        /// DBObj UILayout - DB_TYPE_UI_LAYOUT in the client.
        /// </summary>
        UILayout,

        /// <summary>
        /// DBObj StringTable - DB_TYPE_STRING_TABLE in the client.
        /// </summary>
        StringTable,

        /// <summary>
        /// DBObj LanguageInfo - DB_TYPE_STRING_STATE in the client.
        /// </summary>
        LanguageInfo,

    };
}
