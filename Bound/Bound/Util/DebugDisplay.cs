using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ToBeDetermined;

public class DebugDisplay
{
    private static List<string> strings;
    private int x;
    private int y;
    
    public DebugDisplay(int xpos, int ypos)
    {
        strings = new List<string>();
        x = xpos;
        y = ypos;
    }

    public void addDebug(string str)
    {
        strings.Add(str);
    }

    public void Draw()
    {
        foreach (string s in strings)
        {
            int i = strings.IndexOf(s);
            Game1.spriteBatch.DrawString(Render.debugFont, s, new Vector2(x,y + i * 20), Color.White);
        }
        
        strings.Clear();

    }
}