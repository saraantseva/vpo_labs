using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Domain.Entities
{
    public sealed record class Person(string Name, DateTime DateOfBirth);
}
