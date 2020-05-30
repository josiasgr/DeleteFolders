# DeleteFolders

![GitHub Actions (build master)](https://github.com/josiasgr/DeleteFolders/workflows/GitHub%20Actions%20(build%20master/develop)/badge.svg?branch=master)

![GitHub Actions (build develop)](https://github.com/josiasgr/DeleteFolders/workflows/GitHub%20Actions%20(build%20master/develop)/badge.svg?branch=develop)

Usage: DeleteFolders [BaseFolder] regex-pattern... [Options]

Delete unwanted content from your work folders.

Options

* BaseFolder         [Optional] Base folder, this program will search for subdirectory folders that match the patterns. If not defined, it looks in the current folder.
* regex-pattern       Required, one or multiple regex patterns to match against the subdirectories names.
*-r, --recycle-bin   [Optional] If defined, the deleted folders will be moved to Recycle Bin, if not they will be permanently deleted.
*-q, --quiet         [Optional] Hide any program output.
*-d, --dry-run       [Optional] Emulate the program execution without actually deleting any folder.
