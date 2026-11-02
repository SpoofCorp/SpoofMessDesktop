using SpoofMess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpoofMess.Services;

public interface IMediaService
{
    public IEnumerable<FileObject> GetFilesInCurrentChat();
}
