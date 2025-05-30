
# DatReaderWriter

DatReaderWriter is an open-source library for reading and writing .dat files used by the game Asheron's Call. This tool allows players and developers to access and modify game data files for various purposes, such as creating mods or analyzing game content.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Basic Usage](#basic-usage)
	- [Getting Started](#getting-started)
    - [Update spell names and descriptions](#update-spell-names-and-descriptions)
    - [Rewrite all MotionTables to be 100x speed](#rewrite-all-motiontables-to-be-100x-speed)
- [Todo](#todo)
- [Contributing](#contributing)
- [Thanks](#thanks)
- [License](#license)

## Features

- Read/Write AC end-of-retail .dat files (Including creating new dats!)
- Full Dat BTree seeking / insertion / removal / range queries
- Built-in caching options
- net8.0;netstandard2.0;net48 nuget packages
- *Most* DBObj file formats are in the dats.xml, which could be potentially used to generate readerwriters in other languages

## Basic Usage

- Install `Chorizite.DatReaderWriter` package from nuget.org
- See Tests for further usage.  

### Getting Started
```cs
// Open a set of dat file for reading. This will open all the eor dat files as a single collection.
// (client_portal.dat, client_cell_1.dat, client_local_English.dat, and client_highres.dat)
var datPath = @"C:\Turbing\Asheron's Call\";
var dats= new DatCollection(EORCommonData.DatDirectory, DatAccessType.ReadWrite);

// read files
LayoutDesc? layoutDesc = dats.Get<LayoutDesc>(0x21000000u);

// check iteration of portal dat
Console.WriteLine($"Portal Iteration: {dats.Portal.Iteration.CurrentIteration}");

// get ids of all Animations
IEnumerable<uint> allAnimationIds = dats.GetAllIdsOfType<Animation>();

// determine type from a file id
var type = dats.Local.TypeFromId(0x21000000u);
Assert.AreEqual(DBObjType.LayoutDesc, type);

// write a file with a new iteration
StringTable? stringTable = dats.Get<StringTable>(0x23000001u) ?? throw new Exception("StringTable not found");
stringTable.StringTableData.Add(0x1234u, new StringTableData() {
    Strings = ["foo", "bar"]
});
var newIteration = dats.Local.Iteration.CurrentIteration + 1;
if (!dats.TryWriteFile(stringTable, newIteration)) throw new Exception($"Failed to write StringTable");

// Dispose dat collection to flush any changes and close the files
dats.Dispose();
```

### Update spell names and descriptions
```cs
var portalDat = new PortalDatabase(Path.Combine(config.clientDir, "client_portal.dat"), DatAccessType.ReadWrite);
var spellTable = portalDat.SpellTable ?? throw new Exception("Failed to read spell table");

// update spell name / description (no need to worry about updating Components with newly
// encrypted values, they will be transparently decrypted/encrypted during (un)packing).
spellTable.Spells[1].Name = "Strength Other I (updated)";
spellTable.Spells[1].Description = "Increases the target's Strength by 10 points. (updated)";

//write the updated spell table
if (!portalDat.TryWriteFile(spellTable)) {
    throw new Exception("Failed to write spell table");
}

// close dat
portalDat.Dispose();
```

### Rewrite all MotionTables to be 100x speed
```cs  
var portalDat = new PortalDatabase(Path.Combine(config.clientDir, "client_portal.dat"), DatAccessType.ReadWrite);

// loop through all motion tables and update framerates
foreach (var mTable in portalDat.MotionTables) {
    // build a list of all animations
    var anims = new List<AnimData>();
    anims.AddRange(mTable.Cycles.Values.SelectMany(c => c.Anims));
    anims.AddRange(mTable.Modifiers.Values.SelectMany(c => c.Anims));
    anims.AddRange(mTable.Links.Values.SelectMany(v => v.MotionData.Values.SelectMany(c => c.Anims)));

    // update all animation framerates
    foreach (var anim in anims) {
        anim.Framerate *= 100f;
    }

    // write MotionTable back to the dat
    portalDat.TryWriteFile(mTable);
}

// close dat
portalDat.Dispose();
```

## Todo
- Support for older dat formats
- Clean up source gen so the library can be generated in other languages (kaitai struct?)
- DBObjs left to implement:
    - RenderMaterial

## Contributing

We welcome contributions from the community! If you would like to contribute to DatReaderWriter, please follow these steps:

1. Fork the repository.
2. Create a new branch (git checkout -b feature-branch).
3. Make your changes.
4. Commit your changes (git commit -am 'Add some feature').
5. Push to the branch (git push origin feature-branch).
6. Create a new Pull Request.

## Thanks

In no particular order, thanks to ACE team, GDLE team, gmriggs, OptimShi, paradox, and Yonneh. Used lots of projects as a reference for different parts.

## License

This project is licensed under the MIT License. See the LICENSE.txt file for details.
