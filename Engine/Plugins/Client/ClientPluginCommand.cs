﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Plugins.Client
{
  public abstract class ClientPluginCommand :
    CrossDomainObject,
    ICommand<ClientCommandArgs>
  {
    public abstract ushort Id { get; }
    public abstract void Run(ClientCommandArgs args);
  }
}
