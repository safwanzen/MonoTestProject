using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Hockey;

namespace Hockey;

public class EntityManager
{
    private readonly List<Base> entities = new();
    private readonly List<Base> entitiesToAdd = new();
    private readonly List<Base> entitiesToRemove = new();
    private static EntityManager entitymanager;

    public List<Base> Entities => entities; // why use a getter instead of direct access?

    private EntityManager()
    {
    }

    public static EntityManager Manager
    {
        get
        { 
            entitymanager ??= new EntityManager();
            return entitymanager;
        }
    }

    // cache changes to apply during update
    public void AddObject(Base b)
    {
        entitiesToAdd.Add(b);
    }

    public void RemoveObject(Base b)
    {
        entitiesToRemove.Add(b);
    }

    internal void Update(GameTime gameTime)
    {
        foreach (var e in entities)
            e.Update(gameTime);

        if (entitiesToAdd.Count > 0)
        {
            entities.AddRange(entitiesToAdd);
            entitiesToAdd.Clear();
        }

        if (entitiesToRemove.Count > 0)
        {
            foreach (var e in entitiesToRemove)
            {
                entities.Remove(e);
            }
            entitiesToRemove.Clear();
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        foreach (var e in entities)
            e.Draw(spriteBatch);
    }
}
