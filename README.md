# DatReaderWriter

DatReaderWriter is an open-source library for reading and writing .dat files used by the game Asheron's Call. This tool allows players and developers to access and modify game data files for various purposes, such as creating mods or analyzing game content.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Core Concepts](#core-concepts)
- [Basic Usage](#basic-usage)
	- [Getting Started](#getting-started)
    - [Update spell names and descriptions](#update-spell-names-and-descriptions)
    - [Rewrite all MotionTables to be 100x speed](#rewrite-all-motiontables-to-be-100x-speed)
- [Known Issues](#known-issues)
- [Contributing](#contributing)
- [Thanks](#thanks)
- [License](#license)

## Features

- **Read/Write Support**: Full support for reading and writing AC end-of-retail .dat files (`client_portal.dat`, `client_cell_1.dat`, `client_local_English.dat`, `client_highres.dat`).
- **Data Structures**: Full BTree seeking, insertion, removal, and range queries.
- **Caching**: Built-in caching options (`OnDemand`, `None`, etc.) to optimize performance.
- **Async API**: Full async support for IO operations.
- **Cross-Platform**: Targets `net8.0`, `netstandard2.0`, and `net48`.

## Installation

Install the `Chorizite.DatReaderWriter` package from NuGet:

```bash
dotnet add package Chorizite.DatReaderWriter
```

## Core Concepts

### DatCollection
The `DatCollection` class is the main entry point if you want to work with the standard set of AC dat files. It manages `Portal`, `Cell`, `Local`, and `HighRes` databases together, allowing for cross-reference lookups (though explicit database access is recommended for specific types).

### Individual Databases
You can also open individual dat files using `PortalDatabase`, `CellDatabase`, etc. This gives you more granular control and is often preferred when you know exactly which file you are modifying.

## Basic Usage

### Getting Started

```cs
using DatReaderWriter;
using DatReaderWriter.Enums;
using DatReaderWriter.Options;
using DatReaderWriter.DBObjs;

// Open a set of dat file for reading. This will open all the eor dat files as a single collection.
var datPath = @"C:\Turbing\Asheron's Call\";
using var dats = new DatCollection(datPath, DatAccessType.Read);

// Read a file explicitly from the Portal database
// We use the specific database (Portal) here for clarity and reliability
Region? region = dats.Portal.Get<Region>(0x13000000u);

// this works as well, directly from the collection
region = dats.Get<Region>(0x13000000u);

if (region != null) {
    Console.WriteLine($"Region Name: {region.RegionName}");
}

// Check iteration of portal dat
Console.WriteLine($"Portal Iteration: {dats.Portal.Iteration.CurrentIteration}");

// Determine type from a file id (using the specific database)
var type = dats.Portal.TypeFromId(0x13000000u);
// Returns DBObjType.Region

// Some types will include QualifiedDataIds<TDBObj> that reference other files
// You can call QualifiedDataId.Get(datCollection) to get the actual file object
if (!dat.TryGet<GfxObj>(0x010005E8, out var gfxObj)) {
    throw new Exception($"Failed to read GfxObj: 0x010005E8");
}
var surface = gfxObj.Surfaces.First().Get(dat);
```

### Update spell names and descriptions

```cs
var portalPath = Path.Combine(datPath, "client_portal.dat");
using var portalDat = new PortalDatabase(portalPath, DatAccessType.ReadWrite);

// Access the SpellTable directly via the property on PortalDatabase
var spellTable = portalDat.SpellTable ?? throw new Exception("Failed to read spell table");

// Update spell name / description
// (Changes are in memory until validly written back)
if (spellTable.Spells.ContainsKey(1)) {
    spellTable.Spells[1].Name = "Strength Other I (updated)";
    spellTable.Spells[1].Description = "Increases the target's Strength by 10 points. (updated)";

    // Write the updated spell table back to the dat
    if (!portalDat.TryWriteFile(spellTable)) {
        throw new Exception("Failed to write spell table");
    }
}
```

### Rewrite all MotionTables to be 100x speed

```cs  
using DatReaderWriter.DBObjs;

// ... (dats initialized as above)
var portalPath = Path.Combine(datPath, "client_portal.dat");
using var portalDat = new PortalDatabase(portalPath, DatAccessType.ReadWrite);

// Get all MotionTable IDs
// (This scans the database for files matching the MotionTable type ID range)
var motionTableIds = portalDat.GetAllIdsOfType<MotionTable>();

foreach (var id in motionTableIds) {
    // Read the file
    if (portalDat.TryGet<MotionTable>(id, out var mTable)) {
        // Update framerates in cycles
        foreach (var cycle in mTable.Cycles.Values) {
            foreach (var anim in cycle.Anims) {
                anim.Framerate *= 100f;
            }
        }
        
        // Update framerates in modifiers
        foreach (var modifier in mTable.Modifiers.Values) {
            foreach (var anim in modifier.Anims) {
                anim.Framerate *= 100f;
            }
        }

        // Write the updated MotionTable back
        portalDat.TryWriteFile(mTable);
    }
}
```

## Known Issues
- RenderMaterial files are not yet supported.
- LayoutDesc files are supported, but the structure will need to be cleaned up in future versions.

## Contributing

We welcome contributions from the community! If you would like to contribute to DatReaderWriter, please follow these steps:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes.
4. Commit your changes (`git commit -am 'Add some feature'`).
5. Push to the branch (`git push origin feature-branch`).
6. Create a new Pull Request.

## Thanks

In no particular order, thanks to ACE team, GDLE team, gmriggs, OptimShi, paradox, and Yonneh. Used lots of projects as a reference for different parts.

## License

This project is licensed under the MIT License. See the LICENSE.txt file for details.
