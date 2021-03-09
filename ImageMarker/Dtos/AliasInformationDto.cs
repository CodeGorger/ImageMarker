using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMarker.Dtos
{
    class AliasInformationDto
    {
        public AliasInformationDto()
        {
            Initialized = false;
            AliasesDirAndFile = "";
            AliasesList = new List<string>();
        }
        public bool Initialized;
        public string AliasesDirAndFile;
        public List<string> AliasesList;

    }
}
