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

**Update spell names / descriptions**
```cs
var portalDat = new PortalDatabase(o => {
    o.FilePath = Path.Combine(config.clientDir, "client_portal.dat");
    o.AccessType = DatAccessType.ReadWrite;
});

var spellTable = portalDat.SpellTable ?? throw new Exception("Failed to read spell table");

// get the decrypted spell component ids, so we can re-encrypt them later with the new key
var decryptedSpellComponentIds = spellTable.Spells[1].DecryptedComponents();

// update spell name / description (this changes the encryption key for component ids)
spellTable.Spells[1].Name = "Strength Other I (updated)";
spellTable.Spells[1].Description = "Increases the target's Strength by 10 points. (updated)";

// set the raw components to the original decrypted spell component ids.
// this will encrypt the spell components again with the new key
spellTable.Spells[1].SetRawComponents(decryptedSpellComponentIds);
decryptedSpellComponentIds = spellTable.Spells[1].DecryptedComponents();

//write the updated spell table
if (!portalDat.TryWriteFile(spellTable)) {
    throw new Exception("Failed to write spell table");
}

// close dat
portalDat.Dispose();
```

**Rewrite all MotionTables to be 100x speed:**
```cs  
// open portal dat for writing
using var portalDat = new PortalDatabase(options => {
    options.FilePath = Path.Combine(gameDatDir, "client_portal.dat");
    options.AccessType = DatAccessType.ReadWrite;
});

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
