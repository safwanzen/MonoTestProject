using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class Entity
{
    public virtual void Update(float deltaTime) { }
    public virtual void Draw(SpriteBatch spriteBatch) { }
}
