using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject.UI;

/// <summary>
/// Holds UI elements
/// </summary>
public class UIContainer : IUIElement
{
    List<IUIElement> elementList = new();

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var element in elementList)
        {
            element.Draw(spriteBatch);
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (var element in elementList)
        {
            element.Update(gameTime);
        }
    }

    public void AddElement(IUIElement element)
    {
        elementList.Add(element);
    }
}
