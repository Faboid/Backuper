using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backuper.Core.Services {
    public interface IPathsBuilderService {

        DateTime VersionNameToDateTime(string versionPath);
        string GenerateNewBackupVersionDirectory(string backupsDirectory);

    }
}
