# ACClientLib.DatReaderWriter

ACClientLib.DatReaderWriter is an open-source library for reading and writing .dat files used by the game Asheron's Call. This tool allows players and developers to access and modify game data files for various purposes, such as creating mods or analyzing game content.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Basic Usage](#basic-usage)
- [Contributing](#contributing)
- [Thanks](#thanks)
- [License](#license)

## Features

- Read/Write AC .dat files
- Full BTree insertion/addition/removal/seeking

## Basic Usage

See Tests for full usage.  
  
**Rewrite all MotionTables to be 100x speed:**
```cs  
// open portal dat for writing
using var portalDat = new DatDatabaseReader(options => {
    options.FilePath = Path.Combine(gameDatDir, "client_portal.dat");
    options.AccessType = DatAccessType.ReadWrite;
});

// get all MotionTable entries
var ids = portalDat.Tree.Select(f => f.Id).Where(id => id >= 0x09000000 && id <= 0x0900FFFF);

foreach (var id in ids) {
    // try to get DBObj from file entry id
    if (portalDat.TryReadFile<MotionTable>(id, out var mTable)) {
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
}  
  
// close dat
dat.Dispose();
```

## Contributing

We welcome contributions from the community! If you would like to contribute to ACClientLib.DatReaderWriter, please follow these steps:

1. Fork the repository.
2. Create a new branch (git checkout -b feature-branch).
3. Make your changes.
4. Commit your changes (git commit -am 'Add some feature').
5. Push to the branch (git push origin feature-branch).
6. Create a new Pull Request.

## Thanks

In no particular order, thanks to ACE team, GDLE team, OptimShi, paradox, and Yonneh. Used lots of projects as a reference for different parts.

## License

This project is licensed under the MIT License. See the LICENSE.txt file for details.
