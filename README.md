# Stellinator

Stellinator is a tool for copying files generated by [Stellina](https://vaonis.com/stellina) on a thumb drive. It is based on the observation that Stellina only outputs `.jpeg` and `.tiff` files on good frames (exposures that were not rejected by Stellina's algorithms). Therefore, when presented with a file list like this:

```text
img-0001.fits
img-0001-output.jpeg
img-0002.fits
img-0003.fits
img-0004.fits
img-0004-output.jpeg
```

The images sequenced with `0002` and `0003` were rejected by Stellina. To save time with processing, I filter those out. The tool also renames the files to ensure they are unique.  

By default, the tool maps the files based on the observation object and date. Consider a starting folder that looks like this:

```text
Stellina-38a323
    281
        2021-04-21_04-23-30-observation-IC443
            04-24-43-capture-initial
                img-0001.fits
                img-0001-output.jpeg
                img-0002.fits
                img-0003.fits
                img-0004.fits
                img-0004-output.jpeg
                img-0004.tif
            04-38-50-capture-adjust-focus
                img-0001.fits
                img-0001-output.jpeg
                img-0002.fits
                img-0003.fits
                img-0004.fits
                img-0004-output.jpeg
                img-0004.tif
    283
        2021-04-27_04-35-11-observation-IC443
            04-36-21-capture-initial
                img-0001.fits
                img-0001-output.jpeg
                img-0002.fits
                img-0003.fits
                img-0004.fits
                img-0004-output.jpeg
                img-0004.tif
```

By default, the tool will copy it to a structure like this:

```
IC443
    2021-04-21
        Accepted
            8d8f787e4a80f4-0001.fits
            8d8f787e4a80f4-0002.fits
            8d8f787e4a80f4-0003.fits
            8d8f787e4a80f4-0004.fits
        Rejected
            img-0002.fits
            img-0002(01).fits
            img-0003.fits
            img-0003(01).fits
        Processed
            img-0001-output.jpeg
            img-0001-output(01).jpeg
            img-0004-output.jpeg
            img-0004-output(01).jpeg
            img-0004.tif
    2021-04-27
        Accepted
            9d8f777e4b80f5-0001.fits
            9d8f777e4b80f5-0002.fits
        Rejected
            img-0002.fits
            img-0003.fits
        Processed
            img-0001-output.jpeg
            img-0004-output.jpeg
            img-0004.tif
```
## Getting started

Check out our [releases](https://github.com/JeremyLikness/DeepSkyWorkflows/releases) to download the latest. 

> **NOTE** be sure to run using the `-s` flag to see how your filesystem will be impacted without making changes!

## Options

The following options are available with Stellinator.

|Short|Long|Default|Description|
|---|---|---|---|
|`-d`|`--directory-only`|`false`|Only scans a single directory; does not recurse subdirectories.|
|`-q`|`--quiet-mode`|`false`|Suppress verbose updates.|
|`-s`|`--scan-only`|`false`|Scan only; show logic but do not copy files.|
|`-i`|`--ignore`|`Nothing`|Choose to ignore files. Combine multiple options by separating them with a comma. Options are:<br>`Nothing`<br>`Rejection` will process rejected files as if they are accepted.<br>`Rejected` will ignore and not copy rejected files.<br>`Jpeg`<br>`Tiff`<br>`AllButLast` will only copy the most recent `.jpeg` and `.tif` from Stellina.|
|`-g`|`--group-strategy`|`Date`|Choose how to group images together. Will create directories based on:<br>`Observation` folder per object<br>`Date` folder per object per date<br>`Capture` folder per capture|
|`-t`|`--target-filename-strategy`|`TicksHex`|Choose the strategy to rename target images. Valid values: `Original` preserves filenames<br>`New` uses a prefix you provide<br>`Ticks` uses numeric timestamp<br>`TicksHex` uses hexadecimal timestamp|
|`-n`|`--new-filename`| |Set the name of target files when using target filename strategy `New`.|
| |`--help`| |Display thE help screen.|
| |`--version`| |Display version information.|

You must also pass the source directory which should include `Stellina` in the path name, and the root of the target directory to place files in. Stellinator is smart enough to embed into existing directories if they exist.